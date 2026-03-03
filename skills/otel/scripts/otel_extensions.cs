#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package OpenTelemetry@1.8.0
#:package OpenTelemetry.Exporter.Prometheus.Http@1.8.0
#:package OpenTelemetry.Exporter.Jaeger@1.8.0
#:package OpenTelemetry.Exporter.Zipkin@1.8.0
#:package OpenTelemetry.Extensions.Hosting@1.8.0
#:package OpenTelemetry.Instrumentation.Http@1.8.0
#:package OpenTelemetry.Instrumentation.AspNetCore@1.8.0
#:package System.Text.Json@10.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;

// 配置选项
public class OpenTelemetryOptions
{
    public bool Enabled { get; set; } = true;
    public string ServiceName { get; set; } = "OpenTelemetry-Service";
    public string ServiceVersion { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Development";
    public bool EnableTracing { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
    public bool EnableLogging { get; set; } = true;
    public bool EnableAlerting { get; set; } = true;
    public bool EnablePrometheusExporter { get; set; } = true;
    public bool EnableJaegerExporter { get; set; } = true;
    public bool EnableZipkinExporter { get; set; } = false;
    public string PrometheusEndpoint { get; set; } = "/metrics";
    public string JaegerEndpoint { get; set; } = "http://localhost:14268/api/traces";
    public string ZipkinEndpoint { get; set; } = "http://localhost:9411/api/v2/spans";
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
}

// OpenTelemetry 配置
public class OpenTelemetryConfiguration
{
    public string ServiceName { get; set; } = "OpenTelemetry-Service";
    public string ServiceVersion { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Development";
    public bool EnableTracing { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
    public bool EnableLogging { get; set; } = true;
    public bool EnableAlerting { get; set; } = true;
    public bool EnablePrometheusExporter { get; set; } = true;
    public bool EnableJaegerExporter { get; set; } = true;
    public bool EnableZipkinExporter { get; set; } = false;
    public string PrometheusEndpoint { get; set; } = "/metrics";
    public string JaegerEndpoint { get; set; } = "http://localhost:14268/api/traces";
    public string ZipkinEndpoint { get; set; } = "http://localhost:9411/api/v2/spans";
}

// 告警规则
public class AlertRule
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Severity { get; set; } = "Medium";
    public double Threshold { get; set; }
    public TimeSpan EvaluationPeriod { get; set; } = TimeSpan.FromMinutes(1);
    public List<string> NotificationChannels { get; set; } = new List<string>();
    public bool Enabled { get; set; } = true;
}

// 告警
public class Alert
{
    public string Id { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "Medium";
    public double Value { get; set; }
    public double Threshold { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Firing";
    public Dictionary<string, object> Labels { get; set; } = new Dictionary<string, object>();
}

// OpenTelemetry 服务接口
public interface IOpenTelemetryService
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task ShutdownAsync(CancellationToken cancellationToken = default);
    Task<OpenTelemetryConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateConfigurationAsync(OpenTelemetryConfiguration configuration, CancellationToken cancellationToken = default);
}

// OpenTelemetry 追踪服务接口
public interface IOpenTelemetryTracingService
{
    Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal);
    Task<T> TraceAsync<T>(string operationName, Func<Task<T>> operation, Dictionary<string, object>? tags = null, CancellationToken cancellationToken = default);
    Task TraceAsync(string operationName, Func<Task> operation, Dictionary<string, object>? tags = null, CancellationToken cancellationToken = default);
    void AddActivityTag(Activity activity, string key, object value);
    void AddActivityEvent(Activity activity, string name, Dictionary<string, object>? tags = null);
    void RecordException(Activity activity, Exception exception);
}

// OpenTelemetry 指标服务接口
public interface IOpenTelemetryMetricsService
{
    void IncrementCounter(string name, double value = 1, Dictionary<string, object>? tags = null);
    void RecordGauge(string name, double value, Dictionary<string, object>? tags = null);
    void RecordHistogram(string name, double value, Dictionary<string, object>? tags = null);
    void RecordObservableGauge(string name, Func<double> observer, Dictionary<string, object>? tags = null);
    void RecordObservableCounter(string name, Func<double> observer, Dictionary<string, object>? tags = null);
    void RecordObservableUpDownCounter(string name, Func<double> observer, Dictionary<string, object>? tags = null);
}

// OpenTelemetry 日志服务接口
public interface IOpenTelemetryLoggingService
{
    void LogTrace(string message, Dictionary<string, object>? tags = null);
    void LogDebug(string message, Dictionary<string, object>? tags = null);
    void LogInformation(string message, Dictionary<string, object>? tags = null);
    void LogWarning(string message, Dictionary<string, object>? tags = null);
    void LogError(string message, Exception? exception = null, Dictionary<string, object>? tags = null);
    void LogCritical(string message, Exception? exception = null, Dictionary<string, object>? tags = null);
}

// OpenTelemetry 告警服务接口
public interface IOpenTelemetryAlertingService
{
    Task CreateAlertRuleAsync(AlertRule rule, CancellationToken cancellationToken = default);
    Task<bool> UpdateAlertRuleAsync(string ruleName, AlertRule rule, CancellationToken cancellationToken = default);
    Task<bool> DeleteAlertRuleAsync(string ruleName, CancellationToken cancellationToken = default);
    Task<IEnumerable<AlertRule>> GetAlertRulesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> GetAlertsAsync(CancellationToken cancellationToken = default);
    Task<bool> ResolveAlertAsync(string alertId, CancellationToken cancellationToken = default);
}

// OpenTelemetry 服务实现
public class OpenTelemetryService : IOpenTelemetryService
{
    private readonly ILogger<OpenTelemetryService> _logger;
    private readonly OpenTelemetryOptions _options;
    private bool _isInitialized = false;
    private TracerProvider? _tracerProvider;
    private MeterProvider? _meterProvider;
    private LoggerProvider? _loggerProvider;

    public OpenTelemetryService(ILogger<OpenTelemetryService> logger, IOptions<OpenTelemetryOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing OpenTelemetry service");
        await Task.Delay(1000, cancellationToken); // 模拟初始化过程

        // 初始化追踪
        if (_options.EnableTracing)
        {
            var tracerBuilder = Sdk.CreateTracerProviderBuilder()
                .AddSource("OpenTelemetry-Service")
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(_options.ServiceName, _options.ServiceVersion).AddAttributes(new Dictionary<string, object> { { "environment", _options.Environment } }));

            if (_options.EnableJaegerExporter)
            {
                tracerBuilder.AddJaegerExporter(options =>
                {
                    options.Endpoint = new Uri(_options.JaegerEndpoint);
                });
            }

            if (_options.EnableZipkinExporter)
            {
                tracerBuilder.AddZipkinExporter(options =>
                {
                    options.Endpoint = new Uri(_options.ZipkinEndpoint);
                });
            }

            _tracerProvider = tracerBuilder.Build();
        }

        // 初始化指标
        if (_options.EnableMetrics)
        {
            var meterBuilder = Sdk.CreateMeterProviderBuilder()
                .AddMeter("OpenTelemetry-Service")
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(_options.ServiceName, _options.ServiceVersion).AddAttributes(new Dictionary<string, object> { { "environment", _options.Environment } }));

            if (_options.EnablePrometheusExporter)
            {
                meterBuilder.AddPrometheusHttpListener(options =>
                {
                    options.UriPrefixes = new string[] { "http://localhost:9184/" };
                });
            }

            _meterProvider = meterBuilder.Build();
        }

        // 初始化日志
        if (_options.EnableLogging)
        {
            var loggerBuilder = Sdk.CreateLoggerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(_options.ServiceName, _options.ServiceVersion).AddAttributes(new Dictionary<string, object> { { "environment", _options.Environment } }));

            _loggerProvider = loggerBuilder.Build();
        }

        _isInitialized = true;
        _logger.LogInformation("OpenTelemetry service initialized successfully");
    }

    public async Task ShutdownAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Shutting down OpenTelemetry service");
        await Task.Delay(500, cancellationToken); // 模拟关闭过程

        _tracerProvider?.Dispose();
        _meterProvider?.Dispose();
        _loggerProvider?.Dispose();

        _isInitialized = false;
        _logger.LogInformation("OpenTelemetry service shutdown successfully");
    }

    public async Task<OpenTelemetryConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting OpenTelemetry configuration");
        await Task.Delay(100, cancellationToken); // 模拟获取过程

        return new OpenTelemetryConfiguration
        {
            ServiceName = _options.ServiceName,
            ServiceVersion = _options.ServiceVersion,
            Environment = _options.Environment,
            EnableTracing = _options.EnableTracing,
            EnableMetrics = _options.EnableMetrics,
            EnableLogging = _options.EnableLogging,
            EnableAlerting = _options.EnableAlerting,
            EnablePrometheusExporter = _options.EnablePrometheusExporter,
            EnableJaegerExporter = _options.EnableJaegerExporter,
            EnableZipkinExporter = _options.EnableZipkinExporter,
            PrometheusEndpoint = _options.PrometheusEndpoint,
            JaegerEndpoint = _options.JaegerEndpoint,
            ZipkinEndpoint = _options.ZipkinEndpoint
        };
    }

    public async Task<bool> UpdateConfigurationAsync(OpenTelemetryConfiguration configuration, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating OpenTelemetry configuration");
        await Task.Delay(400, cancellationToken); // 模拟更新过程

        // 注意：在实际应用中，这里应该更新配置并重启相关组件
        _logger.LogInformation("OpenTelemetry configuration updated successfully");
        return true;
    }
}

// OpenTelemetry 追踪服务实现
public class OpenTelemetryTracingService : IOpenTelemetryTracingService
{
    private readonly ILogger<OpenTelemetryTracingService> _logger;
    private readonly ActivitySource _activitySource;

    public OpenTelemetryTracingService(ILogger<OpenTelemetryTracingService> logger)
    {
        _logger = logger;
        _activitySource = new ActivitySource("OpenTelemetry-Service");
    }

    public Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        return _activitySource.StartActivity(name, kind);
    }

    public async Task<T> TraceAsync<T>(string operationName, Func<Task<T>> operation, Dictionary<string, object>? tags = null, CancellationToken cancellationToken = default)
    {
        using var activity = StartActivity(operationName);
        try
        {
            if (tags != null)
            {
                foreach (var (key, value) in tags)
                {
                    AddActivityTag(activity!, key, value);
                }
            }

            var result = await operation();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            RecordException(activity!, ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }
    }

    public async Task TraceAsync(string operationName, Func<Task> operation, Dictionary<string, object>? tags = null, CancellationToken cancellationToken = default)
    {
        using var activity = StartActivity(operationName);
        try
        {
            if (tags != null)
            {
                foreach (var (key, value) in tags)
                {
                    AddActivityTag(activity!, key, value);
                }
            }

            await operation();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            RecordException(activity!, ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }
    }

    public void AddActivityTag(Activity activity, string key, object value)
    {
        if (activity != null)
        {
            activity.SetTag(key, value);
        }
    }

    public void AddActivityEvent(Activity activity, string name, Dictionary<string, object>? tags = null)
    {
        if (activity != null)
        {
            var activityTags = tags?.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)).ToList();
            activity.AddEvent(new ActivityEvent(name, DateTime.UtcNow, activityTags != null ? new ActivityTagsCollection(activityTags) : null));
        }
    }

    public void RecordException(Activity activity, Exception exception)
    {
        if (activity != null)
        {
            activity.RecordException(exception);
        }
    }
}

// OpenTelemetry 指标服务实现
public class OpenTelemetryMetricsService : IOpenTelemetryMetricsService
{
    private readonly ILogger<OpenTelemetryMetricsService> _logger;
    private readonly Meter _meter;
    private readonly Dictionary<string, Counter<double>> _counters = new Dictionary<string, Counter<double>>();
    private readonly Dictionary<string, Histogram<double>> _histograms = new Dictionary<string, Histogram<double>>();
    private readonly object _syncLock = new object();

    public OpenTelemetryMetricsService(ILogger<OpenTelemetryMetricsService> logger)
    {
        _logger = logger;
        _meter = new Meter("OpenTelemetry-Service");
    }

    public void IncrementCounter(string name, double value = 1, Dictionary<string, object>? tags = null)
    {
        Counter<double> counter;
        lock (_syncLock)
        {
            if (!_counters.TryGetValue(name, out counter))
            {
                counter = _meter.CreateCounter<double>(name);
                _counters[name] = counter;
            }
        }

        var tagsCollection = ConvertTagsToCollection(tags);
        counter.Add(value, tagsCollection);
    }

    public void RecordGauge(string name, double value, Dictionary<string, object>? tags = null)
    {
        // 注意：OpenTelemetry 中的 Gauge 通常是通过 ObservableGauge 实现的
        // 这里使用一个简单的方法来模拟
        _logger.LogDebug("Recording gauge: {Name} = {Value}", name, value);
    }

    public void RecordHistogram(string name, double value, Dictionary<string, object>? tags = null)
    {
        Histogram<double> histogram;
        lock (_syncLock)
        {
            if (!_histograms.TryGetValue(name, out histogram))
            {
                histogram = _meter.CreateHistogram<double>(name);
                _histograms[name] = histogram;
            }
        }

        var tagsCollection = ConvertTagsToCollection(tags);
        histogram.Record(value, tagsCollection);
    }

    public void RecordObservableGauge(string name, Func<double> observer, Dictionary<string, object>? tags = null)
    {
        var tagsCollection = ConvertTagsToCollection(tags);
        _meter.CreateObservableGauge(name, observer, tagsCollection);
    }

    public void RecordObservableCounter(string name, Func<double> observer, Dictionary<string, object>? tags = null)
    {
        var tagsCollection = ConvertTagsToCollection(tags);
        _meter.CreateObservableCounter(name, observer, tagsCollection);
    }

    public void RecordObservableUpDownCounter(string name, Func<double> observer, Dictionary<string, object>? tags = null)
    {
        var tagsCollection = ConvertTagsToCollection(tags);
        _meter.CreateObservableUpDownCounter(name, observer, tagsCollection);
    }

    private KeyValuePair<string, object>[]? ConvertTagsToCollection(Dictionary<string, object>? tags)
    {
        return tags?.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)).ToArray();
    }
}

// OpenTelemetry 日志服务实现
public class OpenTelemetryLoggingService : IOpenTelemetryLoggingService
{
    private readonly ILogger<OpenTelemetryLoggingService> _logger;

    public OpenTelemetryLoggingService(ILogger<OpenTelemetryLoggingService> logger)
    {
        _logger = logger;
    }

    public void LogTrace(string message, Dictionary<string, object>? tags = null)
    {
        _logger.LogTrace(FormatMessageWithTags(message, tags));
    }

    public void LogDebug(string message, Dictionary<string, object>? tags = null)
    {
        _logger.LogDebug(FormatMessageWithTags(message, tags));
    }

    public void LogInformation(string message, Dictionary<string, object>? tags = null)
    {
        _logger.LogInformation(FormatMessageWithTags(message, tags));
    }

    public void LogWarning(string message, Dictionary<string, object>? tags = null)
    {
        _logger.LogWarning(FormatMessageWithTags(message, tags));
    }

    public void LogError(string message, Exception? exception = null, Dictionary<string, object>? tags = null)
    {
        _logger.LogError(exception, FormatMessageWithTags(message, tags));
    }

    public void LogCritical(string message, Exception? exception = null, Dictionary<string, object>? tags = null)
    {
        _logger.LogCritical(exception, FormatMessageWithTags(message, tags));
    }

    private string FormatMessageWithTags(string message, Dictionary<string, object>? tags)
    {
        if (tags == null || tags.Count == 0)
        {
            return message;
        }

        var tagsString = string.Join(", ", tags.Select(kv => $"{kv.Key}={kv.Value}"));
        return $"{message} [Tags: {tagsString}]";
    }
}

// OpenTelemetry 告警服务实现
public class OpenTelemetryAlertingService : IOpenTelemetryAlertingService
{
    private readonly ILogger<OpenTelemetryAlertingService> _logger;
    private readonly List<AlertRule> _alertRules = new List<AlertRule>();
    private readonly List<Alert> _alerts = new List<Alert>();
    private readonly object _syncLock = new object();

    public OpenTelemetryAlertingService(ILogger<OpenTelemetryAlertingService> logger)
    {
        _logger = logger;
    }

    public async Task CreateAlertRuleAsync(AlertRule rule, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating alert rule: {RuleName}", rule.Name);
        await Task.Delay(200, cancellationToken); // 模拟创建过程

        lock (_syncLock)
        {
            _alertRules.Add(rule);
        }

        _logger.LogInformation("Alert rule created successfully: {RuleName}", rule.Name);
    }

    public async Task<bool> UpdateAlertRuleAsync(string ruleName, AlertRule rule, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating alert rule: {RuleName}", ruleName);
        await Task.Delay(300, cancellationToken); // 模拟更新过程

        lock (_syncLock)
        {
            var existingRule = _alertRules.FirstOrDefault(r => r.Name == ruleName);
            if (existingRule != null)
            {
                _alertRules.Remove(existingRule);
                _alertRules.Add(rule);
                _logger.LogInformation("Alert rule updated successfully: {RuleName}", ruleName);
                return true;
            }
        }

        _logger.LogWarning("Alert rule not found: {RuleName}", ruleName);
        return false;
    }

    public async Task<bool> DeleteAlertRuleAsync(string ruleName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting alert rule: {RuleName}", ruleName);
        await Task.Delay(200, cancellationToken); // 模拟删除过程

        lock (_syncLock)
        {
            var existingRule = _alertRules.FirstOrDefault(r => r.Name == ruleName);
            if (existingRule != null)
            {
                _alertRules.Remove(existingRule);
                _logger.LogInformation("Alert rule deleted successfully: {RuleName}", ruleName);
                return true;
            }
        }

        _logger.LogWarning("Alert rule not found: {RuleName}", ruleName);
        return false;
    }

    public async Task<IEnumerable<AlertRule>> GetAlertRulesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting alert rules");
        await Task.Delay(100, cancellationToken); // 模拟获取过程

        lock (_syncLock)
        {
            return _alertRules.ToList();
        }
    }

    public async Task<IEnumerable<Alert>> GetAlertsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting alerts");
        await Task.Delay(100, cancellationToken); // 模拟获取过程

        lock (_syncLock)
        {
            return _alerts.ToList();
        }
    }

    public async Task<bool> ResolveAlertAsync(string alertId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resolving alert: {AlertId}", alertId);
        await Task.Delay(200, cancellationToken); // 模拟解决过程

        lock (_syncLock)
        {
            var alert = _alerts.FirstOrDefault(a => a.Id == alertId);
            if (alert != null)
            {
                alert.Status = "Resolved";
                _logger.LogInformation("Alert resolved successfully: {AlertId}", alertId);
                return true;
            }
        }

        _logger.LogWarning("Alert not found: {AlertId}", alertId);
        return false;
    }
}

// 依赖注入扩展
public static class OpenTelemetryServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetryServices(this IServiceCollection services, Action<OpenTelemetryOptions> configureOptions = null)
    {
        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OpenTelemetryOptions>(options => { });
        }

        // 注册服务
        services.AddSingleton<IOpenTelemetryService, OpenTelemetryService>();
        services.AddSingleton<IOpenTelemetryTracingService, OpenTelemetryTracingService>();
        services.AddSingleton<IOpenTelemetryMetricsService, OpenTelemetryMetricsService>();
        services.AddSingleton<IOpenTelemetryLoggingService, OpenTelemetryLoggingService>();
        services.AddSingleton<IOpenTelemetryAlertingService, OpenTelemetryAlertingService>();

        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OpenTelemetry 技能示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OpenTelemetry 服务
        services.AddOpenTelemetryServices(options =>
        {
            options.Enabled = true;
            options.ServiceName = "OpenTelemetry-Example";
            options.ServiceVersion = "1.0.0";
            options.Environment = "Development";
            options.EnableTracing = true;
            options.EnableMetrics = true;
            options.EnableLogging = true;
            options.EnableAlerting = true;
            options.EnablePrometheusExporter = true;
            options.EnableJaegerExporter = true;
            options.EnableZipkinExporter = false;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var otelService = serviceProvider.GetRequiredService<IOpenTelemetryService>();
        var tracingService = serviceProvider.GetRequiredService<IOpenTelemetryTracingService>();
        var metricsService = serviceProvider.GetRequiredService<IOpenTelemetryMetricsService>();
        var loggingService = serviceProvider.GetRequiredService<IOpenTelemetryLoggingService>();
        var alertingService = serviceProvider.GetRequiredService<IOpenTelemetryAlertingService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 初始化 OpenTelemetry
            Console.WriteLine("示例 1: 初始化 OpenTelemetry");
            await otelService.InitializeAsync();
            Console.WriteLine("OpenTelemetry 初始化成功");

            // 示例 2: 使用追踪服务
            Console.WriteLine("\n示例 2: 使用追踪服务");
            var result = await tracingService.TraceAsync<int>("CalculateSum", async () =>
            {
                await Task.Delay(100); // 模拟计算过程
                return 1 + 2;
            }, new Dictionary<string, object> { { "operation", "addition" }, { "numbers", "1+2" });
            Console.WriteLine($"计算结果: {result}");

            // 示例 3: 使用指标服务
            Console.WriteLine("\n示例 3: 使用指标服务");
            metricsService.IncrementCounter("user.operations.count", tags: new Dictionary<string, object> { { "operation", "login" } });
            metricsService.RecordGauge("system.memory.usage", value: 1024, tags: new Dictionary<string, object> { { "type", "used" } });
            metricsService.RecordHistogram("request.duration", value: 150, tags: new Dictionary<string, object> { { "endpoint", "/api/users" } });
            Console.WriteLine("指标记录成功");

            // 示例 4: 使用日志服务
            Console.WriteLine("\n示例 4: 使用日志服务");
            loggingService.LogInformation("User logged in successfully", new Dictionary<string, object> { { "userId", 1 }, { "username", "user1" } });
            Console.WriteLine("日志记录成功");

            // 示例 5: 使用告警服务
            Console.WriteLine("\n示例 5: 使用告警服务");
            var alertRule = new AlertRule
            {
                Name = "HighCPUUsage",
                Description = "CPU usage exceeds threshold",
                Condition = "system.cpu.usage > 80",
                Severity = "High",
                Threshold = 80,
                EvaluationPeriod = TimeSpan.FromMinutes(1),
                NotificationChannels = new List<string> { "email", "slack" }
            };
            await alertingService.CreateAlertRuleAsync(alertRule);
            Console.WriteLine("告警规则创建成功");

            // 示例 6: 获取配置
            Console.WriteLine("\n示例 6: 获取配置");
            var config = await otelService.GetConfigurationAsync();
            Console.WriteLine($"服务名称: {config.ServiceName}");
            Console.WriteLine($"服务版本: {config.ServiceVersion}");
            Console.WriteLine($"环境: {config.Environment}");
            Console.WriteLine($"启用追踪: {config.EnableTracing}");
            Console.WriteLine($"启用指标: {config.EnableMetrics}");
            Console.WriteLine($"启用日志: {config.EnableLogging}");

            Console.WriteLine("\n所有示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 关闭 OpenTelemetry
            await otelService.ShutdownAsync();
        }
    }
}
