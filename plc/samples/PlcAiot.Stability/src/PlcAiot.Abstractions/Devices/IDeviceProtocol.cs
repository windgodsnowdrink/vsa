namespace PlcAiot.Abstractions.Devices;

/// <summary>
/// 设备协议统一契约（在 §6.2 IProtocolAdapter 流式帧出口之上，面向设备生命周期）。
/// Modbus/S7/OPC-UA/Melsec 等都是它的不同实现，经 §3 插件机制加载。
/// </summary>
public interface IDeviceProtocol : IAsyncDisposable
{
    string ProtocolId { get; }
    DeviceCapabilities Capabilities { get; }
    ValueTask ConfigureAsync(DeviceProfile profile, CancellationToken ct);
    ValueTask ConnectAsync(CancellationToken ct);                 // 建连（带租约/超时，§1.11.4）
    ValueTask DisconnectAsync(CancellationToken ct);              // 释放连接（防泄漏）
    IAsyncEnumerable<PointSample> PollAsync(CancellationToken ct); // 统一点表采样出口
    ValueTask WriteAsync(WriteCommand cmd, CancellationToken ct);   // 南向写（强一致）
    ValueTask<HealthProbeResult> HealthProbeAsync(CancellationToken ct); // 心跳/探针
}
