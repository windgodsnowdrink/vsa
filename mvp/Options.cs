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
#:property TargetFramework=net11.0
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

using App;
using Ardalis.ListStartupServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ModelContextProtocol.Server;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks;

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

    // 1. 基础配置选项类 - 数据库配置
    public class DatabaseOptions
    {
        public const string SectionName = "Database";

        [Required(ErrorMessage = "Database ConnectionString is required")]
        public string ConnectionString { get; set; }

        [Range(1, 300, ErrorMessage = "CommandTimeout must be between 1 and 300 seconds")]
        public int CommandTimeout { get; set; } = 30;

        [Range(1, 100, ErrorMessage = "MaxPoolSize must be between 1 and 100")]
        public int MaxPoolSize { get; set; } = 10;

        public bool EnableRetryOnFailure { get; set; } = true;

        [Range(0, 10, ErrorMessage = "RetryCount must be between 0 and 10")]
        public int RetryCount { get; set; } = 3;

        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);

        public DatabaseRetryOptions RetryOptions { get; set; } = new DatabaseRetryOptions();
    }

    public class DatabaseRetryOptions
    {
        [Range(1, 60, ErrorMessage = "InitialDelaySeconds must be between 1 and 60")]
        public int InitialDelaySeconds { get; set; } = 1;

        public double DelayMultiplier { get; set; } = 2.0;

        [Range(1, 300, ErrorMessage = "MaxDelaySeconds must be between 1 and 300")]
        public int MaxDelaySeconds { get; set; } = 60;

        public List<string> RetryableErrorCodes { get; set; } = new List<string>();
    }

    // 2. 服务配置选项类 - 邮件服务配置
    public class EmailServiceOptions
    {
        public const string SectionName = "EmailService";

        [Required(ErrorMessage = "SMTP Host is required")]
        public string SmtpHost { get; set; }

        [Range(1, 65535, ErrorMessage = "SMTP Port must be between 1 and 65535")]
        public int SmtpPort { get; set; } = 587;

        public string Username { get; set; }

        public string Password { get; set; }

        public bool EnableSsl { get; set; } = true;

        [EmailAddress(ErrorMessage = "Invalid sender email address")]
        public string SenderEmail { get; set; }

        [Required(ErrorMessage = "Sender name is required")]
        public string SenderName { get; set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        public EmailThrottlingOptions Throttling { get; set; } = new EmailThrottlingOptions();
    }

    public class EmailThrottlingOptions
    {
        [Range(1, 1000, ErrorMessage = "MaxEmailsPerMinute must be between 1 and 1000")]
        public int MaxEmailsPerMinute { get; set; } = 100;

        public int BurstSize { get; set; } = 10;

        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
    }

    // 3. 缓存配置选项类
    public class CacheOptions
    {
        public const string SectionName = "Cache";

        public CacheType CacheType { get; set; } = CacheType.Memory;

        public RedisOptions Redis { get; set; } = new RedisOptions();

        public MemoryCacheOptions Memory { get; set; } = new MemoryCacheOptions();

        [Range(1, 3600, ErrorMessage = "DefaultExpirationSeconds must be between 1 and 3600")]
        public int DefaultExpirationSeconds { get; set; } = 300;

        public bool EnableCompression { get; set; } = false;
    }

    public enum CacheType
    {
        Memory,
        Redis
    }

    public class RedisOptions
    {
        [Required(ErrorMessage = "Redis connection string is required")]
        public string ConnectionString { get; set; }

        [Range(1, 100, ErrorMessage = "Redis database number must be between 0 and 15")]
        public int DatabaseNumber { get; set; } = 0;

        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);

        public TimeSpan SyncTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public bool AllowAdmin { get; set; } = false;
    }

    public class MemoryCacheOptions
    {
        [Range(1, 1024, ErrorMessage = "Memory cache size limit must be between 1MB and 1024MB")]
        public int SizeLimitMB { get; set; } = 100;

        [Range(1, 100, ErrorMessage = "Memory cache compaction percentage must be between 1% and 100%")]
        public int CompactionPercentage { get; set; } = 5;

        public TimeSpan ExpirationScanFrequency { get; set; } = TimeSpan.FromMinutes(1);
    }

    // 4. 安全配置选项类
    public class SecurityOptions
    {
        public const string SectionName = "Security";

        public JwtOptions Jwt { get; set; } = new JwtOptions();

        public CorsOptions Cors { get; set; } = new CorsOptions();

        public RateLimitingOptions RateLimiting { get; set; } = new RateLimitingOptions();

        public bool EnableAuditLogging { get; set; } = true;
    }

    public class JwtOptions
    {
        [Required(ErrorMessage = "JWT issuer is required")]
        public string Issuer { get; set; }

        [Required(ErrorMessage = "JWT audience is required")]
        public string Audience { get; set; }

        [Required(ErrorMessage = "JWT secret key is required")]
        public string SecretKey { get; set; }

        [Range(1, 365, ErrorMessage = "JWT expiry days must be between 1 and 365")]
        public int ExpiryDays { get; set; } = 7;

        public string EncryptionAlgorithm { get; set; } = "HS256";
    }

    public class CorsOptions
    {
        public List<string> AllowedOrigins { get; set; } = new List<string>();

        public List<string> AllowedMethods { get; set; } = new List<string> { "GET", "POST", "PUT", "DELETE" };

        public List<string> AllowedHeaders { get; set; } = new List<string>();

        public bool AllowCredentials { get; set; } = false;
    }

    public class RateLimitingOptions
    {
        [Range(1, 10000, ErrorMessage = "Requests per window must be between 1 and 10000")]
        public int RequestsPerWindow { get; set; } = 1000;

        public TimeSpan WindowSize { get; set; } = TimeSpan.FromMinutes(1);

        public TimeSpan RetryAfter { get; set; } = TimeSpan.FromMinutes(5);
    }

    // 5. 自定义验证属性 - 生产环境特有验证
    public class RequiredIfNotProductionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // 在非生产环境中，某些配置是必需的
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment != "Production" && value == null)
            {
                return new ValidationResult(ErrorMessage ?? "This field is required in non-production environments");
            }

            return ValidationResult.Success;
        }
    }

    // 6. 配置绑定后处理器
    public interface IConfigureOptions<T> where T : class, new()
    {
        void Configure(T options);
        void Validate(T options);
    }

    public class DatabaseOptionsSetup : IConfigureOptions<DatabaseOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseOptionsSetup> _logger;

        public DatabaseOptionsSetup(IConfiguration configuration, ILogger<DatabaseOptionsSetup> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void Configure(DatabaseOptions options)
        {
            // 从配置绑定后的后处理逻辑
            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                options.ConnectionString = GetDefaultConnectionString();
            }

            // 设置环境特定的默认值
            var environment = _configuration.GetValue<string>("Environment", "Development");
            if (environment == "Development")
            {
                options.RetryCount = 0; // 开发环境不需要重试
                options.EnableRetryOnFailure = false;
            }

            _logger.LogInformation("Database options configured for environment: {Environment}", environment);
        }

        public void Validate(DatabaseOptions options)
        {
            // 自定义复杂验证逻辑
            if (options.RetryCount > 0 && !options.EnableRetryOnFailure)
            {
                throw new InvalidOperationException("RetryOptions cannot be configured when retry is disabled");
            }

            if (options.RetryOptions.MaxDelaySeconds < options.RetryOptions.InitialDelaySeconds)
            {
                throw new InvalidOperationException("MaxDelaySeconds must be greater than or equal to InitialDelaySeconds");
            }
        }

        private string GetDefaultConnectionString()
        {
            return "Server=localhost;Database=MyApp;Trusted_Connection=true;";
        }
    }

    // 7. 配置变更监听器 - 监控配置变化
    public class ConfigurationChangeListener<T> : IOptionsChangeTokenSource<T> where T : class, new()
    {
        private readonly IConfiguration _configuration;
        private readonly string _name;

        public ConfigurationChangeListener(IConfiguration configuration, string name = "")
        {
            _configuration = configuration;
            _name = name;
        }

        public string Name => _name;

        public IChangeToken GetChangeToken()
        {
            return _configuration.GetReloadToken();
        }
    }

    // 8. 配置服务管理器 - 生产环境配置管理
    public class ConfigurationServiceManager
    {
        private readonly IOptionsMonitor<DatabaseOptions> _databaseOptions;
        private readonly IOptionsMonitor<EmailServiceOptions> _emailOptions;
        private readonly IOptionsMonitor<CacheOptions> _cacheOptions;
        private readonly IOptionsMonitor<SecurityOptions> _securityOptions;
        private readonly ILogger<ConfigurationServiceManager> _logger;

        public ConfigurationServiceManager(
            IOptionsMonitor<DatabaseOptions> databaseOptions,
            IOptionsMonitor<EmailServiceOptions> emailOptions,
            IOptionsMonitor<CacheOptions> cacheOptions,
            IOptionsMonitor<SecurityOptions> securityOptions,
            ILogger<ConfigurationServiceManager> logger)
        {
            _databaseOptions = databaseOptions;
            _emailOptions = emailOptions;
            _cacheOptions = cacheOptions;
            _securityOptions = securityOptions;
            _logger = logger;

            #region 配置变更监控

            // 监控数据库配置变化
            _databaseOptions.OnChange(options =>
            {
                _logger.LogWarning("Database configuration changed at {Timestamp}", DateTime.UtcNow);
                HandleDatabaseConfigurationChange(options);
            });

            // 监控邮件服务配置变化
            _emailOptions.OnChange(options =>
            {
                _logger.LogWarning("Email service configuration changed at {Timestamp}", DateTime.UtcNow);
                HandleEmailConfigurationChange(options);
            });

            // 监控缓存配置变化
            _cacheOptions.OnChange(options =>
            {
                _logger.LogWarning("Cache configuration changed at {Timestamp}", DateTime.UtcNow);
                HandleCacheConfigurationChange(options);
            });

            #endregion
        }

        public DatabaseOptions GetDatabaseOptions()
        {
            var options = _databaseOptions.CurrentValue;
            _logger.LogDebug("Retrieved current database options");
            return options;
        }

        public EmailServiceOptions GetEmailOptions()
        {
            var options = _emailOptions.CurrentValue;
            _logger.LogDebug("Retrieved current email service options");
            return options;
        }

        public CacheOptions GetCacheOptions()
        {
            var options = _cacheOptions.CurrentValue;
            _logger.LogDebug("Retrieved current cache options");
            return options;
        }

        public SecurityOptions GetSecurityOptions()
        {
            var options = _securityOptions.CurrentValue;
            _logger.LogDebug("Retrieved current security options");
            return options;
        }

        private void HandleDatabaseConfigurationChange(DatabaseOptions options)
        {
            try
            {
                _logger.LogInformation("Database connection options changed: ConnectionTimeout={ConnectionTimeout}, " +
                                      "RetryCount={RetryCount}, MaxPoolSize={MaxPoolSize}",
                                      options.ConnectionTimeout, options.RetryCount, options.MaxPoolSize);

                // 在这里可以执行连接池刷新、重新初始化数据库连接等操作
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling database configuration change");
            }
        }

        private void HandleEmailConfigurationChange(EmailServiceOptions options)
        {
            try
            {
                _logger.LogInformation("Email service options changed: Host={SmtpHost}, Port={SmtpPort}, " +
                                      "EnableSsl={EnableSsl}", options.SmtpHost, options.SmtpPort, options.EnableSsl);

                // 在这里可以重新配置邮件客户端、刷新连接等
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling email configuration change");
            }
        }

        private void HandleCacheConfigurationChange(CacheOptions options)
        {
            try
            {
                _logger.LogInformation("Cache options changed: Type={CacheType}, DefaultExpiration={DefaultExpirationSeconds}s",
                                      options.CacheType, options.DefaultExpirationSeconds);

                // 在这里可以重建缓存实例、清理缓存等
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling cache configuration change");
            }
        }

        /// <summary>
        /// 验证所有关键配置选项
        /// </summary>
        public ConfigurationValidationResult ValidateAllConfigurations()
        {
            var results = new List<ConfigurationValidationResult>();

            try
            {
                var dbOptions = GetDatabaseOptions();
                ValidateOptions(dbOptions, results);
            }
            catch (Exception ex)
            {
                results.Add(new ConfigurationValidationResult
                {
                    Category = "Database",
                    IsValid = false,
                    Errors = new[] { $"Failed to load database options: {ex.Message}" }
                });
            }

            try
            {
                var emailOptions = GetEmailOptions();
                ValidateOptions(emailOptions, results);
            }
            catch (Exception ex)
            {
                results.Add(new ConfigurationValidationResult
                {
                    Category = "Email",
                    IsValid = false,
                    Errors = new[] { $"Failed to load email options: {ex.Message}" }
                });
            }

            return new ConfigurationValidationResult
            {
                IsValid = results.All(r => r.IsValid),
                Category = "AllConfigurations",
                Errors = results.Where(r => !r.IsValid).SelectMany(r => r.Errors).ToArray()
            };
        }

        private void ValidateOptions<T>(T options, List<ConfigurationValidationResult> results) where T : class
        {
            var validationContext = new ValidationContext(options);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(options, validationContext, validationResults, true);

            results.Add(new ConfigurationValidationResult
            {
                Category = typeof(T).Name,
                IsValid = isValid,
                Errors = validationResults.Select(r => r.ErrorMessage).ToArray()
            });
        }
    }

    public class ConfigurationValidationResult
    {
        public bool IsValid { get; set; }
        public string Category { get; set; }
        public string[] Errors { get; set; } = new string[0];
    }

    // 9. 配置工厂模式 - 动态配置创建
    public interface IConfigurationFactory<T> where T : class, new()
    {
        T CreateConfiguration();
        bool TryCreateConfiguration(out T configuration);
    }

    public class DatabaseConfigurationFactory : IConfigurationFactory<DatabaseOptions>
    {
        private readonly IOptionsSnapshot<DatabaseOptions> _options;
        private readonly ILogger<DatabaseConfigurationFactory> _logger;

        public DatabaseConfigurationFactory(
            IOptionsSnapshot<DatabaseOptions> options,
            ILogger<DatabaseConfigurationFactory> logger)
        {
            _options = options;
            _logger = logger;
        }

        public DatabaseOptions CreateConfiguration()
        {
            var options = _options.Value;

            if (!ValidateConfiguration(options, out var validationErrors))
            {
                _logger.LogWarning("Database configuration validation failed: {Errors}",
                    string.Join(", ", validationErrors));
                throw new OptionsValidationException("DatabaseOptions", typeof(DatabaseOptions), validationErrors);
            }

            return options;
        }

        public bool TryCreateConfiguration(out DatabaseOptions configuration)
        {
            try
            {
                configuration = CreateConfiguration();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create database configuration");
                configuration = null;
                return false;
            }
        }

        private bool ValidateConfiguration(DatabaseOptions options, out string[] errors)
        {
            var validationContext = new ValidationContext(options);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(options, validationContext, validationResults, true);
            errors = validationResults.Select(r => r.ErrorMessage).ToArray();

            return isValid;
        }
    }

    // 10. 特殊环境配置提供程序
    public class EnvironmentSpecificConfigurationProvider<T> where T : class, new()
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public EnvironmentSpecificConfigurationProvider(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public T GetConfigurationForEnvironment()
        {
            var options = new T();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // 从特定环境的配置节绑定
            var sectionName = $"{typeof(T).Name}:{environment}";
            _configuration.GetSection(sectionName).Bind(options);

            // 再从默认配置节绑定（优先级较低）
            var defaultSectionName = typeof(T).Name;
            _configuration.GetSection(defaultSectionName).Bind(options);

            _logger.LogInformation("Loaded configuration for environment: {Environment}", environment);
            return options;
        }
    }

    // 11. 配置加密解密服务
    public interface IConfigurationEncryptionService
    {
        string DecryptValue(string encryptedValue);
        string EncryptValue(string plainValue);
        bool TryDecryptConfiguration<T>(T options, out T decryptedOptions) where T : class, new();
    }

    public class ConfigurationEncryptionService : IConfigurationEncryptionService
    {
        private readonly ILogger<ConfigurationEncryptionService> _logger;

        public ConfigurationEncryptionService(ILogger<ConfigurationEncryptionService> logger)
        {
            _logger = logger;
        }

        public string DecryptValue(string encryptedValue)
        {
            if (string.IsNullOrEmpty(encryptedValue))
                return encryptedValue;

            try
            {
                // 实际实现应该调用加密服务
                // 这里模拟解密逻辑
                if (encryptedValue.StartsWith("enc:"))
                {
                    var decrypted = encryptedValue.Substring(4); // 简单模拟解密
                    _logger.LogDebug("Decrypted configuration value");
                    return decrypted;
                }

                return encryptedValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decrypt configuration value");
                throw new InvalidOperationException("Configuration decryption failed", ex);
            }
        }

        public string EncryptValue(string plainValue)
        {
            if (string.IsNullOrEmpty(plainValue))
                return plainValue;

            // 实际实现应该调用加密服务
            return $"enc:{plainValue}"; // 简单模拟加密
        }

        public bool TryDecryptConfiguration<T>(T options, out T decryptedOptions) where T : class, new()
        {
            try
            {
                decryptedOptions = DecryptConfigurationObject(options);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decrypt configuration object of type {TypeName}",
                    typeof(T).Name);
                decryptedOptions = null;
                return false;
            }
        }

        private T DecryptConfigurationObject<T>(T options) where T : class, new()
        {
            var decryptedOptions = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var value = property.GetValue(options);
                    if (value is string stringValue && stringValue.StartsWith("enc:"))
                    {
                        var decryptedValue = DecryptValue(stringValue);
                        property.SetValue(decryptedOptions, decryptedValue);
                    }
                    else
                    {
                        property.SetValue(decryptedOptions, value);
                    }
                }
            }

            return decryptedOptions;
        }
    }

    // 12. 配置依赖验证服务
    public class ConfigurationDependencyValidator
    {
        private readonly IOptionsMonitor<DatabaseOptions> _databaseOptions;
        private readonly IOptionsMonitor<EmailServiceOptions> _emailOptions;
        private readonly IOptionsMonitor<CacheOptions> _cacheOptions;
        private readonly ILogger<ConfigurationDependencyValidator> _logger;

        public ConfigurationDependencyValidator(
            IOptionsMonitor<DatabaseOptions> databaseOptions,
            IOptionsMonitor<EmailServiceOptions> emailOptions,
            IOptionsMonitor<CacheOptions> cacheOptions,
            ILogger<ConfigurationDependencyValidator> logger)
        {
            _databaseOptions = databaseOptions;
            _emailOptions = emailOptions;
            _cacheOptions = cacheOptions;
            _logger = logger;
        }

        /// <summary>
        /// 验证配置依赖项关系
        /// </summary>
        public ConfigurationValidationResult ValidateDependencies()
        {
            var errors = new List<string>();

            try
            {
                var dbOptions = _databaseOptions.CurrentValue;
                var cacheOptions = _cacheOptions.CurrentValue;

                // 验证数据库和缓存配置的依赖关系
                if (cacheOptions.CacheType == CacheType.Redis &&
                    string.IsNullOrEmpty(cacheOptions.Redis.ConnectionString))
                {
                    errors.Add("Redis cache requires a valid connection string");
                }

                if (dbOptions.EnableRetryOnFailure && dbOptions.RetryCount <= 0)
                {
                    errors.Add("Database retry is enabled but retry count is not valid");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Configuration dependency validation error: {ex.Message}");
            }

            var isValid = !errors.Any();
            if (!isValid)
            {
                _logger.LogError("Configuration dependencies validation failed: {Errors}",
                    string.Join(", ", errors));
            }

            return new ConfigurationValidationResult
            {
                IsValid = isValid,
                Category = "ConfigurationDependencies",
                Errors = errors.ToArray()
            };
        }
    }

    // 13. 配置模块化管理服务
    [OptionsValidator]
    public partial class EmailOptionsValidator : IValidateOptions<EmailServiceOptions>
    {
    }

    [OptionsValidator]
    public partial class DatabaseOptionsValidator : IValidateOptions<DatabaseOptions>
    {
    }

    // 14. 配置健康检查服务
    public class ConfigurationHealthCheck : IHealthCheck
    {
        private readonly IOptionsMonitor<DatabaseOptions> _databaseOptions;
        private readonly IOptionsMonitor<EmailServiceOptions> _emailOptions;
        private readonly ILogger<ConfigurationHealthCheck> _logger;

        public ConfigurationHealthCheck(
            IOptionsMonitor<DatabaseOptions> databaseOptions,
            IOptionsMonitor<EmailServiceOptions> emailOptions,
            ILogger<ConfigurationHealthCheck> logger)
        {
            _databaseOptions = databaseOptions;
            _emailOptions = emailOptions;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var dbOptions = _databaseOptions.CurrentValue;
                var emailOptions = _emailOptions.CurrentValue;

                // 执行基本配置验证
                if (string.IsNullOrEmpty(dbOptions.ConnectionString))
                {
                    return Task.FromResult(HealthCheckResult.Degraded("Database connection string is missing"));
                }

                if (string.IsNullOrEmpty(emailOptions.SmtpHost))
                {
                    return Task.FromResult(HealthCheckResult.Degraded("SMTP host configuration is missing"));
                }

                _logger.LogDebug("Configuration health check passed");
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Configuration health check failed");
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Configuration validation error",
                    ex,
                    new Dictionary<string, object> { ["ErrorType"] = ex.GetType().Name }));
            }
        }
    }

    // 15. 配置使用演示服务
    public class ConfigurationDemoService
    {
        private readonly IOptions<DatabaseOptions> _databaseOptions;
        private readonly IOptionsSnapshot<EmailServiceOptions> _emailOptions;
        private readonly IOptionsMonitor<CacheOptions> _cacheOptions;
        private readonly DatabaseOptionsSetup _dbOptionsSetup;
        private readonly IConfigurationEncryptionService _encryptionService;
        private readonly ILogger<ConfigurationDemoService> _logger;

        public ConfigurationDemoService(
            IOptions<DatabaseOptions> databaseOptions,
            IOptionsSnapshot<EmailServiceOptions> emailOptions,
            IOptionsMonitor<CacheOptions> cacheOptions,
            DatabaseOptionsSetup dbOptionsSetup,
            IConfigurationEncryptionService encryptionService,
            ILogger<ConfigurationDemoService> logger)
        {
            _databaseOptions = databaseOptions;
            _emailOptions = emailOptions;
            _cacheOptions = cacheOptions;
            _dbOptionsSetup = dbOptionsSetup;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        /// <summary>
        /// 演示不同选项访问模式的使用
        /// </summary>
        public void DemonstrateConfigurationAccess()
        {
            Console.WriteLine("1. Configuration Access Patterns:");

            // IOptions - 单例，配置启动时绑定，应用生命周期不变
            var dbOptions = _databaseOptions.Value;
            Console.WriteLine($"   Database ConnectionString: {dbOptions.ConnectionString}");
            Console.WriteLine($"   Database CommandTimeout: {dbOptions.CommandTimeout}");

            // IOptionsSnapshot - 作用域单例，每次请求都会重新计算
            var emailOptions = _emailOptions.Value;
            Console.WriteLine($"   Email SMTP Host: {emailOptions.SmtpHost}");
            Console.WriteLine($"   Email SSL Enabled: {emailOptions.EnableSsl}");

            // IOptionsMonitor - 单例，配置变更时会触发回调
            var cacheOptions = _cacheOptions.CurrentValue;
            if (_cacheOptions is IOptionsSnapshot<CacheOptions> cacheSnapshot)
            {
                var cacheOptionsSnapshot = cacheSnapshot.Value;
                Console.WriteLine($"   Cache Type: {cacheOptionsSnapshot.CacheType}");
            }
        }

        /// <summary>
        /// 演示配置验证
        /// </summary>
        public ConfigurationValidationResult DemonstrateConfigurationValidation()
        {
            Console.WriteLine("\n2. Configuration Validation Demo:");

            // 后处理配置
            var dbOptions = _databaseOptions.Value;
            _dbOptionsSetup.Configure(dbOptions);

            try
            {
                _dbOptionsSetup.Validate(dbOptions);
                Console.WriteLine("   Database configuration validation: PASSED");
                return new ConfigurationValidationResult { IsValid = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Database configuration validation: FAILED - {ex.Message}");
                return new ConfigurationValidationResult
                {
                    IsValid = false,
                    Errors = new[] { ex.Message }
                };
            }
        }

        /// <summary>
        /// 演示加密配置处理
        /// </summary>
        public void DemonstrateEncryptedConfiguration()
        {
            Console.WriteLine("\n3. Encrypted Configuration Demo:");

            var emailOptions = _emailOptions.Value;
            Console.WriteLine($"   Original email password: {emailOptions.Password}");

            // 模拟加密值
            if (emailOptions.Password?.StartsWith("enc:") == true)
            {
                var decryptedPassword = _encryptionService.DecryptValue(emailOptions.Password);
                Console.WriteLine($"   Decrypted email password: {decryptedPassword}");
            }

            // 加密新值
            var encryptedValue = _encryptionService.EncryptValue("new-super-secret-password");
            Console.WriteLine($"   Encrypted sample value: {encryptedValue}");
        }

        /// <summary>
        /// 演示配置变更监控
        /// </summary>
        public void DemonstrateConfigurationMonitoring()
        {
            Console.WriteLine("\n4. Configuration Change Monitoring Demo:");

            // 这个演示需要外部触发配置变更才能看到效果
            Console.WriteLine("   Configuration change monitoring is active");
            Console.WriteLine("   Any configuration changes will be logged");
        }
    }

    // 16. 配置启动验证扩展
    public static class ConfigurationValidationExtensions
    {
        /// <summary>
        /// 添加配置验证到服务容器
        /// </summary>
        public static IServiceCollection AddConfigurationValidation(this IServiceCollection services)
        {
            services.AddSingleton<ConfigurationServiceManager>();
            services.AddSingleton<ConfigurationDependencyValidator>();

            // 添加健康检查
            services.AddHealthChecks()
                .AddCheck<ConfigurationHealthCheck>("configuration-health", tags: new[] { "configuration" });

            return services;
        }

        /// <summary>
        /// 验证关键配置启动时的有效性
        /// </summary>
        public static IServiceProvider ValidateConfigurations(this IServiceProvider serviceProvider)
        {
            var manager = serviceProvider.GetService<ConfigurationServiceManager>();
            var validator = serviceProvider.GetService<ConfigurationDependencyValidator>();
            var logger = serviceProvider.GetService<ILogger<Program>>();

            if (manager != null)
            {
                var validation = manager.ValidateAllConfigurations();
                if (!validation.IsValid)
                {
                    var errorMessages = string.Join(", ", validation.Errors);
                    logger?.LogCritical("Configuration startup validation failed: {Errors}", errorMessages);
                    throw new InvalidOperationException($"Invalid configuration: {errorMessages}");
                }
            }

            if (validator != null)
            {
                var depValidation = validator.ValidateDependencies();
                if (!depValidation.IsValid)
                {
                    var errorMessages = string.Join(", ", depValidation.Errors);
                    logger?.LogCritical("Configuration dependency validation failed: {Errors}", errorMessages);
                    throw new InvalidOperationException($"Configuration dependency error: {errorMessages}");
                }
            }

            logger?.LogInformation("All configuration validations passed successfully");
            return serviceProvider;
        }
    }

    // 17. 程序启动配置类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("Microsoft.Extensions.Options Production Demo");
            Console.WriteLine("=============================================");
            Console.WriteLine();

            #region 配置服务

            // 创建示例配置文件
            CreateSampleConfiguration(builder.Configuration);

            // 配置选项服务
            builder.Services.Configure<DatabaseOptions>(
                builder.Configuration.GetSection(DatabaseOptions.SectionName));

            builder.Services.Configure<EmailServiceOptions>(
                builder.Configuration.GetSection(EmailServiceOptions.SectionName));

            builder.Services.Configure<CacheOptions>(
                builder.Configuration.GetSection(CacheOptions.SectionName));

            builder.Services.Configure<SecurityOptions>(
                builder.Configuration.GetSection(SecurityOptions.SectionName));

            #endregion

            #region 配置验证服务

            // 添加配置验证
            builder.Services.AddConfigurationValidation();

            // 添加自定义配置后处理服务
            builder.Services.AddSingleton<DatabaseOptionsSetup>();

            // 添加加密服务
            builder.Services.AddSingleton<IConfigurationEncryptionService, ConfigurationEncryptionService>();

            // 添加演示服务
            builder.Services.AddTransient<ConfigurationDemoService>();

            #endregion

            var host = builder.Build();

            #region 演示各种配置功能

            // 启动配置验证
            host.ValidateConfigurations();

            var demoService = host.Services.GetRequiredService<ConfigurationDemoService>();

            Console.WriteLine("Demonstrating Microsoft.Extensions.Options production usage:");
            Console.WriteLine();

            // 演示配置访问模式
            demoService.DemonstrateConfigurationAccess();

            // 演示配置验证
            demoService.DemonstrateConfigurationValidation();

            // 演示加密配置处理
            demoService.DemonstrateEncryptedConfiguration();

            // 演示配置监控
            demoService.DemonstrateConfigurationMonitoring();

            // 模拟配置使用场景
            await SimulateConfigurationUsage(host.Services);

            #endregion

            Console.WriteLine("\n=== Demo Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void CreateSampleConfiguration(IConfiguration configuration)
        {
            // 在实际应用中，这些配置通常来自文件或环境变量
            // 这里模拟配置值的设置
            var configDict = new Dictionary<string, string>
            {
                // 数据库配置
                ["Database:ConnectionString"] = "Server=localhost;Database=MyProductionDB;User=appuser;",
                ["Database:CommandTimeout"] = "60",
                ["Database:MaxPoolSize"] = "50",
                ["Database:EnableRetryOnFailure"] = "true",
                ["Database:RetryCount"] = "3",
                ["Database:RetryOptions:InitialDelaySeconds"] = "2",
                ["Database:RetryOptions:MaxDelaySeconds"] = "30",

                // 邮件服务配置
                ["EmailService:SmtpHost"] = "smtp.company.com",
                ["EmailService:SmtpPort"] = "587",
                ["EmailService:Username"] = "noreply@company.com",
                ["EmailService:Password"] = "enc:s3cr3t-p4ssw0rd",
                ["EmailService:SenderEmail"] = "noreply@company.com",
                ["EmailService:SenderName"] = "My Company",

                // 缓存配置
                ["Cache:CacheType"] = "Redis",
                ["Cache:Redis:ConnectionString"] = "localhost:6379",
                ["Cache:Redis:DatabaseNumber"] = "1",

                // 安全配置
                ["Security:Jwt:Issuer"] = "https://mycompany.com",
                ["Security:Jwt:Audience"] = "https://myapp.mycompany.com",
                ["Security:Jwt:SecretKey"] = "enc:very-long-secret-key-here",
                ["Security:Jwt:ExpiryDays"] = "30"
            };

            // 模拟配置绑定
            foreach (var kvp in configDict)
            {
                Environment.SetEnvironmentVariable(kvp.Key.Replace(":", "__"), kvp.Value);
            }
        }

        private static async Task SimulateConfigurationUsage(IServiceProvider services)
        {
            Console.WriteLine("\n5. Simulating Configuration Usage in Services:");

            var dbConfigMonitor = services.GetRequiredService<IOptionsMonitor<DatabaseOptions>>();
            var emailConfigSnapshot = services.GetRequiredService<IOptionsSnapshot<EmailServiceOptions>>();
            var cacheConfig = services.GetRequiredService<IOptions<CacheOptions>>();

            try
            {
                // 模拟多次使用配置 - 展示IOptionsMonitor的实时性
                for (int i = 0; i < 3; i++)
                {
                    var dbOptions = dbConfigMonitor.CurrentValue;
                    Console.WriteLine($"   DB ConnectionTimeout: {dbOptions.ConnectionTimeout}");

                    var emailOptions = emailConfigSnapshot.Value;
                    Console.WriteLine($"   Email Timeout: {emailOptions.Timeout}");

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Configuration usage error: {ex.Message}");
            }
        }
    }

    // 核心选项接口类型
    // IOptions<T> - 单例，配置启动时绑定
    // IOptionsSnapshot<T> - 作用域单例，每次请求重新计算
    // IOptionsMonitor<T> - 单例，支持配置变更通知
    // IOptionsFactory<T> - 配置选项工厂
    // IOptionsChangeTokenSource<T> - 配置变更令牌源

    // 配置验证最佳实践
    public class DatabaseOptions
    {
        // using System.ComponentModel.DataAnnotations;

        [Required(ErrorMessage = "Connection string is required")]
        public string ConnectionString { get; set; }

        [Range(1, 300, ErrorMessage = "Command timeout must be between 1 and 300")]
        public int CommandTimeout { get; set; } = 30;


        // 配置变更监控
        _optionsMonitor.OnChange(options =>
        {
            _logger.LogWarning("Configuration changed at {Timestamp}", DateTime.UtcNow);
            HandleConfigurationChange(options);
        });

    }

// 生产环境配置使用模式
// IOptions - 静态配置：
// 适用于启动后不会改变的配置，如应用版本、静态端点等.
// IOptionsSnapshot - 请求级配置：
// 适用于每次请求都需要独立实例的配置，如用户特定配置.
// IOptionsMonitor - 动态配置：
// 适用于需要响应配置变更的场景，如数据库连接字符串、缓存策略等.

    //配置后处理和扩展
    public class DatabaseOptionsSetup : IConfigureOptions<DatabaseOptions>
    {
        public void Configure(DatabaseOptions options)
        {
            // 配置绑定后的后处理逻辑
            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                options.ConnectionString = GetDefaultConnectionString();
            }
        }

        public void Validate(DatabaseOptions options)
        {
            // 复杂业务规则验证
        }
    }

    // 环境特定配置
    public class EnvironmentSpecificConfigurationProvider<T> where T : class, new()
    {
        public T GetConfigurationForEnvironment()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var options = new T();

            // 优先级：环境特定配置 > 默认配置
            _configuration.GetSection($"{typeof(T).Name}:{environment}").Bind(options);
            _configuration.GetSection(typeof(T).Name).Bind(options);

            return options;
         }
    }

    //  安全配置处理
    public interface IConfigurationEncryptionService
    {
        string DecryptValue(string encryptedValue);
        string EncryptValue(string plainValue);
    }
    // 避免在配置中存储明文敏感信息
    // 使用加密服务处理连接字符串、API密钥等敏感数据

    // 配置依赖验证
    public class ConfigurationDependencyValidator
    {
        public ConfigurationValidationResult ValidateDependencies()
        {
            // 验证配置项之间的依赖关系
            // 确保相关的配置项保持一致性
        }
    }

    // 启动时配置验证
    public static IServiceProvider ValidateConfigurations(this IServiceProvider serviceProvider)
    {
    // 应用启动时验证关键配置的完整性
    // 防止配置错误导致应用启动失败
    }

    // 健康检查集成
    services.AddHealthChecks().AddCheck<ConfigurationHealthCheck>("configuration-health");

    public class ConfigurationHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            // 定期检查配置的有效性和服务能力
        }
    }

    // 推荐的配置结构
    string cfg = """
    {
      "Database": {
        "ConnectionString": "Server=localhost;Database=App;",
        "CommandTimeout": 30,
        "MaxPoolSize": 10,
        "EnableRetryOnFailure": true,
        "RetryCount": 3,
        "RetryOptions": {
          "InitialDelaySeconds": 1,
          "MaxDelaySeconds": 60
        }
      }
    }
    """

    // 生产环境特殊处理
    // 配置热重载：支持运行时配置更新
    // 敏感信息加密：防止配置泄露
    // 环境隔离：不同环境使用不同配置
    // 依赖验证：确保配置项的一致性
    // 健康检查：监控配置可用性

    // 配置访问生命周期
    // 接口	生命周期	变更通知	使用场景
    // IOptions<T>	单例	不支持	静态配置
    // IOptionsSnapshot<T>	作用域	不支持	请求级配置
    // IOptionsMonitor<T>	单例	支持	动态配置
}