using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Plc.Host.Foundation;

/// <summary>
/// 基础能力模块引导器：用 Scrutor 程序集扫描自动发现 <see cref="ICapabilityModule"/> 实现，
/// 注册为单例（便于按接口统一解析与排序），并逐个调用 <c>RegisterServices</c> 完成能力装配。
/// <para>
/// 这是「基础能力模块化」的核心扩展点：新增能力只需新增一个实现 <see cref="ICapabilityModule"/> 的类，
/// 无需改动宿主启动代码。
/// </para>
/// </summary>
public static class ModuleBootstrapper
{
    /// <summary>
    /// 扫描指定程序集（默认宿主入口程序集）中的能力模块，注册并装配。
    /// </summary>
    public static IServiceCollection AddCapabilityModules(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var scanAssemblies = assemblies.Length > 0
            ? assemblies
            : new[] { typeof(ModuleBootstrapper).Assembly };

        // 1) Scrutor：将全部 ICapabilityModule 实现注册为单例，供宿主按接口解析与排序。
        services.Scan(scan => scan
            .FromAssemblies(scanAssemblies)
            .AddClasses(c => c.AssignableTo<ICapabilityModule>())
            .As<ICapabilityModule>()
            .WithSingletonLifetime());

        // 2) 直接实例化（无参构造）并调用 RegisterServices，避免提前 BuildServiceProvider 导致的半装配问题。
        var modules = scanAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ICapabilityModule).IsAssignableFrom(t)
                        && !t.IsAbstract
                        && t.GetConstructor(Type.EmptyTypes) is not null)
            .Select(t => (ICapabilityModule)Activator.CreateInstance(t)!)
            .OrderBy(m => m.Order)
            .ToList();

        foreach (var module in modules)
        {
            module.RegisterServices(services);
        }

        return services;
    }
}
