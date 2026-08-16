// plugin-host.cs — .NET 10 File-based App（标准 JIT 插件宿主样例）
// 运行： dotnet run plugin-host.cs
//
// 场景：中心/边缘服务器的插件化宿主，需运行时热插拔协议驱动，且「卸载要干净」（§3.5）。
// 解决方案：IPlugin:IAsyncDisposable 生命周期契约 + PluginManager 有序注册/启动/反向卸载。
// 技术要点：IAsyncDisposable 保证释放连接/定时器/Channel；反向卸载避免依赖悬空；
//          真实环境插件来自 DLL（McMaster.NETCore.Plugins / AssemblyLoadContext，契约不变，见 §1.12/§3.5）。

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// —— 顶层语句（程序入口，必须位于类型声明之前）——
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var mgr = new PluginManager();
mgr.Register(new ModbusPlugin());
await mgr.StartAllAsync(cts.Token);
Console.WriteLine("插件宿主运行中（Ctrl+C 卸载）…");
try { await Task.Delay(Timeout.Infinite, cts.Token); }
catch (OperationCanceledException) { }
await mgr.DisposeAsync();   // 触发有序清理契约

// —— 类型声明（位于顶层语句之后）——
public interface IPlugin : IAsyncDisposable
{
    string Name { get; }
    Task StartAsync(CancellationToken ct);
}

// 示例插件：真实场景由 DLL 经 ALC 加载，此处用进程内实现演示生命周期
public sealed class ModbusPlugin : IPlugin
{
    private readonly Timer _heartbeat;
    public string Name => "Modbus";
    public ModbusPlugin() => _heartbeat = new Timer(_ => Console.WriteLine("[Modbus] 心跳"), null, 0, 1000);
    public Task StartAsync(CancellationToken ct) { Console.WriteLine("[Modbus] 已启动"); return Task.CompletedTask; }
    public async ValueTask DisposeAsync()        // 清理契约：释放定时器与连接
    {
        Console.WriteLine("[Modbus] 释放定时器与连接…");
        await _heartbeat.DisposeAsync();
    }
}

// 宿主：负责插件的注册、启动、以及「有序卸载」
public sealed class PluginManager : IAsyncDisposable
{
    private readonly List<IPlugin> _plugins = new();
    public void Register(IPlugin p) => _plugins.Add(p);
    public async Task StartAllAsync(CancellationToken ct)
    {
        foreach (var p in _plugins) await p.StartAsync(ct);
    }
    public async ValueTask DisposeAsync()
    {
        // 反向卸载：后注册先停，避免被依赖方先消失导致悬空
        for (int i = _plugins.Count - 1; i >= 0; i--)
            await _plugins[i].DisposeAsync();
        _plugins.Clear();
        Console.WriteLine("[Host] 所有插件已有序卸载");
    }
}
