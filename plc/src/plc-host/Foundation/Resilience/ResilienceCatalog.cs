using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace Plc.Host.Foundation.Resilience;

/// <summary>
/// 基于 Polly 8 的弹性管线目录。
/// <para>
/// 宿主启动时构建命名 <see cref="ResiliencePipeline"/>，各能力模块注入本目录，对外部调用
/// （设备网关、远程 API、IO）包裹重试 / 熔断 / 超时等弹性策略。这是「基础能力弹性化」的统一入口，
/// 避免各模块各自硬编码重试逻辑。
/// </para>
/// <para>
/// 调用方使用核心回调形式 <c>ExecuteAsync(async ctx =&gt; { ... })</c>；若需返回值，用闭包捕获即可。
/// </para>
/// </summary>
public sealed class ResilienceCatalog
{
    private readonly ResiliencePipeline _default;

    public ResilienceCatalog()
    {
        // default：指数退避重试(含抖动) + 熔断 + 全局超时。
        // 适配 PLC/设备场景下常见的瞬时抖动与下游不可用，既保护下游也避免上游雪崩。
        _default = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromSeconds(15),
            })
            .AddTimeout(TimeSpan.FromSeconds(5))
            .Build();
    }

    /// <summary>获取弹性管线，缺省 "default"。</summary>
    public ResiliencePipeline Get(string name = "default") => _default;

    /// <summary>当前已注册管线名称（用于 /sys/capabilities 展示）。</summary>
    public IReadOnlyCollection<string> Names => new[] { "default" };
}
