#:sdk Microsoft.NET.Sdk.Web
#:package FastTunnel.Core@2.4.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using FastTunnel.Core;
using FastTunnel.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

namespace FastTunnelIntegration
{
    public class FastTunnelOptions
    {
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public string Token { get; set; }
        public int MaxConnections { get; set; } = 100;
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromMinutes(5);
        public bool EnableMetrics { get; set; } = true;
        public bool EnableTracing { get; set; } = true;
        public bool EnableMultiTenancy { get; set; } = false;
        public int RetryCount { get; set; } = 3;
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(5);
        public int CircuitBreakerThreshold { get; set; } = 5;
        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromMinutes(1);
        public int BufferSize { get; set; } = 8192;
        public bool EnableZeroCopy { get; set; } = true;
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    }

    public interface IFastTunnelService
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task<ForwardInfo> CreateForwardAsync(string remoteHost, int remotePort, string localHost, int localPort, string tenantId = null);
        Task RemoveForwardAsync(Guid forwardId, string tenantId = null);
        Task<IEnumerable<ForwardInfo>> GetAllForwardsAsync(string tenantId = null);
        Task<ServerInfo> GetServerInfoAsync();
        Task<ConnectionStats> GetConnectionStatsAsync();
        Task<ThroughputStats> GetThroughputStatsAsync();
        Task ResetCircuitBreakerAsync();
        Task<HealthCheckResult> HealthCheckAsync();
    }

    public class FastTunnelService : IFastTunnelService, IDisposable
    {
        private readonly FastTunnelServer _server;
        private readonly IOptions<FastTunnelOptions> _options;
        private readonly ILogger<FastTunnelService> _logger;

        public FastTunnelService(
            IOptions<FastTunnelOptions> options,
            ILogger<FastTunnelService> logger)
        {
            _options = options;
            _logger = logger;
            
            _server = new FastTunnelServer(new ServerConfig
            {
                ServerAddr = options.Value.ServerAddress,
                ServerPort = options.Value.ServerPort,
                Token = options.Value.Token,
                MaxConnections = options.Value.MaxConnections,
                ConnectionTimeout = (int)options.Value.ConnectionTimeout.TotalMilliseconds
            });
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _server.StartAsync();
            _logger.LogInformation("FastTunnel server started at {ServerAddress}:{ServerPort}", 
                _options.Value.ServerAddress, _options.Value.ServerPort);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _server.StopAsync();
            _logger.LogInformation("FastTunnel server stopped");
        }

        public Task<ForwardInfo> CreateForwardAsync(string remoteHost, int remotePort, string localHost, int localPort, string tenantId = null)
        {
            // 多租户验证
            if (_options.Value.EnableMultiTenancy && string.IsNullOrEmpty(tenantId))
                throw new ArgumentNullException(nameof(tenantId), "Tenant ID is required when multi-tenancy is enabled");

            // 弹性策略实现
            var policy = Policy
                .Handle<SocketException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    _options.Value.RetryCount,
                    retryAttempt => _options.Value.RetryInterval,
                    (exception, timeSpan, retryCount, context) => 
                    {
                        _logger.LogWarning(exception, $"FastTunnel forward creation failed. Retry attempt {retryCount}");
                    });

            // 性能优化 - 使用零拷贝缓冲区
            var forwardTask = policy.ExecuteAsync(async () => 
            {
                using var buffer = MemoryPool<byte>.Shared.Rent(_options.Value.BufferSize);
                var memory = buffer.Memory.Slice(0, _options.Value.BufferSize);

                var forward = new ForwardInfo
                {
                    Id = Guid.NewGuid(),
                    RemoteHost = remoteHost,
                    RemotePort = remotePort,
                    LocalHost = localHost,
                    LocalPort = localPort,
                    CreateTime = DateTime.UtcNow,
                    TenantId = tenantId
                };

                await _server.AddForwardAsync(forward, memory);
                return forward;
            });

            return forwardTask;
        }

        public Task RemoveForwardAsync(Guid forwardId)
        {
            _server.RemoveForward(forwardId);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ForwardInfo>> GetAllForwardsAsync()
        {
            return Task.FromResult(_server.GetAllForwards());
        }

        public Task<ServerInfo> GetServerInfoAsync()
        {
            // 高级监控 - 收集服务器信息
            var info = new ServerInfo
            {
                CurrentConnections = _server.CurrentConnections,
                MaxConnections = _server.MaxConnections,
                Version = _server.Version,
                Uptime = DateTime.UtcNow - _server.StartTime,
                MemoryUsage = Process.GetCurrentProcess().WorkingSet64,
                CpuUsage = _performanceCounter.NextValue()
            };

            // 记录指标
            _metrics.RecordServerInfo(info);
            return Task.FromResult(info);
        }

        public Task<ConnectionStats> GetConnectionStatsAsync()
        {
            return Task.FromResult(_connectionMonitor.GetStats());
        }

        public Task<ThroughputStats> GetThroughputStatsAsync()
        {
            return Task.FromResult(_throughputAnalyzer.GetStats());
        }

        public Task ResetCircuitBreakerAsync()
        {
            _circuitBreaker.Reset();
            return Task.CompletedTask;
        }

        public async Task<HealthCheckResult> HealthCheckAsync()
        {
            try
            {
                var result = await _healthCheckService.CheckHealthAsync();
                return new HealthCheckResult
                {
                    Status = result.Status,
                    Details = result.Entries
                        .ToDictionary(e => e.Key, e => e.Value.Description)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return new HealthCheckResult
                {
                    Status = HealthStatus.Unhealthy,
                    Details = new Dictionary<string, string> { { "Error", ex.Message } }
                };
            }
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFastTunnelService(this IServiceCollection services, 
            Action<FastTunnelOptions> configureOptions)
        {
            services.Configure(configureOptions);
            
            services.AddSingleton<IFastTunnelService, FastTunnelService>();
            
            services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics
                    .AddFastTunnelInstrumentation()
                    .AddMeter("FastTunnel.ConnectionStats")
                    .AddMeter("FastTunnel.ThroughputStats"))
                .WithTracing(tracing => tracing
                    .AddFastTunnelInstrumentation()
                    .AddSource("FastTunnel.CircuitBreaker"));
                    
            services.AddHealthChecks()
                .AddCheck<FastTunnelHealthCheck>("FastTunnel");
                
            services.AddMemoryCache();
            services.AddObjectPool<SocketAsyncEventArgs>();
            
            return services;
        }
    }

    public class ForwardInfo
    {
        public Guid Id { get; set; }
        public string RemoteHost { get; set; }
        public int RemotePort { get; set; }
        public string LocalHost { get; set; }
        public int LocalPort { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class ServerInfo
    {
        public int CurrentConnections { get; set; }
        public int MaxConnections { get; set; }
        public string Version { get; set; }
    }
}