namespace Shared;

public class TelemetryOptions
{
    public string? ServiceName { get; set; }
    public string? ServiceNamespace { get; set; }
    public string? ServiceVersion { get; set; }

    public string? MeterName { get; set; }
    public bool ConfigureMeter { get; set; } = true;
    public string? ActivitySourceName { get; set; }
    public bool ConfigureActivitySource { get; set; } = true;

    public string? OtlpExporterEndpoint { get; set; }
    public bool EnableOtlpExporter => !string.IsNullOrWhiteSpace(OtlpExporterEndpoint);
    public bool EnableConsoleExporter { get; set; }

    public DataDogOptions? DataDog { get; set; }
    public bool EnableDataDogExporter => DataDog != null && DataDog.ApiKey != null;
}

public class DataDogOptions
{
    public string? ApiKey { get; set; }

    public string Env { get; set; } = "dev";
    public string Source { get; set; } = "sc-oct-2022-otel-demo";
    public string Team { get; set; } = "Cobras";
    public string Location { get; set; } = "Local";
}