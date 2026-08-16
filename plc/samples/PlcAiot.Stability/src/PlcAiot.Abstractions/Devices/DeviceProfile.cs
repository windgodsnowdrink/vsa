namespace PlcAiot.Abstractions.Devices;

/// <summary>字节序（平台内部定义，避免与 System.Buffers.Endianness 歧义）。</summary>
public enum DeviceEndianness { Big, Little }

/// <summary>
/// 统一设备模型：一个具体 PLC 有哪些寄存器、什么类型、什么字节序、怎么轮询，
/// 全部外置为 DeviceProfile（JSON/YAML）。新增/更换 PLC = 新增/改一个 Profile，零代码改动（§1.12.1）。
/// </summary>
public sealed record DeviceProfile
{
    public string DeviceId { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string Protocol { get; init; } = "Modbus";      // Modbus/S7/OpcUa/Melsec/...
    public string Transport { get; init; } = "Tcp";        // Tcp/Rtu/Udp
    public DeviceEndpoint Endpoint { get; init; } = new();
    public DeviceEndianness Endianness { get; init; } = DeviceEndianness.Big;
    public int PollingMs { get; init; } = 1000;
    public IReadOnlyList<RegisterPoint> Points { get; init; } = [];
    public string ProfileSchemaVersion { get; init; } = "1.0";
}
