using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Datadog.Logs;
using System.Net;

namespace Shared.DataDog;

internal class DDogLogExporter : BaseExporter<LogRecord>
{
    private readonly ResourceBuilder _resourceBuilder;
    private readonly ILogEventSink _sink;
    private readonly MessageTemplateParser _messageTemplateParser;

    public DDogLogExporter(ResourceBuilder resourceBuilder, TelemetryOptions opts)
    {
        _resourceBuilder = resourceBuilder;
        _messageTemplateParser = new MessageTemplateParser();

        var defaultDatadogConfiguration = new DatadogConfiguration(
            url: "https://http-intake.logs.datadoghq.eu",
            port: 443,
            useSSL: true);
        // hack - we're piggybacking on Serilog Sink implementation - sorry Serilog
        _sink = DatadogSink.Create(
            apiKey: opts.DataDog!.ApiKey,
            source: opts.DataDog!.Source,
            service: opts.ServiceName, host: Dns.GetHostName(), tags: new[]
            {
                $"team:{opts.DataDog.Team}",
                $"location:{opts.DataDog.Location}"
            },
            config: defaultDatadogConfiguration);
    }
    public override ExportResult Export(in Batch<LogRecord> logRecordBatch)
    {
        using (SuppressInstrumentationScope.Begin())
        {
            try
            {
                foreach (var record in logRecordBatch)
                {
                    var message = record.State?.ToString();
                    var properties = record.StateValues?.Select(x => new LogEventProperty(x.Key, new ScalarValue(x.Value)));

                    LogEvent logEvent = MapToLogEvent(record, message, properties);

                    // DDog formatted trace and span id - needed for DDog trace correlation
                    ApplyDataDogAttributes(record, logEvent);

                    // log events do get batched in the sink itself
                    _sink.Emit(logEvent);
                }
            }
            catch (Exception ex)
            {
                return ExportResult.Failure;
            }

            return ExportResult.Success;
        }
    }

    private static void ApplyDataDogAttributes(LogRecord record, LogEvent logEvent)
    {
        var ddTraceId = Convert.ToUInt64(record.TraceId.ToString().Substring(16), 16).ToString();
        var ddSpanId = Convert.ToUInt64(record.SpanId.ToString(), 16).ToString();
        logEvent.AddOrUpdateProperty(new LogEventProperty("trace_id", new ScalarValue(ddTraceId)));
        logEvent.AddOrUpdateProperty(new LogEventProperty("span_id", new ScalarValue(ddSpanId)));
    }

    private LogEvent MapToLogEvent(LogRecord record, string? message, IEnumerable<LogEventProperty>? properties)
    {
        return new LogEvent(
        timestamp: record.Timestamp,
        level: record.LogLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        },
        exception: record.Exception,
        messageTemplate: _messageTemplateParser.Parse(message),
        properties: properties ?? Enumerable.Empty<LogEventProperty>()
        );
    }

    protected override bool OnShutdown(int timeoutMilliseconds)
    {
        return true;
    }
}