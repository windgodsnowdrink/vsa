// ────────────────────────────────────────────────────────────────────────────
// Slice: Protocols（Siemens S7 / Modbus TCP / Melsec MC / Omron FINS + 模拟会话）
// 对应 8 项需求 §1 PLC 协议实现。使用 Contracts 定义的 DeviceCapabilities/RegisterType。
// ────────────────────────────────────────────────────────────────────────────
using System.Collections.Immutable;
using PlcVsa.Contracts.Devices;
using U32 = System.UInt32;

namespace PlcVsa.Server.Slices;

public enum DeviceModel
{
    Siemens_S7_1200, Siemens_S7_1500, Siemens_S7_300, Siemens_S7_400,
    Modbus_Tcp, Modbus_Rtu,
    Melsec_Q, Melsec_A, Melsec_iQ_R,
    Omron_Fins, Omron_NJ,
    AB_CompactLogix,
}

/// <summary>共享 Capabilities 模板（协议只读字段）。</summary>
file static class ProtoCaps
{
    public static DeviceCapabilities Rw(int max = 100) => new()
    {
        SupportsWrite = true,
        SupportsSubscribe = false,
        MaxRegistersPerFrame = max,
        NativeDataTypes = ImmutableHashSet.Create("Bool", "Byte", "Int16", "UInt16", "Int32", "UInt32", "Float", "Double", "String"),
        MaxConcurrentSessions = 4,
    };
}

public sealed class SiemensS7Protocol : IDeviceProtocol
{
    private readonly DeviceModel _model;
    public SiemensS7Protocol(DeviceModel m = DeviceModel.Siemens_S7_1200) => _model = m;

    public string ProtocolId => _model switch
    {
        DeviceModel.Siemens_S7_1500 => "siemens-s7-1500",
        DeviceModel.Siemens_S7_300  => "siemens-s7-300",
        DeviceModel.Siemens_S7_400  => "siemens-s7-400",
        _ => "siemens-s7-1200",
    };
    public string DisplayName => $"Siemens S7 ({_model})";
    public DeviceCapabilities Capabilities => ProtoCaps.Rw(220);
    public IDeviceSession CreateSession(DeviceConnectionOptions o) =>
        new SimulatedSession(ProtocolId, o.Host + ":" + o.Port);
}

public sealed class ModbusTcpProtocol : IDeviceProtocol
{
    public string ProtocolId => "modbus-tcp";
    public string DisplayName => "Modbus TCP (Port 502)";
    public DeviceCapabilities Capabilities => ProtoCaps.Rw(125);
    public IDeviceSession CreateSession(DeviceConnectionOptions o) =>
        new SimulatedSession("modbus-tcp", o.Host + ":" + o.Port);
}

public sealed class MelsecMcProtocol : IDeviceProtocol
{
    public string ProtocolId => "melsec-mc";
    public string DisplayName => "Mitsubishi Melsec MC (Binary 3E Frame)";
    public DeviceCapabilities Capabilities => ProtoCaps.Rw(960);
    public IDeviceSession CreateSession(DeviceConnectionOptions o) =>
        new SimulatedSession("melsec-mc", o.Host + ":" + o.Port);
}

public sealed class OmronFinsProtocol : IDeviceProtocol
{
    public string ProtocolId => "omron-fins";
    public string DisplayName => "Omron FINS (UDP/TCP)";
    public DeviceCapabilities Capabilities => ProtoCaps.Rw(500);
    public IDeviceSession CreateSession(DeviceConnectionOptions o) =>
        new SimulatedSession("omron-fins", o.Host + ":" + o.Port);
}

/// <summary>演示用模拟会话：返回确定性伪数据（按地址哈希生成稳定值）。生产级接入 IoTClient。</summary>
public sealed class SimulatedSession : IDeviceSession
{
    private readonly string _proto;
    private readonly string _endpoint;
    private readonly U32 _seed;
    public SimulatedSession(string proto, string endpoint)
    {
        _proto = proto;
        _endpoint = endpoint;
        _seed = (U32)(endpoint.GetHashCode(StringComparison.Ordinal) & 0x7FFF_FFFF);
    }
    public ValueTask DisposeAsync() => default;

    public Task<byte[]> ReadAsync(ReadRequest r, CancellationToken ct)
    {
        var rand = new Random((int)(_seed + r.StartAddress * 7));
        var bytes = new byte[r.Count * 2];
        for (int i = 0; i < bytes.Length; i++) bytes[i] = (byte)rand.Next(0, 256);
        return Task.FromResult(bytes);
    }

    public Task WriteAsync(WriteRequest r, CancellationToken ct) => Task.CompletedTask;
}
