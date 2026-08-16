using PlcAiot.Abstractions.Devices;

namespace PlcAiot.Devices;

/// <summary>通用点表导入/导出（互换格式，§1.12.5）。</summary>
public interface IPointTableExchange
{
    Task ExportAsync(string deviceId, Stream outCsv, CancellationToken ct = default);
    Task<DeviceProfile> ImportAsync(Stream inCsv, CancellationToken ct = default);
}
