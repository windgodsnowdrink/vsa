using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace DemoDevicePlugin;

/// <summary>
/// 演示插件：以 PLC 设备协议形态加载，验证“插件化”骨架。
/// 后续（优先级 3）真实 Modbus/BACnet/Fatek 等协议驱动将按此契约改造为插件。
/// </summary>
public sealed class DemoModbusPlugin : IPlugin
{
    public string Id => "demo.modbus";
    public string Name => "Demo Modbus 设备协议";
    public string Version => "1.0.0";

    public Task StartAsync(IPluginContext context, CancellationToken cancellationToken = default)
    {
        context.Logger.LogInformation(
            "[DemoModbus] 设备协议插件已启动（演示 PLC 插件化；目录={Dir}）",
            context.PluginDirectory);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        // 演示：此处释放驱动句柄 / 注销轮询任务等
        return Task.CompletedTask;
    }
}
