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
    public DDogLogExporter()
    {
    }
    public override ExportResult Export(in Batch<LogRecord> logRecordBatch)
    {
        using var scope = SuppressInstrumentationScope.Begin();

        // TODO: Implementation
        
        return ExportResult.Success;
    }

    protected override bool OnShutdown(int timeoutMilliseconds)
    {
        return true;
    }
}