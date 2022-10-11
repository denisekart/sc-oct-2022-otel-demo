using OpenTelemetry;
using System.Diagnostics;

namespace Shared.DataDog;

internal class DDogActivityExporter : BaseExporter<Activity>
{
    public override ExportResult Export(in Batch<Activity> batch)
    {
        using var scope = SuppressInstrumentationScope.Begin();

        // TODO: Implementation

        return ExportResult.Success;
    }
}
