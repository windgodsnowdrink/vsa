#:sdk Microsoft.NET.Sdk
#:package csharp-nats@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Options@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using System;
using System.Buffers;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NATS.Client;

// 配置选项
public class NatsOptions
{
    public string Url { get; set; } = "nats://localhost:4222";
    public int ConnectionTimeout { get; set; } = 5000;
    public int MaxPayloadSize { get; set; } = 1048576; // 1MB
    public int ReconnectWait { get; set; } = 2000;
}

// 高性能消息发布器
public class NatsPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly ChannelWriter<byte[]> _channelWriter;
    private readonly Channel<byte[]> _channel = Channel.CreateBounded<byte[]>(10000);

    public NatsPublisher(NatsOptions options)
    {
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = options.Url;
        opts.Timeout = options.ConnectionTimeout;
        opts.ReconnectWait = options.ReconnectWait;
        opts.MaxPayload = options.MaxPayloadSize;

        _connection = new ConnectionFactory().CreateConnection(opts);
        _channelWriter = _channel.Writer;

        // 后台处理线程
        Task.Run(ProcessMessagesAsync);
    }

    private async Task ProcessMessagesAsync()
    {
        await foreach (var message in _channel.Reader.ReadAllAsync())
        {
            try
            {
                _connection.Publish("demo.subject", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message publish failed: {ex.Message}");
            }
        }
    }

    public ValueTask PublishAsync(byte[] message) => _channelWriter.WriteAsync(message);

    public void Dispose()
    {
        _channelWriter.Complete();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}

// 消息订阅器
public class NatsSubscriber : IDisposable
{
    private readonly IConnection _connection;
    private IAsyncSubscription _subscription;
    private readonly ChannelWriter<byte[]> _messageChannel;

    public NatsSubscriber(NatsOptions options, ChannelWriter<byte[]> messageChannel)
    {
        _messageChannel = messageChannel;

        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = options.Url;
        opts.Timeout = options.ConnectionTimeout;
        opts.MaxPayload = options.MaxPayloadSize;

        _connection = new ConnectionFactory().CreateConnection(opts);
    }

    public void Start(string subject, string queueGroup = null)
    {
        _subscription = string.IsNullOrEmpty(queueGroup)
            ? _connection.SubscribeAsync(subject)
            : _connection.SubscribeAsync(subject, queueGroup);

        _subscription.MessageHandler += (_, args) =>
        {
            _messageChannel.TryWrite(args.Message.Data);
        };

        _subscription.Start();
    }

    public void Dispose()
    {
        _subscription?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}

// DI扩展
public static class NatsServiceCollectionExtensions
{
    public static IServiceCollection AddNats(this IServiceCollection services, Action<NatsOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<NatsPublisher>();
        services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<NatsOptions>>().Value;
            var channel = Channel.CreateBounded<byte[]>(10000);
            return new NatsSubscriber(options, channel.Writer);
        });

        return services;
    }
}

// 请求/响应模式实现
public class NatsRequestResponse : IDisposable
{
    private readonly IConnection _connection;
    private readonly IAsyncSubscription _subscription;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _pendingRequests = new();

    public NatsRequestResponse(NatsOptions options)
    {
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = options.Url;
        _connection = new ConnectionFactory().CreateConnection(opts);
        
        // 为每个实例生成唯一回复主题
        var replyTo = $"_INBOX.{Guid.NewGuid():N}";
        _subscription = _connection.SubscribeAsync(replyTo);
        _subscription.MessageHandler += (_, args) =>
        {
            var correlationId = Encoding.UTF8.GetString(args.Message.Header["correlation-id"]);
            if (_pendingRequests.TryRemove(correlationId, out var tcs))
            {
                tcs.TrySetResult(args.Message.Data);
            }
        };
        _subscription.Start();
    }

    public async Task<byte[]> RequestAsync(string subject, byte[] payload, TimeSpan timeout)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        var tcs = new TaskCompletionSource<byte[]>();
        _pendingRequests[correlationId] = tcs;

        var msg = new Msg(subject, payload);
        msg.Header["correlation-id"] = Encoding.UTF8.GetBytes(correlationId);
        msg.Header["reply-to"] = Encoding.UTF8.GetBytes(_subscription.Subject);
        
        _connection.Publish(msg);
        
        using var cts = new CancellationTokenSource(timeout);
        cts.Token.Register(() => tcs.TrySetCanceled());
        
        return await tcs.Task;
    }

    public void Dispose()
    {
        _subscription?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}

// 消息持久化实现
public class NatsPersistentStorage
{
    private readonly IConnection _connection;
    private readonly string _storageSubject;
    private readonly Channel<byte[]> _persistenceChannel = Channel.CreateBounded<byte[]>(1000);

    public NatsPersistentStorage(NatsOptions options, string storageSubject)
    {
        _storageSubject = storageSubject;
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = options.Url;
        _connection = new ConnectionFactory().CreateConnection(opts);
        
        // 后台持久化线程
        Task.Run(PersistMessagesAsync);
    }

    private async Task PersistMessagesAsync()
    {
        await foreach (var message in _persistenceChannel.Reader.ReadAllAsync())
        {
            try
            {
                // 实际生产中可替换为数据库操作
                File.AppendAllText("nats_messages.log", $"{DateTime.UtcNow:O}:{Convert.ToBase64String(message)}\n");
                _connection.Publish(_storageSubject, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message persistence failed: {ex.Message}");
            }
        }
    }

    public ValueTask PersistAsync(byte[] message) => _persistenceChannel.Writer.WriteAsync(message);
}

// 分布式追踪集成
public static class NatsTracingExtensions
{
    public static IServiceCollection AddNatsTracing(this IServiceCollection services, Action<NatsTracingOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<NatsTracingInterceptor>();
        return services;
    }
}

public class NatsTracingOptions
{
    public string ServiceName { get; set; } = "nats-service";
}

public class NatsTracingInterceptor
{
    private readonly ActivitySource _activitySource;

    public NatsTracingInterceptor(IOptions<NatsTracingOptions> options)
    {
        _activitySource = new ActivitySource(options.Value.ServiceName);
    }

    public Activity? StartPublishActivity(string subject)
    {
        var activity = _activitySource.StartActivity($"NATS Publish: {subject}", ActivityKind.Producer);
        activity?.AddTag("messaging.system", "nats");
        activity?.AddTag("messaging.destination", subject);
        return activity;
    }

    public Activity? StartConsumeActivity(string subject)
    {
        var activity = _activitySource.StartActivity($"NATS Consume: {subject}", ActivityKind.Consumer);
        activity?.AddTag("messaging.system", "nats");
        activity?.AddTag("messaging.destination", subject);
        return activity;
    }
}

// 性能监控
public class NatsMetrics
{
    private readonly Meter _meter;
    private readonly Counter<long> _messagesPublished;
    private readonly Counter<long> _messagesConsumed;
    private readonly Histogram<double> _publishLatency;

    public NatsMetrics(string meterName = "Nats")
    {
        _meter = new Meter(meterName);
        _messagesPublished = _meter.CreateCounter<long>("nats.messages.published", "count", "Number of published messages");
        _messagesConsumed = _meter.CreateCounter<long>("nats.messages.consumed", "count", "Number of consumed messages");
        _publishLatency = _meter.CreateHistogram<double>("nats.publish.latency", "ms", "Publish latency in milliseconds");
    }

    public void RecordPublish(string subject, double latencyMs) 
    {
        _messagesPublished.Add(1, new("subject", subject));
        _publishLatency.Record(latencyMs, new("subject", subject));
    }

    public void RecordConsume(string subject) => _messagesConsumed.Add(1, new("subject", subject));
}

// 集群配置扩展
public static class NatsClusterExtensions
{
    public static IServiceCollection AddNatsCluster(this IServiceCollection services, Action<NatsClusterOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<NatsClusterManager>();
        return services;
    }
}

public class NatsClusterOptions
{
    public string[] Nodes { get; set; } = Array.Empty<string>();
    public string ClusterName { get; set; } = "default-cluster";
    public TimeSpan NodeDiscoveryInterval { get; set; } = TimeSpan.FromSeconds(30);
}

public class NatsClusterManager : IDisposable
{
    private readonly IConnection _connection;
    private readonly Timer _discoveryTimer;
    private readonly NatsClusterOptions _options;

    public NatsClusterManager(IOptions<NatsClusterOptions> options, NatsPublisher publisher)
    {
        _options = options.Value;
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = string.Join(",", _options.Nodes);
        _connection = new ConnectionFactory().CreateConnection(opts);
        
        _discoveryTimer = new Timer(DiscoverNodes, null, 
            TimeSpan.Zero, _options.NodeDiscoveryInterval);
    }

    private void DiscoverNodes(object? state)
    {
        try
        {
            var discovered = _connection.DiscoveredServers;
            // 处理发现的节点
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Node discovery failed: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _discoveryTimer?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}

// 示例用法
public static class DemoUsage
{
    public static async Task RunAsync()
    {
        var services = new ServiceCollection();
        services.AddNats(options =>
        {
            options.Url = "nats://localhost:4222";
            options.MaxPayloadSize = 10 * 1024 * 1024; // 10MB
        });
        
        // 添加扩展功能
        services.AddNatsTracing(o => o.ServiceName = "order-service");
        services.AddNatsCluster(o => 
        {
            o.Nodes = ["nats://node1:4222", "nats://node2:4222"];
            o.ClusterName = "order-cluster";
        });

        var provider = services.BuildServiceProvider();
        var publisher = provider.GetRequiredService<NatsPublisher>();
        var subscriber = provider.GetRequiredService<NatsSubscriber>();
        var requestResponse = provider.GetRequiredService<NatsRequestResponse>();
        var persistentStorage = new NatsPersistentStorage(
            provider.GetRequiredService<IOptions<NatsOptions>>().Value, 
            "persistent.orders");

        // 启动订阅
        var messageChannel = Channel.CreateBounded<byte[]>(10000);
        subscriber.Start("demo.subject");

        // 发布消息
        await publisher.PublishAsync(Encoding.UTF8.GetBytes("Hello NATS!"));
        
        // 请求/响应示例
        var response = await requestResponse.RequestAsync("get.order", 
            Encoding.UTF8.GetBytes("order-123"), 
            TimeSpan.FromSeconds(5));
            
        // 持久化示例
        await persistentStorage.PersistAsync(Encoding.UTF8.GetBytes("Important data"));

        // 处理接收到的消息
        await foreach (var message in messageChannel.Reader.ReadAllAsync())
        {
            Console.WriteLine($"Received: {Encoding.UTF8.GetString(message)}");
        }
    }
}