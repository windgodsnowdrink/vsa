// ────────────────────────────────────────────────────────────────────────────
// Slice: Catalog + IoTClient Factory（使用 PlcVsa.Contracts.Devices 中的共享接口）
// 对应 8 项需求 §8（IoTClient 风格统一 API）+ §2 协议目录
// ────────────────────────────────────────────────────────────────────────────
using System.Collections.Concurrent;
using System.Diagnostics;
using PlcVsa.Contracts.Devices;
using PlcVsa.Server.Infrastructure;
using PlcAddr = System.String;

namespace PlcVsa.Server.Slices;

/// <summary>§2 协议目录：按 ProtocolId → IDeviceProtocol 注册中心（单例）。</summary>
public sealed class DeviceCatalog : IDeviceCatalog
{
    private readonly ConcurrentDictionary<string, IDeviceProtocol> _map = new();
    public void Register(IDeviceProtocol p) => _map[p.ProtocolId] = p;
    public IDeviceProtocol? Get(string id) => _map.TryGetValue(id, out var p) ? p : null;
    public IReadOnlyCollection<IDeviceProtocol> All => _map.Values.ToList();
}

/// <summary>§8 IoTClient 工厂（封装 IIoTClient 创建）。</summary>
public sealed class IoTClientFactory : IIoTClientFactory
{
    private readonly IDeviceCatalog _catalog;
    public IoTClientFactory(IDeviceCatalog c) => _catalog = c;

    public IIoTClient Create(string protocolId, DeviceConnectionOptions opts)
    {
        var p = _catalog.Get(protocolId) ?? throw new InvalidOperationException($"未注册协议: {protocolId}");
        return new ProtocolClientAdapter(p, opts);
    }
}

/// <summary>
/// 适配层：将底层 IDeviceProtocol → 高层 IIoTClient。
/// 满足 Contracts 的统一 Read(address, dataType) / Write(address, value) 语义。
/// </summary>
public sealed class ProtocolClientAdapter : IIoTClient
{
    private readonly IDeviceProtocol _p;
    private readonly IDeviceSession _s;
    public string Protocol => _p.ProtocolId;

    public ProtocolClientAdapter(IDeviceProtocol p, DeviceConnectionOptions opts)
    {
        _p = p;
        _s = p.CreateSession(opts);
    }

    public async ValueTask DisposeAsync() => await _s.DisposeAsync();

    public Result<T> Read<T>(string address, string? dataType = null)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var dt = DataTypeParser.Parse(dataType);
            var (start, count, rt) = AddressParser.Parse(_p.ProtocolId, address, dt);
            var bytes = _s.ReadAsync(new ReadRequest(start, count, rt)).GetAwaiter().GetResult();
            var val = ByteConverter.Read<T>(bytes, dt);
            return Result<T>.Ok(val);
        }
        catch (Exception ex)
        {
            return Result<T>.Fail($"{ex.Message} (elapsed={sw.ElapsedMilliseconds}ms)");
        }
    }

    public Result Write<T>(string address, T value)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var dt = DataTypeParser.Infer(value);
            var (start, count, rt) = AddressParser.Parse(_p.ProtocolId, address, dt);
            var bytes = ByteConverter.Write(value, dt);
            _s.WriteAsync(new WriteRequest(start, rt, ByteConverter.ToWords(bytes))).GetAwaiter().GetResult();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"{ex.Message} (elapsed={sw.ElapsedMilliseconds}ms)");
        }
    }
}
