namespace Plc.Host.Foundation;

/// <summary>
/// 基础能力模块标记接口（Tier2：宿主信任、编译期内置）。
/// <para>
/// 与 Tier1 的 <c>IPlugin</c>（ALC 隔离、第三方设备协议、运行时加载）明确区分：
/// 能力模块是平台 curated 的基础能力栈（Scrutor/Polly/Carter/ScottPlot/HybridCache/OTel…），
/// 以宿主信任代码形态编译进宿主，经 <see cref="ModuleBootstrapper"/> 用 Scrutor 程序集扫描自动发现并注册。
/// </para>
/// <para>
/// 约定：能力模块若需暴露 HTTP 端点，可同时实现 <c>ICarterModule</c>（Carter 自动发现）；
/// 若需对外部调用做弹性保护，可注入 <see cref="Resilience.ResilienceCatalog"/>。
/// 后续「其他全部基础能力的模块化」均沿用此契约，新增能力 = 新增一个实现类即可。
/// </para>
/// </summary>
public interface ICapabilityModule
{
    /// <summary>能力唯一标识，如 "sys.info"、"cache.hybrid"。</summary>
    string Id { get; }

    /// <summary>展示名。</summary>
    string Name { get; }

    /// <summary>装配/启动顺序，数值越小越早（默认 0）。</summary>
    int Order => 0;

    /// <summary>向宿主 DI 注册该能力所需的服务。可选实现（默认空）。</summary>
    void RegisterServices(IServiceCollection services)
    {
    }
}
