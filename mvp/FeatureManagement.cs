#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package Microsoft.Extensions.Hosting@10.0.0-rc.1.25451.107
#:package System.Threading.Channels@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool.DependencyInjection@9.9.0
#:package Swashbuckle.AspNetCore@9.0.4
#:package Swashbuckle.AspNetCore.Swagger@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerGen@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerUI@9.0.4
#:package Scalar.AspNetCore@2.8.0
#:package Ardalis.ListStartupServices@1.1.4
#:package Serilog@4.3.0
#:package Serilog.AspNetCore@9.0.0
#:package Serilog.Enrichers.Environment@3.0.1
#:package Serilog.Enrichers.Process@3.0.0
#:package Serilog.Enrichers.Span@3.1.0
#:package Serilog.Enrichers.Thread@4.0.0
#:package Serilog.Exceptions@8.4.0
#:package Serilog.Extensions.Hosting@9.0.0
#:package Serilog.Extensions.Logging@9.0.2
#:package Serilog.Formatting.Compact@3.0.0
#:package Serilog.Settings.Configuration@9.0.0
#:package Serilog.Sinks.Async@2.1.0
#:package Serilog.Sinks.Console@6.0.0
#:package Serilog.Sinks.EventLog@4.0.0
#:package Serilog.Sinks.Http@9.2.0
#:package Serilog.Sinks.RollingFileAlternate@2.0.9
#:package Serilog.Sinks.Seq@9.0.0
#:package Serilog.Sinks.SpectreConsole@0.3.3
#:package Serilog.Sinks.Trace@4.0.0
#:package ModelContextProtocol@0.3.0-preview.4
#:package ModelContextProtocol.Core@0.3.0-preview.4
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=True
#:property Platform=Any CPU
#:property PackAsTool=True
#:property PackageType=McpServer
#:property PackageReadmeFile=README.md
#:property PackageId=AOT.SampleMcpServer
#:property PackageVersion=0.0.1-beta
#:property PackageTags=AI; MCP; server; stdio
#:property Description=An MCP server using the MCP C# SDK.

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Ardalis.ListStartupServices;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ModelContextProtocol.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

builder.Logging.ClearProviders();               // 只保留下面的 ConsoleProvider，避免干扰
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);                   // 使用颜色化的控制台日志
builder.Logging.SetMinimumLevel(LogLevel.Trace); // 设为 Trace，最细粒度
builder.Logging.AddFilter("Microsoft.Extensions.Http", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Extensions.ServiceDiscovery", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Extensions.Resilience", LogLevel.Trace);
builder.Logging.AddFilter("RestEase.HttpClientFactory", LogLevel.Trace);
builder.Logging.AddFilter("App.ServiceDiscoveryHandler", LogLevel.Trace);
Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "error", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Fatal)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "fatal", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Information)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "info", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Warning)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "warning", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog(Log.Logger, true).ConfigureLogging((context, logging) =>
{
    logging.ClearProviders();
    logging.AddConfiguration(context.Configuration);
    logging.AddSerilog(Log.Logger, true);
});
builder.Services.AddOptions();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination", "X-Platform-Type", "X-CSRF-TOKEN-HEADERNAME", "X-CSRF-TOKEN", "X-Forwarded-For", "X-Forwarded-Host", "X-Forwarded-Proto", "X-Forwarded-Prefix", "X-Body-Hash")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
// });

// 测试MCP
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<App.RandomNumberTools>();

builder.Services.Configure<Ardalis.ListStartupServices.ServiceConfig>(config =>
{
    config.Services = [.. builder.Services];
    config.Path = "/services";
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapScalarApiReference(); // scalar
}
else
{
    app.UseExceptionHandler("/Error");
}
app.UseSerilogRequestLogging(opt =>
{
    opt.IncludeQueryInRequestPath = true;
});
app.UseRouting();
app.UseCors();
// app.UseAuthorization();
// app.UseSwagger();
// app.UseSwaggerUI(options =>
// {
//     options.SwaggerEndpoint("/swagger/v1/swagger.json", "Robot API V1.1.0.0");
// });
app.UseShowAllServicesMiddleware();
app.MapGet("/", () => "Mcp Agent Integration!");
app.UseEndpoints(options =>
{
    options.MapControllers();
});

await app.RunAsync();

namespace App
{
    public partial class Program;

    // 1. 功能标志枚举 - 统一管理所有功能标志
    public enum AppFeatures
    {
        PremiumSupport,
        NewUserInterface,
        MultiLanguageSupport,
        AdvancedAnalytics,
        BetaTestFeature,
        EmergencyMaintenanceMode,
        PerformanceOptimization,
        ThirdPartyIntegration
    }

    // 2. 自定义功能过滤器 - 基于用户角色的功能启用
    [FilterAlias("UserRole")]
    public class UserRoleFeatureFilter : IFeatureFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserRoleFeatureFilter> _logger;

        public UserRoleFeatureFilter(
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserRoleFeatureFilter> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            try
            {
                // 获取配置的角色要求
                var requiredRoles = context.Settings.Get<string[]>("AllowedRoles");
                var allowedUsers = context.Settings.Get<string[]>("AllowedUsers");

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
                {
                    _logger.LogDebug("User not authenticated, feature disabled");
                    return false;
                }

                // 检查用户角色
                if (requiredRoles != null && requiredRoles.Any())
                {
                    var userRoles = httpContext.User.Claims
                        .Where(c => c.Type == "role")
                        .Select(c => c.Value)
                        .ToArray();

                    var hasRequiredRole = requiredRoles.Any(role =>
                        userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));

                    if (hasRequiredRole)
                    {
                        _logger.LogDebug("User has required role for feature");
                        return true;
                    }
                }

                // 检查特定用户
                if (allowedUsers != null && allowedUsers.Any())
                {
                    var userId = httpContext.User.Claims
                        .FirstOrDefault(c => c.Type == "sub")?.Value;

                    var isAllowedUser = allowedUsers.Any(user =>
                        user.Equals(userId, StringComparison.OrdinalIgnoreCase));

                    if (isAllowedUser)
                    {
                        _logger.LogDebug("User is in allowed list for feature");
                        return true;
                    }
                }

                _logger.LogDebug("User does not meet feature requirements");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating user role feature filter");
                return false; // 安全默认：禁用功能
            }
        }
    }

    // 3. 时间段功能过滤器 - 基于时间启用功能
    [FilterAlias("TimeWindow")]
    public class TimeWindowFeatureFilter : IFeatureFilter
    {
        private readonly ILogger<TimeWindowFeatureFilter> _logger;

        public TimeWindowFeatureFilter(ILogger<TimeWindowFeatureFilter> logger)
        {
            _logger = logger;
        }

        public async Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            try
            {
                var startTime = context.Settings.Get<DateTime?>("StartTime");
                var endTime = context.Settings.Get<DateTime?>("EndTime");
                var currentTime = DateTime.UtcNow;

                _logger.LogDebug("Evaluating time window feature - Current: {CurrentTime}, " +
                               "Start: {StartTime}, End: {EndTime}",
                               currentTime, startTime, endTime);

                // 检查开始时间
                if (startTime.HasValue && currentTime < startTime.Value)
                {
                    _logger.LogInformation("Feature disabled - Before start time");
                    return false;
                }

                // 检查结束时间
                if (endTime.HasValue && currentTime > endTime.Value)
                {
                    _logger.LogInformation("Feature disabled - After end time");
                    return false;
                }

                _logger.LogInformation("Feature enabled - Within time window");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating time window feature filter");
                return false;
            }
        }
    }

    // 4. 基于百分比的功能过滤器 - 渐进式功能发布
    [FilterAlias("Percentage")]
    public class PercentageFeatureFilter : IFeatureFilter
    {
        private readonly ILogger<PercentageFeatureFilter> _logger;
        private static readonly Random _random = new Random();

        public PercentageFeatureFilter(ILogger<PercentageFeatureFilter> logger)
        {
            _logger = logger;
        }

        public async Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            try
            {
                var percentage = context.Settings.Get<double>("Value");
                var salt = context.Settings.Get<string>("Salt") ?? "";

                if (percentage < 0 || percentage > 100)
                {
                    _logger.LogWarning("Invalid percentage value {Percentage} for feature", percentage);
                    return false;
                }

                // 简单的基于用户ID的百分比路由（实际实现可能需要更复杂的哈希算法）
                var userIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var userIdentifier = userIdentity?.Name ?? Guid.NewGuid().ToString();
                var hashValue = (userIdentifier + salt).GetHashCode() % 100;

                var isEnabled = hashValue >= 0 && hashValue < percentage;

                _logger.LogDebug("Percentage feature evaluation - User: {User}, " +
                               "Hash: {HashValue}, Percentage: {Percentage}, Enabled: {IsEnabled}",
                               userIdentifier, hashValue, percentage, isEnabled);

                return isEnabled;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating percentage feature filter");
                return false;
            }
        }
    }

    // 5. 环境功能过滤器 - 根据环境启用功能
    [FilterAlias("Environment")]
    public class EnvironmentFeatureFilter : IFeatureFilter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EnvironmentFeatureFilter> _logger;

        public EnvironmentFeatureFilter(
            IConfiguration configuration,
            ILogger<EnvironmentFeatureFilter> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            try
            {
                var allowedEnvironments = context.Settings.Get<string[]>("AllowedEnvironments");
                var configEnvironment = _configuration.GetValue<string>("Environment") ??
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                    "Development";

                _logger.LogDebug("Evaluating environment feature - Current: {Environment}, " +
                               "Allowed: {AllowedEnvironments}",
                               configEnvironment,
                               string.Join(",", allowedEnvironments ?? new string[0]));

                if (allowedEnvironments == null || !allowedEnvironments.Any())
                {
                    return true; // 没有环境限制时默认启用
                }

                return allowedEnvironments.Contains(configEnvironment, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating environment feature filter");
                return false;
            }
        }
    }

    // 6. 功能管理层服务 - 生产级功能管理包装器
    public class FeatureManagementService
    {
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<FeatureManagementService> _logger;
        private readonly ITelemetryService _telemetry;

        public FeatureManagementService(
            IFeatureManager featureManager,
            ILogger<FeatureManagementService> logger,
            ITelemetryService telemetry)
        {
            _featureManager = featureManager;
            _logger = logger;
            _telemetry = telemetry;
        }

        /// <summary>
        /// 安全检查功能是否启用，包含错误处理和遥测
        /// </summary>
        public async Task<bool> IsFeatureEnabledAsync(AppFeatures feature)
        {
            var featureName = feature.ToString();

            try
            {
                var isEnabled = await _featureManager.IsEnabledAsync(featureName);

                _logger.LogDebug("Feature {FeatureName} status checked: {IsEnabled}", featureName, isEnabled);

                // 发送遥测数据
                await _telemetry.TrackFeatureUsageAsync(featureName, isEnabled, "Check");

                return isEnabled;
            }
            catch (FeatureManagementException ex)
            {
                _logger.LogError(ex, "Feature management exception for {FeatureName}", featureName);

                await _telemetry.TrackFeatureErrorAsync(featureName, ex);

                // 安全默认：禁用功能
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error checking feature {FeatureName}", featureName);

                await _telemetry.TrackFeatureErrorAsync(featureName, ex);

                // 安全默认：禁用功能
                return false;
            }
        }

        /// <summary>
        /// 获取功能配置元数据
        /// </summary>
        public async Task<FeatureMetadata> GetFeatureMetadataAsync(AppFeatures feature)
        {
            var featureName = feature.ToString();

            return new FeatureMetadata
            {
                FeatureName = featureName,
                IsEnabled = await IsFeatureEnabledAsync(feature),
                Description = GetFeatureDescription(feature),
                IntroducedVersion = GetFeatureVersion(feature)
            };
        }

        /// <summary>
        /// 跟踪功能使用情况
        /// </summary>
        public async Task TrackFeatureUsageAsync(AppFeatures feature, FeatureUsageType usageType)
        {
            var featureName = feature.ToString();
            await _telemetry.TrackFeatureUsageAsync(featureName, true, usageType.ToString());
        }

        private string GetFeatureDescription(AppFeatures feature)
        {
            return feature switch
            {
                AppFeatures.PremiumSupport => "Premium customer support portal",
                AppFeatures.NewUserInterface => "New user interface redesign",
                AppFeatures.MultiLanguageSupport => "Multi-language support for the application",
                AppFeatures.AdvancedAnalytics => "Advanced analytics and reporting features",
                AppFeatures.BetaTestFeature => "Beta test features for early adopters",
                AppFeatures.EmergencyMaintenanceMode => "Emergency maintenance mode activation",
                AppFeatures.PerformanceOptimization => "Performance optimization features",
                AppFeatures.ThirdPartyIntegration => "Third-party service integration",
                _ => "Unknown feature"
            };
        }

        private string GetFeatureVersion(AppFeatures feature)
        {
            return feature switch
            {
                AppFeatures.PremiumSupport => "1.2.0",
                AppFeatures.NewUserInterface => "2.0.0",
                AppFeatures.MultiLanguageSupport => "1.1.0",
                AppFeatures.AdvancedAnalytics => "1.3.0",
                AppFeatures.BetaTestFeature => "1.0.0-beta",
                AppFeatures.EmergencyMaintenanceMode => "1.0.0",
                AppFeatures.PerformanceOptimization => "1.2.5",
                AppFeatures.ThirdPartyIntegration => "1.4.0",
                _ => "1.0.0"
            };
        }
    }

    // 7. 功能遥测服务
    public interface ITelemetryService
    {
        Task TrackFeatureUsageAsync(string featureName, bool isEnabled, string usageType);
        Task TrackFeatureErrorAsync(string featureName, Exception exception);
        Task TrackFeatureToggleAsync(string featureName, bool previousState, bool newState);
    }

    public class TelemetryService : ITelemetryService
    {
        private readonly ILogger<TelemetryService> _logger;

        public TelemetryService(ILogger<TelemetryService> logger)
        {
            _logger = logger;
        }

        public async Task TrackFeatureUsageAsync(string featureName, bool isEnabled, string usageType)
        {
            _logger.LogInformation("FEATURE_USAGE: {FeatureName}, Enabled: {IsEnabled}, Type: {UsageType}",
                featureName, isEnabled, usageType);

            // 在生产环境中这些数据应该发送到监控服务
            await Task.CompletedTask;
        }

        public async Task TrackFeatureErrorAsync(string featureName, Exception exception)
        {
            _logger.LogError(exception, "FEATURE_ERROR: {FeatureName}, Exception: {ExceptionType}",
                featureName, exception.GetType().Name);

            await Task.CompletedTask;
        }

        public async Task TrackFeatureToggleAsync(string featureName, bool previousState, bool newState)
        {
            _logger.LogWarning("FEATURE_TOGGLE: {FeatureName}, Previous: {PreviousState}, New: {NewState}",
                featureName, previousState, newState);

            await Task.CompletedTask;
        }
    }

    // 8. 功能元数据模型
    public class FeatureMetadata
    {
        public string FeatureName { get; set; }
        public bool IsEnabled { get; set; }
        public string Description { get; set; }
        public string IntroducedVersion { get; set; }
        public DateTime LastChecked { get; set; } = DateTime.UtcNow;
        public List<string> Dependencies { get; set; } = new List<string>();
    }

    public enum FeatureUsageType
    {
        Check,
        Activate,
        Error
    }

    // 9. 功能缓存服务 - 生产环境性能优化
    public class FeatureCachingService
    {
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<FeatureCachingService> _logger;
        private readonly ConcurrentDictionary<string, CachedFeatureResult> _featureCache =
            new ConcurrentDictionary<string, CachedFeatureResult>();

        private const int CACHE_DURATION_MINUTES = 5;
        private const int MAX_CACHE_ENTRIES = 1000;

        public FeatureCachingService(
            IFeatureManager featureManager,
            ILogger<FeatureCachingService> logger)
        {
            _featureManager = featureManager;
            _logger = logger;
        }

        /// <summary>
        /// 缓存功能状态检查结果
        /// </summary>
        public async Task<bool> IsFeatureEnabledWithCacheAsync(string feature)
        {
            try
            {
                // 检查缓存
                if (_featureCache.TryGetValue(feature, out var cachedResult))
                {
                    // 检查缓存是否过期
                    if (DateTime.UtcNow <= cachedResult.ExpiryTime)
                    {
                        _logger.LogDebug("Feature {FeatureName} state retrieved from cache", feature);
                        return cachedResult.IsEnabled;
                    }
                    else
                    {
                        // 缓存过期，移除
                        _featureCache.TryRemove(feature, out _);
                        _logger.LogDebug("Feature {FeatureName} cache expired", feature);
                    }
                }

                // 从远程服务获取状态
                var isEnabled = await _featureManager.IsEnabledAsync(feature);
                var expiryTime = DateTime.UtcNow.AddMinutes(CACHE_DURATION_MINUTES);

                // 更新缓存
                var cacheEntry = new CachedFeatureResult
                {
                    IsEnabled = isEnabled,
                    ExpiryTime = expiryTime,
                    LastUpdated = DateTime.UtcNow
                };

                _featureCache[feature] = cacheEntry;

                // 防止缓存无限增长
                if (_featureCache.Count > MAX_CACHE_ENTRIES)
                {
                    CleanupOldCacheEntries();
                }

                _logger.LogDebug("Feature {FeatureName} state cached with expiry at {ExpiryTime}",
                    feature, expiryTime);

                return isEnabled;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking feature {FeatureName} with cache", feature);

                // 缓存降级机制：当缓存服务可用但远程检查失败时返回缓存值
                if (_featureCache.TryGetValue(feature, out var cachedResult) &&
                    DateTime.UtcNow <= cachedResult.ExpiryTime.AddMinutes(30)) // 允许使用过期缓存
                {
                    _logger.LogWarning("Using stale cache data for feature {FeatureName} due to error", feature);
                    return cachedResult.IsEnabled;
                }

                // 安全默认降级
                _logger.LogWarning("Feature {FeatureName} check failed, returning false", feature);
                return false;
            }
        }

        private void CleanupOldCacheEntries()
        {
            var keysToRemove = _featureCache
                .Where(kvp => kvp.Value.ExpiryTime < DateTime.UtcNow)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                _featureCache.TryRemove(key, out _);
            }

            _logger.LogDebug("Cleaned up {Count} expired cache entries", keysToRemove.Count);
        }

        public void InvalidateCache(string feature)
        {
            _featureCache.TryRemove(feature, out _);
            _logger.LogDebug("Cache invalidated for feature {FeatureName}", feature);
        }

        public void ClearAllCache()
        {
            _featureCache.Clear();
            _logger.LogInformation("All feature cache cleared");
        }
    }

    public class CachedFeatureResult
    {
        public bool IsEnabled { get; set; }
        public DateTime ExpiryTime { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    // 10. 功能健康检查服务
    public class FeatureManagementHealthCheck : IHealthCheck
    {
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<FeatureManagementHealthCheck> _logger;

        public FeatureManagementHealthCheck(
            IFeatureManager featureManager,
            ILogger<FeatureManagementHealthCheck> logger)
        {
            _featureManager = featureManager;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // 检查关键功���的状态
                var criticalFeatures = new[]
                {
                nameof(AppFeatures.EmergencyMaintenanceMode),
                nameof(AppFeatures.PerformanceOptimization)
            };

                var featureStatuses = new Dictionary<string, bool>();

                foreach (var feature in criticalFeatures)
                {
                    try
                    {
                        var isEnabled = await _featureManager.IsEnabledAsync(feature);
                        featureStatuses[feature] = isEnabled;

                        _logger.LogDebug("Health check - Feature {FeatureName}: {IsEnabled}",
                            feature, isEnabled);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to check feature {FeatureName} during health check",
                            feature);
                        featureStatuses[feature] = false;
                    }
                }

                var allCriticalFeaturesWorking = featureStatuses.All(kvp => kvp.Value);
                var data = featureStatuses.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object)kvp.Value.ToString());

                if (allCriticalFeaturesWorking)
                {
                    return HealthCheckResult.Healthy("All critical features are properly configured", data);
                }
                else
                {
                    return HealthCheckResult.Degraded(
                        "Some critical features are misconfigured",
                        null,
                        data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feature management health check failed");
                return HealthCheckResult.Unhealthy(
                    "Feature management service is unavailable",
                    ex);
            }
        }
    }

    // 11. 功能监控和统计服务
    public interface IFeatureMonitoringService
    {
        void RecordFeatureAccess(string featureName, bool wasEnabled, bool wasAccessed);
        void RecordFeatureError(string featureName, Exception exception);
        FeatureUsageStatistics GetStatistics();
        void ResetStatistics();
    }

    public class FeatureMonitoringService : IFeatureMonitoringService
    {
        private readonly ILogger<FeatureMonitoringService> _logger;
        private readonly ConcurrentDictionary<string, FeatureUsageStats> _featureStats =
            new ConcurrentDictionary<string, FeatureUsageStats>();

        public FeatureMonitoringService(ILogger<FeatureMonitoringService> logger)
        {
            _logger = logger;
        }

        public void RecordFeatureAccess(string featureName, bool wasEnabled, bool wasAccessed)
        {
            var stats = _featureStats.GetOrAdd(featureName, _ => new FeatureUsageStats());

            stats.TotalAccesses++;

            if (wasEnabled && wasAccessed)
            {
                stats.EnabledAccesses++;
            }
            else if (!wasEnabled && wasAccessed)
            {
                stats.DisabledAccesses++;
                _logger.LogWarning("Attempted to access disabled feature: {FeatureName}", featureName);
            }

            _logger.LogDebug("Feature {FeatureName} access recorded - Enabled: {WasEnabled}, Accessed: {WasAccessed}",
                featureName, wasEnabled, wasAccessed);
        }

        public void RecordFeatureError(string featureName, Exception exception)
        {
            var stats = _featureStats.GetOrAdd(featureName, _ => new FeatureUsageStats());
            stats.ErrorCount++;
            stats.LastError = exception.Message;
            stats.LastErrorTime = DateTime.UtcNow;

            _logger.LogError(exception, "Feature {FeatureName} error recorded", featureName);
        }

        public FeatureUsageStatistics GetStatistics()
        {
            return new FeatureUsageStatistics
            {
                FeatureUsage = _featureStats.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                TotalAccesses = _featureStats.Values.Sum(s => s.TotalAccesses),
                TotalErrors = _featureStats.Values.Sum(s => s.ErrorCount),
                Timestamp = DateTime.UtcNow
            };
        }

        public void ResetStatistics()
        {
            _featureStats.Clear();
            _logger.LogInformation("Feature monitoring statistics reset");
        }
    }

    public class FeatureUsageStats
    {
        public long TotalAccesses { get; set; }
        public long EnabledAccesses { get; set; }
        public long DisabledAccesses { get; set; }
        public long ErrorCount { get; set; }
        public string LastError { get; set; }
        public DateTime LastErrorTime { get; set; }
        public DateTime LastAccessTime { get; set; } = DateTime.UtcNow;

        public double EnabledRatio => TotalAccesses > 0 ? (double)EnabledAccesses / TotalAccesses : 0;
    }

    public class FeatureUsageStatistics
    {
        public Dictionary<string, FeatureUsageStats> FeatureUsage { get; set; } =
            new Dictionary<string, FeatureUsageStats>();
        public long TotalAccesses { get; set; }
        public long TotalErrors { get; set; }
        public DateTime Timestamp { get; set; }

        public Dictionary<string, double> FeatureEnabledRatios =>
            FeatureUsage.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.EnabledRatio);
    }

    // 12. ASP.NET Core控制器 - 功能管理API演示
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureManagementController : ControllerBase
    {
        private readonly FeatureManagementService _featureService;
        private readonly FeatureMonitoringService _monitoringService;
        private readonly IFeatureManagerSnapshot _featureManagerSnapshot;

        public FeatureManagementController(
            FeatureManagementService featureService,
            FeatureMonitoringService monitoringService,
            IFeatureManagerSnapshot featureManagerSnapshot)
        {
            _featureService = featureService;
            _monitoringService = monitoringService;
            _featureManagerSnapshot = featureManagerSnapshot;
        }

        /// <summary>
        /// 获取所有功能的状态
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<ApiResponse<Dictionary<string, bool>>>> GetAllFeatureStatus()
        {
            var featureStatus = new Dictionary<string, bool>();
            var errors = new List<string>();

            #region 检查所有已定义功能

            var features = Enum.GetValues(typeof(AppFeatures)).Cast<AppFeatures>();

            foreach (var feature in features)
            {
                var featureName = feature.ToString();
                try
                {
                    var isEnabled = await _featureService.IsFeatureEnabledAsync(feature);
                    featureStatus[featureName] = isEnabled;

                    _monitoringService.RecordFeatureAccess(featureName, isEnabled, true);
                }
                catch (Exception ex)
                {
                    errors.Add($"Error checking {featureName}: {ex.Message}");
                    _monitoringService.RecordFeatureError(featureName, ex);
                    featureStatus[featureName] = false; // 默认禁用
                }
            }

            #endregion

            var response = new ApiResponse<Dictionary<string, bool>>
            {
                Success = true,
                Data = featureStatus,
                Message = "Feature status retrieved successfully",
                Errors = errors
            };

            return Ok(response);
        }

        /// <summary>
        /// 获取特定功能的元数据
        /// </summary>
        [HttpGet("metadata/{feature}")]
        public async Task<ActionResult<ApiResponse<FeatureMetadata>>> GetFeatureMetadata(string feature)
        {
            if (!Enum.TryParse<AppFeatures>(feature, out var appFeature))
            {
                return BadRequest(new ApiResponse<FeatureMetadata>
                {
                    Success = false,
                    Message = $"Invalid feature name: {feature}"
                });
            }

            try
            {
                var metadata = await _featureService.GetFeatureMetadataAsync(appFeature);

                return Ok(new ApiResponse<FeatureMetadata>
                {
                    Success = true,
                    Data = metadata,
                    Message = $"Metadata for feature {feature} retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _monitoringService.RecordFeatureError(feature, ex);

                return StatusCode(500, new ApiResponse<FeatureMetadata>
                {
                    Success = false,
                    Message = $"Error retrieving metadata for feature {feature}",
                    Errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// 检查功能是否启用（使用快照）
        /// </summary>
        [HttpGet("check/{feature}")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckFeature(string feature)
        {
            var featureExists = Enum.TryParse<AppFeatures>(feature, out var appFeature);
            if (!featureExists)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Invalid feature name: {feature}"
                });
            }

            try
            {
                // 使用快照获取功能状态（在同一个请求中保持一致）
                var isEnabled = await _featureManagerSnapshot.IsEnabledAsync(feature);

                _monitoringService.RecordFeatureAccess(feature, isEnabled, true);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = isEnabled,
                    Message = $"Feature {feature} check result: {isEnabled}"
                });
            }
            catch (FeatureManagementException ex)
            {
                _monitoringService.RecordFeatureError(feature, ex);

                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Feature management error",
                    Errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _monitoringService.RecordFeatureError(feature, ex);

                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Unexpected error occurred",
                    Errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// 获取功能使用统计
        /// </summary>
        [HttpGet("statistics")]
        public ActionResult<ApiResponse<FeatureUsageStatistics>> GetStatistics()
        {
            try
            {
                var stats = _monitoringService.GetStatistics();

                return Ok(new ApiResponse<FeatureUsageStatistics>
                {
                    Success = true,
                    Data = stats,
                    Message = "Feature statistics retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<FeatureUsageStatistics>
                {
                    Success = false,
                    Message = "Error retrieving feature statistics",
                    Errors = new[] { ex.Message }
                });
            }
        }
    }

    // 13. 功能特性服务 - 业务逻辑级别的功能启用检查
    public class FeatureFlagService
    {
        private readonly FeatureManagementService _featureService;
        private readonly ILogger<FeatureFlagService> _logger;

        public FeatureFlagService(
            FeatureManagementService featureService,
            ILogger<FeatureFlagService> logger)
        {
            _featureService = featureService;
            _logger = logger;
        }

        /// <summary>
        /// 只有在功能启用时才执行操作
        /// </summary>
        public async Task<T> ExecuteWhenFeatureEnabledAsync<T>(
            AppFeatures feature,
            Func<Task<T>> operation,
            Func<Task<T>> fallbackOperation = null)
        {
            var isFeatureEnabled = await _featureService.IsFeatureEnabledAsync(feature);

            if (isFeatureEnabled)
            {
                _logger.LogInformation("Executing operation for enabled feature {FeatureName}",
                    feature.ToString());

                await _featureService.TrackFeatureUsageAsync(feature, FeatureUsageType.Activate);

                return await operation();
            }
            else
            {
                _logger.LogWarning("Feature {FeatureName} is disabled, skipping operation",
                    feature.ToString());

                if (fallbackOperation != null)
                {
                    _logger.LogDebug("Executing fallback operation for disabled feature {FeatureName}",
                        feature.ToString());
                    return await fallbackOperation();
                }

                throw new FeatureDisabledException(feature.ToString());
            }
        }

        /// <summary>
        /// 带条件的功能执行
        /// </summary>
        public async Task ExecuteWithFeatureConditionAsync(
            AppFeatures feature,
            Func<bool> condition,
            Func<Task> operation)
        {
            var isFeatureEnabled = await _featureService.IsFeatureEnabledAsync(feature);

            if (isFeatureEnabled && condition())
            {
                _logger.LogInformation("Executing conditional operation for feature {FeatureName}",
                    feature.ToString());

                await _featureService.TrackFeatureUsageAsync(feature, FeatureUsageType.Activate);

                await operation();
            }
            else
            {
                _logger.LogDebug("Conditional operation skipped for feature {FeatureName} - " +
                               "Enabled: {IsFeatureEnabled}, Condition: {ConditionResult}",
                               feature.ToString(), isFeatureEnabled, condition());
            }
        }
    }

    public class FeatureDisabledException : Exception
    {
        public FeatureDisabledException(string featureName)
            : base($"Feature '{featureName}' is currently disabled")
        {
            FeatureName = featureName;
        }

        public string FeatureName { get; }
    }

    // 14. API响应模型
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    // 15. 配置引导类
    public static class FeatureManagementConfiguration
    {
        /// <summary>
        /// 配置功能管理层服务
        /// </summary>
        public static void ConfigureFeatureManagement(
            IServiceCollection services,
            IConfiguration configuration)
        {
            #region 基础功能管理配置

            services.AddFeatureManagement(configuration.GetSection("FeatureManagement"))
                .UseDisabledFeaturesHandler(new CustomDisabledFeaturesHandler());

            #endregion

            #region 自定义功能过滤器注册

            services.AddFeatureFilter<UserRoleFeatureFilter>();
            services.AddFeatureFilter<TimeWindowFeatureFilter>();
            services.AddFeatureFilter<PercentageFeatureFilter>();
            services.AddFeatureFilter<EnvironmentFeatureFilter>();

            #endregion

            #region 生产环境监控和跟踪服务

            services.AddSingleton<FeatureManagementService>();
            services.AddSingleton<IFeatureMonitoringService, FeatureMonitoringService>();
            services.AddSingleton<ITelemetryService, TelemetryService>();
            services.AddSingleton<FeatureCachingService>(); // 缓存服务可选
            services.AddSingleton<FeatureFlagService>();

            #endregion

            #region 健康检查集成

            services.AddHealthChecks()
                .AddCheck<FeatureManagementHealthCheck>(
                    "feature-management",
                    tags: new[] { "features" });

            #endregion
        }

        /// <summary>
        /// 配置生产环境功能管理策略
        /// </summary>
        public static void ConfigureProductionFeaturePolicies(IServiceCollection services)
        {
            // 可以添加自定义的功能管理策略和中间件
            services.AddTransient<FeatureManagementMiddleware>();
        }
    }

    public class CustomDisabledFeaturesHandler : IDisabledFeaturesHandler
    {
        public async Task HandleDisabledFeatureAsync(string featureName, HttpContext context)
        {
            // 自定义禁用功能的处理逻辑
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync($"Feature '{featureName}' is not available");
        }
    }

    // 16. 功能管理中间件 - 请求级功能检查
    public class FeatureManagementMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<FeatureManagementMiddleware> _logger;

        public FeatureManagementMiddleware(
            RequestDelegate next,
            IFeatureManager featureManager,
            ILogger<FeatureManagementMiddleware> logger)
        {
            _next = next;
            _featureManager = featureManager;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 检查维护模式功能
            if (await _featureManager.IsEnabledAsync(nameof(AppFeatures.EmergencyMaintenanceMode)))
            {
                _logger.LogWarning("Application in maintenance mode");

                context.Response.StatusCode = 503; // Service Unavailable
                await context.Response.WriteAsync("Application is currently in maintenance mode");
                return;
            }

            await _next(context);
        }
    }

    // 17. 主程序类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("Microsoft.FeatureManagement Production Demo");
            Console.WriteLine("===========================================");
            Console.WriteLine();

            #region 服务配置

            // 配置基础ASP.NET Core服务
            builder.Services.AddHttpContextAccessor();

            // 配置功能管理层
            FeatureManagementConfiguration.ConfigureFeatureManagement(
                builder.Services,
                builder.Configuration);

            // 添加测试功能控制器
            builder.Services.AddControllers();

            // 配置内存配置（模拟实际配置源）
            SetupSampleConfiguration(builder.Configuration);

            // 添加日志服务
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            #endregion

            var host = builder.Build();

            #region 演示各种功能管理功能

            Console.WriteLine("1. Feature Management Services Configuration:");
            Console.WriteLine("   - Registered all custom feature filters");
            Console.WriteLine("   - Configured feature management service");
            Console.WriteLine("   - Added monitoring and caching services");

            Console.WriteLine("\n2. Feature Status Demo:");
            var featureManager = host.Services.GetRequiredService<IFeatureManager>();
            var features = Enum.GetValues(typeof(AppFeatures)).Cast<AppFeatures>().Take(5);

            foreach (var feature in features)
            {
                var featureName = feature.ToString();
                try
                {
                    var isEnabled = await featureManager.IsEnabledAsync(featureName);
                    Console.WriteLine($"   {featureName}: {(isEnabled ? "ENABLED" : "DISABLED")}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   {featureName}: ERROR - {ex.Message}");
                }
            }

            Console.WriteLine("\n3. Feature Metadata Demo:");
            var featureService = host.Services.GetRequiredService<FeatureManagementService>();
            var premiumSupportMetadata = await featureService.GetFeatureMetadataAsync(AppFeatures.PremiumSupport);
            Console.WriteLine($"   Feature: {premiumSupportMetadata.FeatureName}");
            Console.WriteLine($"   Enabled: {premiumSupportMetadata.IsEnabled}");
            Console.WriteLine($"   Description: {premiumSupportMetadata.Description}");
            Console.WriteLine($"   Version: {premiumSupportMetadata.IntroducedVersion}");

            Console.WriteLine("\n4. Conditional Feature Execution Demo:");
            var featureFlagService = host.Services.GetRequiredService<FeatureFlagService>();

            try
            {
                var result = await featureFlagService.ExecuteWhenFeatureEnabledAsync(
                    AppFeatures.AdvancedAnalytics,
                    async () =>
                    {
                        Console.WriteLine("   Executing advanced analytics feature");
                        await Task.Delay(100);
                        return "Advanced analytics result";
                    },
                    async () =>
                    {
                        Console.WriteLine("   Fallback operation executed");
                        await Task.Delay(50);
                        return "Basic analytics result";
                    });

                Console.WriteLine($"   Feature execution result: {result}");
            }
            catch (FeatureDisabledException ex)
            {
                Console.WriteLine($"   Feature execution failed: {ex.Message}");
            }

            Console.WriteLine("\n5. Feature Monitoring Demo:");
            var monitoringService = host.Services.GetRequiredService<IFeatureMonitoringService>();

            // 模拟一些功能访问
            monitoringService.RecordFeatureAccess("PremiumSupport", true, true);
            monitoringService.RecordFeatureAccess("NewUserInterface", false, false);
            monitoringService.RecordFeatureAccess("MultiLanguageSupport", true, true);

            var stats = monitoringService.GetStatistics();
            Console.WriteLine($"   Total feature accesses: {stats.TotalAccesses}");
            Console.WriteLine($"   Total feature errors: {stats.TotalErrors}");
            foreach (var kvp in stats.FeatureEnabledRatios.Take(3))
            {
                Console.WriteLine($"   {kvp.Key} enabled ratio: {kvp.Value:P2}");
            }

            #endregion

            Console.WriteLine("\n=== Demo Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void SetupSampleConfiguration(IConfiguration configuration)
        {
            #region 模拟功能配置

            var featureConfig = new Dictionary<string, string>
            {
                // PremiumSupport - 配置时间窗口和用户角色过滤器
                ["FeatureManagement:PremiumSupport"] = "true",
                ["FeatureManagement:PremiumSupport:EnabledFor"] = "[{\"Name\":\"TimeWindow\",\"Parameters\":{\"StartTime\":\"2023-01-01T00:00:00Z\",\"EndTime\":\"2025-12-31T23:59:59Z\"}}," +
                                                                 "{\"Name\":\"UserRole\",\"Parameters\":{\"AllowedRoles\":[\"Admin\",\"Premium\"],\"AllowedUsers\":[\"user123\"]}}]",

                // NewUserInterface - 配置百分比发布
                ["FeatureManagement:NewUserInterface"] = "true",
                ["FeatureManagement:NewUserInterface:EnabledFor"] = "[{\"Name\":\"Percentage\",\"Parameters\":{\"Value\":30.0,\"Salt\":\"ui-beta-2024\"}}]",

                // MultiLanguageSupport - 配置环境过滤器
                ["FeatureManagement:MultiLanguageSupport"] = "true",
                ["FeatureManagement:MultiLanguageSupport:EnabledFor"] = "[{\"Name\":\"Environment\",\"Parameters\":{\"AllowedEnvironments\":[\"Production\",\"Staging\"]}}]",

                // AdvancedAnalytics - 基础布尔值开关
                ["FeatureManagement:AdvancedAnalytics"] = "false",

                // EmergencyMaintenanceMode - 紧急维护模式
                ["FeatureManagement:EmergencyMaintenanceMode"] = "false",

                // 性能相关功能启用
                ["FeatureManagement:PerformanceOptimization"] = "true",

                // 第三方集成功能
                ["FeatureManagement:ThirdPartyIntegration"] = "true",
                ["FeatureManagement:ThirdPartyIntegration:EnabledFor"] = "[{\"Name\":\"UserRole\",\"Parameters\":{\"AllowedRoles\":[\"Admin\"]}}]"
            };

            // 实际应用中这些配置可能来自Azure App Configuration、数据库或配置文件
            foreach (var kvp in featureConfig)
            {
                Environment.SetEnvironmentVariable(
                    kvp.Key.Replace(":", "__"),
                    kvp.Value);
            }

            #endregion
        }
    }

    // 核心组件说明
    // IFeatureManager - 基础功能管理接口
    // IFeatureManagerSnapshot - 快照功能管理，保证请求一致性
    // IFeatureFilter - 自定义功能过滤器接口
    // FeatureFilterEvaluationContext - 过滤器评估上下文

    // 功能过滤器类型
    // 内置过滤器：
    // TimeWindowFilter
    // PercentageFilter
    // TargetingFilter

    // 自定义过滤器示例
    [FilterAlias("UserRole")]
    public class UserRoleFeatureFilter : IFeatureFilter
    {
        public async Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            // 基于用户角色和身份的功能启用逻辑
        }

        // 生产级配置最佳实践
        // 功能状态配置结构
        string cfg = """
              {
              "FeatureManagement": {
                "PremiumSupport": {
                  "Enabled": "true",
                  "EnabledFor": [
                    {
                      "Name": "TimeWindow",
                      "Parameters": {
                        "StartTime": "2024-01-01T00:00:00Z",
                        "EndTime": "2024-12-31T23:59:59Z"
                      }
                    }
                  ]
                }
              }
            }
            """

        // 安全处理和降级机制
        // 错误处理
        public async Task<bool> IsFeatureEnabledAsync(AppFeatures feature)
        {
            try
            {
                return await _featureManager.IsEnabledAsync(feature.ToString());
            }
            catch (FeatureManagementException)
            {
                // 安全默认：禁用功能
                return false;
            }

            // 缓存降级
            // 当远程功能配置服务不可用时使用缓存或默认值
            if (DateTime.UtcNow <= cachedResult.ExpiryTime)
            {
                return cachedResult.IsEnabled;
            }

            // 功能启用的不同模式
            services.AddFeatureManagement().AddFeatureFilter<PercentageFilter>();

            // 复杂决策树
            builder.Services.AddFeatureFilter<UserRoleFeatureFilter>()
                .AddFeatureFilter<TimeWindowFeatureFilter>()
                .AddFeatureFilter<PercentageFeatureFilter>()
                .AddFeatureFilter<EnvironmentFeatureFilter>();
        }
    }

    // 性能优化策略
    // 功能缓存
    public class FeatureCachingService
    {
        private readonly ConcurrentDictionary<string, CachedFeatureResult> _featureCache;

        // 缓存功能状态以减少外部调用
    }

    // 快照使用
    public class FeatureManagementController : ControllerBase
    {
        private readonly IFeatureManagerSnapshot _featureManagerSnapshot;

        // 在同一个请求中保持功能状态一致性
    }

    // 监控和遥测集成
    // 功能使用跟踪
    public interface ITelemetryService
    {
        Task TrackFeatureUsageAsync(string featureName, bool isEnabled, string usageType);
        Task TrackFeatureErrorAsync(string featureName, Exception exception);
    }

    // 健康检查
    services.AddHealthChecks().AddCheck<FeatureManagementHealthCheck>("feature-management", tags: new[] { "features" });

    // 推荐的功能管理实践
    public enum AppFeatures
    {
        PremiumSupport,           // 功能模块清晰命名
        NewUserInterface,        // 避免使用魔法字符串
        MultiLanguageSupport,
        AdvancedAnalytics
    }

    // 条件功能执行
    public async Task<T> ExecuteWhenFeatureEnabledAsync<T>(
    AppFeatures feature,
    Func<Task<T>> operation,
    Func<Task<T>> fallbackOperation = null)

    // 环境适配配置
    public class EnvironmentSpecificConfigurationProvider<T> where T : class, new()
    {
        public T GetConfigurationForEnvironment()
        {
            // 根据环境加载不同的功能配置
        }
    }

    // 功能依赖管理
    public class FeatureMetadata
    {
        public List<string> Dependencies { get; set; } = new List<string>();
        // 管理功能间的依赖关系
    }
}