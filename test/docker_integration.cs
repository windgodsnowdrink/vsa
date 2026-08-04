#:sdk Microsoft.NET.Sdk.Web
#:package Docker.DotNet@3.125.12
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.19.6
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property UserSecretsId 210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS Linux
#:property DockerComposeProjectPath ..\docker-compose.dcproj

using Docker.DotNet;
using Docker.DotNet.Models;
using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using System.Buffers;

namespace DockerIntegration
{
    public static class DockerServiceCollectionExtensions
    {
        public static IServiceCollection AddDockerIntegration(this IServiceCollection services, Action<DockerOptions> configure = null)
        {
            var options = new DockerOptions();
            configure?.Invoke(options);

            services.AddSingleton(options);
            services.AddSingleton<DockerClient>(sp => new DockerClientConfiguration(new Uri(options.DockerEndpoint)).CreateClient());
            services.AddSingleton<DockerContainerManager>();
            services.AddSingleton<DockerImageBuilder>();
            services.AddSingleton<DockerNetworkManager>();
            services.AddSingleton<DockerMonitorService>();
            services.AddSingleton<DockerLogCollector>();
            services.AddSingleton<DockerHealthChecker>();
            
            // 高性能通道用于容器事件处理
            services.AddSingleton(Channel.CreateUnbounded<ContainerEvent>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            }));
            
            // 对象池优化
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton(sp => sp.GetRequiredService<ObjectPoolProvider>().Create(new DefaultPooledObjectPolicy<byte[]>()));
            
            return services;
        }
    }

    public class DockerOptions
    {
        public string DockerEndpoint { get; set; } = "npipe://./pipe/docker_engine";
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }

    public class DockerContainerManager
    {
        private readonly DockerClient _dockerClient;
        private readonly Channel<ContainerEvent> _eventChannel;
        private readonly ILogger<DockerContainerManager> _logger;

        public DockerContainerManager(DockerClient dockerClient, Channel<ContainerEvent> eventChannel, ILogger<DockerContainerManager> logger)
        {
            _dockerClient = dockerClient;
            _eventChannel = eventChannel;
            _logger = logger;
        }

        public async Task<string> CreateContainerAsync(CreateContainerParameters parameters, CancellationToken cancellationToken = default)
        {
            var response = await _dockerClient.Containers.CreateContainerAsync(parameters, cancellationToken);
            await _dockerClient.Containers.StartContainerAsync(response.ID, new ContainerStartParameters(), cancellationToken);
            
            await _eventChannel.Writer.WriteAsync(new ContainerEvent
            {
                ContainerId = response.ID,
                EventType = ContainerEventType.Started,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
            
            return response.ID;
        }

        // 其他容器管理方法...
    }

    public class DockerImageBuilder
    {
        private readonly DockerClient _dockerClient;
        private readonly ILogger<DockerImageBuilder> _logger;

        public DockerImageBuilder(DockerClient dockerClient, ILogger<DockerImageBuilder> logger)
        {
            _dockerClient = dockerClient;
            _logger = logger;
        }

        public async Task BuildImageAsync(ImageBuildParameters parameters, Stream dockerfileContext, CancellationToken cancellationToken = default)
        {
            using var response = await _dockerClient.Images.BuildImageFromDockerfileAsync(
                dockerfileContext, 
                parameters, 
                cancellationToken);
            
            // 处理构建输出流...
        }
    }

    public class DockerNetworkManager
    {
        private readonly DockerClient _dockerClient;
        private readonly ILogger<DockerNetworkManager> _logger;

        public DockerNetworkManager(DockerClient dockerClient, ILogger<DockerNetworkManager> logger)
        {
            _dockerClient = dockerClient;
            _logger = logger;
        }

        public async Task<string> CreateNetworkAsync(NetworkCreateParameters parameters, CancellationToken cancellationToken = default)
        {
            var response = await _dockerClient.Networks.CreateNetworkAsync(parameters, cancellationToken);
            return response.ID;
        }
    }

    public enum ContainerEventType
    {
        Started,
        Stopped,
        Restarted,
        Removed
    }

    public record ContainerEvent
    {
        public string ContainerId { get; init; }
        public ContainerEventType EventType { get; init; }
        public DateTime Timestamp { get; init; }
    }

    public class DockerMonitorService
    {
        private readonly DockerClient _dockerClient;
        private readonly ILogger<DockerMonitorService> _logger;
        private readonly ChannelWriter<ContainerMetrics> _metricsChannel;

        public DockerMonitorService(DockerClient dockerClient, ILogger<DockerMonitorService> logger)
        {
            _dockerClient = dockerClient;
            _logger = logger;
            _metricsChannel = Channel.CreateUnbounded<ContainerMetrics>().Writer;
        }

        public async Task StartMonitoringAsync(string containerId, CancellationToken cancellationToken = default)
        {
            var stats = await _dockerClient.Containers.GetContainerStatsAsync(containerId, new ContainerStatsParameters
            {
                Stream = true
            }, cancellationToken);

            // 使用Span零拷贝处理性能数据
            await foreach (var stat in stats.WithCancellation(cancellationToken))
            {
                var metrics = new ContainerMetrics
                {
                    ContainerId = containerId,
                    CpuUsage = stat.CPUStats.CPUUsage.TotalUsage,
                    MemoryUsage = stat.MemoryStats.Usage,
                    Timestamp = DateTime.UtcNow
                };
                await _metricsChannel.WriteAsync(metrics, cancellationToken);
            }
        }
    }

    public class DockerLogCollector
    {
        private readonly DockerClient _dockerClient;
        private readonly ILogger<DockerLogCollector> _logger;

        public DockerLogCollector(DockerClient dockerClient, ILogger<DockerLogCollector> logger)
        {
            _dockerClient = dockerClient;
            _logger = logger;
        }

        public async Task<Stream> GetContainerLogsAsync(string containerId, CancellationToken cancellationToken = default)
        {
            return await _dockerClient.Containers.GetContainerLogsAsync(containerId, new ContainerLogsParameters
            {
                ShowStdout = true,
                ShowStderr = true,
                Follow = false,
                Timestamps = true
            }, cancellationToken);
        }
    }

    public class DockerHealthChecker
    {
        private readonly DockerClient _dockerClient;
        private readonly ILogger<DockerHealthChecker> _logger;

        public DockerHealthChecker(DockerClient dockerClient, ILogger<DockerHealthChecker> logger)
        {
            _dockerClient = dockerClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckContainerHealthAsync(string containerId, CancellationToken cancellationToken = default)
        {
            var inspect = await _dockerClient.Containers.InspectContainerAsync(containerId, cancellationToken);
            return new HealthCheckResult
            {
                ContainerId = containerId,
                Status = inspect.State.Health.Status,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    public record ContainerMetrics
    {
        public string ContainerId { get; init; }
        public ulong CpuUsage { get; init; }
        public ulong MemoryUsage { get; init; }
        public DateTime Timestamp { get; init; }
    }

    public record HealthCheckResult
    {
        public string ContainerId { get; init; }
        public string Status { get; init; }
        public DateTime Timestamp { get; init; }
    }
}