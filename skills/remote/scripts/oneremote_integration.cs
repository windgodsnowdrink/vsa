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

namespace OneRemoteIntegration
{
    // 配置选项
    public class OneRemoteOptions
    {
        // 服务器配置
        public string ServerAddress { get; set; } = "localhost";
        public int ServerPort { get; set; } = 8080;
        public string AuthToken { get; set; } = string.Empty;
        
        // 连接配置
        public int MaxConnections { get; set; } = 100;
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromMinutes(5);
        public int ConnectionPoolSize { get; set; } = 100;
        
        // 重试配置
        public int RetryCount { get; set; } = 3;
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(5);
        
        // 熔断配置
        public int CircuitBreakerThreshold { get; set; } = 5;
        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromMinutes(1);
        
        // 性能配置
        public int BufferSize { get; set; } = 8192;
        public bool EnableZeroCopy { get; set; } = true;
        public int MemoryPoolSize { get; set; } = 1024 * 1024;
        
        // 监控配置
        public bool EnableMetrics { get; set; } = true;
        public bool EnableTracing { get; set; } = true;
        
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

    // 连接信息
    public class ConnectionInfo
    {
        public Guid Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Protocol { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // 会话信息
    public class SessionInfo
    {
        public Guid Id { get; set; }
        public Guid ConnectionId { get; set; }
        public string Protocol { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
    }

    // 服务器信息
    public class ServerInfo
    {
        public string ServerName { get; set; }
        public string Version { get; set; }
        public int CurrentConnections { get; set; }
        public int MaxConnections { get; set; }
        public DateTime Uptime { get; set; }
    }

    // 连接统计
    public class ConnectionStats
    {
        public int ActiveConnections { get; set; }
        public int FailedConnections { get; set; }
        public int TotalConnections { get; set; }
        public TimeSpan AverageConnectionTime { get; set; }
        public DateTime LastConnectionTime { get; set; }
    }

    // 吞吐量统计
    public class ThroughputStats
    {
        public long BytesSent { get; set; }
        public long BytesReceived { get; set; }
        public double BytesPerSecond { get; set; }
        public int OperationsPerSecond { get; set; }
        public DateTime LastResetTime { get; set; }
    }

    // 健康检查结果
    public class HealthCheckResult
    {
        public string Status { get; set; }
        public Dictionary<string, string> Details { get; set; } = new Dictionary<string, string>();
        public DateTime CheckTime { get; set; }
    }

    // 命令执行结果
    public class CommandResult
    {
        public string Output { get; set; }
        public int ExitCode { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // OneRemote 服务接口
    public interface IOneRemoteService
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task<ConnectionInfo> ConnectAsync(string host, int port, string protocol = "ssh", CancellationToken cancellationToken = default);
        Task DisconnectAsync(Guid connectionId, CancellationToken cancellationToken = default);
        Task<SessionInfo> CreateSessionAsync(string host, int port, string protocol = "ssh", CancellationToken cancellationToken = default);
        Task CloseSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
        Task<CommandResult> ExecuteCommandAsync(Guid sessionId, string command, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
        Task<T> SendHttpRequestAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default);
        Task<byte[]> SendTcpRequestAsync(string host, int port, byte[] data, CancellationToken cancellationToken = default);
        Task<IEnumerable<ConnectionInfo>> GetActiveConnectionsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<SessionInfo>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
        Task<int> GetConnectionPoolUsageAsync(CancellationToken cancellationToken = default);
        Task<ServerInfo> GetServerInfoAsync(CancellationToken cancellationToken = default);
        Task<ConnectionStats> GetConnectionStatsAsync(CancellationToken cancellationToken = default);
        Task<ThroughputStats> GetThroughputStatsAsync(CancellationToken cancellationToken = default);
        Task ResetCircuitBreakerAsync(CancellationToken cancellationToken = default);
        Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default);
        Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default);
    }

    // OneRemote 服务实现
    public class OneRemoteService : IOneRemoteService, IDisposable
    {
        private readonly IOptions<OneRemoteOptions> _options;
        private readonly ILogger<OneRemoteService> _logger;
        private readonly Channel<SocketAsyncEventArgs> _socketPool;
        private readonly MemoryPool<byte> _memoryPool;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
        private readonly ConnectionStats _connectionStats;
        private readonly ThroughputStats _throughputStats;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<Guid, ConnectionInfo> _activeConnections;
        private readonly Dictionary<Guid, SessionInfo> _activeSessions;
        private readonly object _statsLock = new object();
        private readonly object _connectionsLock = new object();
        private bool _disposed = false;

        public OneRemoteService(
            IOptions<OneRemoteOptions> options,
            ILogger<OneRemoteService> logger)
        {
            _options = options;
            _logger = logger;
            _connectionStats = new ConnectionStats { LastConnectionTime = DateTime.UtcNow };
            _throughputStats = new ThroughputStats { LastResetTime = DateTime.UtcNow };
            _activeConnections = new Dictionary<Guid, ConnectionInfo>();
            _activeSessions = new Dictionary<Guid, SessionInfo>();

            // 初始化Socket对象池
            _socketPool = Channel.CreateBounded<SocketAsyncEventArgs>(
                new BoundedChannelOptions(options.Value.ConnectionPoolSize)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = false,
                    SingleWriter = false
                });

            // 初始化内存池
            _memoryPool = MemoryPool<byte>.Shared;

            // 初始化HttpClient
            _httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All,
                UseCookies = true,
                AllowAutoRedirect = true
            });
            _httpClient.Timeout = options.Value.ConnectionTimeout;

            // 配置重试策略
            _retryPolicy = Policy
                .Handle<SocketException>()
                .Or<TimeoutException>()
                .Or<HttpRequestException>()
                .WaitAndRetryAsync(
                    options.Value.RetryCount,
                    retryAttempt => options.Value.RetryInterval,
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, $"OneRemote operation failed. Retry attempt {retryCount}");
                        lock (_statsLock)
                        {
                            _connectionStats.FailedConnections++;
                        }
                    });

            // 配置熔断策略
            _circuitBreaker = Policy
                .Handle<SocketException>()
                .Or<TimeoutException>()
                .Or<HttpRequestException>()
                .CircuitBreakerAsync(
                    options.Value.CircuitBreakerThreshold,
                    options.Value.CircuitBreakerDuration,
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
            // 初始化Socket池
            for (int i = 0; i < _options.Value.ConnectionPoolSize; i++)
            {
                var socketArgs = new SocketAsyncEventArgs();
                if (_options.Value.EnableZeroCopy)
                {
                    var memory = _memoryPool.Rent(_options.Value.BufferSize);
                    socketArgs.SetBuffer(memory.Memory);
                }
                else
                {
                    socketArgs.SetBuffer(new byte[_options.Value.BufferSize], 0, _options.Value.BufferSize);
                }

                await _socketPool.Writer.WriteAsync(socketArgs, cancellationToken);
            }

            _logger.LogInformation("OneRemote service started with {ConnectionPoolSize} connections in pool",
                _options.Value.ConnectionPoolSize);
        }

        public async Task<ConnectionInfo> ConnectAsync(string host, int port, string protocol = "ssh", CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var socketArgs = await _socketPool.Reader.ReadAsync(cancellationToken);
                    try
                    {
                        var ipAddress = (await Dns.GetHostAddressesAsync(host, cancellationToken)).FirstOrDefault();
                        if (ipAddress == null)
                            throw new InvalidOperationException($"Could not resolve host: {host}");

                        socketArgs.RemoteEndPoint = new IPEndPoint(ipAddress, port);

                        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        await socket.ConnectAsync(socketArgs, cancellationToken);

                        var connectionId = Guid.NewGuid();
                        var connectionInfo = new ConnectionInfo
                        {
                            Id = connectionId,
                            Host = host,
                            Port = port,
                            Protocol = protocol,
                            Status = "Connected",
                            Timestamp = DateTime.UtcNow
                        };

                        lock (_statsLock)
                        {
                            _connectionStats.ActiveConnections++;
                            _connectionStats.TotalConnections++;
                            _connectionStats.LastConnectionTime = DateTime.UtcNow;
                        }

                        lock (_connectionsLock)
                        {
                            _activeConnections[connectionId] = connectionInfo;
                        }

                        _logger.LogInformation("Connected to {Host}:{Port} using {Protocol} protocol", host, port, protocol);

                        return connectionInfo;
                    }
                    finally
                    {
                        await _socketPool.Writer.WriteAsync(socketArgs, cancellationToken);
                    }
                });
            });
        }

        public async Task<SessionInfo> CreateSessionAsync(string host, int port, string protocol = "ssh", CancellationToken cancellationToken = default)
        {
            var connection = await ConnectAsync(host, port, protocol, cancellationToken);
            return new SessionInfo
            {
                Id = Guid.NewGuid(),
                ConnectionId = connection.Id,
                Protocol = protocol,
                StartTime = DateTime.UtcNow,
                Status = "Active"
            };
        }

        public Task DisconnectAsync(Guid connectionId, CancellationToken cancellationToken = default)
        {
            lock (_statsLock)
            {
                if (_connectionStats.ActiveConnections > 0)
                {
                    _connectionStats.ActiveConnections--;
                }
            }

            lock (_connectionsLock)
            {
                if (_activeConnections.ContainsKey(connectionId))
                {
                    _activeConnections.Remove(connectionId);
                }
            }

            _logger.LogInformation("Disconnected from connection {ConnectionId}", connectionId);
            return Task.CompletedTask;
        }

        public Task CloseSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            lock (_connectionsLock)
            {
                if (_activeSessions.ContainsKey(sessionId))
                {
                    _activeSessions.Remove(sessionId);
                }
            }

            _logger.LogInformation("Closed session {SessionId}", sessionId);
            return Task.CompletedTask;
        }

        public Task<ServerInfo> GetServerInfoAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ServerInfo
            {
                ServerName = _options.Value.ServerAddress,
                Version = "1.0.0",
                CurrentConnections = _connectionStats.ActiveConnections,
                MaxConnections = _options.Value.MaxConnections,
                Uptime = DateTime.UtcNow
            });
        }

        public Task<ConnectionStats> GetConnectionStatsAsync(CancellationToken cancellationToken = default)
        {
            lock (_statsLock)
            {
                return Task.FromResult(new ConnectionStats
                {
                    ActiveConnections = _connectionStats.ActiveConnections,
                    FailedConnections = _connectionStats.FailedConnections,
                    TotalConnections = _connectionStats.TotalConnections,
                    AverageConnectionTime = _connectionStats.AverageConnectionTime,
                    LastConnectionTime = _connectionStats.LastConnectionTime
                });
            }
        }

        public Task<ThroughputStats> GetThroughputStatsAsync(CancellationToken cancellationToken = default)
        {
            lock (_statsLock)
            {
                return Task.FromResult(new ThroughputStats
                {
                    BytesSent = _throughputStats.BytesSent,
                    BytesReceived = _throughputStats.BytesReceived,
                    BytesPerSecond = _throughputStats.BytesPerSecond,
                    OperationsPerSecond = _throughputStats.OperationsPerSecond,
                    LastResetTime = _throughputStats.LastResetTime
                });
            }
        }

        public Task ResetCircuitBreakerAsync(CancellationToken cancellationToken = default)
        {
            _circuitBreaker.Reset();
            _logger.LogInformation("Circuit breaker reset");
            return Task.CompletedTask;
        }

        public Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default)
        {
            var result = new HealthCheckResult
            {
                Status = "Healthy",
                CheckTime = DateTime.UtcNow
            };

            result.Details.Add("ServerAddress", _options.Value.ServerAddress);
            result.Details.Add("ServerPort", _options.Value.ServerPort.ToString());
            result.Details.Add("MaxConnections", _options.Value.MaxConnections.ToString());
            result.Details.Add("ActiveConnections", _connectionStats.ActiveConnections.ToString());
            result.Details.Add("FailedConnections", _connectionStats.FailedConnections.ToString());
            result.Details.Add("CircuitState", _circuitBreaker.CircuitState.ToString());

            _logger.LogInformation("Health check completed with status: {Status}", result.Status);

            return Task.FromResult(result);
        }

        public async Task<CommandResult> ExecuteCommandAsync(Guid sessionId, string command, CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var startTime = DateTime.UtcNow;
                    _logger.LogInformation("Executing command on session {SessionId}: {Command}", sessionId, command);

                    // 模拟命令执行
                    await Task.Delay(100, cancellationToken);

                    var result = new CommandResult
                    {
                        Output = $"Command executed: {command}",
                        ExitCode = 0,
                        ExecutionTime = DateTime.UtcNow - startTime,
                        Timestamp = DateTime.UtcNow
                    };

                    _logger.LogInformation("Command executed successfully on session {SessionId}, ExitCode: {ExitCode}", sessionId, result.ExitCode);
                    return result;
                });
            });
        }

        public async Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("Sending HTTP request: {Method} {Uri}", request.Method, request.RequestUri);
                    var response = await _httpClient.SendAsync(request, cancellationToken);
                    _logger.LogInformation("HTTP request completed with status: {StatusCode}", response.StatusCode);
                    return response;
                });
            });
        }

        public async Task<T> SendHttpRequestAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var response = await SendHttpRequestAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(content);
        }

        public async Task<byte[]> SendTcpRequestAsync(string host, int port, byte[] data, CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var socketArgs = await _socketPool.Reader.ReadAsync(cancellationToken);
                    try
                    {
                        var ipAddress = (await Dns.GetHostAddressesAsync(host, cancellationToken)).FirstOrDefault();
                        if (ipAddress == null)
                            throw new InvalidOperationException($"Could not resolve host: {host}");

                        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        await socket.ConnectAsync(ipAddress, port, cancellationToken);

                        // 发送数据
                        await socket.SendAsync(data, SocketFlags.None, cancellationToken);

                        // 接收响应
                        var buffer = new byte[4096];
                        var received = await socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
                        var responseData = new byte[received];
                        Array.Copy(buffer, responseData, received);

                        _logger.LogInformation("TCP request completed, sent {SentBytes} bytes, received {ReceivedBytes} bytes", data.Length, received);
                        return responseData;
                    }
                    finally
                    {
                        await _socketPool.Writer.WriteAsync(socketArgs, cancellationToken);
                    }
                });
            });
        }

        public Task<IEnumerable<ConnectionInfo>> GetActiveConnectionsAsync(CancellationToken cancellationToken = default)
        {
            lock (_connectionsLock)
            {
                return Task.FromResult(_activeConnections.Values.AsEnumerable());
            }
        }

        public Task<IEnumerable<SessionInfo>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
        {
            lock (_connectionsLock)
            {
                return Task.FromResult(_activeSessions.Values.AsEnumerable());
            }
        }

        public async Task<int> GetConnectionPoolUsageAsync(CancellationToken cancellationToken = default)
        {
            // 计算连接池使用情况
            var usage = 0;
            while (_socketPool.Reader.TryPeek(out _))
            {
                usage++;
            }
            return usage;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return HealthCheckAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            // 清理资源
            while (_socketPool.Reader.TryRead(out var socketArgs))
            {
                socketArgs.Dispose();
            }

            _socketPool.Writer.Complete();
            _httpClient.Dispose();
            
            _logger.LogInformation("OneRemote service stopped");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _socketPool.Writer.Complete();
                _httpClient.Dispose();
                _disposed = true;
            }
        }
    }

    // 健康检查
    public class OneRemoteHealthCheck : IHealthCheck
    {
        private readonly IOneRemoteService _oneRemoteService;

        public OneRemoteHealthCheck(IOneRemoteService oneRemoteService)
        {
            _oneRemoteService = oneRemoteService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _oneRemoteService.HealthCheckAsync(cancellationToken);
                if (result.Status == "Healthy")
                {
                    return HealthCheckResult.Healthy("OneRemote service is healthy", result.Details);
                }
                else
                {
                    return HealthCheckResult.Unhealthy("OneRemote service is unhealthy", null, result.Details);
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("OneRemote service health check failed", ex);
            }
        }
    }

    // 依赖注入扩展
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOneRemoteService(this IServiceCollection services, Action<OneRemoteOptions> configureOptions)
        {
            // 配置选项
            services.Configure(configureOptions);

            // 注册服务
            services.AddSingleton<IOneRemoteService, OneRemoteService>();

            // 注册健康检查
            services.AddHealthChecks()
                .AddCheck<OneRemoteHealthCheck>("OneRemote");

            // 注册内存缓存
            services.AddMemoryCache();

            // 注册对象池
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton(serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                var policy = new DefaultPooledObjectPolicy<byte[]>();
                return provider.Create(policy);
            });

            return services;
        }
    }

    // 主程序入口
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("OneRemote Integration Service");
            Console.WriteLine("=" * 50);

            // 构建服务容器
            var builder = new ServiceCollection();

            // 配置日志
            builder.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 注册 OneRemote 服务
            builder.AddOneRemoteService(options =>
            {
                options.ServerAddress = "localhost";
                options.ServerPort = 8080;
                options.AuthToken = "your-auth-token";
                options.MaxConnections = 100;
                options.ConnectionTimeout = TimeSpan.FromMinutes(5);
                options.ConnectionPoolSize = 100;
                options.RetryCount = 3;
                options.RetryInterval = TimeSpan.FromSeconds(5);
                options.CircuitBreakerThreshold = 5;
                options.CircuitBreakerDuration = TimeSpan.FromMinutes(1);
                options.BufferSize = 8192;
                options.EnableZeroCopy = true;
                options.MemoryPoolSize = 1024 * 1024;
                options.EnableMetrics = true;
                options.EnableTracing = true;
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
            var oneRemoteService = serviceProvider.GetRequiredService<IOneRemoteService>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // 启动服务
                await oneRemoteService.StartAsync();
                logger.LogInformation("OneRemote service started successfully");

                // 测试连接
                var connection = await oneRemoteService.ConnectAsync("localhost", 22, "ssh");
                logger.LogInformation("Connected successfully: {ConnectionId}", connection.Id);

                // 获取服务器信息
                var serverInfo = await oneRemoteService.GetServerInfoAsync();
                logger.LogInformation("Server info: {ServerName}, Version: {Version}", serverInfo.ServerName, serverInfo.Version);

                // 获取连接统计
                var connectionStats = await oneRemoteService.GetConnectionStatsAsync();
                logger.LogInformation("Connection stats: Active={Active}, Total={Total}, Failed={Failed}",
                    connectionStats.ActiveConnections, connectionStats.TotalConnections, connectionStats.FailedConnections);

                // 健康检查
                var healthCheck = await oneRemoteService.HealthCheckAsync();
                logger.LogInformation("Health check: {Status}", healthCheck.Status);

                // 断开连接
                await oneRemoteService.DisconnectAsync(connection.Id);
                logger.LogInformation("Disconnected successfully: {ConnectionId}", connection.Id);

                // 获取连接统计（断开后）
                connectionStats = await oneRemoteService.GetConnectionStatsAsync();
                logger.LogInformation("Connection stats after disconnect: Active={Active}, Total={Total}, Failed={Failed}",
                    connectionStats.ActiveConnections, connectionStats.TotalConnections, connectionStats.FailedConnections);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during OneRemote service operation");
            }
            finally
            {
                // 停止服务
                await oneRemoteService.StopAsync();
                logger.LogInformation("OneRemote service stopped successfully");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
