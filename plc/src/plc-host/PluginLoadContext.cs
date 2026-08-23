using System.Reflection;
using System.Runtime.Loader;

namespace Plc.Host.Plugins;

/// <summary>
/// 可卸载的插件隔离上下文。
/// 遵循 DotNetCorePlugins(McMaster) 的核心思路：每个插件独立 ALC + 依赖解析器，
/// 以支持运行时卸载与热重载。此处用原生 AssemblyLoadContext 实现以兼容 .NET 11 Preview，
/// 不引入外部包；如需改用 McMaster.NETCore.Plugins 包，仅替换本类与 PluginManager 的加载调用即可。
/// </summary>
public sealed class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginAssemblyDirectory)
        : base(isCollectible: true)
    {
        // 以插件程序集所在目录为锚点解析其依赖（.deps.json / 同目录 dll）
        _resolver = new AssemblyDependencyResolver(pluginAssemblyDirectory);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // 契约层(Plc.Plugins.Contracts)等共享程序集由宿主默认 ALC 提供，此处返回 null 让其回退到默认上下文，
        // 从而保证 IPlugin 等类型身份一致（避免 “无法加载类型” 的 ALC 隔离经典坑）。
        var path = _resolver.ResolveAssemblyToPath(assemblyName);
        if (path is null)
        {
            return null;
        }

        return LoadFromAssemblyPath(path);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (path is null)
        {
            return IntPtr.Zero;
        }

        return LoadUnmanagedDllFromPath(path);
    }
}
