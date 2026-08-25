namespace Plc.Host.Gateway;

/// <summary>
/// 网关 <c>gateway.listDevices</c> 返回的只读 DTO。
/// 字段名与网关侧 <c>DeviceCfg</c> 的 PascalCase 属性一致（StreamJsonRpc 2.x 默认 Newtonsoft 序列化）。
/// </summary>
public sealed record GatewayDevice(
    string Id,
    string Protocol,
    string Host,
    int Port,
    int? UnitId,
    int? Rack,
    int? Slot,
    int PollIntervalMs,
    bool Enabled);
