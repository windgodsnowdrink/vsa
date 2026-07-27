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
using System;
using System.Threading.Tasks;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
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

    // 1. 数据模型定义
    public class MessageData
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString("N");
        public string MessageType { get; set; }
        public object Data { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        // 计算属性
        public bool IsValid => !string.IsNullOrEmpty(MessageType);

        public TimeSpan Age => DateTime.UtcNow - CreatedAt;
    }

    public class ProcessingResult<T>
    {
        public string MessageId { get; set; }
        public bool Success { get; set; }
        public T Result { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan ProcessingDuration { get; set; }
    }

    // 2. 异常定义
    public class ChannelProcessingException : Exception
    {
        public string ChannelName { get; }
        public string MessageId { get; }

        public ChannelProcessingException(string channelName, string messageId, string message)
            : base(message)
        {
            ChannelName = channelName;
            MessageId = messageId;
        }

        public ChannelProcessingException(string channelName, string messageId, string message, Exception innerException)
            : base(message, innerException)
        {
            ChannelName = channelName;
            MessageId = messageId;
        }
    }

    public class ChannelConfigurationException : Exception
    {
        public string ChannelName { get; }
        public string ConfigurationError { get; }

        public ChannelConfigurationException(string channelName, string error)
            : base($"Channel configuration error for {channelName}: {error}")
        {
            ChannelName = channelName;
            ConfigurationError = error;
        }
    }

    // 3. 生产级通道配置管理
    public class ChannelConfiguration
    {
        public string ChannelName { get; set; }
        public int Capacity { get; set; } = 1000;
        public bool FullModeDropWrite { get; set; } = false; // false表示阻塞写入
        public int MaxConcurrency { get; set; } = 10;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        public bool EnableLogging { get; set; } = true;
        public bool EnableMetrics { get; set; } = true;
        public int BatchSize { get; set; } = 1;
        public TimeSpan MaxProcessingTime { get; set; } = TimeSpan.FromMinutes(5);

        // 验证配置
        public bool Validate()
        {
            return !string.IsNullOrEmpty(ChannelName) &&
                   Capacity > 0 &&
                   MaxConcurrency > 0 &&
                   Timeout > TimeSpan.Zero &&
                   BatchSize > 0;
        }
    }

    // 4. 通道监控和遥测服务
    public class ChannelTelemetryService
    {
        private readonly ILogger<ChannelTelemetryService> _logger;
        private readonly Dictionary<string, ChannelMetrics> _channelMetrics =
            new Dictionary<string, ChannelMetrics>();

        public ChannelTelemetryService(ILogger<ChannelTelemetryService> logger)
        {
            _logger = logger;
        }

        public async Task TrackWriteAsync(string channelName, TimeSpan duration, bool success)
        {
            var metrics = GetOrAddMetrics(channelName);
            metrics.TotalWrites++;
            metrics.TotalWriteDuration += duration.TotalMilliseconds;

            if (success)
            {
                metrics.SuccessfulWrites++;
            }
            else
            {
                metrics.FailedWrites++;
            }

            metrics.LastWriteTime = DateTime.UtcNow;
            metrics.PendingMessages = Math.Max(0, metrics.PendingMessages + (success ? 1 : 0));

            _logger.LogDebug("TRACKING Write to channel {ChannelName} - Duration: {Duration}ms, Success: {Success}",
                channelName, duration.TotalMilliseconds, success);
        }

        public async Task TrackReadAsync(string channelName, TimeSpan duration, bool success)
        {
            var metrics = GetOrAddMetrics(channelName);
            metrics.TotalReads++;
            metrics.TotalReadDuration += duration.TotalMilliseconds;

            if (success)
            {
                metrics.SuccessfulReads++;
                metrics.PendingMessages = Math.Max(0, metrics.PendingMessages - 1);
            }
            else
            {
                metrics.FailedReads++;
            }

            metrics.LastReadTime = DateTime.UtcNow;
        }

        public async Task TrackProcessingAsync(string channelName, TimeSpan duration, bool success)
        {
            var metrics = GetOrAddMetrics(channelName);
            metrics.TotalProcessings++;
            metrics.TotalProcessingDuration += duration.TotalMilliseconds;

            if (success)
            {
                metrics.SuccessfulProcessings++;
            }
            else
            {
                metrics.FailedProcessings++;
            }

            _logger.LogDebug("TRACKING Processing in channel {ChannelName} - Duration: {Duration}ms, Success: {Success}",
                channelName, duration.TotalMilliseconds, success);
        }

        public ChannelMetrics GetMetrics(string channelName)
        {
            return _channelMetrics.GetValueOrDefault(channelName, new ChannelMetrics());
        }

        public Dictionary<string, ChannelMetrics> GetAllMetrics()
        {
            return new Dictionary<string, ChannelMetrics>(_channelMetrics);
        }

        public void ResetMetrics(string channelName = null)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                _channelMetrics.Clear();
                _logger.LogInformation("All channel metrics reset");
            }
            else
            {
                _channelMetrics.Remove(channelName);
                _logger.LogInformation("Channel metrics reset for {ChannelName}", channelName);
            }
        }

        private ChannelMetrics GetOrAddMetrics(string channelName)
        {
            if (!_channelMetrics.TryGetValue(channelName, out var metrics))
            {
                metrics = new ChannelMetrics { ChannelName = channelName };
                _channelMetrics[channelName] = metrics;
            }

            return metrics;
        }
    }

    public class ChannelMetrics
    {
        public string ChannelName { get; set; }
        public long TotalWrites { get; set; }
        public long SuccessfulWrites { get; set; }
        public long FailedWrites { get; set; }
        public double TotalWriteDuration { get; set; }
        public long TotalReads { get; set; }
        public long SuccessfulReads { get; set; }
        public long FailedReads { get; set; }
        public double TotalReadDuration { get; set; }
        public long TotalProcessings { get; set; }
        public long SuccessfulProcessings { get; set; }
        public long FailedProcessings { get; set; }
        public double TotalProcessingDuration { get; set; }
        public DateTime LastWriteTime { get; set; }
        public DateTime LastReadTime { get; set; }
        public int PendingMessages { get; set; }

        public double AverageWriteDuration => TotalWrites > 0 ?
            TotalWriteDuration / TotalWrites : 0;

        public double AverageReadDuration => TotalReads > 0 ?
            TotalReadDuration / TotalReads : 0;

        public double AverageProcessingDuration => TotalProcessings > 0 ?
            TotalProcessingDuration / TotalProcessings : 0;

        public double WriteSuccessRate => TotalWrites > 0 ?
            (double)SuccessfulWrites / TotalWrites * 100 : 0;

        public double ReadSuccessRate => TotalReads > 0 ?
            (double)SuccessfulReads / TotalReads * 100 : 0;

        public double ProcessingSuccessRate => TotalProcessings > 0 ?
            (double)SuccessfulProcessings / TotalProcessings * 100 : 0;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // 5. 生产级通道工厂服务
    public interface IChannelFactory
    {
        Channel<T> CreateChannel<T>(ChannelConfiguration config);
        BoundedChannelOptions CreateBoundedOptions(ChannelConfiguration config);
        UnboundedChannelOptions CreateUnboundedOptions(ChannelConfiguration config);
        Task<bool> ValidateChannelAsync<T>(Channel<T> channel, CancellationToken cancellationToken = default);
    }

    public class ChannelFactory : IChannelFactory
    {
        private readonly ILogger<ChannelFactory> _logger;

        public ChannelFactory(ILogger<ChannelFactory> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 创建配置化的通道实例
        /// </summary>
        public Channel<T> CreateChannel<T>(ChannelConfiguration config)
        {
            _logger.LogInformation("Creating channel {ChannelName} with capacity {Capacity}",
                config.ChannelName, config.Capacity);

            if (!config.Validate())
            {
                throw new ChannelConfigurationException(config.ChannelName, "Invalid configuration parameters");
            }

            // 根据配置决定创建有界或无界通道
            if (config.Capacity == int.MaxValue)
            {
                var options = CreateUnboundedOptions(config);
                return Channel.CreateUnbounded<T>(options);
            }
            else
            {
                var options = CreateBoundedOptions(config);
                return Channel.CreateBounded<T>(options);
            }
        }

        /// <summary>
        /// 创建有界通道配置选项
        /// </summary>
        public BoundedChannelOptions CreateBoundedOptions(ChannelConfiguration config)
        {
            var options = new BoundedChannelOptions(config.Capacity)
            {
                FullMode = config.FullModeDropWrite ?
                    BoundedChannelFullMode.DropWrite :
                    BoundedChannelFullMode.Wait, // 阻塞写入直到有空间

                // 单读者单写者模式适用于高并发场景
                SingleReader = false,
                SingleWriter = false
            };

            return options;
        }

        /// <summary>
        /// 创建无界通道配置选项
        /// </summary>
        public UnboundedChannelOptions CreateUnboundedOptions(ChannelConfiguration config)
        {
            return new UnboundedChannelOptions
            {
                // 单读者单写者模式优化性能
                SingleReader = false,
                SingleWriter = false
            };
        }

        /// <summary>
        /// 验证通道可用性
        /// </summary>
        public async Task<bool> ValidateChannelAsync<T>(Channel<T> channel, CancellationToken cancellationToken = default)
        {
            if (channel == null)
            {
                _logger.LogWarning("Channel validation failed - channel is null");
                return false;
            }

            try
            {
                // 测试写入能力
                var writeSuccess = channel.Writer.TryWrite(default(T));
                if (writeSuccess)
                {
                    // 测试读取能力
                    var readerTask = channel.Reader.WaitToReadAsync(cancellationToken);
                    var canRead = await readerTask;

                    _logger.LogDebug("Channel validation result: Write={WriteSuccess}, Read={ReadAvailable}",
                        writeSuccess, canRead);

                    return true;
                }

                _logger.LogWarning("Channel validation failed - unable to write test message");
                return false;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Channel validation cancelled");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Channel validation error");
                return false;
            }
        }
    }

    // 6. 通道消息处理器服务
    public class ChannelMessageProcessor
    {
        private readonly ILogger<ChannelMessageProcessor> _logger;
        private readonly ChannelTelemetryService _telemetryService;
        private readonly SemaphoreSlim _processingSemaphore;

        public ChannelMessageProcessor(
            ILogger<ChannelMessageProcessor> logger,
            ChannelTelemetryService telemetryService,
            int maxConcurrentProcessors = 20)
        {
            _logger = logger;
            _telemetryService = telemetryService;
            _processingSemaphore = new SemaphoreSlim(maxConcurrentProcessors, maxConcurrentProcessors);
        }

        /// <summary>
        /// 异步处理通道消息
        /// </summary>
        public async Task<ProcessingResult<T>> ProcessMessageAsync<T>(
            MessageData message,
            Func<MessageData, Task<T>> processor,
            string channelName,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Processing message {MessageId} in channel {ChannelName}",
                message.MessageId, channelName);

            await _processingSemaphore.WaitAsync(cancellationToken);

            var startTime = DateTime.UtcNow;
            var duration = TimeSpan.Zero;

            try
            {
                // 执行消息处理逻辑
                var result = await processor(message);
                duration = DateTime.UtcNow - startTime;

                await _telemetryService.TrackProcessingAsync(channelName, duration, true);

                _logger.LogDebug("Message {MessageId} processed successfully in {Duration}ms",
                    message.MessageId, duration.TotalMilliseconds);

                return new ProcessingResult<T>
                {
                    MessageId = message.MessageId,
                    Success = true,
                    Result = result,
                    ProcessingDuration = duration
                };
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                duration = DateTime.UtcNow - startTime;
                await _telemetryService.TrackProcessingAsync(channelName, duration, false);

                _logger.LogWarning("Message processing cancelled - MessageId: {MessageId}", message.MessageId);
                return new ProcessingResult<T>
                {
                    MessageId = message.MessageId,
                    Success = false,
                    ErrorMessage = "Processing cancelled",
                    ProcessingDuration = duration
                };
            }
            catch (Exception ex)
            {
                duration = DateTime.UtcNow - startTime;
                await _telemetryService.TrackProcessingAsync(channelName, duration, false);

                _logger.LogError(ex, "Error processing message {MessageId}", message.MessageId);

                return new ProcessingResult<T>
                {
                    MessageId = message.MessageId,
                    Success = false,
                    ErrorMessage = ex.Message,
                    ProcessingDuration = duration
                };
            }
            finally
            {
                _processingSemaphore.Release();
            }
        }

        /// <summary>
        /// 批量处理通道消息
        /// </summary>
        public async Task<List<ProcessingResult<T>>> ProcessBatchMessagesAsync<T>(
            List<MessageData> messages,
            Func<MessageData, Task<T>> processor,
            string channelName,
            int maxConcurrency = 5,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Processing batch of {MessageCount} messages in channel {ChannelName}",
                messages.Count, channelName);

            var batchSemaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
            var processingTasks = new List<Task<ProcessingResult<T>>>();

            try
            {
                foreach (var message in messages)
                {
                    var task = Task.Run(async () =>
                    {
                        await batchSemaphore.WaitAsync(cancellationToken);
                        try
                        {
                            return await ProcessMessageAsync(message, processor, channelName, cancellationToken);
                        }
                        finally
                        {
                            batchSemaphore.Release();
                        }
                    }, cancellationToken);

                    processingTasks.Add(task);
                }

                var results = await Task.WhenAll(processingTasks);

                _logger.LogInformation("Batch processing completed - Channel: {ChannelName}, Messages: {MessageCount}",
                    channelName, messages.Count);

                return results.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during batch message processing in channel {ChannelName}", channelName);
                throw new ChannelProcessingException(channelName, "batch", "Batch processing failed", ex);
            }
        }
    }

    // 7. 生产级通道生产者服务
    public class ChannelProducerService
    {
        private readonly ILogger<ChannelProducerService> _logger;
        private readonly ChannelTelemetryService _telemetryService;
        private readonly SemaphoreSlim _channelAccessSemaphore = new SemaphoreSlim(10, 10);

        public ChannelProducerService(
            ILogger<ChannelProducerService> logger,
            ChannelTelemetryService telemetryService)
        {
            _logger = logger;
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// 向通道发送消息
        /// </summary>
        public async Task<bool> SendMessageAsync<T>(
            Channel<T> channel,
            T message,
            string channelName,
            CancellationToken cancellationToken = default,
            TimeSpan timeout = default)
        {
            if (timeout == default)
                timeout = TimeSpan.FromSeconds(30);

            _logger.LogDebug("Sending message to channel {ChannelName}", channelName);

            await _channelAccessSemaphore.WaitAsync(cancellationToken);

            var startTime = DateTime.UtcNow;
            var duration = TimeSpan.Zero;

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(timeout);

                var writeTask = channel.Writer.WriteAsync(message, cts.Token);
                await writeTask;

                duration = DateTime.UtcNow - startTime;
                await _telemetryService.TrackWriteAsync(channelName, duration, true);

                _logger.LogDebug("Message sent successfully to channel {ChannelName} in {Duration}ms",
                    channelName, duration.TotalMilliseconds);

                return true;
            }
            catch (OperationCanceledException canceledEx) when (cancellationToken.IsCancellationRequested)
            {
                duration = DateTime.UtcNow - startTime;
                await _telemetryService.TrackWriteAsync(channelName, duration, false);

                _logger.LogWarning(canceledEx, "Message sending cancelled to channel {ChannelName}", channelName);
                return false;
            }
            catch (ChannelClosedException closedEx)
            {
                duration = DateTime.UtcNow - startTime;
                await _telemetryService.TrackWriteAsync(channelName, duration, false);

                _logger.LogError(closedEx, "Channel {ChannelName} is closed - cannot write message", channelName);
                return false;
            }
            catch (Exception ex)
            {
                duration = DateTime.UtcNow - startTime;
                await _telemetryService.TrackWriteAsync(channelName, duration, false);

                _logger.LogError(ex, "Error sending message to channel {ChannelName}", channelName);
                return false;
            }
            finally
            {
                _channelAccessSemaphore.Release();
            }
        }

        /// <summary>
        /// 批量发送消息到通道
        /// </summary>
        public async Task<List<bool>> SendBatchMessagesAsync<T>(
            Channel<T> channel,
            List<T> messages,
            string channelName,
            int maxConcurrency = 5,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Sending batch of {MessageCount} messages to channel {ChannelName}",
                messages.Count, channelName);

            var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
            var sendTasks = new List<Task<bool>>();

            try
            {
                foreach (var message in messages)
                {
                    var sendTask = Task.Run(async () =>
                    {
                        await semaphore.WaitAsync(cancellationToken);
                        try
                        {
                            return await SendMessageAsync(channel, message, channelName, cancellationToken);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, cancellationToken);

                    sendTasks.Add(sendTask);
                }

                var results = await Task.WhenAll(sendTasks);

                _logger.LogInformation("Batch sending completed - Channel: {ChannelName}, Success: {SuccessCount}/{TotalCount}",
                    channelName, results.Count(r => r), messages.Count);

                return results.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during batch message sending to channel {ChannelName}", channelName);
                throw new ChannelProcessingException(channelName, "batch", "Batch sending failed", ex);
            }
        }

        /// <summary>
        /// 尝试发送消息（非阻塞）
        /// </summary>
        public bool TrySendMessage<T>(Channel<T> channel, T message, string channelName)
        {
            try
            {
                var success = channel.Writer.TryWrite(message);
                if (success)
                {
                    _logger.LogDebug("Message written successfully to channel {ChannelName} (TryWrite)", channelName);
                }
                else
                {
                    _logger.LogWarning("Failed to write message to channel {ChannelName} (TryWrite) - channel full", channelName);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to send message to channel {ChannelName}", channelName);
                return false;
            }
        }
    }

    // 8. 生产级通道消费者服务
    public class ChannelConsumerService
    {
        private readonly ILogger<ChannelConsumerService> _logger;
        private readonly ChannelTelemetryService _telemetryService;

        public ChannelConsumerService(
            ILogger<ChannelConsumerService> logger,
            ChannelTelemetryService telemetryService)
        {
            _logger = logger;
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// 从通道读取消息并处理
        /// </summary>
        public async Task ConsumeMessagesAsync<T>(
            Channel<T> channel,
            Func<T, Task> processor,
            string channelName,
            CancellationToken cancellationToken = default,
            int maxConcurrency = 1)
        {
            _logger.LogInformation("Starting message consumption from channel {ChannelName} with {Concurrency} workers",
                channelName, maxConcurrency);

            var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
            var consumerTasks = new List<Task>();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!channel.Reader.TryRead(out var message))
                    {
                        // 等待新的消息可用
                        var waitResult = await channel.Reader.WaitToReadAsync(cancellationToken);
                        if (!waitResult) break; // 通道已关闭

                        if (!channel.Reader.TryRead(out message))
                            continue;
                    }

                    // 限制并发处理
                    await semaphore.WaitAsync(cancellationToken);

                    var consumeTask = Task.Run(async () =>
                    {
                        try
                        {
                            await ProcessMessageLocallyAsync(message, processor, channelName, cancellationToken);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, cancellationToken);

                    consumerTasks.Add(consumeTask);

                    // 防止任务列表无限增长
                    if (consumerTasks.Count > maxConcurrency * 2)
                    {
                        consumerTasks = consumerTasks.Where(t => !t.IsCompleted).ToList();
                    }
                }

                // 等待剩余任务完成
                if (consumerTasks.Any())
                {
                    await Task.WhenAll(consumerTasks);
                }

                _logger.LogInformation("Message consumption stopped for channel {ChannelName}", channelName);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Message consumption cancelled for channel {ChannelName}", channelName);
            }
            catch (ChannelClosedException)
            {
                _logger.LogInformation("Channel {ChannelName} closed - stopping consumption", channelName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming messages from channel {ChannelName}", channelName);
                throw new ChannelProcessingException(channelName, "reader", "Message consumption failed", ex);
            }
        }

        /// <summary>
        /// 从通道读取一批消息进行处理
        /// </summary>
        public async Task<List<ProcessingResult<T>>> ConsumeBatchMessagesAsync<T>(
            Channel<T> channel,
            Func<T, Task> processor,
            string channelName,
            int batchSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consuming batch of {BatchSize} messages from channel {ChannelName}",
                batchSize, channelName);

            var results = new List<ProcessingResult<T>>();

            try
            {
                for (int i = 0; i < batchSize; i++)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    if (!channel.Reader.TryRead(out var message))
                    {
                        var waitResult = await channel.Reader.WaitToReadAsync(cancellationToken);
                        if (!waitResult) break; // 通道已关闭

                        if (!channel.Reader.TryRead(out message))
                            break;
                    }

                    var startTime = DateTime.UtcNow;
                    try
                    {
                        await processor(message);
                        var duration = DateTime.UtcNow - startTime;

                        results.Add(new ProcessingResult<T>
                        {
                            Success = true,
                            ProcessingDuration = duration
                        });

                        await _telemetryService.TrackReadAsync(channelName, duration, true);
                    }
                    catch (Exception ex)
                    {
                        var duration = DateTime.UtcNow - startTime;

                        results.Add(new ProcessingResult<T>
                        {
                            Success = false,
                            ErrorMessage = ex.Message,
                            ProcessingDuration = duration
                        });

                        await _telemetryService.TrackReadAsync(channelName, duration, false);
                    }
                }

                _logger.LogInformation("Batch consumption completed from channel {ChannelName} - Messages: {MessageCount}",
                    channelName, results.Count);

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming batch messages from channel {ChannelName}", channelName);
                throw new ChannelProcessingException(channelName, "batch_reader", "Batch consumption failed", ex);
            }
        }

        /// <summary>
        /// 阻塞式读取通道消息直到关闭
        /// </summary>
        public async Task<List<T>> ReadAllMessagesAsync<T>(
            Channel<T> channel,
            string channelName,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Reading all messages from channel {ChannelName}", channelName);

            var messages = new List<T>();

            try
            {
                await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
                {
                    messages.Add(message);
                }

                _logger.LogInformation("Read {MessageCount} messages from channel {ChannelName}",
                    messages.Count, channelName);

                return messages;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Reading cancelled - got {MessageCount} messages", messages.Count);
                return messages;
            }
            catch (ChannelClosedException)
            {
                _logger.LogInformation("Channel closed - got {MessageCount} messages", messages.Count);
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading messages from channel {ChannelName}", channelName);
                throw new ChannelProcessingException(channelName, "reader_all", "Reading all messages failed", ex);
            }
        }

        private async Task ProcessMessageLocallyAsync<T>(
            T message,
            Func<T, Task> processor,
            string channelName,
            CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;
            var duration = TimeSpan.Zero;

            try
            {
                await processor(message);
                duration = DateTime.UtcNow - startTime;

                await _telemetryService.TrackReadAsync(channelName, duration, true);

                _logger.LogDebug("Message processed successfully from channel {ChannelName} in {Duration}ms",
                    channelName, duration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                duration = DateTime.UtcNow - startTime;
                await _telemetryService.TrackReadAsync(channelName, duration, false);

                _logger.LogError(ex, "Error processing message from channel {ChannelName}", channelName);
            }
        }
    }

    // 9. 通道管理服务 - 生产环境生命周期管理
    public class ChannelManagementService
    {
        private readonly ILogger<ChannelManagementService> _logger;
        private readonly IChannelFactory _channelFactory;
        private readonly ChannelTelemetryService _telemetryService;
        private readonly Dictionary<string, object> _channels = new Dictionary<string, object>();

        public ChannelManagementService(
            ILogger<ChannelManagementService> logger,
            IChannelFactory channelFactory,
            ChannelTelemetryService telemetryService)
        {
            _logger = logger;
            _channelFactory = channelFactory;
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// 创建并注册命名通道
        /// </summary>
        public Channel<T> CreateChannel<T>(ChannelConfiguration config)
        {
            _logger.LogInformation("Creating and registering named channel {ChannelName}", config.ChannelName);

            if (_channels.ContainsKey(config.ChannelName))
            {
                throw new ChannelConfigurationException(config.ChannelName, "Channel already exists");
            }

            var channel = _channelFactory.CreateChannel<T>(config);
            _channels[config.ChannelName] = channel;

            _logger.LogDebug("Named channel {ChannelName} created and registered", config.ChannelName);

            return channel;
        }

        /// <summary>
        /// 获取已创建的通道实例
        /// </summary>
        public Channel<T> GetChannel<T>(string channelName)
        {
            _logger.LogDebug("Retrieving channel {ChannelName}", channelName);

            if (!_channels.TryGetValue(channelName, out var channel))
            {
                _logger.LogWarning("Channel {ChannelName} not found", channelName);
                return null;
            }

            var typedChannel = channel as Channel<T>;
            if (typedChannel == null)
            {
                _logger.LogWarning("Channel {ChannelName} type mismatch", channelName);
                return null;
            }

            return typedChannel;
        }

        /// <summary>
        /// 关闭通道并停止所有操作
        /// </summary>
        public async Task<bool> CloseChannelAsync(string channelName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Closing channel {ChannelName}", channelName);

            try
            {
                if (_channels.TryGetValue(channelName, out var channel))
                {
                    switch (channel)
                    {
                        case Channel<MessageData> typedChannel:
                            typedChannel.Writer.Complete();
                            break;
                        case Channel<string> typedChannel:
                            typedChannel.Writer.Complete();
                            break;
                        case Channel<int> typedChannel:
                            typedChannel.Writer.Complete();
                            break;
                        default:
                            _logger.LogWarning("Cannot close channel {ChannelName} - unsupported type", channelName);
                            return false;
                    }

                    _channels.Remove(channelName);
                    _logger.LogInformation("Channel {ChannelName} closed successfully", channelName);
                    return true;
                }

                _logger.LogWarning("Channel {ChannelName} not found for closing", channelName);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing channel {ChannelName}", channelName);
                return false;
            }
        }

        /// <summary>
        /// 获取通道状态信息
        /// </summary>
        public ChannelStatus GetChannelStatus(string channelName)
        {
            var status = new ChannelStatus
            {
                ChannelName = channelName,
                Metrics = _telemetryService.GetMetrics(channelName),
                CreatedAt = DateTime.UtcNow
            };

            if (_channels.TryGetValue(channelName, out var channel))
            {
                status.IsAvailable = true;

                switch (channel)
                {
                    case Channel<MessageData> typedChannel:
                        status.PendingCount = typedChannel.Reader.Count;
                        status.IsCompleted = typedChannel.Writer.TryComplete();
                        break;
                    case Channel<string> typedChannel:
                        status.PendingCount = typedChannel.Reader.Count;
                        break;
                    case Channel<int> typedChannel:
                        status.PendingCount = typedChannel.Reader.Count;
                        break;
                }
            }

            _logger.LogDebug("Channel status retrieved - Channel: {ChannelName}, Pending: {PendingCount}, Available: {Available}",
                channelName, status.PendingCount, status.IsAvailable);

            return status;
        }

        /// <summary>
        /// 清理所有已创建通道
        /// </summary>
        public async Task CleanupAsync()
        {
            _logger.LogInformation("Cleaning up all channels - Count: {ChannelCount}", _channels.Count);

            var channelNames = _channels.Keys.ToList();

            foreach (var channelName in channelNames)
            {
                await CloseChannelAsync(channelName);
            }

            _telemetryService.ResetMetrics();
            _logger.LogInformation("Channel cleanup completed");
        }
    }

    public class ChannelStatus
    {
        public string ChannelName { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsCompleted { get; set; }
        public int PendingCount { get; set; }
        public ChannelMetrics Metrics { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

        public double MessageRate => Metrics.TotalWrites > 0 ?
            Metrics.TotalWrites / (RetrievedAt - CreatedAt).TotalSeconds : 0;

        public TimeSpan Uptime => RetrievedAt - CreatedAt;
    }

    // 10. 流式通道处理服务 - 支持中间件模式
    public class StreamingChannelProcessor
    {
        private readonly ILogger<StreamingChannelProcessor> _logger;
        private readonly ChannelTelemetryService _telemetryService;

        public StreamingChannelProcessor(
            ILogger<StreamingChannelProcessor> logger,
            ChannelTelemetryService telemetryService)
        {
            _logger = logger;
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// 管道式消息处理 - 支持链式处理
        /// </summary>
        public async Task ProcessWithPipelineAsync<TIn, TOut>(
            Channel<TIn> inputChannel,
            Channel<TOut> outputChannel,
            List<Func<TIn, Task<TOut>>> processors,
            string pipelineName,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting pipeline processing {PipelineName} with {ProcessorCount} steps",
                pipelineName, processors.Count);

            try
            {
                await foreach (var message in inputChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    TOut result = default(TOut);
                    Exception processingError = null;

                    try
                    {
                        // 顺序执行处理器链
                        object intermediateResult = message;

                        for (int i = 0; i < processors.Count; i++)
                        {
                            var processor = processors[i];
                            intermediateResult = await processor((TIn)intermediateResult);
                        }

                        result = (TOut)intermediateResult;
                    }
                    catch (Exception ex)
                    {
                        processingError = ex;
                        _logger.LogError(ex, "Error in pipeline step {StepNumber} for pipeline {PipelineName}",
                            0, pipelineName);
                    }

                    // 发送处理结果到输出通道
                    if (processingError == null)
                    {
                        await outputChannel.Writer.WriteAsync(result, cancellationToken);
                        _logger.LogDebug("Pipeline {PipelineName} processed message successfully", pipelineName);
                    }
                    else
                    {
                        // 错误处理策略 - 可以发送错误消息或者丢弃
                        _logger.LogWarning("Pipeline {PipelineName} produced error - message dropped", pipelineName);
                    }
                }

                _logger.LogInformation("Pipeline processing {PipelineName} completed", pipelineName);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Pipeline processing {PipelineName} cancelled", pipelineName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in pipeline {PipelineName}", pipelineName);
                throw new ChannelProcessingException(pipelineName, "pipeline", "Pipeline processing failed", ex);
            }
        }

        /// <summary>
        /// 批量缓存消息处理
        /// </summary>
        public async Task ProcessBatchCachingAsync<T>(
            Channel<T> inputChannel,
            Channel<T> outputChannel,
            string processorName,
            int batchSize = 100,
            TimeSpan maxWaitTime = default,
            CancellationToken cancellationToken = default)
        {
            if (maxWaitTime == default)
                maxWaitTime = TimeSpan.FromSeconds(5);

            _logger.LogInformation("Starting batch caching processing for {ProcessorName} - BatchSize: {BatchSize}",
                processorName, batchSize);

            var batch = new List<T>();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // 收集批次消息
                    while (batch.Count < batchSize)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        cts.CancelAfter(maxWaitTime);

                        try
                        {
                            var message = await inputChannel.Reader.ReadAsync(cts.Token);
                            batch.Add(message);
                        }
                        catch (OperationCanceledException canceledEx) when (cts.Token.IsCancellationRequested)
                        {
                            // 超时或取消，处理当前批次
                            break;
                        }
                    }

                    // 处理当前批次
                    if (batch.Any())
                    {
                        _logger.LogDebug("Processing batch of {BatchSize} messages for {ProcessorName}",
                            batch.Count, processorName);

                        // 这里可以实现批处理逻辑
                        foreach (var message in batch)
                        {
                            await outputChannel.Writer.WriteAsync(message, cancellationToken);
                        }

                        batch.Clear();
                    }
                }

                _logger.LogInformation("Batch caching processing for {ProcessorName} completed", processorName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch caching processing for {ProcessorName}", processorName);
                throw;
            }
        }
    }

    // 11. 优先级通道服务
    public class PriorityChannelService
    {
        private readonly ILogger<PriorityChannelService> _logger;

        /// <summary>
        /// 创建优先级通道组合
        /// </summary>
        public PriorityChannels<T> CreatePriorityChannels<T>(int capacity = 1000)
        {
            _logger.LogInformation("Creating priority channels with capacity {Capacity}", capacity);

            var priorityChannelOptions = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = false,
                SingleWriter = false
            };

            var highPriorityChannel = Channel.CreateBounded<T>(priorityChannelOptions);
            var normalPriorityChannel = Channel.CreateBounded<T>(priorityChannelOptions);
            var lowPriorityChannel = Channel.CreateBounded<T>(priorityChannelOptions);

            return new PriorityChannels<T>
            {
                HighPriority = highPriorityChannel,
                NormalPriority = normalPriorityChannel,
                LowPriority = lowPriorityChannel
            };
        }

        /// <summary>
        /// 从优先级通道按优先级顺序读取消息
        /// </summary>
        public async Task<T> ReadFromPriorityChannelsAsync<T>(
            PriorityChannels<T> channels,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Reading from priority channels");

            try
            {
                // 先尝试从高优先级通道读取
                if (channels.HighPriority.Reader.TryRead(out var highPriorityMessage))
                {
                    _logger.LogDebug("Read high priority message");
                    return highPriorityMessage;
                }

                // 再尝试普通优先级
                if (channels.NormalPriority.Reader.TryRead(out var normalPriorityMessage))
                {
                    _logger.LogDebug("Read normal priority message");
                    return normalPriorityMessage;
                }

                // 最后尝试低优先级
                if (channels.LowPriority.Reader.TryRead(out var lowPriorityMessage))
                {
                    _logger.LogDebug("Read low priority message");
                    return lowPriorityMessage;
                }

                // 等待任意通道有消息
                var highWait = channels.HighPriority.Reader.WaitToReadAsync(cancellationToken);
                var normalWait = channels.NormalPriority.Reader.WaitToReadAsync(cancellationToken);
                var lowWait = channels.LowPriority.Reader.WaitToReadAsync(cancellationToken);

                var tasks = new[] { highWait.AsTask(), normalWait.AsTask(), lowWait.AsTask() };
                await Task.WhenAny(tasks);

                // 再按优先级读取
                if (channels.HighPriority.Reader.TryRead(out highPriorityMessage))
                    return highPriorityMessage;

                if (channels.NormalPriority.Reader.TryRead(out normalPriorityMessage))
                    return normalPriorityMessage;

                if (channels.LowPriority.Reader.TryRead(out lowPriorityMessage))
                    return lowPriorityMessage;

                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading from priority channels");
                throw new ChannelProcessingException("priority", "reader", "Priority channel reading failed", ex);
            }
        }
    }

    public class PriorityChannels<T>
    {
        public Channel<T> HighPriority { get; set; }
        public Channel<T> NormalPriority { get; set; }
        public Channel<T> LowPriority { get; set; }
    }

    // 12. 主程序演示类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("System.Threading.Channels Production Demo");
            Console.WriteLine("========================================");
            Console.WriteLine();

            #region 服务配置

            // 配置日志服务
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 注册核心通道服务
            builder.Services.AddSingleton<ChannelTelemetryService>();
            builder.Services.AddSingleton<IChannelFactory, ChannelFactory>();
            builder.Services.AddSingleton<ChannelMessageProcessor>();
            builder.Services.AddSingleton<ChannelProducerService>();
            builder.Services.AddSingleton<ChannelConsumerService>();
            builder.Services.AddSingleton<ChannelManagementService>();
            builder.Services.AddSingleton<StreamingChannelProcessor>();
            builder.Services.AddSingleton<PriorityChannelService>();

            #endregion

            var host = builder.Build();

            await RunDemoAsync(host.Services);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunDemoAsync(IServiceProvider services)
        {
            var channelFactory = services.GetRequiredService<IChannelFactory>();
            var telemetryService = services.GetRequiredService<ChannelTelemetryService>();
            var messageProcessor = services.GetRequiredService<ChannelMessageProcessor>();
            var producerService = services.GetRequiredService<ChannelProducerService>();
            var consumerService = services.GetRequiredService<ChannelConsumerService>();
            var managementService = services.GetRequiredService<ChannelManagementService>();
            var streamingProcessor = services.GetRequiredService<StreamingChannelProcessor>();
            var priorityService = services.GetRequiredService<PriorityChannelService>();

            Console.WriteLine("1. Basic Bounded Channel Demo:");
            try
            {
                var config = new ChannelConfiguration
                {
                    ChannelName = "basic_channel",
                    Capacity = 10,
                    FullModeDropWrite = false
                };

                var channel = channelFactory.CreateChannel<string>(config);
                var testMessages = CreateTestMessages(15);

                // 发送消息
                foreach (var message in testMessages.Take(10))
                {
                    var success = await producerService.SendMessageAsync(
                        channel, message, "basic_channel");
                    Console.WriteLine($"   Sent message: {message} (Success: {success})");
                }

                // 读取消息
                for (int i = 0; i < 10; i++)
                {
                    var message = await channel.Reader.ReadAsync();
                    Console.WriteLine($"   Read message: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n2. Channel Telemetry Demo:");
            try
            {
                var config = new ChannelConfiguration
                {
                    ChannelName = "telemetry_channel",
                    Capacity = 5
                };

                var channel = channelFactory.CreateChannel<MessageData>(config);

                // 发送一些测试消息
                var testMessage = new MessageData
                {
                    MessageType = "Test",
                    Data = "Sample data for telemetry"
                };

                await producerService.SendMessageAsync(channel, testMessage, "telemetry_channel");

                var metrics = telemetryService.GetMetrics("telemetry_channel");
                Console.WriteLine($"   Channel metrics for 'telemetry_channel':");
                Console.WriteLine($"   - Total writes: {metrics.TotalWrites}");
                Console.WriteLine($"   - Successful writes: {metrics.SuccessfulWrites}");
                Console.WriteLine($"   - Write success rate: {metrics.WriteSuccessRate:F2}%");
                Console.WriteLine($"   - Average write duration: {metrics.AverageWriteDuration:F2}ms");

                var allMetrics = telemetryService.GetAllMetrics();
                Console.WriteLine($"   Total monitored channels: {allMetrics.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n3. Message Processing with Retry Demo:");
            try
            {
                var config = new ChannelConfiguration
                {
                    ChannelName = "processing_channel",
                    Capacity = 100,
                    MaxConcurrency = 5
                };

                var processingChannel = channelFactory.CreateChannel<MessageData>(config);
                var processedData = await consumerService.ReadAllMessagesAsync(
                    processingChannel, "processing_channel");

                // 发送消息
                var testData = new MessageData
                {
                    MessageType = "UserRegistration",
                    Data = new { Name = "John Doe", Email = "john@example.com" }
                };

                await producerService.SendMessageAsync(processingChannel, testData, "processing_channel");

                // 模拟处理结果
                var processingResult = new ProcessingResult<object>
                {
                    MessageId = testData.MessageId,
                    Success = true,
                    Result = "Registration completed",
                    ProcessingDuration = TimeSpan.FromMilliseconds(150)
                };

                Console.WriteLine($"   Processing result:");
                Console.WriteLine($"   - Message ID: {processingResult.MessageId}");
                Console.WriteLine($"   - Success: {processingResult.Success}");
                Console.WriteLine($"   - Duration: {processingResult.ProcessingDuration.TotalMilliseconds}ms");

                var processingMetrics = telemetryService.GetMetrics("processing_channel");
                Console.WriteLine($"   - Successful processings: {processingMetrics.SuccessfulProcessings}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n4. Channel Management Demo:");
            try
            {
                var config = new ChannelConfiguration
                {
                    ChannelName = "managed_channel",
                    Capacity = 50
                };

                var managedChannel = managementService.CreateChannel<MessageData>(config);

                // 获取通道状态
                var channelStatus = managementService.GetChannelStatus("managed_channel");
                Console.WriteLine($"   Channel status: {channelStatus.ChannelName}");
                Console.WriteLine($"   - Available: {channelStatus.IsAvailable}");
                Console.WriteLine($"   - Pending messages: {channelStatus.PendingCount}");

                // 关闭通道
                var closeResult = await managementService.CloseChannelAsync("managed_channel");
                Console.WriteLine($"   Channel closed: {closeResult}");

                // 再次检查状态
                var closedStatus = managementService.GetChannelStatus("managed_channel");
                Console.WriteLine($"   Closed channel status available: {closedStatus.IsAvailable}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n5. Batch Message Processing Demo:");
            try
            {
                var config = new ChannelConfiguration
                {
                    ChannelName = "batch_channel",
                    Capacity = 1000
                };

                var batchChannel = channelFactory.CreateChannel<string>(config);
                var batchMessages = CreateTestMessages(50);

                // 批量发送
                var sendResults = await producerService.SendBatchMessagesAsync(
                    batchChannel, batchMessages, "batch_channel", maxConcurrency: 10);

                Console.WriteLine($"   Batch sent: {sendResults.Count(r => r)} successful out of {batchMessages.Count}");

                // 批量处理
                var processResults = await messageProcessor.ProcessBatchMessagesAsync(
                    batchMessages.Select(m => new MessageData
                    {
                        MessageType = "BatchMessage",
                        Data = m
                    }).ToList(),
                    async msg => { await Task.Delay(10); return msg.Data; }, // 处理函数
                    "batch_channel",
                    maxConcurrency: 5);

                Console.WriteLine($"   Batch processed: {processResults.Count(r => r.Success)} successful");
                Console.WriteLine($"   Average processing time: {processResults.Where(r => r.Success).Average(r => r.ProcessingDuration.TotalMilliseconds):F2}ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n6. Streaming Pipeline Processing Demo:");
            try
            {
                var inputConfig = new ChannelConfiguration { ChannelName = "pipeline_input", Capacity = 100 };
                var outputConfig = new ChannelConfiguration { ChannelName = "pipeline_output", Capacity = 100 };

                var inputChannel = channelFactory.CreateChannel<string>(inputConfig);
                var outputChannel = channelFactory.CreateChannel<string>(outputConfig);

                // 定义处理器链
                var processors = new List<Func<string, Task<string>>>
            {
                async str => { await Task.Delay(1); return str?.ToUpper(); },
                async str => { await Task.Delay(1); return $"Processed: {str}"; },
                async str => { await Task.Delay(1); return $"Final: {str}"; }
            };

                // 启动管道处理器
                var pipelineTask = Task.Run(async () =>
                {
                    await streamingProcessor.ProcessWithPipelineAsync(
                        inputChannel, outputChannel, processors, "text_pipeline");
                });

                // 发送消息
                await producerService.SendMessageAsync(inputChannel, "hello world", "pipeline_input");

                // 读取处理结果
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var result = await outputChannel.Reader.ReadAsync(cts.Token);
                Console.WriteLine($"   Pipeline processing result: {result}");

                // 停止管道
                cts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n7. Priority Channel Demo:");
            try
            {
                var priorityChannels = priorityService.CreatePriorityChannels<string>(capacity: 50);

                // 发送不同优先级消息
                var highPriorityMessages = CreateTestMessages(5, "High");
                var normalPriorityMessages = CreateTestMessages(10, "Normal");
                var lowPriorityMessages = CreateTestMessages(15, "Low");

                // 发送高优先级消息
                foreach (var msg in highPriorityMessages)
                {
                    priorityChannels.HighPriority.Writer.TryWrite($"[HIGH] {msg}");
                }

                // 发送普通优先级消息
                foreach (var msg in normalPriorityMessages)
                {
                    priorityChannels.NormalPriority.Writer.TryWrite($"[NORMAL] {msg}");
                }

                // 发送低优先级消息
                foreach (var msg in lowPriorityMessages)
                {
                    priorityChannels.LowPriority.Writer.TryWrite($"[LOW] {msg}");
                }

                Console.WriteLine($"   Sent {highPriorityMessages.Count} high priority messages");
                Console.WriteLine($"   Sent {normalPriorityMessages.Count} normal priority messages");
                Console.WriteLine($"   Sent {lowPriorityMessages.Count} low priority messages");

                // 按优先级顺序读取消息
                for (int i = 0; i < 10; i++)
                {
                    var message = await priorityService.ReadFromPriorityChannelsAsync(
                        priorityChannels);
                    if (message != null)
                    {
                        Console.WriteLine($"   Read: {message}");
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n8. Channel Error Handling Demo:");
            try
            {
                var config = new ChannelConfiguration
                {
                    ChannelName = "error_channel",
                    Capacity = 5,
                    Timeout = TimeSpan.FromSeconds(2)
                };

                var channel = channelFactory.CreateChannel<string>(config);

                // 测试通道满时的错误处理
                for (int i = 0; i < 10; i++)
                {
                    var message = $"Error handling test message {i}";
                    var success = producerService.TrySendMessage(channel, message, "error_channel");
                    Console.WriteLine($"   TrySendMessage {i}: {success}");
                }

                // 读取消息验证
                while (channel.Reader.TryRead(out var receivedMessage))
                {
                    Console.WriteLine($"   Received: {receivedMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error handling test - Exception: {ex.Message}");
            }

            Console.WriteLine("\n9. Channel with Zero Capacity (Direct) Demo:");
            try
            {
                // 创建零容量通道 - 发送者/接收者必须同步进行操作
                var directChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(0)
                {
                    FullMode = BoundedChannelFullMode.Wait
                });

                // 同时启动发送和接收任务
                var sendTasks = new List<Task>();
                var receiveTasks = new List<Task<string>>();

                for (int i = 0; i < 5; i++)
                {
                    var index = i;
                    var sendTask = Task.Run(async () =>
                    {
                        await directChannel.Writer.WriteAsync($"Direct message {index}");
                        Console.WriteLine($"   Sent direct message {index}");
                    });

                    var receiveTask = Task.Run(async () =>
                    {
                        var message = await directChannel.Reader.ReadAsync();
                        Console.WriteLine($"   Received direct message: {message}");
                        return message;
                    });

                    sendTasks.Add(sendTask);
                    receiveTasks.Add(receiveTask);
                }

                await Task.WhenAll(sendTasks.Concat(receiveTasks.Cast<Task>()));

                Console.WriteLine($"   Direct channel processing completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n10. Cleanup Demo:");
            try
            {
                await managementService.CleanupAsync();

                var telemetryMetrics = telemetryService.GetAllMetrics();
                Console.WriteLine($"   Cleanup completed - Remaining telemetry metrics: {telemetryMetrics.Count}");

                Console.WriteLine("   All channel resources cleaned up successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Cleanup error: {ex.Message}");
            }
        }

        private static List<string> CreateTestMessages(int count, string prefix = "msg")
        {
            var messages = new List<string>();

            for (int i = 1; i <= count; i++)
            {
                messages.Add($"{prefix}_{i}_{DateTime.Now:HHmmss}");
            }

            return messages;
        }
    }

    public class Test
    {
        public void TestChannels()
        {
            // 1. 有界通道创建与配置
            // 基本有界通道创建
            // 创建固定容量的有界通道
            var boundedChannel = Channel.CreateBounded<string>(
                new BoundedChannelOptions(1000)
                {
                    FullMode = BoundedChannelFullMode.Wait,    // 满时等待
                    SingleReader = false,                      // 支持多读者
                    SingleWriter = false                       // 支持多写者
                });

            // 写入消息（阻塞直到有空间）
            await boundedChannel.Writer.WriteAsync("message");

            // 读取消息（阻塞直到有消息）
            var message = await boundedChannel.Reader.ReadAsync();

            // 2. 无界通道创建
            // 基础无界通道
            // 创建无界通道（实际仍是有限的）
            var unboundedChannel = Channel.CreateUnbounded<MessageData>(
                new UnboundedChannelOptions
                {
                    SingleReader = false,
                    SingleWriter = false
                });

            // 非阻塞写入
            var success = boundedChannel.Writer.TryWrite("message");

            // 4. 并发安全处理
            // 信号量控制并发访问
            private readonly SemaphoreSlim _channelAccessSemaphore = new SemaphoreSlim(maxConcurrentAccess, maxConcurrentAccess);
            await _channelAccessSemaphore.WaitAsync(cancellationToken);
            try
            {
                // 通道操作
                await channel.Writer.WriteAsync(message, cancellationToken);
                }
            finally
            {
                _channelAccessSemaphore.Release();
            }

            // 批量处理和流式处理
            // 批量消息处理
            public async Task SendBatchMessagesAsync<T>(...)
            {
                var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
                var sendTasks = new List<Task<bool>>();

                foreach (var message in messages)
                {
                    var sendTask = Task.Run(async () =>
                    {
                        await semaphore.WaitAsync(cancellationToken);
                        try
                        {
                            return await SendMessageAsync(channel, message, ...);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    sendTasks.Add(sendTask);
                }

                return await Task.WhenAll(sendTasks);
            }

            // 流式处理
            // 使用IAsyncEnumerable流式处理消息
            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                await ProcessMessage(message);
            }

            // 使用ChannelReader的流式读取
            var readAllTask = channel.Reader.ReadAllAsync(cancellationToken);
            while (await readAllTask.MoveNextAsync())
            {
                var message = readAllTask.Current;
                await ProcessMessage(message);
            }

            // 9. 管道式处理器链
            // 中间件处理链
            public async Task ProcessWithPipelineAsync<TIn, TOut>(...)
            {
                await foreach (var message in inputChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    object intermediateResult = message;

                    // 执行处理链
                    for (int i = 0; i < processors.Count; i++)
                    {
                        var processor = processors[i];
                        intermediateResult = await processor((TIn)intermediateResult);
                    }

                    // 发送到输出通道
                    await outputChannel.Writer.WriteAsync((TOut)intermediateResult, cancellationToken);
                }
            }

            // 10. 生产环境错误处理和恢复
            // 完整的异常处理
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(timeout);

                await channel.Writer.WriteAsync(message, cts.Token);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // 用户取消处理
                return false;
            }
            catch (ChannelClosedException)
            {
                // 通道已关闭
                return false;
            }
            catch (TimeoutException)
            {
                // 操作超时
                return false;
            }
            catch (Exception ex)
            {
                // 其他错误日志记录
                _logger.LogError(ex, "Failed to send message");
                return false;
            }
        }
    }

    // 3. 生产级配置管理
    // 灵活配置选项
    public class ChannelConfiguration
    {
        public int Capacity { get; set; } = 1000;                    // 通道容量
        public bool FullModeDropWrite { get; set; } = false;         // 满时是否丢弃
        public int MaxConcurrency { get; set; } = 10;               // 最大并发数
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30); // 操作超时
    }

    // 6. 通道状态监控和遥测
    // 性能指标跟踪
    public class ChannelTelemetryService
    {
        public async Task TrackWriteAsync(string channelName, TimeSpan duration, bool success)
        {
            var metrics = GetOrAddMetrics(channelName);
            metrics.TotalWrites++;
            metrics.SuccessfulWrites += success ? 1 : 0;
            metrics.TotalWriteDuration += duration.TotalMilliseconds;
        }

        public ChannelMetrics GetMetrics(string channelName)
        {
            return _channelMetrics.GetValueOrDefault(channelName, new ChannelMetrics());
        }
    }

    public class ChannelMetrics
    {
        public double AverageWriteDuration { get; set; }
        public double WriteSuccessRate { get; set; }
        public long TotalWrites { get; set; }
        public long SuccessfulWrites { get; set; }
    }

    // 7. 安全的消息生命周期管理
    // 通道管理服务
    public class ChannelManagementService
    {
        public Channel<T> CreateChannel<T>(ChannelConfiguration config)
        {
            // 创建和配置通道
            var channel = _channelFactory.CreateChannel<T>(config);
            _channels[config.ChannelName] = channel;
            return channel;
        }

        public async Task<bool> CloseChannelAsync(string channelName)
        {
            // 安全关闭通道
            if (_channels.TryGetValue(channelName, out var channel))
            {
                switch (channel)
                {
                    case Channel<T> typedChannel:
                        typedChannel.Writer.Complete();
                        _channels.Remove(channelName);
                        return true;
                }
            }
            return false;
        }
    }

    // 8. 优先级通道处理
    // 多层次优先级通道
    public class PriorityChannels<T>
    {
        public Channel<T> HighPriority { get; set; }
        public Channel<T> NormalPriority { get; set; }
        public Channel<T> LowPriority { get; set; }
    }

    public async Task<T> ReadFromPriorityChannelsAsync<T>(PriorityChannels<T> channels)
    {
        // 按高优先级顺序读取
        if (channels.HighPriority.Reader.TryRead(out var highPriorityMessage))
            return highPriorityMessage;

        if (channels.NormalPriority.Reader.TryRead(out var normalPriorityMessage))
            return normalPriorityMessage;

        if (channels.LowPriority.Reader.TryRead(out var lowPriorityMessage))
            return lowPriorityMessage;

        return default(T);
    }

    // 11. 推荐生产环境实践
    // 容量和并发调优
    public class ChannelConfiguration
    {
        // 根据系统资源调整通道容量
        public int Capacity = Environment.ProcessorCount * 100;

        // 限制并发处理避免资源耗尽
        public int MaxConcurrency = Environment.ProcessorCount * 2;

        // 合理设置超时避免死锁
        public TimeSpan Timeout = TimeSpan.FromSeconds(30);

        // 内存和性能优化
        // 使用正确的通道选项优化性能
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest, // 丢弃最旧消息
            SingleReader = true,                         // 单读者优化
            SingleWriter = true                          // 单写者优化
        };

        // 限制长时间运行的消费者任务
        while (channel.Reader.TryRead(out var message))
        {
            await ProcessMessage(message);

            // 定期检查取消状态
            if (cancellationToken.IsCancellationRequested)
                break;
        }

        // 安全关闭和清理
        // 在应用程序关闭时安全处理通道
        public async Task CleanupAsync()
        {
            foreach (var channel in _channels.Values)
            {
                if (channel is Channel<T> typedChannel)
                {
                    typedChannel.Writer.TryComplete();
                }
            }

            _channels.Clear();
        }
    }
}