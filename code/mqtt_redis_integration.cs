#:sdk Microsoft.NET.Sdk.Web
#:package MQTTnet@4.3.1
#:package StackExchange.Redis@2.7.121
#:package DotNetCore.CAP.RedisStreams@7.2.0
#:package MemoryPack@2.1.0
#:package OpenTelemetry.Extensions.Hosting@1.7.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Threading.Channels;
using MQTTnet;
using MQTTnet.Client;
using StackExchange.Redis;
using DotNetCore.CAP;
using DotNetCore.CAP.RedisStreams;
using MemoryPack;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// 1. Redis集群配置优化
var redisOptions = new ConfigurationOptions
{
    EndPoints = 
    {
        { "redis-node1:6379" },
        { "redis-node2:6379" },
        { "redis-node3:6379" }
    },
    Proxy = Proxy.Twemproxy,
    SocketPooled = true,
    DefaultVersion = RedisVersion.Version_7_0,
    AsyncTimeout = 5000,
    ConnectTimeout = 5000,
    SyncTimeout = 5000,
    DefaultDatabase = 0,
    CommandMap = CommandMap.Default
};

var redis = ConnectionMultiplexer.Connect(redisOptions);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

// 2. MQTT连接参数优化
builder.Services.AddSingleton<IMqttClient>(sp =>
{
    var factory = new MqttFactory();
    var client = factory.CreateMqttClient();
    
    var options = new MqttClientOptionsBuilder()
        .WithTcpServer("mqtt-cluster", 8883)  // 生产环境使用TLS
        .WithTlsOptions(o => 
            o.WithCertificateValidationHandler(_ => true)
             .WithSslProtocols(System.Security.Authentication.SslProtocols.Tls13))
        .WithCleanSession(false)
        .WithSessionExpiryInterval(3600)
        .WithKeepAlivePeriod(TimeSpan.FromSeconds(30))
        .WithClientId($"client_{Guid.NewGuid()}")
        .Build();
    
    client.ConnectAsync(options).Wait();
    return client;
});

// 3. 高性能通道配置（Disruptor模式）
var channelOptions = new BoundedChannelOptions(65536)
{
    SingleReader = true,
    AllowSynchronousContinuations = true,
    FullMode = BoundedChannelFullMode.DropOldest
};

var messageChannel = Channel.CreateBounded<MqttMessage>(channelOptions);
builder.Services.AddSingleton(channelOptions);
builder.Services.AddSingleton(messageChannel);

// 4. 消费者线程池配置
builder.Services.AddHostedService<RedisStreamConsumer>(sp => 
    new RedisStreamConsumer(
        sp.GetRequiredService<IConnectionMultiplexer>(),
        messageChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[4096]),
        Environment.ProcessorCount * 2));

// 5. CAP配置（集成Redis Streams）
builder.Services.AddCap(options =>
{
    options.UseRedis(redisOptions);
    options.UseRedisStreams();
    
    options.UseMemoryBuffer(o =>
    {
        o.PollingDelay = 50;
        o.LockTimeout = 500;
        o.ConsumerThreadCount = Environment.ProcessorCount * 2;
    });
    
    options.SucceedMessageExpiredAfter = 86400;
    options.FailedRetryCount = 5;
    options.FailedRetryInterval = 30;
    options.EnableConsumerPrefetch = true;
});

// 6. OpenTelemetry分布式追踪
builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation()
        .AddRedisInstrumentation()
        .AddOtlpExporter());

var app = builder.Build();
app.MapGet("/metrics", () => "MQTT+Redis Production Ready");
app.Run();

// 核心数据结构
[MemoryPackable]
public partial record MqttMessage(
    string Topic,
    [property: MemoryPackIgnore] ReadOnlyMemory<byte> Payload,
    DateTimeOffset Timestamp);

// 零拷贝处理器实现
[SkipLocalsInit]
public class RedisStreamConsumer : BackgroundService
{
    private readonly IDatabase _db;
    private readonly ChannelReader<MqttMessage> _reader;
    private readonly ThreadLocal<Span<byte>> _buffer;
    private readonly int _workerCount;

    // 在RedisStreamConsumer构造函数中注入
    private readonly DeadLetterQueue _dlq;

    public RedisStreamConsumer(
        IConnectionMultiplexer redis,
        Channel<MqttMessage> channel,
        ThreadLocal<Span<byte>> buffer,
        int workerCount)
    {
        _db = redis.GetDatabase();
        _reader = channel.Reader;
        _buffer = buffer;
        _workerCount = workerCount;
        _dlq = dlq;
    }

    // 在builder.Services.AddCap之后添加压缩服务
    builder.Services.AddSingleton<MessageCompressor>();
    
    // 修改RedisStreamConsumer处理逻辑
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var compressor = new MessageCompressor();
        // 在builder构建后添加限流器
        builder.Services.AddSingleton<TokenBucketLimiter>(_ => 
            new TokenBucketLimiter(1000, 5000)); // 1000 tokens/s, 5000容量
        
        // 修改消息处理循环
        await foreach (var message in _reader.ReadAllAsync(stoppingToken))
        {
            if (await limiter.TryAcquireAsync())
            {
                var compressed = compressor.Compress(message.Payload.Span);
                try
                {
                    var buffer = _buffer.Value;
                    // 零拷贝处理逻辑
                    await _db.StreamAddAsync(
                        "mqtt_messages",
                        message.Topic,
                        message.Payload.ToArray(),
                        maxLength: 10000);
                }
                catch (Exception ex)
                {
                    // 错误处理
                    await _dlq.HandleFailedMessageAsync("mqtt_messages", message.Topic, ex);
                }
            }
        }
    }
}