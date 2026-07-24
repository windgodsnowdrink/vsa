#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Http@10.0.0
#:package Microsoft.Extensions.Diagnostics.HealthChecks@10.0.0
#:package System.Net.Http@8.0.0
#:package System.Text.Json@8.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:package System.Runtime.CompilerServices.Unsafe@6.0.0
#:package System.Net.Sockets@8.0.0
#:package System.Threading.Channels@8.0.0
#:package Polly@7.2.4
#:package System.Buffers@4.5.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property Optimize=true
#:property EnableCompressionInSingleFile=true
#:property SelfContained=true

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;

namespace Remotely.Integration
{
    // 配置选项
    public class RemotelyOptions
    {
        // 服务器配置
        public string ServerUrl { get; set; } = "https://localhost:5001";
        public string OrganizationId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceAlias { get; set; } = string.Empty;
        
        // 连接配置
        public int HeartbeatInterval { get; set; } = 30;
        public int MaxConnectionRetries { get; set; } = 5;
        public int RetryDelaySeconds { get; set; } = 30;
        
        // 功能配置
        public bool EnableRemoteControl { get; set; } = true;
        public bool EnableFileTransfer { get; set; } = true;
        public bool EnableChat { get; set; } = true;
        
        // 性能配置
        public int BufferSize { get; set; } = 8192;
        public bool EnableZeroCopy { get; set; } = true;
        public int MemoryPoolSize { get; set; } = 1024 * 1024;
        
        // 安全配置
        public bool EnableCompression { get; set; } = true;
        public bool EnableEncryption { get; set; } = true;
        
        // AOT 配置
        public bool PublishAot { get; set; } = true;
        public string TrimMode { get; set; } = "partial";
        public bool ReadyToRun { get; set; } = true;
        public bool TieredCompilation { get; set; } = true;
        public bool Optimize { get; set; } = true;
    }

    // 设备信息
    public class Device
    {
        public string Id { get; set; }
        public string Alias { get; set; }
        public string OrganizationId { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }
        public string Architecture { get; set; }
        public int HeartbeatInterval { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
    }

    // 聊天消息
    public class ChatMessage
    {
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string OrganizationId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsFromDevice { get; set; }
    }

    // 远程控制会话
    public class RemoteControlSession
    {
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string OrganizationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
    }

    // 文件传输信息
    public class FileTransfer
    {
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string OrganizationId { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsUpload { get; set; }
        public bool IsComplete { get; set; }
    }

    // Remotely 服务接口
    public interface IRemotelyService
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task<Device> GetDeviceInfoAsync(CancellationToken cancellationToken = default);
        Task<bool> CheckConnectionAsync(CancellationToken cancellationToken = default);
        Task SendChatMessageAsync(string message, CancellationToken cancellationToken = default);
        Task<byte[]> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task UploadFileAsync(string filePath, byte[] fileContents, CancellationToken cancellationToken = default);
        Task<RemoteControlSession> StartRemoteControlAsync(CancellationToken cancellationToken = default);
        Task StopRemoteControlAsync(string sessionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ChatMessage>> GetChatMessagesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<FileTransfer>> GetFileTransfersAsync(CancellationToken cancellationToken = default);
        Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default);
    }

    // Remotely 服务实现
    public class RemotelyService : IRemotelyService, IDisposable
    {
        private readonly IOptions<RemotelyOptions> _options;
        private readonly ILogger<RemotelyService> _logger;
        private readonly HttpClient _httpClient;
        private readonly Channel<string> _commandChannel;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private bool _disposed;
        private bool _isConnected;

        public RemotelyService(
            IOptions<RemotelyOptions> options,
            ILogger<RemotelyService> logger)
        {
            _options = options;
            _logger = logger;
            _commandChannel = Channel.CreateUnbounded<string>();

            // 初始化HttpClient
            _httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All,
                UseCookies = true,
                AllowAutoRedirect = true
            });
            _httpClient.BaseAddress = new Uri(_options.Value.ServerUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            // 配置重试策略
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Handle<TimeoutException>()
                .Handle<SocketException>()
                .WaitAndRetryAsync(
                    _options.Value.MaxConnectionRetries,
                    retryAttempt => TimeSpan.FromSeconds(_options.Value.RetryDelaySeconds),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, $"Remotely connection failed. Retry attempt {retryCount}");
                    });

            // 配置熔断策略
            _circuitBreaker = Policy
                .Handle<HttpRequestException>()
                .Handle<TimeoutException>()
                .Handle<SocketException>()
                .CircuitBreakerAsync(
                    5,
                    TimeSpan.FromMinutes(1),
                    (exception, duration) =>
                    {
                        _logger.LogError(exception, $"Circuit breaker opened for {duration.TotalSeconds} seconds");
                    },
                    () =>
                    {
                        _logger.LogInformation("Circuit breaker reset");
                    },
                    () =>
                    {
                        _logger.LogInformation("Circuit breaker half-open");
                    });
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                if (!_isConnected)
                {
                    _logger.LogInformation("Starting Remotely service");
                    
                    // 连接到服务器
                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var response = await _httpClient.GetAsync("api/device/heartbeat", cancellationToken);
                        response.EnsureSuccessStatusCode();
                        _isConnected = true;
                        _logger.LogInformation("Connected to Remotely server: {ServerUrl}", _options.Value.ServerUrl);
                    });

                    // 启动心跳循环
                    _ = Task.Run(async () => await HeartbeatLoopAsync(cancellationToken), cancellationToken);
                    
                    // 启动命令处理循环
                    _ = Task.Run(async () => await CommandProcessingLoopAsync(cancellationToken), cancellationToken);
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                if (_isConnected)
                {
                    _logger.LogInformation("Stopping Remotely service");
                    _isConnected = false;
                    
                    // 发送断开连接通知
                    try
                    {
                        await _httpClient.PostAsync("api/device/disconnect", null, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error sending disconnect notification");
                    }
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task<Device> GetDeviceInfoAsync(CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.GetAsync($"api/device/{_options.Value.DeviceId}", cancellationToken);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var device = JsonSerializer.Deserialize<Device>(content);
                    _logger.LogInformation("Retrieved device info: {DeviceAlias}", device.Alias);
                    return device;
                });
            });
        }

        public async Task<bool> CheckConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.GetAsync("api/device/heartbeat", cancellationToken);
                    response.EnsureSuccessStatusCode();
                });
                _isConnected = true;
                _logger.LogInformation("Connection check successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Connection check failed");
                _isConnected = false;
                return false;
            }
        }

        public async Task SendChatMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            await _circuitBreaker.ExecuteAsync(async () =>
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var chatMessage = new ChatMessage
                    {
                        DeviceId = _options.Value.DeviceId,
                        OrganizationId = _options.Value.OrganizationId,
                        Message = message,
                        Timestamp = DateTime.UtcNow,
                        IsFromDevice = true
                    };

                    var content = new StringContent(
                        JsonSerializer.Serialize(chatMessage),
                        Encoding.UTF8,
                        "application/json");

                    var response = await _httpClient.PostAsync("api/chat/send", content, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    _logger.LogInformation("Chat message sent: {Message}", message);
                });
            });
        }

        public async Task<byte[]> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.GetAsync($"api/file/download?filePath={Uri.EscapeDataString(filePath)}&deviceId={_options.Value.DeviceId}", cancellationToken);
                    response.EnsureSuccessStatusCode();
                    var fileContents = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                    _logger.LogInformation("File downloaded: {FilePath}, Size: {Size} bytes", filePath, fileContents.Length);
                    return fileContents;
                });
            });
        }

        public async Task UploadFileAsync(string filePath, byte[] fileContents, CancellationToken cancellationToken = default)
        {
            await _circuitBreaker.ExecuteAsync(async () =>
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var content = new MultipartFormDataContent();
                    content.Add(new ByteArrayContent(fileContents), "file", Path.GetFileName(filePath));
                    content.Add(new StringContent(filePath), "filePath");
                    content.Add(new StringContent(_options.Value.DeviceId), "deviceId");

                    var response = await _httpClient.PostAsync("api/file/upload", content, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    _logger.LogInformation("File uploaded: {FilePath}, Size: {Size} bytes", filePath, fileContents.Length);
                });
            });
        }

        public async Task<RemoteControlSession> StartRemoteControlAsync(CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var content = new StringContent(
                        JsonSerializer.Serialize(new { DeviceId = _options.Value.DeviceId }),
                        Encoding.UTF8,
                        "application/json");

                    var response = await _httpClient.PostAsync("api/remotecontrol/start", content, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    var sessionContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    var session = JsonSerializer.Deserialize<RemoteControlSession>(sessionContent);
                    _logger.LogInformation("Remote control session started: {SessionId}", session.Id);
                    return session;
                });
            });
        }

        public async Task StopRemoteControlAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            await _circuitBreaker.ExecuteAsync(async () =>
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var content = new StringContent(
                        JsonSerializer.Serialize(new { SessionId = sessionId }),
                        Encoding.UTF8,
                        "application/json");

                    var response = await _httpClient.PostAsync("api/remotecontrol/stop", content, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    _logger.LogInformation("Remote control session stopped: {SessionId}", sessionId);
                });
            });
        }

        public async Task<IEnumerable<ChatMessage>> GetChatMessagesAsync(CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.GetAsync($"api/chat/messages?deviceId={_options.Value.DeviceId}", cancellationToken);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var messages = JsonSerializer.Deserialize<IEnumerable<ChatMessage>>(content);
                    _logger.LogInformation("Retrieved {MessageCount} chat messages", messages.Count());
                    return messages;
                });
            });
        }

        public async Task<IEnumerable<FileTransfer>> GetFileTransfersAsync(CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.GetAsync($"api/file/transfers?deviceId={_options.Value.DeviceId}", cancellationToken);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var transfers = JsonSerializer.Deserialize<IEnumerable<FileTransfer>>(content);
                    _logger.LogInformation("Retrieved {TransferCount} file transfers", transfers.Count());
                    return transfers;
                });
            });
        }

        public Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default)
        {
            var result = new HealthCheckResult
            {
                Status = _isConnected ? "Healthy" : "Unhealthy",
                CheckTime = DateTime.UtcNow
            };

            result.Details.Add("ServerUrl", _options.Value.ServerUrl);
            result.Details.Add("DeviceId", _options.Value.DeviceId);
            result.Details.Add("DeviceAlias", _options.Value.DeviceAlias);
            result.Details.Add("IsConnected", _isConnected.ToString());
            result.Details.Add("EnableRemoteControl", _options.Value.EnableRemoteControl.ToString());
            result.Details.Add("EnableFileTransfer", _options.Value.EnableFileTransfer.ToString());
            result.Details.Add("EnableChat", _options.Value.EnableChat.ToString());

            _logger.LogInformation("Health check completed with status: {Status}", result.Status);
            return Task.FromResult(result);
        }

        private async Task HeartbeatLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _isConnected)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_options.Value.HeartbeatInterval), cancellationToken);
                    await CheckConnectionAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // 忽略取消操作异常
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error in heartbeat loop");
                }
            }
        }

        private async Task CommandProcessingLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _isConnected)
            {
                try
                {
                    var command = await _commandChannel.Reader.ReadAsync(cancellationToken);
                    _logger.LogInformation("Processing command: {Command}", command);
                    // 处理命令逻辑
                }
                catch (OperationCanceledException)
                {
                    // 忽略取消操作异常
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error in command processing loop");
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _commandChannel.Writer.Complete();
                _httpClient.Dispose();
                _connectionLock.Dispose();
                _disposed = true;
            }
        }
    }

    // 健康检查
    public class RemotelyHealthCheck : IHealthCheck
    {
        private readonly IRemotelyService _remotelyService;

        public RemotelyHealthCheck(IRemotelyService remotelyService)
        {
            _remotelyService = remotelyService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _remotelyService.HealthCheckAsync(cancellationToken);
                if (result.Status == "Healthy")
                {
                    return HealthCheckResult.Healthy("Remotely service is healthy", result.Details);
                }
                else
                {
                    return HealthCheckResult.Unhealthy("Remotely service is unhealthy", null, result.Details);
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Remotely service health check failed", ex);
            }
        }
    }

    // 依赖注入扩展
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRemotelyService(this IServiceCollection services, Action<RemotelyOptions> configureOptions)
        {
            // 配置选项
            services.Configure(configureOptions);

            // 注册服务
            services.AddSingleton<IRemotelyService, RemotelyService>();

            // 注册健康检查
            services.AddHealthChecks()
                .AddCheck<RemotelyHealthCheck>("Remotely");

            // 注册内存缓存
            services.AddMemoryCache();

            return services;
        }
    }

    // 主程序入口
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Remotely Integration Service");
            Console.WriteLine("=" * 50);

            // 构建服务容器
            var builder = new ServiceCollection();

            // 配置日志
            builder.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 注册 Remotely 服务
            builder.AddRemotelyService(options =>
            {
                options.ServerUrl = "https://localhost:5001";
                options.OrganizationId = "your-organization-id";
                options.DeviceId = "your-device-id";
                options.DeviceAlias = "Test Device";
                options.HeartbeatInterval = 30;
                options.MaxConnectionRetries = 5;
                options.RetryDelaySeconds = 30;
                options.EnableRemoteControl = true;
                options.EnableFileTransfer = true;
                options.EnableChat = true;
                options.BufferSize = 8192;
                options.EnableZeroCopy = true;
                options.MemoryPoolSize = 1024 * 1024;
                options.EnableCompression = true;
                options.EnableEncryption = true;
                options.PublishAot = true;
                options.TrimMode = "partial";
                options.ReadyToRun = true;
                options.TieredCompilation = true;
                options.Optimize = true;
            });

            var serviceProvider = builder.BuildServiceProvider();

            // 获取服务
            var remotelyService = serviceProvider.GetRequiredService<IRemotelyService>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // 启动服务
                await remotelyService.StartAsync();
                logger.LogInformation("Remotely service started successfully");

                // 检查连接
                var isConnected = await remotelyService.CheckConnectionAsync();
                logger.LogInformation("Connection check: {IsConnected}", isConnected);

                // 获取设备信息
                var deviceInfo = await remotelyService.GetDeviceInfoAsync();
                logger.LogInformation("Device info: {DeviceAlias}, OS: {OS}", deviceInfo.Alias, deviceInfo.OS);

                // 发送聊天消息
                await remotelyService.SendChatMessageAsync("Hello from Remotely Integration Service");
                logger.LogInformation("Chat message sent successfully");

                // 健康检查
                var healthCheck = await remotelyService.HealthCheckAsync();
                logger.LogInformation("Health check: {Status}", healthCheck.Status);

                // 停止服务
                await remotelyService.StopAsync();
                logger.LogInformation("Remotely service stopped successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Remotely service operation");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
