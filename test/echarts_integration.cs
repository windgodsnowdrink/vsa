#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Mvc.NewtonsoftJson@8.0.0
#:package ECharts.Net@1.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Threading.Channels;
using System.Threading.Tasks;
using ECharts.Net;

public static class EChartsExtensions
{
    public static IServiceCollection AddEChartsServices(this IServiceCollection services, Action<EChartsOptions> configure)
    {
        services.Configure(configure);
        
        // 高性能渲染通道
        services.AddSingleton<Channel<EChartsRenderJob>>(sp => 
            Channel.CreateBounded<EChartsRenderJob>(new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            }));
            
        // 对象池配置
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton(sp => 
            sp.GetRequiredService<ObjectPoolProvider>().Create(new EChartsInstancePooledObjectPolicy()));
            
        // 线程本地缓存
        services.AddSingleton<ThreadLocal<ArrayPool<byte>>>(_ => 
            new ThreadLocal<ArrayPool<byte>>(() => ArrayPool<byte>.Shared));
            
        // 尾延迟优化统计服务
        services.AddSingleton<TailLatencyOptimizedStatistics>();
        
        // 主渲染服务
        services.AddHostedService<EChartsRenderService>();
        
        return services;
    }
}

public class EChartsOptions
    {
        /// <summary>
        /// 最大并行渲染数，默认使用处理器核心数
        /// </summary>
        public int MaxParallelRenders { get; set; } = Environment.ProcessorCount;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 渲染超时时间(毫秒)
        /// </summary>
        public int RenderTimeoutMs { get; set; } = 5000;
        
        /// <summary>
        /// 主题配置路径
        /// </summary>
        public string ThemePath { get; set; } = "themes/default.json";
        
        /// <summary>
        /// 动态数据更新间隔(毫秒)
        /// </summary>
        public int DataUpdateInterval { get; set; } = 1000;
        
        /// <summary>
        /// 是否启用WebGL加速
        /// </summary>
        public bool UseWebGL { get; set; } = true;
        
        /// <summary>
        /// 是否启用3D图表支持
        /// </summary>
        public bool Enable3DCharts { get; set; } = true;
        
        /// <summary>
        /// 最大数据点数
        /// </summary>
        public int MaxDataPoints { get; set; } = 10000;
    }

public record EChartsRenderJob(
    string ChartId, 
    string OptionJson, 
    TaskCompletionSource<byte[]> CompletionSource,
    string ThemeName = "default",
    bool IsDynamicData = false);

public class EChartsInstancePooledObjectPolicy : IPooledObjectPolicy<EChartsInstance>
{
    private readonly IOptions<EChartsOptions> _options;
    
    public EChartsInstancePooledObjectPolicy(IOptions<EChartsOptions> options)
    {
        _options = options;
    }
    
    public EChartsInstance Create() 
    {
        var instance = new EChartsInstance();
        
        // 初始化时加载主题
        if(File.Exists(_options.Value.ThemePath))
        {
            var themeJson = File.ReadAllText(_options.Value.ThemePath);
            instance.LoadTheme(themeJson);
        }
        
        // 启用WebGL加速
        if(_options.Value.UseWebGL)
        {
            instance.EnableWebGL();
        }
        
        return instance;
    }
    
    public bool Return(EChartsInstance obj)
    {
        obj.Reset();
        return true;
    }
}

/// <summary>
    /// 2D/3D图表示例实现
    /// </summary>
    public static class EChartsExamples
    {
        /// <summary>
        /// 创建2D柱状图
        /// </summary>
        public static string CreateBarChart(string title, List<string> categories, List<double> values)
        {
            return $"""
            {{
                "title": {{"text": "{title}"}},
                "tooltip": {{}},
                "legend": {{"data": ["销量"]}},
                "xAxis": {{"data": {JsonSerializer.Serialize(categories)}}},
                "yAxis": {{}},
                "series": [{{"name": "销量", "type": "bar", "data": {JsonSerializer.Serialize(values)}}}]
            }}
            """;
        }

        /// <summary>
        /// 创建3D柱状图
        /// </summary>
        public static string Create3DBarChart(string title, List<string> categories, List<double> values)
        {
            return $"""
            {{
                "title": {{"text": "{title}"}},
                "tooltip": {{}},
                "grid3D": {{}},
                "xAxis3D": {{"type": "category", "data": {JsonSerializer.Serialize(categories)}}},
                "yAxis3D": {{}},
                "zAxis3D": {{}},
                "series": [{{"type": "bar3D", "data": {JsonSerializer.Serialize(values.Select((v,i) => new {{value = v, itemStyle = new {{ color: getColorByIndex(i) }} }}))}}}]
            }}
            """;
        }

        /// <summary>
        /// 创建2D饼图
        /// </summary>
        public static string CreatePieChart(string title, List<string> names, List<double> values)
        {
            return $"""
            {{
                "title": {{"text": "{title}"}},
                "tooltip": {{"trigger": "item"}},
                "legend": {{"orient": "vertical", "left": "left"}},
                "series": [
                    {{
                        "name": "访问来源",
                        "type": "pie",
                        "radius": "50%",
                        "data": {JsonSerializer.Serialize(names.Zip(values, (n,v) => new {{ value = v, name = n }}))},
                        "emphasis": {{
                            "itemStyle": {{
                                "shadowBlur": 10,
                                "shadowOffsetX": 0,
                                "shadowColor": "rgba(0, 0, 0, 0.5)"
                            }}
                        }}
                    }}
                ]
            }}
            """;
        }

        /// <summary>
        /// 创建3D饼图
        /// </summary>
        public static string Create3DPieChart(string title, List<string> names, List<double> values)
        {
            return $"""
            {{
                "title": {{"text": "{title}"}},
                "tooltip": {{"trigger": "item"}},
                "visualMap": {{
                    "min": 0,
                    "max": {values.Max()},
                    "inRange": {{
                        "color": ["#313695", "#4575b4", "#74add1", "#abd9e9", "#e0f3f8", "#ffffbf", "#fee090", "#fdae61", "#f46d43", "#d73027", "#a50026"]
                    }}
                }},
                "series": [
                    {{
                        "name": "访问来源",
                        "type": "pie3D",
                        "data": {JsonSerializer.Serialize(names.Zip(values, (n,v) => new {{ value = v, name = n }}))},
                        "roseType": "radius",
                        "label": {{
                            "show": true,
                            "formatter": "{b}: {c}"
                        }},
                        "labelLine": {{
                            "show": true
                        }},
                        "itemStyle": {{
                            "borderRadius": 5,
                            "borderWidth": 2
                        }}
                    }}
                ]
            }}
            """;
        }

        /// <summary>
        /// 创建2D折线图
        /// </summary>
        public static string CreateLineChart(string title, List<string> categories, List<double> values)
        {
            return $"""
            {{
                "title": {{"text": "{title}"}},
                "tooltip": {{"trigger": "axis"}},
                "legend": {{"data": ["销量"]}},
                "xAxis": {{"type": "category", "boundaryGap": false, "data": {JsonSerializer.Serialize(categories)}}},
                "yAxis": {{"type": "value"}},
                "series": [{{"name": "销量", "type": "line", "data": {JsonSerializer.Serialize(values)}, "smooth": true}}]
            }}
            """;
        }

        /// <summary>
        /// 创建3D折线图
        /// </summary>
        public static string Create3DLineChart(string title, List<string> categories, List<double> values)
        {
            return $"""
            {{
                "title": {{"text": "{title}"}},
                "tooltip": {{"trigger": "axis"}},
                "grid3D": {{}},
                "xAxis3D": {{"type": "category", "data": {JsonSerializer.Serialize(categories)}}},
                "yAxis3D": {{"type": "value"}},
                "zAxis3D": {{"type": "value"}},
                "series": [{{"type": "line3D", "data": {JsonSerializer.Serialize(values.Select((v,i) => new {{value = v, itemStyle = new {{ color: getColorByIndex(i) }} }}))}, "smooth": true}}]
            }}
            """;
        }

        private static string getColorByIndex(int index)
        {
            var colors = new[] { "#c23531", "#2f4554", "#61a0a8", "#d48265", "#91c7ae", "#749f83", "#ca8622" };
            return colors[index % colors.Length];
        }
    }

    public class EChartsRenderService : BackgroundService
    {
    private readonly Channel<EChartsRenderJob> _renderChannel;
    private readonly ObjectPool<EChartsInstance> _instancePool;
    private readonly TailLatencyOptimizedStatistics _statistics;
    private readonly IOptions<EChartsOptions> _options;
    private readonly ThreadLocal<ArrayPool<byte>> _threadLocalBuffer;
    
    public EChartsRenderService(
        Channel<EChartsRenderJob> renderChannel,
        ObjectPool<EChartsInstance> instancePool,
        TailLatencyOptimizedStatistics statistics,
        IOptions<EChartsOptions> options,
        ThreadLocal<ArrayPool<byte>> threadLocalBuffer)
    {
        _renderChannel = renderChannel;
        _instancePool = instancePool;
        _statistics = statistics;
        _options = options;
        _threadLocalBuffer = threadLocalBuffer;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    var parallelOptions = new ParallelOptions
    {
        MaxDegreeOfParallelism = _options.Value.MaxParallelRenders,
        CancellationToken = stoppingToken
    };
    
    // 动态数据更新服务
    var dynamicDataService = new DynamicDataService(_options.Value.DataUpdateInterval);
    
    await Parallel.ForEachAsync(_renderChannel.Reader.ReadAllAsync(stoppingToken), 
        parallelOptions, 
        async (job, ct) =>
        {
            var instance = _instancePool.Get();
            try
            {
                var buffer = _threadLocalBuffer.Value.Rent(1024 * 1024);
                try
                {
                    // 应用主题
                    if(!string.IsNullOrEmpty(job.ThemeName))
                    {
                        await instance.ApplyThemeAsync(job.ThemeName, ct);
                    }
                    
                    // 处理动态数据
                    string optionJson = job.OptionJson;
                    if(job.IsDynamicData)
                    {
                        optionJson = await dynamicDataService.UpdateDataAsync(optionJson, ct);
                    }
                    
                    var sw = ValueStopwatch.StartNew();
                    var result = await instance.RenderAsync(optionJson, buffer, ct);
                    _statistics.RecordLatency(sw.GetElapsedTime().TotalMilliseconds);
                    job.CompletionSource.SetResult(result);
                }
                finally
                {
                    _threadLocalBuffer.Value.Return(buffer);
                }
            }
            finally
            {
                _instancePool.Return(instance);
            }
        });
}

/// <summary>
/// 动态数据服务，用于定时更新图表数据
/// </summary>
private class DynamicDataService
{
    private readonly int _updateInterval;
    private readonly Random _random = new();
    
    public DynamicDataService(int updateInterval)
    {
        _updateInterval = updateInterval;
    }
    
    public async Task<string> UpdateDataAsync(string optionJson, CancellationToken ct)
    {
        await Task.Delay(_updateInterval, ct);
        
        // 模拟数据更新 - 生产环境应替换为真实数据源
        var data = JsonSerializer.Deserialize<JsonDocument>(optionJson);
        if(data.RootElement.TryGetProperty("series", out var series))
        {
            foreach(var item in series.EnumerateArray())
            {
                if(item.TryGetProperty("data", out var dataProp))
                {
                    // 随机更新数据点
                    var newData = dataProp.EnumerateArray().Select(x => 
                        x.ValueKind == JsonValueKind.Number ? 
                        x.GetDouble() + _random.NextDouble() - 0.5 : 
                        x.Clone()).ToArray();
                    
                    // 这里简化处理，实际生产环境应使用更安全的JSON修改方式
                    optionJson = optionJson.Replace(
                        dataProp.ToString(), 
                        JsonSerializer.Serialize(newData));
                }
            }
        }
        
        return optionJson;
    }
}
}