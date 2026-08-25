using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;
using McMaster.NETCore.Plugins;
using Plc.Plugins.Contracts;

namespace PlcGateway;

/// <summary>
/// 宿主设备目录 + 插件加载器（需求 3/5：DotNetCorePlugins）。
/// 宿主只依赖共享契约，SNET 等重依赖留在插件程序集内隔离。
/// </summary>
public sealed class GatewayHost : IDeviceCatalog
{
    private readonly ConcurrentDictionary<string, IDeviceProtocol> _protocols = new();
    private readonly ConcurrentDictionary<string, DeviceCfg> _devices = new();
    private readonly ILogger _log;
    private readonly IServiceProvider _services;

    public GatewayHost(ILogger<GatewayHost> log, IServiceProvider services)
    {
        _log = log;
        _services = services;
    }

    public void Register(IDeviceProtocol protocol) => _protocols[protocol.ProtocolId] = protocol;
    public IDeviceProtocol? Get(string protocolId) => _protocols.GetValueOrDefault(protocolId);
    public IReadOnlyCollection<IDeviceProtocol> All => _protocols.Values.ToList();

    /// <summary>注册 manifest 中的设备（按 id 索引），供 RPC 按设备路由读写。</summary>
    public void RegisterDevice(DeviceCfg device) => _devices[device.Id] = device;
    public DeviceCfg? GetDevice(string deviceId) => _devices.GetValueOrDefault(deviceId);
    public IReadOnlyCollection<DeviceCfg> Devices => _devices.Values.ToList();

    /// <summary>扫描插件目录，按 DotNetCorePlugins 加载每个 dll，共享契约程序集与 System.IO.Ports。</summary>
    public void LoadPlugins(string directory)
    {
        if (!Directory.Exists(directory)) return;
        foreach (var dll in Directory.GetFiles(directory, "*.dll"))
        {
            try
            {
                // 共享类型数组：把这些契约类型放进插件的加载上下文，使其与宿主共用同一份程序集，
                // 避免“宿主 IPlugin 与插件 IPlugin 是两个类型”导致 IsAssignableFrom 失败。
                // System.IO.Ports 也需共享：宿主持有 10.0.0.11，与 SNET 强签名引用版本一致，避免插件 ALC 解析不到。
                var sharedTypes = new[]
                {
                    typeof(IPlugin),
                    typeof(IDeviceProtocol),
                    typeof(IDeviceSession),
                    typeof(IDeviceCatalog),
                    typeof(IPluginContext),
                    typeof(System.IO.Ports.SerialPort),
                };
                var loader = PluginLoader.CreateFromAssemblyFile(
                    dll,
                    sharedTypes,
                    cfg => { cfg.IsUnloadable = false; });

                var asm = loader.LoadDefaultAssembly();
                var types = GetLoadableTypes(asm);
                foreach (var type in types
                             .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null))
                {
                    var plugin = (IPlugin)Activator.CreateInstance(type)!;
                    plugin.StartAsync(new HostPluginContext(_services, _log, Path.GetDirectoryName(dll) ?? directory), default)
                        .GetAwaiter().GetResult();
                    _log.LogInformation("插件已加载: {Name} ({Id})", plugin.Name, plugin.Id);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "插件加载失败: {Dll}", dll);
            }
        }
    }

    /// <summary>枚举程序集类型，容忍部分类型因依赖缺失无法加载的情况（ReflectionTypeLoadException）。</summary>
    private static IEnumerable<Type> GetLoadableTypes(Assembly asm)
    {
        try
        {
            return asm.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null)!;
        }
    }
}

/// <summary>插件运行时上下文实现（宿主侧注入的服务 / 日志 / 目录 / 停止信号）。</summary>
internal sealed class HostPluginContext : IPluginContext
{
    public HostPluginContext(IServiceProvider services, ILogger logger, string pluginDirectory)
    {
        Services = services;
        Logger = logger;
        PluginDirectory = pluginDirectory;
    }

    public IServiceProvider Services { get; }
    public ILogger Logger { get; }
    public string PluginDirectory { get; }
    public CancellationToken AppStopping { get; } = CancellationToken.None;
}
