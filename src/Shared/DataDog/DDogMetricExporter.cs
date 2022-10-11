using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace Shared.DataDog;

internal class DDogMetricExporter : BaseExporter<Metric>
{
    public override ExportResult Export(in Batch<Metric> batch)
    {
        using var scope = SuppressInstrumentationScope.Begin();

        foreach (var metric in batch)
        {
            foreach (ref readonly var metricPoint in metric.GetMetricPoints())
            {
            }
        }

        return ExportResult.Success;
    }
}
