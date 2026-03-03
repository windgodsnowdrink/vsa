#:sdk Microsoft.NET.Sdk.Web
#:package Highsoft.Highcharts@9.3.2
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;

namespace HighchartsIntegration
{
    /// <summary>
    /// Highcharts配置选项
    /// </summary>
    public class HighchartsOptions
    {
        /// <summary>
        /// 最大并行渲染数
        /// </summary>
        public int MaxParallelRenders { get; set; } = Environment.ProcessorCount;
        
        /// <summary>
        /// 是否启用WebGL加速渲染
        /// </summary>
        public bool UseWebGL { get; set; } = true;
        
        /// <summary>
        /// 动态数据更新间隔(毫秒)
        /// </summary>
        public int DataUpdateInterval { get; set; } = 1000;
        
        /// <summary>
        /// 是否启用3D图表支持
        /// </summary>
        public bool Enable3D { get; set; } = true;
        
        /// <summary>
        /// 主题配置文件路径
        /// </summary>
        public string ThemePath { get; set; } = "themes/"
    }

    /// <summary>
    /// Highcharts渲染作业
    /// </summary>
    public record HighchartsRenderJob(
        string ChartType,
        string Title,
        string JsonData,
        string ThemeName = "default",
        bool IsDynamicData = false
    )
    {
        /// <summary>
        /// 图表配置JSON
        /// </summary>
        public string ChartConfig => JsonData;
    };

    /// <summary>
    /// Highcharts实例对象池策略
    /// </summary>
    public class HighchartsInstancePooledObjectPolicy : IPooledObjectPolicy<Highsoft.Highcharts.Highcharts>
    {
        private readonly HighchartsOptions _options;
        
        public HighchartsInstancePooledObjectPolicy(IOptions<HighchartsOptions> options)
        {
            _options = options.Value;
        }
        
        public Highsoft.Highcharts.Highcharts Create()
        {
            var instance = new Highsoft.Highcharts.Highcharts("chart");
            
            if (_options.UseWebGL)
            {
                instance.SetOption("chart.zoomType", "xy");
                instance.SetOption("chart.renderTo", "container");
            }
            
            return instance;
        }
        
        public bool Return(Highsoft.Highcharts.Highcharts obj)
        {
            obj.Dispose();
            return true;
        }
    }

    /// <summary>
    /// Highcharts渲染服务
    /// </summary>
    public class HighchartsRenderService : BackgroundService
    {
        private readonly Channel<HighchartsRenderJob> _renderChannel;
        private readonly ObjectPool<Highsoft.Highcharts.Highcharts> _instancePool;
        private readonly HighchartsOptions _options;
        private readonly ILogger<HighchartsRenderService> _logger;
        
        public HighchartsRenderService(
            Channel<HighchartsRenderJob> renderChannel,
            ObjectPool<Highsoft.Highcharts.Highcharts> instancePool,
            IOptions<HighchartsOptions> options,
            ILogger<HighchartsRenderService> logger)
        {
            _renderChannel = renderChannel;
            _instancePool = instancePool;
            _options = options.Value;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var job in _renderChannel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using var instance = _instancePool.Get();
                    var result = RenderChart(instance, job);
                    // 处理渲染结果...
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Highcharts渲染失败");
                }
            }
        }
        
        private string RenderChart(Highsoft.Highcharts.Highcharts instance, HighchartsRenderJob job)
        {
            // 实现具体图表渲染逻辑
            return instance.ToJson();
        }
    }

    /// <summary>
    /// Highcharts服务扩展
    /// </summary>
    public static class HighchartsServiceExtensions
    {
        /// <summary>
        /// 添加Highcharts服务
        /// </summary>
        public static IServiceCollection AddHighchartsServices(this IServiceCollection services, Action<HighchartsOptions> configure = null)
        {
            services.Configure(configure ?? (opts => { }));
            
            services.AddSingleton<ObjectPool<Highsoft.Highcharts.Highcharts>>(sp =>
            {
                var policy = sp.GetRequiredService<HighchartsInstancePooledObjectPolicy>();
                return new DefaultObjectPool<Highsoft.Highcharts.Highcharts>(policy, Environment.ProcessorCount * 2);
            });
            
            services.AddSingleton<HighchartsInstancePooledObjectPolicy>();
            services.AddSingleton(Channel.CreateUnbounded<HighchartsRenderJob>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            }));
            
            services.AddHostedService<HighchartsRenderService>();
            
            return services;
        }
    }

    /// <summary>
    /// Highcharts图表示例
    /// </summary>
    public static class HighchartsExamples
    {
        /// <summary>
        /// 创建2D柱状图配置
        /// </summary>
        public static string CreateBarChart(string title, List<string> categories, List<double> values)
        {
            return $"""
            {{
                "chart": {{"type": "bar"}},
                "title": {{"text": "{title}"}},
                "xAxis": {{"categories": {JsonSerializer.Serialize(categories)}}},
                "series": [{{"name": "数据", "data": {JsonSerializer.Serialize(values)}}}]
            }}
            """;
        }
        
        /// <summary>
        /// 创建3D柱状图配置
        /// </summary>
        public static string Create3DBarChart(string title, List<string> categories, List<double> values)
        {
            return $"""
            {{
                "chart": {{"type": "column", "options3d": {{"enabled": true, "alpha": 15, "beta": 15, "depth": 50}}}},
                "title": {{"text": "{title}"}},
                "xAxis": {{"categories": {JsonSerializer.Serialize(categories)}}},
                "series": [{{"name": "数据", "data": {JsonSerializer.Serialize(values)}}}]
            }}
            """;
        }
        
        /// <summary>
        /// 创建动态数据折线图配置
        /// </summary>
        public static string CreateDynamicLineChart(string title, List<string> categories, List<double> values)
        {
            return $"""
            {{
                "chart": {{"type": "line"}},
                "title": {{"text": "{title}"}},
                "plotOptions": {{
                    "series": {{
                        "animation": false,
                        "marker": {{ "enabled": false }}
                    }}
                }},
                "xAxis": {{"categories": {JsonSerializer.Serialize(categories)}}},
                "series": [{{
                    "data": {JsonSerializer.Serialize(values)},
                    "dataLabels": {{ "enabled": true }}
                }}]
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
                "chart": {{"type": "column", "options3d": {{"enabled": true, "alpha": 15, "beta": 15, "depth": 50}}}},
                "title": {{"text": "{title}"}},
                "xAxis": {{"categories": {JsonSerializer.Serialize(categories)}}},
                "series": [{{"name": "数据", "data": {JsonSerializer.Serialize(values)}}}]
            }}
            """;
        }
        
        /// <summary>
        /// 创建饼图
        /// </summary>
        public static string CreatePieChart(string title, List<string> names, List<double> values)
        {
            return $"""
            {{
                "chart": {{"type": "pie"}},
                "title": {{"text": "{title}"}},
                "series": [{{
                    "name": "占比",
                    "data": {JsonSerializer.Serialize(names.Zip(values, (n,v) => new {{ name = n, y = v }}))}
                }}]
            }}
            """;
        }
        
        /// <summary>
        /// 创建折线图
        /// </summary>
        public static string CreateLineChart(string title, List<string> categories, List<double> values)
        {
            return $"""
            {{
                "chart": {{"type": "line"}},
                "title": {{"text": "{title}"}},
                "xAxis": {{"categories": {JsonSerializer.Serialize(categories)}}},
                "series": [{{"name": "趋势", "data": {JsonSerializer.Serialize(values)}}}]
            }}
            """;
        }
    }
}