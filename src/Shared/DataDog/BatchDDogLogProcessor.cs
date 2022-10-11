using OpenTelemetry;
using OpenTelemetry.Logs;

namespace Shared.DataDog;

internal class BatchDDogLogProcessor : BatchLogRecordExportProcessor
{
    public BatchDDogLogProcessor(BaseExporter<LogRecord> exporter) : base(exporter)
    {
    }
}
