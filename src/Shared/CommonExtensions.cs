using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace Shared;
public static class CommonExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services, Action<TelemetryOptions>? configure = null)
    {
        var opts = new TelemetryOptions();
        configure?.Invoke(opts);

        var resource = ConfigureResourceBuilder(opts);
        var defaulto11yName = Assembly.GetEntryAssembly()!.FullName!;
        var activitySourceName = opts.ActivitySourceName ?? defaulto11yName;
        var meterName = opts.MeterName ?? defaulto11yName;

        services
            .AddOpenTelemetryTracing(t => ConfigureTracing(t, opts, resource, activitySourceName))
            .AddOpenTelemetryMetrics(m => ConfigureMetrics(m, opts, resource, meterName))
            .AddLogging(l => ConfigureLogging(l, opts, resource));

        if (opts.ConfigureMeter)
        {
            services.AddSingleton(new Meter(meterName));
        }
        if (opts.ConfigureActivitySource)
        {
            services.AddSingleton(new ActivitySource(activitySourceName));
        }

        return services;
    }

    private static void ConfigureLogging(ILoggingBuilder builder, TelemetryOptions opts, ResourceBuilder resource)
    {
        builder
            .ClearProviders()
            .AddOpenTelemetry(builder =>
            {
                builder.SetResourceBuilder(resource);
                builder.IncludeScopes = true;

                if (opts.EnableOtlpExporter)
                {
                    builder.AddOtlpExporter(c => c.Endpoint = new Uri(opts.OtlpExporterEndpoint!));
                }

                if (opts.EnableConsoleExporter)
                {
                    builder.AddConsoleExporter();
                }

                if (opts.EnableDataDogExporter)
                {
                    builder.AddProcessor(new BatchDDogLogProcessor(new DDogLogExporter(resource, opts!)));
                }
            });
    }

    private static void ConfigureMetrics(MeterProviderBuilder builder, TelemetryOptions opts, ResourceBuilder resource, string meterName)
    {
        builder
            .SetResourceBuilder(resource)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter(meterName);

        if (opts.EnableOtlpExporter)
        {
            builder.AddOtlpExporter(c => c.Endpoint = new Uri(opts.OtlpExporterEndpoint!));
        }

        if (opts.EnableConsoleExporter)
        {
            builder.AddConsoleExporter();
        }

    }

    private static void ConfigureTracing(TracerProviderBuilder builder, TelemetryOptions opts, ResourceBuilder resource, string activitySourceName)
    {
        builder
            .SetResourceBuilder(resource)
            .AddAspNetCoreInstrumentation(c => c.RecordException = true)
            .AddHttpClientInstrumentation(c => c.RecordException = true)
            .AddSqlClientInstrumentation(c =>
            {
                c.RecordException = true;
                c.SetDbStatementForText = true;
                c.EnableConnectionLevelAttributes = true;
            })
            .AddSource(activitySourceName);

        if (opts.EnableOtlpExporter)
        {
            builder.AddOtlpExporter(c => c.Endpoint = new Uri(opts.OtlpExporterEndpoint!));
        }

        if (opts.EnableConsoleExporter)
        {
            builder.AddConsoleExporter();
        }
    }

    private static ResourceBuilder ConfigureResourceBuilder(TelemetryOptions opts)
    {
        return ResourceBuilder
            .CreateDefault()
            .AddTelemetrySdk()
            .AddService(
                serviceName: opts.ServiceName,
                serviceNamespace: opts.ServiceNamespace,
                serviceVersion: opts.ServiceVersion
            );
    }
}
