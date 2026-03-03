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
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using System.IO;

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

    / 1. 本地化资源管理器 - 生产级封装
public class LocalizationResourceManager
    {
        private readonly IStringLocalizer _localizer;
        private readonly ILogger<LocalizationResourceManager> _logger;

        public LocalizationResourceManager(
            IStringLocalizer<LocalizationResourceManager> localizer,
            ILogger<LocalizationResourceManager> logger)
        {
            _localizer = localizer;
            _logger = logger;
        }

        /// <summary>
        /// 安全获取本地化字符串 - 防止空键值和回退处理
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="args">格式化参数</param>
        /// <returns>本地化字符串</returns>
        public string GetLocalizedString(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning("Attempted to localize with null or empty key");
                return string.Empty;
            }

            try
            {
                // 使用带参数的本地化器获取字符串
                var localizedString = _localizer[key, args];

                // 记录未找到本地化字符串的情况（用于资源文件维护）
                if (localizedString.ResourceNotFound)
                {
                    _logger.LogWarning("Localized string not found for key: {Key}", key);
                    return key; // 返回键值作为后备
                }

                return localizedString.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting localized string for key: {Key}", key);
                return key; // 错误时返回键值作为后备
            }
        }

        /// <summary>
        /// 获取所有可用的本地化字符串 - 用于调试和验证
        /// </summary>
        /// <returns>本地化字符串集合</returns>
        public IEnumerable<LocalizedString> GetAllStrings()
        {
            try
            {
                return _localizer.GetAllStrings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting all localized strings");
                return Enumerable.Empty<LocalizedString>();
            }
        }
    }

    // 2. 本地化验证服务 - 生产级本地化验证实现
    public class LocalizedValidationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizedValidationService(IStringLocalizer<LocalizedValidationService> localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// 验证用户输入并返回本地化错误消息
        /// </summary>
        public ValidationResult ValidateUser(UserRegistrationModel user)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(user.Name))
            {
                errors.Add(_localizer["User_Name_Required"]);
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                errors.Add(_localizer["User_Email_Required"]);
            }
            else if (!IsValidEmail(user.Email))
            {
                errors.Add(_localizer["User_Email_Invalid"]);
            }

            if (user.Age < 18)
            {
                errors.Add(_localizer["User_Age_Minimum", 18]);
            }

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                ErrorMessages = errors
            };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }

    public class UserRegistrationModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
    }

    // 3. 多语言内容服务 - 根据文化获取适当内容
    public class MultilingualContentService
    {
        private readonly IStringLocalizer _localizer;
        private readonly ILogger<MultilingualContentService> _logger;

        public MultilingualContentService(
            IStringLocalizer<MultilingualContentService> localizer,
            ILogger<MultilingualContentService> logger)
        {
            _localizer = localizer;
            _logger = logger;
        }

        /// <summary>
        /// 获取本地化的帮助文档内容
        /// </summary>
        public HelpContent GetHelpContent(string topic)
        {
            return new HelpContent
            {
                Title = _localizer[$"Help_{topic}_Title"],
                Description = _localizer[$"Help_{topic}_Description"],
                Examples = GetLocalizedExamples(topic)
            };
        }

        /// <summary>
        /// 获取本地化的示例内容
        /// </summary>
        private List<string> GetLocalizedExamples(string topic)
        {
            var examples = new List<string>();
            var exampleCount = _localizer[$"Help_{topic}_ExampleCount"].Value;

            if (int.TryParse(exampleCount, out int count))
            {
                for (int i = 1; i <= count; i++)
                {
                    examples.Add(_localizer[$"Help_{topic}_Example_{i}"]);
                }
            }

            return examples;
        }

        /// <summary>
        /// 获取本地化的菜单项
        /// </summary>
        public List<MenuItem> GetMenuItems()
        {
            return new List<MenuItem>
        {
            new MenuItem
            {
                Id = "home",
                Text = _localizer["Menu_Home"],
                Icon = _localizer["Menu_Home_Icon"]
            },
            new MenuItem
            {
                Id = "about",
                Text = _localizer["Menu_About"],
                Icon = _localizer["Menu_About_Icon"]
            },
            new MenuItem
            {
                Id = "contact",
                Text = _localizer["Menu_Contact"],
                Icon = _localizer["Menu_Contact_Icon"]
            }
        };
        }
    }

    public class HelpContent
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Examples { get; set; } = new List<string>();
    }

    public class MenuItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
    }

    // 4. 本地化策略配置类 - 生产级本地化配置
    public static class LocalizationConfiguration
    {
        /// <summary>
        /// 配置应用程序的本地化服务
        /// </summary>
        public static void ConfigureLocalization(IServiceCollection services)
        {
            #region 基础本地化配置

            // 配置本地化服务
            services.AddLocalization(options =>
            {
                // 指定资源文件的根路径
                options.ResourcesPath = "Resources";
            });

            #endregion

            #region MVC本地化配置

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix) // 视图本地化支持
                .AddDataAnnotationsLocalization(); // 数据注解本地化支持

            #endregion

            #region 生产级本地化选项配置

            services.Configure<RequestLocalizationOptions>(options =>
            {
                // 定义支持的文化列表
                var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"), // 英语（美国）
                new CultureInfo("zh-CN"), // 中文（简体中文）
                new CultureInfo("zh-TW"), // 中文（繁体中文）
                new CultureInfo("ja-JP"), // 日语（日本）
                new CultureInfo("fr-FR"), // 法语（法国）
                new CultureInfo("de-DE")  // 德语（德国）
            };

                // 设置支持的文化
                options.SetDefaultCulture(supportedCultures[0].Name)
                    .AddSupportedCultures(supportedCultures.Select(c => c.Name).ToArray())
                    .AddSupportedUICultures(supportedCultures.Select(c => c.Name).ToArray());

                // 配置请求文化提供者
                options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                // 1. 查询字符串提供者（?culture=zh-CN）
                new QueryStringRequestCultureProvider(),
                
                // 2. Cookie提供者
                new CookieRequestCultureProvider(),
                
                // 3. Accept-Language请求头提供者
                new AcceptLanguageHeaderRequestCultureProvider(),
                
                // 4. 自定义提供者（从用户配置获取）
                new CustomUserCultureProvider()
            };

                // 验证和记录本地化配置
                options.FallBackToParentCultures = true;   // 启用文化回退
                options.FallBackToParentUICultures = true; // 启用UI文化回退
            });

            #endregion
        }

        /// <summary>
        /// 配置生产环境的资源文件加载策略
        /// </summary>
        public static void ConfigureProductionLocalization(IServiceCollection services)
        {
            #region 资源管理优化配置

            // 配置本地化资源管理器
            services.Configure<LocalizationOptions>(options =>
            {
                // 生产环境中避免使用开发模式
                options.ResourcesPath = "Resources";
                options.ApplyCulturesToContentRootPath = false;
            });

            #endregion

            #region 缓存和性能优化配置

            // 可以添加自定义资源缓存管理器
            services.AddSingleton<IResourceCacheManager, ProductionResourceCacheManager>();

            #endregion

            #region 监控配置

            services.AddSingleton<ILocalizationMonitor, LocalizationMonitor>();

            #endregion
        }
    }

    // 5. 自定义文化提供者 - 从用户配置获取用户偏好文化
    public class CustomUserCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            // 从用户配置、数据库或其他服务获取用户偏好文化
            var userCulture = await GetUserPreferredCultureAsync(httpContext);

            if (!string.IsNullOrEmpty(userCulture))
            {
                // 返回用户偏好文化
                return new ProviderCultureResult(userCulture);
            }

            // 返回null让其他提供者处理
            return null;
        }

        /// <summary>
        /// 模拟从用户信息获取偏好文化的方法
        /// </summary>
        private async Task<string> GetUserPreferredCultureAsync(HttpContext context)
        {
            // 实际实现可以从：
            // 1. 用户配置表查询
            // 2. JWT令牌解析
            // 3. 用户会话数据
            // 4. 公司或组织默认设置

            var userId = context.Request.Headers["X-User-ID"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userId))
            {
                // 模拟数据库查询
                await Task.Delay(10); // 模拟异步数据库调用

                // 根据用户ID返回相应的文化设置
                // 在生产环境中，这里应该调用实际的数据访问层
                return "zh-CN";
            }

            return null;
        }
    }

    // 6. 资源缓存管理器 - 生产级缓存优化
    public interface IResourceCacheManager
    {
        bool TryGetCachedResource(string culture, string key, out string value);
        void CacheResource(string culture, string key, string value);
        void InvalidateCache(string culture);
        void ClearAllCache();
    }

    public class ProductionResourceCacheManager : IResourceCacheManager
    {
        private readonly ConcurrentDictionary<string, string> _resourceCache;
        private readonly ILogger<ProductionResourceCacheManager> _logger;
        private readonly TimeSpan _cacheExpiry;

        public ProductionResourceCacheManager(ILogger<ProductionResourceCacheManager> logger)
        {
            _resourceCache = new ConcurrentDictionary<string, string>();
            _logger = logger;

            // 生产环境中设置合适的缓存过期时间
            _cacheExpiry = TimeSpan.FromHours(1);
        }

        public bool TryGetCachedResource(string culture, string key, out string value)
        {
            var cacheKey = $"{culture}:{key}";

            if (_resourceCache.TryGetValue(cacheKey, out value))
            {
                _logger.LogDebug("Retrieved cached resource for culture {Culture} and key {Key}", culture, key);
                return true;
            }

            value = null;
            return false;
        }

        public void CacheResource(string culture, string key, string value)
        {
            var cacheKey = $"{culture}:{key}";
            _resourceCache[cacheKey] = value;
            _logger.LogDebug("Cached resource for culture {Culture} and key {Key}", culture, key);
        }

        public void InvalidateCache(string culture)
        {
            var keysToRemove = _resourceCache.Keys.Where(k => k.StartsWith($"{culture}:")).ToList();
            foreach (var key in keysToRemove)
            {
                _resourceCache.TryRemove(key, out _);
            }
            _logger.LogInformation("Invalidated cache for culture {Culture}", culture);
        }

        public void ClearAllCache()
        {
            _resourceCache.Clear();
            _logger.LogInformation("Cleared all localization resource cache");
        }
    }

    // 7. 本地化监控服务 - 用于生产环境中追踪和诊断
    public interface ILocalizationMonitor
    {
        void RecordResourceAccess(string culture, string key, bool found, TimeSpan duration);
        LocalizationStats GetStatistics();
        void ResetStatistics();
    }

    public class LocalizationMonitor : ILocalizationMonitor
    {
        private readonly ILogger<LocalizationMonitor> _logger;

        // 统计数据
        private long _totalResourceAccesses = 0;
        private long _missingResourceAccesses = 0;
        private readonly ConcurrentDictionary<string, long> _cultureAccesses = new ConcurrentDictionary<string, long>();
        private readonly ConcurrentDictionary<string, long> _keyAccesses = new ConcurrentDictionary<string, long>();

        public LocalizationMonitor(ILogger<LocalizationMonitor> logger)
        {
            _logger = logger;
        }

        public void RecordResourceAccess(string culture, string key, bool found, TimeSpan duration)
        {
            Interlocked.Increment(ref _totalResourceAccesses);

            // 记录文化访问统计
            _cultureAccesses.AddOrUpdate(culture, 1, (k, v) => v + 1);

            // 记录键访问统计
            _keyAccesses.AddOrUpdate(key, 1, (k, v) => v + 1);

            // 记录缺失资源
            if (!found)
            {
                Interlocked.Increment(ref _missingResourceAccesses);
                _logger.LogWarning("Missing localization resource - Culture: {Culture}, Key: {Key}", culture, key);
            }

            // 记录慢速访问（超过100ms）
            if (duration.TotalMilliseconds > 100)
            {
                _logger.LogWarning("Slow localization resource access - Culture: {Culture}, Key: {Key}, Duration: {Duration}ms",
                    culture, key, duration.TotalMilliseconds);
            }
        }

        public LocalizationStats GetStatistics()
        {
            return new LocalizationStats
            {
                TotalAccesses = _totalResourceAccesses,
                MissingResources = _missingResourceAccesses,
                CultureAccessStats = _cultureAccesses.ToDictionary(k => k.Key, v => v.Value),
                KeyAccessStats = _keyAccesses.OrderByDescending(x => x.Value).Take(10).ToDictionary(k => k.Key, v => v.Value)
            };
        }

        public void ResetStatistics()
        {
            Interlocked.Exchange(ref _totalResourceAccesses, 0);
            Interlocked.Exchange(ref _missingResourceAccesses, 0);
            _cultureAccesses.Clear();
            _keyAccesses.Clear();
        }
    }

    public class LocalizationStats
    {
        public long TotalAccesses { get; set; }
        public long MissingResources { get; set; }
        public Dictionary<string, long> CultureAccessStats { get; set; } = new Dictionary<string, long>();
        public Dictionary<string, long> KeyAccessStats { get; set; } = new Dictionary<string, long>();
    }

    // 8. 本地化服务定位器 - 用于非ASP.NET Core环境获取本地化服务
    public class LocalizationServiceLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public LocalizationServiceLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取指定文化的本地化器
        /// </summary>
        public IStringLocalizer GetStringLocalizer<T>(CultureInfo culture)
        {
            var factory = _serviceProvider.GetRequiredService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(T));

            // 设置当前文化（在异步上下文中）
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            return localizer;
        }

        /// <summary>
        /// 动态获取资源键的本地化值
        /// </summary>
        public string GetString<T>(string key, CultureInfo culture, params object[] arguments)
        {
            var originalUICulture = CultureInfo.CurrentUICulture;
            var originalCulture = CultureInfo.CurrentCulture;

            try
            {
                CultureInfo.CurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;

                var factory = _serviceProvider.GetRequiredService<IStringLocalizerFactory>();
                var localizer = factory.Create(typeof(T));

                return localizer[key, arguments].Value;
            }
            finally
            {
                // 恢复原始文化设置
                CultureInfo.CurrentUICulture = originalUICulture;
                CultureInfo.CurrentCulture = originalCulture;
            }
        }
    }

    // 9. ASP.NET Core控制器示例 - 演示MVC本地化使用
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizedUserController : ControllerBase
    {
        private readonly IStringLocalizer<LocalizedUserController> _localizer;
        private readonly LocalizedValidationService _validationService;
        private readonly MultilingualContentService _contentService;

        public LocalizedUserController(
            IStringLocalizer<LocalizedUserController> localizer,
            LocalizedValidationService validationService,
            MultilingualContentService contentService)
        {
            _localizer = localizer;
            _validationService = validationService;
            _contentService = contentService;
        }

        /// <summary>
        /// 获取用户信息（本地化响应）
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<UserDto> GetUser(int id)
        {
            try
            {
                // 模拟获取用户
                var user = new UserDto
                {
                    Id = id,
                    Name = $"User {id}",
                    Status = "Active"
                };

                // 使用本地化响应消息
                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Data = user,
                    Message = _localizer["User_Found_Successfully"]
                });
            }
            catch (Exception)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = _localizer["User_Not_Found", id]
                });
            }
        }

        /// <summary>
        /// 创建用户（本地化验证）
        /// </summary>
        [HttpPost]
        public ActionResult<ApiResponse<bool>> CreateUser(UserRegistrationModel model)
        {
            // 验证用户输入
            var validationResult = _validationService.ValidateUser(model);

            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = _localizer["Validation_Failed"],
                    Errors = validationResult.ErrorMessages
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = _localizer["User_Created_Successfully"]
            });
        }

        /// <summary>
        /// 获取本地化帮助内容
        /// </summary>
        [HttpGet("help/{topic}")]
        public ActionResult<ApiResponse<HelpContent>> GetUserHelp(string topic)
        {
            try
            {
                var helpContent = _contentService.GetHelpContent(topic);
                return Ok(new ApiResponse<HelpContent>
                {
                    Success = true,
                    Data = helpContent,
                    Message = _localizer["Help_Content_Retrieved"]
                });
            }
            catch (Exception)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = _localizer["Help_Topic_Not_Found", topic]
                });
            }
        }

        /// <summary>
        /// 获取本地化菜单项
        /// </summary>
        [HttpGet("menu")]
        public ActionResult<ApiResponse<List<MenuItem>>> GetMenuItems()
        {
            var menuItems = _contentService.GetMenuItems();
            return Ok(new ApiResponse<List<MenuItem>>
            {
                Success = true,
                Data = menuItems,
                Message = _localizer["Menu_Items_Retrieved"]
            });
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        [HttpGet("statistics")]
        public ActionResult<ApiResponse<LocalizationStats>> GetStatistics(
            [FromServices] ILocalizationMonitor monitor)
        {
            var stats = monitor.GetStatistics();
            return Ok(new ApiResponse<LocalizationStats>
            {
                Success = true,
                Data = stats,
                Message = _localizer["Statistics_Retrieved"]
            });
        }
    }

    // 10. API响应模型
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }

    // 11. View模型本地化 - MVC视图中使用
    public class LocalizedUserProfileViewModel
    {
        private readonly IStringLocalizer<LocalizedUserProfileViewModel> _localizer;

        public LocalizedUserProfileViewModel(IStringLocalizer<LocalizedUserProfileViewModel> localizer)
        {
            _localizer = localizer;
        }

        public string Title => _localizer["UserProfile_Title"];
        public string NameLabel => _localizer["UserProfile_Name_Label"];
        public string EmailLabel => _localizer["UserProfile_Email_Label"];
        public string SubmitButtonText => _localizer["UserProfile_Submit_Button"];

        public User User { get; set; } = new User();

        /// <summary>
        /// 获取本地化状态文本
        /// </summary>
        public string GetStatusText(string status)
        {
            return _localizer[$"UserProfile_Status_{status}"];
        }
    }

    // 12. 中间件 - 请求本地化中间件配置
    public class RequestLocalizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLocalizationMiddleware> _logger;

        public RequestLocalizationMiddleware(RequestDelegate next, ILogger<RequestLocalizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 记录当前请求文化的日志信息
            _logger.LogInformation("Request culture: {Culture}, UI culture: {UICulture}",
                CultureInfo.CurrentCulture.Name,
                CultureInfo.CurrentUICulture.Name);

            // 继续执行请求管道
            await _next(context);
        }
    }

    // 13. 学徒模式本地化包装器 - 提供更简单易用的API
    public class ApprenticeLocalizedStringHelper
    {
        private readonly IStringLocalizerFactory _localizerFactory;

        public ApprenticeLocalizedStringHelper(IStringLocalizerFactory localizerFactory)
        {
            _localizerFactory = localizerFactory;
        }

        /// <summary>
        /// 简单易用的本地化字符串获取方法
        /// </summary>
        public string GetString(Type resourceType, string key, params object[] args)
        {
            var localizer = _localizerFactory.Create(resourceType);
            return localizer[key, args].Value;
        }

        /// <summary>
        /// 获取指定文化的本地化字符串
        /// </summary>
        public string GetString(Type resourceType, string culture, string key, params object[] args)
        {
            var originalCulture = CultureInfo.CurrentUICulture;

            try
            {
                CultureInfo.CurrentUICulture = new CultureInfo(culture);
                return GetString(resourceType, key, args);
            }
            finally
            {
                CultureInfo.CurrentUICulture = originalCulture;
            }
        }

        /// <summary>
        /// 本地化异常消息包装
        /// </summary>
        public LocalizedException CreateLocalizedException(Type resourceType, string key, params object[] args)
        {
            var message = GetString(resourceType, key, args);
            return new LocalizedException(message);
        }
    }

    public class LocalizedException : Exception
    {
        public LocalizedException(string message) : base(message) { }
    }

    // 14. 程序启动配置类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("Microsoft.Extensions.Localization Production Demo");
            Console.WriteLine("=================================================");
            Console.WriteLine();

            #region 服务配置

            // 配置本地化服务
            LocalizationConfiguration.ConfigureLocalization(builder.Services);
            LocalizationConfiguration.ConfigureProductionLocalization(builder.Services);

            // 添加日志服务
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            #endregion

            var host = builder.Build();

            #region 演示各种本地化功能

            Console.WriteLine("1. Service Configuration:");
            Console.WriteLine("   - Configured localization with support cultures: en-US, zh-CN, zh-TW, ja-JP, fr-FR, de-DE");
            Console.WriteLine("   - Set up view and data annotation localization");
            Console.WriteLine("   - Configured multiple request culture providers");

            Console.WriteLine("\n2. Resource Access Demo:");
            var resourceManager = host.Services.GetRequiredService<LocalizationResourceManager>();
            var strings = resourceManager.GetAllStrings().Take(5);
            foreach (var str in strings)
            {
                Console.WriteLine($"   Resource key '{str.Name}': {str.Value}");
            }

            Console.WriteLine("\n3. Validation Localization Demo:");
            var validationService = host.Services.GetRequiredService<LocalizedValidationService>();
            var userModel = new UserRegistrationModel { Name = "", Email = "invalid-email", Age = 15 };
            var validationResult = validationService.ValidateUser(userModel);
            Console.WriteLine($"   Validation result: Success = {validationResult.IsValid}");
            foreach (var error in validationResult.ErrorMessages)
            {
                Console.WriteLine($"   Validation error: {error}");
            }

            Console.WriteLine("\n4. Multilingual Content Demo:");
            var contentService = host.Services.GetRequiredService<MultilingualContentService>();
            var menuItems = contentService.GetMenuItems();
            Console.WriteLine($"   Retrieved {menuItems.Count} menu items");
            foreach (var item in menuItems.Take(3)) // 显示前3个项目
            {
                Console.WriteLine($"   Menu item '{item.Id}': {item.Text}");
            }

            Console.WriteLine("\n5. Culture Switching Demo:");
            // 切换文化并显示不同本地化内容
            var cultures = new[] { "en-US", "zh-CN", "fr-FR" };
            foreach (var culture in cultures)
            {
                var localizedString = resourceManager.GetLocalizedString("Welcome_Message", culture);
                Console.WriteLine($"   Welcome in {culture}: {localizedString}");
            }

            #endregion

            Console.WriteLine("\n=== Demo Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public void Test()
        {
            // 生产级配置建议
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("zh-CN"),
                    new CultureInfo("ja-JP")
                };

                options.SetDefaultCulture(supportedCultures[0].Name)
                    .AddSupportedCultures(supportedCultures.Select(c => c.Name).ToArray())
                    .AddSupportedUICultures(supportedCultures.Select(c => c.Name).ToArray());
            });

            // 请求文化提供者顺序
            // 查询字符串 → Cookie → 请求头 → 自定义提供者
            // 资源路径配置
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources"; // 资源文件根路径
            });

            // 安全和容错策略
            // 防止键空值
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning("Attempted to localize with null or empty key");
                return string.Empty;
            }

            // 回退处理
            if (localizedString.ResourceNotFound)
            {
                _logger.LogWarning("Localized string not found for key: {Key}", key);
                return key; // 返回键值作为后备
            }
        }

        // ASP.NET Core集成要点
        // 控制器本地化
        [ApiController]
        public class LocalizedUserController : ControllerBase
        {
            private readonly IStringLocalizer<LocalizedUserController> _localizer;

            public LocalizedUserController(IStringLocalizer<LocalizedUserController> localizer)
            {
                _localizer = localizer;
            }

            [HttpGet("{id}")]
            public ActionResult GetUser(int id)
            {
                return Ok(new { message = _localizer["User_Found_Successfully"] });
            }
        }

        // MVC视图本地化
        services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

        // 数据注解本地化
        services.AddMvc().AddDataAnnotationsLocalization();

        // 性能优化策略
        // 缓存机制：
        // 资源文件自动缓存
        // 可以添加自定义缓存层
        options.FallBackToParentCultures = true;
        options.FallBackToParentUICultures = true;

        // 监控和诊断功能
        // 资源访问监控
        public interface ILocalizationMonitor
        {
            void RecordResourceAccess(string culture, string key, bool found, TimeSpan duration);
            LocalizationStats GetStatistics();
        }

        // 缺失资源警告
        if (!found)
        {
            _logger.LogWarning("Missing localization resource - Culture: {Culture}, Key: {Key}", culture, key);
        }

    // 中间件集成
    app.UseRequestLocalization(); // 必须在其他中间件之前
}

// 推荐的生产实践
// 资源文件结构化管理：按模块和功能分类
// 键命名规范：使用清晰的层次结构命名
// 参数占位符统一：使用标准格式参数化字符串
// 默认文化设置：确保有合理的默认文化
// 性能监控：监控本地化调用性能和资源缺失情况
// 测试覆盖：对每种支持的语言进行测试

    // 资源文件命名规则
    // 类型基础资源文件
    // MyType.en-US.resx
    // MyType.zh-CN.resx

    // 视图本地化文件
    // Index.en-US.resx
    // Index.zh-CN.resx

    // 15. 生产级资源文件组织建议
    /*
    推荐的资源文件组织结构：

    /Resources/
    ├── Controllers/
    │   ├── LocalizedUserController.en-US.resx
    │   ├── LocalizedUserController.zh-CN.resx
    │   └── LocalizedUserController.fr-FR.resx
    ├── Services/
    │   ├── LocalizationResourceManager.en-US.resx
    │   ├── LocalizationResourceManager.zh-CN.resx
    │   └── LocalizationResourceManager.fr-FR.resx
    ├── Models/
    │   ├── UserRegistrationModel.en-US.resx
    │   ├── UserRegistrationModel.zh-CN.resx
    │   └── UserRegistrationModel.fr-FR.resx
    ├── Validation/
    │   ├── LocalizedValidationService.en-US.resx
    │   ├── LocalizedValidationService.zh-CN.resx
    │   └── LocalizedValidationService.fr-FR.resx
    └── Shared/
        ├── Messages.en-US.resx
        ├── Messages.zh-CN.resx
        └── Messages.fr-FR.resx

    资源键命名约定：
    - 使用清晰的层次结构：[模块]_[功能]_[描述]
    - 避免硬编码字符串
    - 统一参数占位符使用：{0}, {1}, etc.
    */

    //  核心组件说明
    // IStringLocalizer - 核心本地化接口
    // IStringLocalizer<T> - 泛型本地化接口，用于类型安全
    // IStringLocalizerFactory - 本地化器工厂
    // RequestLocalizationOptions - 请求本地化配置选项
    // ProviderCultureResult - 文化提供者结果
}