using Plc.Plugins.Contracts;
using StreamJsonRpc;

namespace PlcGateway;

/// <summary>
/// StreamJsonRpc 暴露的组件间 RPC（需求 7）：前端 / 网关 ↔ 网关。
/// 读写按 manifest 设备 id 路由到对应协议与会话，读取结果同时写入
/// Disruptor 流水线（需求 4：按需读也进入消息记忆 / 日志流）。
/// </summary>
public sealed class GatewayRpc
{
    private readonly GatewayHost _host;
    private readonly FramePipeline _pipeline;

    public GatewayRpc(GatewayHost host, FramePipeline pipeline)
    {
        _host = host;
        _pipeline = pipeline;
    }

    [JsonRpcMethod("gateway.listProtocols")]
    public string[] ListProtocols() => _host.All.Select(p => p.ProtocolId).ToArray();

    [JsonRpcMethod("gateway.listDevices")]
    public DeviceCfg[] ListDevices() => _host.Devices.ToArray();

    [JsonRpcMethod("gateway.read")]
    public async Task<byte[]> ReadAsync(string deviceId, ushort start, ushort count, int type)
    {
        var dev = _host.GetDevice(deviceId)
            ?? throw new InvalidOperationException("未找到设备: " + deviceId);
        var protocol = _host.Get(dev.Protocol)
            ?? throw new InvalidOperationException("设备 " + deviceId + " 的协议未注册: " + dev.Protocol);

        await using var session = protocol.CreateSession(
            new DeviceConnectionOptions(dev.Host, dev.Port, (byte)(dev.UnitId ?? 1)));
        var data = await session.ReadAsync(new ReadRequest(start, count, (RegisterType)type));

        // 按需读同样进入流水线：消息记忆 / 日志 / 转发统一入口
        _pipeline.Publish(new RawFrame
        {
            DeviceId = dev.Id,
            Timestamp = DateTime.Now,
            Payload = data,
            Note = "rpc.read:" + dev.Protocol
        });
        return data;
    }

    [JsonRpcMethod("gateway.write")]
    public async Task WriteAsync(string deviceId, ushort start, int type, ushort[] values)
    {
        var dev = _host.GetDevice(deviceId)
            ?? throw new InvalidOperationException("未找到设备: " + deviceId);
        var protocol = _host.Get(dev.Protocol)
            ?? throw new InvalidOperationException("设备 " + deviceId + " 的协议未注册: " + dev.Protocol);

        await using var session = protocol.CreateSession(
            new DeviceConnectionOptions(dev.Host, dev.Port, (byte)(dev.UnitId ?? 1)));
        await session.WriteAsync(new WriteRequest(start, (RegisterType)type, values));

        _pipeline.Publish(new RawFrame
        {
            DeviceId = dev.Id,
            Timestamp = DateTime.Now,
            Payload = Array.Empty<byte>(),
            Note = "rpc.write:" + dev.Protocol
        });
    }
}
