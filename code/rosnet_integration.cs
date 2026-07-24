#:sdk Microsoft.NET.Sdk.Web
#:package Roslynator.Analyzers@4.1.0
#:package Roslynator.Formatting.Analyzers@4.1.0
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.19.6
#:package ROS.NET@1.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property DockerDefaultTargetOS Linux

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ROS;
using ROS.Nodes;
using ROS.Parameter;
using ROS.Publishers;
using ROS.Subscribers;

namespace RosNetIntegration
{
    public class RosOptions
    {
        public string RosMasterUri { get; set; } = "http://localhost:11311";
        public string NodeName { get; set; } = "dotnet_node";
        public string TopicName { get; set; } = "chatter";
        public int MaxRetryCount { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(1);
        public bool EnableTracing { get; set; } = true;
        public bool EnableMetrics { get; set; } = true;
    }

    public interface IRosService
    {
        Task PublishAsync(string message, CancellationToken cancellationToken = default);
        Task<string> SubscribeAsync(CancellationToken cancellationToken = default);
    }

    public class RosService : IRosService, IDisposable
    {
        private readonly ILogger<RosService> _logger;
        private readonly ActivitySource _activitySource;
        private readonly RosNode _node;
        private readonly Publisher<std_msgs.String> _publisher;
        private readonly Subscriber<std_msgs.String> _subscriber;
        private readonly Channel<string> _messageChannel = Channel.CreateUnbounded<string>();
        private readonly IOptionsMonitor<RosOptions> _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ObjectPool<RosNode> _nodePool;

        public RosService(
            ILogger<RosService> logger, 
            IOptionsMonitor<RosOptions> options,
            IHttpClientFactory httpClientFactory,
            ObjectPool<RosNode> nodePool,
            ActivitySource activitySource)
        {
            _logger = logger;
            _options = options;
            _httpClientFactory = httpClientFactory;
            _nodePool = nodePool;
            _activitySource = activitySource;
            
            _node = _nodePool.Get();
            _node.Initialize(_options.CurrentValue.NodeName, _options.CurrentValue.RosMasterUri);
            
            _publisher = _node.CreatePublisher<std_msgs.String>(_options.CurrentValue.TopicName);
            _subscriber = _node.CreateSubscriber<std_msgs.String>(_options.CurrentValue.TopicName, msg =>
            {
                using var activity = _activitySource.StartActivity("ROS.MessageProcessing");
                try
                {
                    _messageChannel.Writer.TryWrite(msg.Data);
                    _logger.LogInformation($"Received: {msg.Data}");
                    activity?.AddTag("message.size", msg.Data.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing ROS message");
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                }
            });

            _node.Start();
            _logger.LogInformation($"ROS node {_options.CurrentValue.NodeName} started");
        }

        public async Task PublishAsync(string message, CancellationToken cancellationToken = default)
        {
            await _publisher.PublishAsync(new std_msgs.String { Data = message }, cancellationToken);
            _logger.LogInformation($"Published: {message}");
        }

        public async Task<string> SubscribeAsync(CancellationToken cancellationToken = default)
        {
            return await _messageChannel.Reader.ReadAsync(cancellationToken);
        }

        public void Dispose()
        {
            _node?.Dispose();
            _messageChannel.Writer.Complete();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRosService(this IServiceCollection services, Action<RosOptions> configure = null)
        {
            services.Configure(configure ?? (opt => { }));
            
            services.AddSingleton<ActivitySource>(new ActivitySource("ROS.NET"));
            services.AddHttpClient();
            
            services.AddSingleton<ObjectPool<RosNode>>(sp => 
            {
                var policy = new DefaultPooledObjectPolicy<RosNode>();
                return new DefaultObjectPool<RosNode>(policy, Environment.ProcessorCount * 2);
            });
            
            services.AddSingleton<IRosService, RosService>();
            
            // Add health check
            services.AddHealthChecks()
                .AddCheck<RosHealthCheck>("ros_health");
                
            // Add metrics
            services.AddMetrics()
                .AddRosMetrics();
                
            return services;
        }
    }
    
    public class RosHealthCheck : IHealthCheck
    {
        private readonly IRosService _rosService;
        
        public RosHealthCheck(IRosService rosService)
        {
            _rosService = rosService;
        }
        
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Add actual health check logic here
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("ROS service unhealthy", ex));
            }
        }
    }
    
    public static class RosMetricsExtensions
    {
        public static IMetricsBuilder AddRosMetrics(this IMetricsBuilder builder)
        {
            builder.AddMeter("ROS.NET");
            return builder;
        }
    }

    public class RosBackgroundService : BackgroundService
    {
        private readonly IRosService _rosService;
        private readonly ILogger<RosBackgroundService> _logger;
        private readonly IOptionsMonitor<RosOptions> _options;
        private readonly ActivitySource _activitySource;
        private readonly IMeterFactory _meterFactory;
        private readonly Counter<int> _messageCounter;
        
        public RosBackgroundService(
            IRosService rosService, 
            ILogger<RosBackgroundService> logger,
            IOptionsMonitor<RosOptions> options,
            ActivitySource activitySource,
            IMeterFactory meterFactory)
        {
            _rosService = rosService;
            _logger = logger;
            _options = options;
            _activitySource = activitySource;
            _meterFactory = meterFactory;
            
            var meter = _meterFactory.Create("ROS.NET");
            _messageCounter = meter.CreateCounter<int>("ros.messages.published");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var activity = _activitySource.StartActivity("ROS.BackgroundService");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var message = $"Hello ROS.NET at {DateTime.Now}";
                    
                    using (var publishActivity = _activitySource.StartActivity("ROS.Publish"))
                    {
                        await _rosService.PublishAsync(message, stoppingToken);
                        _messageCounter.Add(1);
                        publishActivity?.AddTag("message", message);
                    }
                    
                    using (var subscribeActivity = _activitySource.StartActivity("ROS.Subscribe"))
                    {
                        var receivedMessage = await _rosService.SubscribeAsync(stoppingToken);
                        _logger.LogInformation($"Received message: {receivedMessage}");
                        subscribeActivity?.AddTag("message", receivedMessage);
                    }
                    
                    await Task.Delay(_options.CurrentValue.PollingInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ROS service");
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddRosService(options =>
                    {
                        options.RosMasterUri = "http://localhost:11311";
                        options.NodeName = "dotnet_ros_node";
                        options.TopicName = "chatter";
                    });
                    services.AddHostedService<RosBackgroundService>();
                })
                .Build();

            host.Run();
        }
    }
}