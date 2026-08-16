using System.Globalization;
using System.IO;
using PlcAiot.Abstractions.Devices;

namespace PlcAiot.Devices;

/// <summary>
/// CSV 点表导入/导出实现。换 PLC 品牌：旧品牌导出 CSV → 映射 Tag → 新品牌导入生成新 Profile，
/// 历史点表/告警阈值可平移（借鉴 OPC-UA Nodeset 思路，§1.12.5）。
/// </summary>
public sealed class CsvPointTableExchange(IDeviceCatalog catalog) : IPointTableExchange
{
    public async Task ExportAsync(string deviceId, Stream outCsv, CancellationToken ct = default)
    {
        var profile = catalog.All.First(p => p.DeviceId == deviceId);
        using var writer = new StreamWriter(outCsv, leaveOpen: true);
        await writer.WriteLineAsync("Tag,Address,FunctionCode,DataType,ByteOrder,Scale,Offset,Unit");
        foreach (var pt in profile.Points)
            await writer.WriteLineAsync(
                $"{pt.Tag},{pt.Address},{pt.FunctionCode},{pt.DataType},{pt.ByteOrder},{pt.Scale.ToString(CultureInfo.InvariantCulture)},{pt.Offset.ToString(CultureInfo.InvariantCulture)},{pt.Unit ?? ""}");
        await writer.FlushAsync(ct);
    }

    public async Task<DeviceProfile> ImportAsync(Stream inCsv, CancellationToken ct = default)
    {
        using var reader = new StreamReader(inCsv);
        await reader.ReadLineAsync(ct);                  // 跳过表头
        var points = new List<RegisterPoint>();
        string? line;
        while ((line = await reader.ReadLineAsync(ct)) is not null)
        {
            var c = line.Split(',');
            points.Add(new RegisterPoint
            {
                Tag = c[0],
                Address = ushort.Parse(c[1], CultureInfo.InvariantCulture),
                FunctionCode = byte.Parse(c[2], CultureInfo.InvariantCulture),
                DataType = c[3],
                ByteOrder = Enum.TryParse<ByteOrder>(c[4], out var bo) ? bo : ByteOrder.ABCD,
                Scale = double.Parse(c[5], CultureInfo.InvariantCulture),
                Offset = double.Parse(c[6], CultureInfo.InvariantCulture),
                Unit = c.Length > 7 ? c[7] : null
            });
        }
        return new DeviceProfile
        {
            DeviceId = $"IMPORTED-{points.Count}",
            DisplayName = "Imported from CSV",
            Protocol = "Modbus",
            Points = points
        };
    }
}
