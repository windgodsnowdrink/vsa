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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Diagnostics;

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

    // 1. 自定义日志等级 - 生产环境特有的日志等级
    public enum ProductionLogLevel
    {
        Audit = 1000,    // 审计日志，用于合规性记录
        Performance = 2000, // 性能日志，用于监控和诊断
        Business = 3000  // 业务日志，记录重要业务事件
    }

    // 2. 审计日志服务 - 专门处理审计日志的生产级服务
    public class AuditLoggingService
    {
        private readonly ILogger<AuditLoggingService> _logger;
        private readonly IUserContextService _userContext;

        public AuditLoggingService(
            ILogger<AuditLoggingService> logger,
            IUserContextService userContext)
        {
            _logger = logger;
            _userContext = userContext;
        }

        /// <summary>
        /// 记录用户操作审计日志
        /// </summary>
        public void LogUserAction(string action, object data = null, string userId = null)
        {
            var user = userId ?? _userContext.GetCurrentUserId();
            var timestamp = DateTime.UtcNow;
            var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

            // 使用审计日志等级记录
            _logger.Log(
                LogLevel.Information, // 实际映射到审计日志
                new EventId(1001, "UserAudit"),
                new AuditLogData
                {
                    UserId = user,
                    Action = action,
                    Data = data,
                    Timestamp = timestamp,
                    CorrelationId = correlationId
                },
                null,
                (state, ex) => $"AUDIT: User={state.UserId}, Action={state.Action}, " +
                             $"CorrelationId={state.CorrelationId}, Data={state.Data?.ToString() ?? "None"}");
        }

        /// <summary>
        /// 记录安全相关审计日志
        /// </summary>
        public void LogSecurityEvent(string eventType, string description, bool isSuccess, string userId = null)
        {
            var user = userId ?? _userContext.GetCurrentUserId();
            var ipAddress = _userContext.GetCurrentUserIP();

            var securityData = new SecurityLogData
            {
                EventType = eventType,
                Description = description,
                IsSuccess = isSuccess,
                UserId = user,
                IPAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            _logger.Log(
                LogLevel.Warning, // 安全事件通常使用Warning或Error等级
                new EventId(2001, "SecurityAudit"),
                securityData,
                null,
                (state, ex) => $"SECURITY: Type={state.EventType}, User={state.UserId}, " +
                             $"IP={state.IPAddress}, Success={state.IsSuccess}, Desc={state.Description}");
        }
    }

    // 3. 审计日志数据模型
    public class AuditLogData
    {
        public string UserId { get; set; }
        public string Action { get; set; }
        public object Data { get; set; }
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
        public string ApplicationName { get; set; } = "MyApp";
    }

    public class SecurityLogData
    {
        public string EventType { get; set; }
        public string Description { get; set; }
        public bool IsSuccess { get; set; }
        public string UserId { get; set; }
        public string IPAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string AdditionalInfo { get; set; }
    }

    // 4. 性能日志服务 - 用于监控关键操作性能
    public class PerformanceLoggingService
    {
        private readonly ILogger<PerformanceLoggingService> _logger;
        private readonly ITelemetryService _telemetry;

        public PerformanceLoggingService(
            ILogger<PerformanceLoggingService> logger,
            ITelemetryService telemetry)
        {
            _logger = logger;
            _telemetry = telemetry;
        }

        /// <summary>
        /// 执行操作并记录性能指标
        /// </summary>
        public async Task<T> ExecuteWithPerformanceLoggingAsync<T>(
            string operationName,
            Func<Task<T>> operation,
            bool sendTelemetry = true)
        {
            var stopwatch = Stopwatch.StartNew();
            Exception error = null;

            try
            {
                _logger.LogDebug("Starting operation '{OperationName}'", operationName);

                var result = await operation();

                stopwatch.Stop();

                // 记录成功的性能日志
                _logger.LogInformation(
                    "Operation '{OperationName}' completed successfully in {ElapsedMilliseconds}ms",
                    operationName, stopwatch.ElapsedMilliseconds);

                if (sendTelemetry && stopwatch.ElapsedMilliseconds > 1000) // 超过1秒的操作发送遥测
                {
                    await _telemetry.TrackOperationAsync(operationName, stopwatch.ElapsedMilliseconds, true);
                }

                return result;
            }
            catch (Exception ex)
            {
                error = ex;
                stopwatch.Stop();

                // 记录失败的性能日志
                _logger.LogError(
                    ex,
                    "Operation '{OperationName}' failed after {ElapsedMilliseconds}ms",
                    operationName, stopwatch.ElapsedMilliseconds);

                if (sendTelemetry)
                {
                    await _telemetry.TrackOperationAsync(operationName, stopwatch.ElapsedMilliseconds, false);
                }

                throw;
            }
            finally
            {
                // 性能监控日志
                var level = stopwatch.ElapsedMilliseconds > 5000 ? LogLevel.Error :
                           stopwatch.ElapsedMilliseconds > 1000 ? LogLevel.Warning : LogLevel.Information;

                _logger.Log(level,
                    new EventId(3001, "PerformanceMetric"),
                    new PerformanceLogData
                    {
                        OperationName = operationName,
                        DurationMs = stopwatch.ElapsedMilliseconds,
                        IsSuccess = error == null,
                        ErrorType = error?.GetType().Name ?? "None",
                        CorrelationId = Activity.Current?.Id
                    },
                    error,
                    (state, ex) => $"PERFORMANCE: Op={state.OperationName}, Duration={state.DurationMs}ms, " +
                                 $"Success={state.IsSuccess}, Error={state.ErrorType}");
            }
        }
    }

    public class PerformanceLogData
    {
        public string OperationName { get; set; }
        public long DurationMs { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorType { get; set; }
        public string CorrelationId { get; set; }
    }

    // 5. 业务日志服务 - 记录业务相关的重要事件
    public class BusinessLoggingService
    {
        private readonly ILogger<BusinessLoggingService> _logger;

        public BusinessLoggingService(ILogger<BusinessLoggingService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 记录业务事件
        /// </summary>
        public void LogBusinessEvent(BusinessEventType eventType, object data, string referenceId = null)
        {
            var eventName = eventType.ToString();
            var reference = referenceId ?? "N/A";

            _logger.LogInformation(
                new EventId((int)eventType, eventName),
                "BUSINESS_EVENT: Type={EventType}, Reference={ReferenceId}, Data={Data}",
                eventName, reference, data?.ToString());
        }

        /// <summary>
        /// 记录业务流程启动
        /// </summary>
        public void LogProcessStarted(string processName, object parameters = null)
        {
            _logger.LogTrace(
                new EventId(4001, "ProcessStarted"),
                "BUSINESS_PROCESS_STARTED: Process={ProcessName}, Parameters={Parameters}",
                processName, parameters?.ToString());
        }

        /// <summary>
        /// 记录业务流程结束
        /// </summary>
        public void LogProcessEnded(string processName, bool isSuccess, string result = null)
        {
            var logLevel = isSuccess ? LogLevel.Information : LogLevel.Error;

            _logger.Log(
                logLevel,
                new EventId(4002, "ProcessEnded"),
                "BUSINESS_PROCESS_ENDED: Process={ProcessName}, Success={IsSuccess}, Result={Result}",
                processName, isSuccess, result);
        }
    }

    public enum BusinessEventType
    {
        OrderCreated = 10001,
        UserRegistered = 10002,
        PaymentProcessed = 10003,
        ReportGenerated = 10004,
        DataExported = 10005,
        NotificationSent = 10006
    }

    // 6. 异常日志服务 - 生产环境异常详细记录
    public class ExceptionLoggingService
    {
        private readonly ILogger<ExceptionLoggingService> _logger;

        public ExceptionLoggingService(ILogger<ExceptionLoggingService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 详细记录异常信息，包括上下文和诊断信息
        /// </summary>
        public void LogException(Exception exception, string context, object additionalData = null)
        {
            // 创建异常上下文数据
            var exceptionContext = new ExceptionLogData
            {
                ExceptionType = exception.GetType().FullName,
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace,
                Context = context,
                AdditionalData = additionalData,
                MachineName = Environment.MachineName,
                ApplicationName = "MyApplication",
                Timestamp = DateTime.UtcNow,
                CorrelationId = Activity.Current?.Id ?? Guid.NewGuid().ToString()
            };

            // 记录异常日志
            _logger.LogError(
                new EventId(5001, "ApplicationException"),
                exception,
                "EXCEPTION_DETAILED: Context={Context}, Type={ExceptionType}, " +
                "Message={ExceptionMessage}, CorrelationId={CorrelationId}, Data={AdditionalData}. " +
                "Stack Trace: {StackTrace}",
                context, exceptionContext.ExceptionType, exceptionContext.ExceptionMessage,
                exceptionContext.CorrelationId, exceptionContext.AdditionalData, exceptionContext.StackTrace);
        }

        /// <summary>
        /// 记录业务异常
        /// </summary>
        public void LogBusinessException(BusinessException businessException, string operation)
        {
            var businessErrorContext = new BusinessErrorLogData
            {
                ErrorCode = businessException.ErrorCode,
                ErrorMessage = businessException.Message,
                Operation = operation,
                BusinessData = businessException.BusinessData,
                CorrelationId = Activity.Current?.Id,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogWarning(
                new EventId(businessException.ErrorCode, "BusinessError"),
                "BUSINESS_ERROR: Code={ErrorCode}, Operation={Operation}, Message={ErrorMessage}, " +
                "Data={BusinessData}, CorrelationId={CorrelationId}",
                businessException.ErrorCode, operation, businessException.Message,
                businessException.BusinessData, businessErrorContext.CorrelationId);
        }
    }

    public class ExceptionLogData
    {
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string Context { get; set; }
        public object AdditionalData { get; set; }
        public string MachineName { get; set; }
        public string ApplicationName { get; set; }
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
    }

    public class BusinessException : Exception
    {
        public int ErrorCode { get; }
        public object BusinessData { get; }

        public BusinessException(int errorCode, string message, object businessData = null)
            : base(message)
        {
            ErrorCode = errorCode;
            BusinessData = businessData;
        }
    }

    // 7. 自定义日志提供程序 - 生产环境日志存储提供程序
    public class ProductionFileLoggerProvider : ILoggerProvider
    {
        private readonly string _logFilePath;
        private readonly LogLevel _minLogLevel;
        private readonly ConcurrentDictionary<string, ProductionFileLogger> _loggers =
            new ConcurrentDictionary<string, ProductionFileLogger>();

        public ProductionFileLoggerProvider(string logFilePath, LogLevel minLogLevel = LogLevel.Information)
        {
            _logFilePath = logFilePath;
            _minLogLevel = minLogLevel;

            // 确保日志目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name =>
                new ProductionFileLogger(name, _logFilePath, _minLogLevel));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }

    // 8. 自定义文件日志记录器
    public class ProductionFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _logFilePath;
        private readonly LogLevel _minLogLevel;

        public ProductionFileLogger(string categoryName, string logFilePath, LogLevel minLogLevel)
        {
            _categoryName = categoryName;
            _logFilePath = logFilePath;
            _minLogLevel = minLogLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new LoggerScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            // 生产环境日志格式
            var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] " +
                           $"[{logLevel}] " +
                           $"[{_categoryName}] " +
                           $"[{eventId.Id}:{eventId.Name}] " +
                           $"{message}" +
                           (exception != null ? $" ExceptionType: {exception.GetType().Name}, " +
                                              $"StackTrace: {exception.StackTrace}" : "") +
                           Environment.NewLine;

            try
            {
                // 异步写入日志文件
                File.AppendAllText(_logFilePath, logEntry);
            }
            catch (Exception ex)
            {
                // fallback到控制台，避免日志丢失
                Console.Error.WriteLine($"Failed to write to log file: {ex.Message}");
                Console.Error.WriteLine(logEntry);
            }
        }
    }

    public class LoggerScope : IDisposable
    {
        private readonly object _state;

        public LoggerScope(object state)
        {
            _state = state;
        }

        public void Dispose()
        {
            // 清理scope资源
        }
    }

    // 9. 日志过滤和配置服务
    public static class LoggingConfiguration
    {
        /// <summary>
        /// 配置生产环境日志服务
        /// </summary>
        public static void ConfigureProductionLogging(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                #region 基础日志级别配置

                // 从配置读取最小日志级别
                var defaultLogLevel = configuration.GetValue<LogLevel>("Logging:DefaultLevel", LogLevel.Information);

                builder.SetMinimumLevel(defaultLogLevel);

                #endregion

                #region 控制台日志配置

                builder.AddConsole(options =>
                {
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                    options.UseUtcTimestamp = true;
                })
                .AddConsoleFormatter<JsonConsoleFormatter, ConsoleFormatterOptions>(options =>
                {
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
                    options.UseUtcTimestamp = true;
                });

                #endregion

                #region 文件日志配置

                var logFilePath = configuration.GetValue<string>("Logging:FilePath", "./logs/app.log");
                var fileInfoLogLevel = configuration.GetValue<LogLevel>("Logging:FileInfoLevel", LogLevel.Information);

                builder.AddProvider(new ProductionFileLoggerProvider(logFilePath, fileInfoLogLevel));

                #endregion

                #region 日志过滤规则配置

                // 为特定命名空间设置不同的日志级别
                builder.AddFilter("Microsoft", LogLevel.Warning)
                       .AddFilter("System", LogLevel.Warning)
                       .AddFilter("MyApp.Services", LogLevel.Information)
                       .AddFilter("MyApp.Controllers", LogLevel.Information)
                       .AddFilter("MyApp.DataAccess", LogLevel.Debug) // 数据访问可以记录更详细信息
                       .AddFilter("MyApp.Security", LogLevel.Trace);  // 安全相关需要最详细日志

                #endregion

                #region 生产环境特殊配置

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                {
                    // 生产环境中禁用详细调试日志
                    builder.AddFilter(level => level >= LogLevel.Information);

                    // 启用结构化日志
                    builder.AddJsonConsole(options =>
                    {
                        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
                        options.UseUtcTimestamp = true;
                        options.IncludeScopes = true;
                    });
                }

                #endregion
            });
        }

        /// <summary>
        /// 配置日志增强服务
        /// </summary>
        public static void ConfigureLoggingEnhancements(IServiceCollection services)
        {
            // 注册日志相关服务
            services.AddTransient<AuditLoggingService>();
            services.AddTransient<PerformanceLoggingService>();
            services.AddTransient<BusinessLoggingService>();
            services.AddTransient<ExceptionLoggingService>();
            services.AddSingleton<ILogAggregator, LogAggregator>();
            services.AddSingleton<ILogHealthMonitor, LogHealthMonitor>();
        }
    }

    // 10. 日志聚合器 - 用于统计和监控日志使用情况
    public interface ILogAggregator
    {
        void RecordLog(LogLevel level, string category);
        LogStatistics GetStatistics();
        void ResetStatistics();
    }

    public class LogAggregator : ILogAggregator
    {
        private readonly ConcurrentDictionary<string, long> _logCounters = new ConcurrentDictionary<string, long>();

        public void RecordLog(LogLevel level, string category)
        {
            var key = $"{category}:{level}";
            _logCounters.AddOrUpdate(key, 1, (k, v) => v + 1);

            // 每记录1000条日志输出一次统计（用于监控日志量）
            if (_logCounters.Values.Sum() % 1000 == 0)
            {
                Console.WriteLine($"Log aggregator: Total logs recorded {_logCounters.Values.Sum()}");
            }
        }

        public LogStatistics GetStatistics()
        {
            return new LogStatistics
            {
                TotalLogs = _logCounters.Values.Sum(),
                LevelDistribution = _logCounters
                    .Where(kvp => kvp.Key.Contains(':'))
                    .GroupBy(kvp => kvp.Key.Split(':')[1])
                    .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value)),
                CategoryDistribution = _logCounters
                    .Where(kvp => kvp.Key.Contains(':'))
                    .GroupBy(kvp => kvp.Key.Split(':')[0])
                    .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value))
            };
        }

        public void ResetStatistics()
        {
            _logCounters.Clear();
        }
    }

    public class LogStatistics
    {
        public long TotalLogs { get; set; }
        public Dictionary<string, long> LevelDistribution { get; set; } = new Dictionary<string, long>();
        public Dictionary<string, long> CategoryDistribution { get; set; } = new Dictionary<string, long>();
    }

    // 11. 日志健康监控服务
    public interface ILogHealthMonitor
    {
        Task<bool> CheckLogHealthAsync();
        void ReportLogPerformance(long durationMs, bool success);
    }

    public class LogHealthMonitor : ILogHealthMonitor
    {
        private readonly ILogger<LogHealthMonitor> _logger;
        private readonly ConcurrentQueue<LogPerformanceData> _performanceData = new ConcurrentQueue<LogPerformanceData>();
        private const int MAX_PERFORMANCE_RECORDS = 1000;

        public LogHealthMonitor(ILogger<LogHealthMonitor> logger)
        {
            _logger = logger;
        }

        public async Task<bool> CheckLogHealthAsync()
        {
            try
            {
                // 模拟日志健康检查
                await Task.Delay(10);

                // 检查最近的日志性能数据
                var recentLogs = _performanceData.Take(100).ToList();
                var slowLogs = recentLogs.Count(x => x.DurationMs > 5000);

                if (slowLogs > recentLogs.Count * 0.1) // 超过10%的慢日志
                {
                    _logger.LogWarning("High ratio of slow logs detected: {SlowLogPercentage}%",
                        (slowLogs / (double)recentLogs.Count) * 100);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Log health check failed");
                return false;
            }
        }

        public void ReportLogPerformance(long durationMs, bool success)
        {
            var data = new LogPerformanceData
            {
                DurationMs = durationMs,
                Success = success,
                Timestamp = DateTime.UtcNow
            };

            _performanceData.Enqueue(data);

            // 限制队列大小，避免内存泄漏
            while (_performanceData.Count > MAX_PERFORMANCE_RECORDS)
            {
                _performanceData.TryDequeue(out _);
            }

            // 监控极慢的日志写入操作
            if (durationMs > 10000) // 超过10秒
            {
                _logger.LogError("Extremely slow log operation detected: {DurationMs}ms, Success: {Success}",
                    durationMs, success);
            }
            else if (durationMs > 5000) // 超过5秒
            {
                _logger.LogWarning("Slow log operation detected: {DurationMs}ms, Success: {Success}",
                    durationMs, success);
            }
        }
    }

    public class LogPerformanceData
    {
        public long DurationMs { get; set; }
        public bool Success { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // 12. 用户上下文服务 - 模拟用户身份和上下文获取
    public interface IUserContextService
    {
        string GetCurrentUserId();
        string GetCurrentUserIP();
        Dictionary<string, object> GetCurrentContext();
    }

    public class UserContextService : IUserContextService
    {
        public string GetCurrentUserId()
        {
            // 在实际应用中，从认证服务或上下文中获取用户ID
            return "SYSTEM_USER"; // 模拟系统用户
        }

        public string GetCurrentUserIP()
        {
            // 在实际应用中，从HttpContext获取客户端IP
            return "127.0.0.1";
        }

        public Dictionary<string, object> GetCurrentContext()
        {
            return new Dictionary<string, object>
            {
                ["UserId"] = GetCurrentUserId(),
                ["IPAddress"] = GetCurrentUserIP(),
                ["SessionId"] = Guid.NewGuid().ToString(),
                ["Timestamp"] = DateTime.UtcNow
            };
        }
    }

    // 13. 遥测服务接口 - 用于日志数据发送到监控系统
    public interface ITelemetryService
    {
        Task TrackOperationAsync(string operationName, long durationMs, bool isSuccess);
        Task TrackExceptionAsync(Exception exception, string context);
        Task TrackBusinessEventAsync(string eventName, object data);
    }

    public class TelemetryService : ITelemetryService
    {
        private readonly ILogger<TelemetryService> _logger;

        public TelemetryService(ILogger<TelemetryService> logger)
        {
            _logger = logger;
        }

        public async Task TrackOperationAsync(string operationName, long durationMs, bool isSuccess)
        {
            // 在生产环境中发送到APM工具（如Application Insights）
            _logger.LogDebug("Telemetry operation tracked: {OperationName}, Duration: {DurationMs}ms, Success: {IsSuccess}",
                operationName, durationMs, isSuccess);

            await Task.CompletedTask; // 模拟异步发送
        }

        public async Task TrackExceptionAsync(Exception exception, string context)
        {
            _logger.LogDebug("Telemetry exception tracked: {ExceptionType}, Context: {Context}",
                exception.GetType().Name, context);

            await Task.CompletedTask;
        }

        public async Task TrackBusinessEventAsync(string eventName, object data)
        {
            _logger.LogDebug("Telemetry business event tracked: {EventName}, Data: {Data}",
                eventName, data?.ToString());

            await Task.CompletedTask;
        }
    }

    // 14. 演示控制器服务 - 展示如何在业务逻辑中使用各种日志服务
    public class DemoBusinessService
    {
        private readonly AuditLoggingService _auditLogger;
        private readonly PerformanceLoggingService _perfLogger;
        private readonly BusinessLoggingService _businessLogger;
        private readonly ExceptionLoggingService _exceptionLogger;
        private readonly ILogger<DemoBusinessService> _logger;

        public DemoBusinessService(
            AuditLoggingService auditLogger,
            PerformanceLoggingService perfLogger,
            BusinessLoggingService businessLogger,
            ExceptionLoggingService exceptionLogger,
            ILogger<DemoBusinessService> logger)
        {
            _auditLogger = auditLogger;
            _perfLogger = perfLogger;
            _businessLogger = businessLogger;
            _exceptionLogger = exceptionLogger;
            _logger = logger;
        }

        /// <summary>
        /// 演示完整的用户注册流程，包含各种日志类型
        /// </summary>
        public async Task<bool> RegisterUserAsync(string userName, string email, int age)
        {
            #region 审计日志

            _auditLogger.LogUserAction("UserRegistrationStarted", new { UserName = userName, Email = email });

            #endregion

            #region 业务流程日志

            _businessLogger.LogProcessStarted("UserRegistration", new { UserName = userName });

            #endregion

            try
            {
                #region 性能监控操作

                var result = await _perfLogger.ExecuteWithPerformanceLoggingAsync(
                    "UserRegistration",
                    async () =>
                    {
                        // 模拟用户验证
                        await ValidateUserAsync(userName, email, age);

                        // 模拟用户创建
                        var user = await CreateUserAsync(userName, email, age);

                        // 模拟邮件发送
                        await SendWelcomeEmailAsync(user);

                        return true;
                    });

                #endregion

                #region 业务事件日志

                _businessLogger.LogBusinessEvent(
                    BusinessEventType.UserRegistered,
                    new { UserId = 1, UserName = userName, Email = email },
                    "1");

                #endregion

                #region 业务流程结束日志

                _businessLogger.LogProcessEnded("UserRegistration", true, "User created successfully");

                #endregion

                #region 审计日志结束

                _auditLogger.LogUserAction("UserRegistrationCompleted", new { UserId = 1, UserName = userName });

                #endregion

                return result;
            }
            catch (BusinessException ex)
            {
                #region 业务异常日志

                _exceptionLogger.LogBusinessException(ex, "RegisterUser");
                _businessLogger.LogProcessEnded("UserRegistration", false, ex.Message);

                #endregion

                _logger.LogError(ex, "Business error during user registration for {UserName}", userName);
                return false;
            }
            catch (Exception ex)
            {
                #region 系统异常日志

                _exceptionLogger.LogException(ex, "UserRegistration", new { UserName = userName });
                _businessLogger.LogProcessEnded("UserRegistration", false, "System error occurred");

                #endregion

                _logger.LogError(ex, "System error during user registration for {UserName}", userName);
                throw; // 重新抛出异常给上层处理
            }
        }

        private async Task ValidateUserAsync(string userName, string email, int age)
        {
            await Task.Delay(100); // 模拟验证延迟

            _logger.LogDebug("Validating user: {UserName}, {Email}, {Age}", userName, email, age);

            if (string.IsNullOrEmpty(userName))
            {
                throw new BusinessException(1001, "Username is required", new { Email = email });
            }

            if (age < 18)
            {
                throw new BusinessException(1002, "User must be at least 18 years old",
                    new { UserName = userName, Age = age });
            }
        }

        private async Task<User> CreateUserAsync(string userName, string email, int age)
        {
            await Task.Delay(200); // 模拟创建延迟

            _logger.LogInformation("Creating user: {UserName}", userName);

            var user = new User
            {
                Id = 1,
                Name = userName,
                Email = email,
                Age = age,
                CreatedAt = DateTime.UtcNow
            };

            return user;
        }

        private async Task SendWelcomeEmailAsync(User user)
        {
            await Task.Delay(300); // 模拟邮件发送延迟

            _logger.LogDebug("Sending welcome email to user {UserId}", user.Id);

            // 模拟可能的邮件发送失败
            if (new Random().Next(10) == 0) // 10%概率失败
            {
                _logger.LogWarning("Welcome email failed to send for user {UserId}", user.Id);
            }
            else
            {
                _logger.LogTrace("Welcome email sent successfully to {UserEmail}", user.Email);
            }
        }
    }

    // 15. 用户实体类
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // 16. 主程序类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("Microsoft.Extensions.Logging Production Demo");
            Console.WriteLine("=============================================");
            Console.WriteLine();

            #region 配置服务

            // 配置基础服务
            builder.Services.AddSingleton<IUserContextService, UserContextService>();
            builder.Services.AddSingleton<ITelemetryService, TelemetryService>();

            // 配置日志服务
            LoggingConfiguration.ConfigureProductionLogging(
                builder.Services,
                builder.Configuration);

            LoggingConfiguration.ConfigureLoggingEnhancements(builder.Services);

            #endregion

            var host = builder.Build();

            #region 演示各种生产级日志功能

            var businessService = host.Services.GetRequiredService<DemoBusinessService>();
            var logAggregator = host.Services.GetRequiredService<ILogAggregator>();
            var perfLogger = host.Services.GetRequiredService<PerformanceLoggingService>();

            Console.WriteLine("1. Service Configuration:");
            Console.WriteLine("   - Configured structured JSON console logging");
            Console.WriteLine("   - Added custom ProductionFileLoggerProvider");
            Console.WriteLine("   - Set up log filtering rules");
            Console.WriteLine("   - Registered audit and business log services");

            Console.WriteLine("\n2. Comprehensive Logging Demo:");

            // 演示成功用户注册
            Console.WriteLine("   Testing successful user registration:");
            var success = await businessService.RegisterUserAsync("John Doe", "john@example.com", 25);
            Console.WriteLine($"   Registration result: {(success ? "SUCCESS" : "FAILED")}");

            // 演示失败用户注册（业务异常）
            Console.WriteLine("\n   Testing failed user registration (business validation):");
            var failedValidation = await businessService.RegisterUserAsync("", "invalid-email", 15);
            Console.WriteLine($"   Registration result: {(failedValidation ? "SUCCESS" : "FAILED")}");

            // 演示性能监控
            Console.WriteLine("\n3. Performance Monitoring Demo:");
            await perfLogger.ExecuteWithPerformanceLoggingAsync(
                "DataProcessing",
                async () =>
                {
                    await Task.Delay(1500); // 模拟耗时操作
                    Console.WriteLine("   Long-running operation completed");
                    return "Processing result";
                });

            #endregion

            Console.WriteLine("\n=== Demo Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }


        public void Test()
        {
            // 生产环境日志配置
            builder.AddLogging(loggingBuilder =>
            {
                // 设置生产环境最小日志级别
                loggingBuilder.SetMinimumLevel(LogLevel.Information);

                // 配置过滤规则
                loggingBuilder.AddFilter("Microsoft", LogLevel.Warning)
                                .AddFilter("System", LogLevel.Warning);

                // 启用结构化日志
                loggingBuilder.AddJsonConsole();
            });

            // 结构化日志记录
            _logger.LogInformation(
                new EventId(1001, "UserAction"),
                "User {UserId} performed action {Action} at {Timestamp}",
                userId, action, DateTime.UtcNow);

            // 异常详细记录
            try
            {
                await operation();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed with exception {ExceptionType}: {ExceptionMessage}",
                    ex.GetType().Name, ex.Message);

                // 添加额外上下文信息
                _logger.LogError(ex, "Context: {ContextData}", new { UserId = userId, Operation = "ProcessOrder" });

                throw;
            }

            // 性能监控日志
            var stopwatch = Stopwatch.StartNew();
            try
            {
                // 执行操作
            }
            finally
            {
                stopwatch.Stop();
                _logger.Log(level, "Operation {OperationName} took {DurationMs}ms",
                    operationName, stopwatch.ElapsedMilliseconds);
            }

            // 日志配置示例
            string cfg = """
            {
                "Logging": {
                "DefaultLevel": "Information",
                "FilePath": "./logs/application.log",
                "FileInfoLevel": "Warning",
                "LogLevel": {
                    "MyApp": "Information",
                    "Microsoft": "Warning",
                    "Microsoft.Hosting.Lifetime": "Information"
                }
                }
            }
            """
        }

        // 日志健康检查
        public interface ILogHealthMonitor
        {
            Task<bool> CheckLogHealthAsync();
            void ReportLogPerformance(long durationMs, bool success);
        }
    }

    // 日志级别和分类
    // Trace (0) - 最详细，通常仅在开发环境中使用
    // Debug (1) - 调试信息，生产环境中可能禁用
    // Information (2) - 一般信息，记录业务流程
    // Warning (3) - 警告信息，不影响功能但仍需关注
    // Error (4) - 错误信息，功能受到影响
    // Critical (5) - 严重错误，需要立即处理

    // 生产级日志最佳实践
    // 日志内容安全：
    // 不要记录密码或敏感信息
    // 对敏感数据进行脱敏处理
    // 防止日志注入攻击
    // 性能考虑：
    // 避免昂贵的日志消息格式化
    // 使用Log[Level]方法的重载版本
    // 推荐：惰性格式化
    // _logger.LogInformation("User {UserId} action took {Duration}ms", userId, duration);

    // 不推荐：提前格式化
    // _logger.LogInformation($"User {userId} action took {duration}ms");
    // 日志轮转和管理
    // 文件日志应该实施轮转策略
    // 可以使用第三方库如Serilog实现

    // 审计日志模式
    // 记录谁在什么时候做了什么
    // 包含相关联的操作ID（CorrelationId）
    // 使用特定的日志等级和EventId

    // 生产环境特殊处理
    // 启用日志轮转和压缩
    // 配置合适的日志保留策略
    // 实施日志服务器部署
    // 启用遥测和监控集成
}