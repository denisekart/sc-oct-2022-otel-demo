using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace Shared.DataDog;

internal class DDogMetricExporter : BaseExporter<Metric>
{
    public override ExportResult Export(in Batch<Metric> batch)
    {
        using var scope = SuppressInstrumentationScope.Begin();

        // TODO: Implementation

        return ExportResult.Success;
    }
}
