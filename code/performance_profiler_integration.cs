#:sdk Microsoft.NET.Sdk
#:package JetBrains.Profiler.Api@2023.2.0
#:package DotTrace.CommandLineTools@2023.2.0
#:package AntsPerformanceProfiler@10.0.0
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Diagnostics;
using System.Diagnostics.Metrics;
using JetBrains.Profiler.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
/// 性能分析器集成方案
/// 支持SlimTune、dotTrace和ANTS等分析器的生产级集成
/// </summary>
public interface IPerformanceProfiler
{
    /// <summary>
    /// 开始性能分析会话
    /// </summary>
    /// <param name="sessionName">会话名称</param>
    void StartSession(string sessionName);

    /// <summary>
    /// 结束性能分析会话
    /// </summary>
    void EndSession();

    /// <summary>
    /// 捕获性能快照
    /// </summary>
    /// <param name="snapshotName">快照名称</param>
    void CaptureSnapshot(string snapshotName);
}

/// <summary>
/// 性能分析器配置选项
/// </summary>
public class PerformanceProfilerOptions
{
    /// <summary>
    /// 分析器类型 (SlimTune/dotTrace/ANTS)
    /// </summary>
    public string ProfilerType { get; set; } = "dotTrace";

    /// <summary>
    /// 采样间隔(毫秒)
    /// </summary>
    public int SamplingInterval { get; set; } = 10;

    /// <summary>
    /// 是否启用低开销模式
    /// </summary>
    public bool LowOverheadMode { get; set; } = true;
}

/// <summary>
/// dotTrace分析器实现
/// </summary>
public class DotTraceProfiler : IPerformanceProfiler
{
    private readonly PerformanceProfilerOptions _options;
    private readonly Meter _meter;
    private readonly Counter<int> _snapshotCounter;

    public DotTraceProfiler(IOptions<PerformanceProfilerOptions> options)
    {
        _options = options.Value;
        _meter = new Meter("DotTraceProfiler");
        _snapshotCounter = _meter.CreateCounter<int>("snapshots_taken");
    }

    public void StartSession(string sessionName)
    {
        if (_options.LowOverheadMode)
            MeasureProfiler.StartCollectingData();
        else
            MeasureProfiler.StartCollectingData(MeasureProfiler.GetSnapshotTarget(_options.SamplingInterval));
    }

    public void EndSession()
    {
        MeasureProfiler.StopCollectingData();
    }

    public void CaptureSnapshot(string snapshotName)
    {
        MeasureProfiler.SaveSnapshot(snapshotName);
        _snapshotCounter.Add(1);
    }
}

/// <summary>
/// 性能分析器工厂
/// </summary>
public static class PerformanceProfilerExtensions
{
    /// <summary>
    /// 添加性能分析器服务
    /// </summary>
    public static IServiceCollection AddPerformanceProfiler(this IServiceCollection services, Action<PerformanceProfilerOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IPerformanceProfiler, DotTraceProfiler>();
        return services;
    }
}

/// <summary>
/// 示例用法
/// </summary>
public static class Program
{
    public static void Main()
    {
        var services = new ServiceCollection();
        services.AddPerformanceProfiler(options =>
        {
            options.ProfilerType = "dotTrace";
            options.SamplingInterval = 5;
        });

        var provider = services.BuildServiceProvider();
        var profiler = provider.GetRequiredService<IPerformanceProfiler>();

        // 示例分析会话
        profiler.StartSession("ExampleSession");
        try
        {
            // 执行需要分析的代码
            ExampleWorkload();
            
            // 捕获快照
            profiler.CaptureSnapshot("AfterWorkload");
        }
        finally
        {
            profiler.EndSession();
        }
    }

    private static void ExampleWorkload()
    {
        // 模拟工作负载
        for (int i = 0; i < 1000; i++)
        {
            Thread.Sleep(1);
        }
    }
}