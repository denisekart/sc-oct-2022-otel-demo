namespace Shared;

public class TelemetryOptions
{
    public string? ServiceName { get; set; }
    public string? ServiceNamespace { get; set; }
    public string? ServiceVersion { get; set; }

    /// <summary>
    /// The default meter name (or assembly name if omitted)
    /// </summary>
    public string? MeterName { get; set; }
    /// <summary>
    /// If set to true, a singleton instance of a Meter will be added to the DI
    /// </summary>
    public bool ConfigureMeter { get; set; } = true;
    /// <summary>
    /// The default activity source name (or assembly name if omitted)
    /// </summary>
    public string? ActivitySourceName { get; set; }
    /// <summary>
    /// If set to true a singleton instance of an ActivitySource will be added to the DI
    /// </summary>
    public bool ConfigureActivitySource { get; set; } = true;

    /// <summary>
    /// The gRPC OpenTelemetry collector endpoint.
    /// </summary>
    public string? OtlpExporterEndpoint { get; set; }

    /// <summary>
    /// True if an endpoint is defined
    /// </summary>
    public bool EnableOtlpExporter => !string.IsNullOrWhiteSpace(OtlpExporterEndpoint);

    /// <summary>
    /// If set to true, telemetry will also be exported to the console
    /// </summary>
    public bool EnableConsoleExporter { get; set; }
}