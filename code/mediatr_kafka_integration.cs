#:sdk Microsoft.NET.Sdk.Web
#:package Confluent.Kafka@2.3.0
#:package MediatR@12.1.1
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

public class KafkaEventBusOptions
{
    public string BootstrapServers { get; set; }
    public string Topic { get; set; }
    public string GroupId { get; set; }
}

public class KafkaEventBus<T> : IDistributedEventBus<T> where T : INotification
{
    private readonly IProducer<Null, byte[]> _producer;
    private readonly IConsumer<Null, byte[]> _consumer;
    private readonly Channel<T> _channel;
    private readonly IMediator _mediator;
    private readonly KafkaEventBusOptions _options;

    public KafkaEventBus(IMediator mediator, IOptions<KafkaEventBusOptions> options)
    {
        _mediator = mediator;
        _options = options.Value;
        
        var producerConfig = new ProducerConfig { BootstrapServers = _options.BootstrapServers };
        var consumerConfig = new ConsumerConfig 
        { 
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
        _producer = new ProducerBuilder<Null, byte[]>(producerConfig).Build();
        _consumer = new ConsumerBuilder<Null, byte[]>(consumerConfig).Build();
        _channel = Channel.CreateUnbounded<T>();
    }

    public async Task Publish(T notification, CancellationToken cancellationToken = default)
    {
        var message = new Message<Null, byte[]> 
        { 
            Value = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(notification) 
        };
        await _producer.ProduceAsync(_options.Topic, message, cancellationToken);
    }

    public async Task StartConsuming(CancellationToken cancellationToken = default)
    {
        _consumer.Subscribe(_options.Topic);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                var notification = System.Text.Json.JsonSerializer.Deserialize<T>(consumeResult.Message.Value);
                await _mediator.Publish(notification, cancellationToken);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
    }

    // 在KafkaEventBus中添加性能计数器
    private readonly Counter<int> _messagesProcessed;

    public KafkaEventBus(/*...*/, IMetrics metrics)
    {
        _messagesProcessed = metrics.CreateCounter<int>("kafka_messages_processed", "count", "Total processed messages");
    }

    // 在消息处理时记录指标
    _messagesProcessed.Increment();


    // 实现指数退避重试策略
    private async Task ProcessWithRetry(ConsumeResult<Null, byte[]> result, CancellationToken ct)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        await retryPolicy.ExecuteAsync(async () => {
            var notification = JsonSerializer.Deserialize<T>(result.Message.Value);
            await _mediator.Publish(notification, ct);
        });
    }

    // 在消息头中添加追踪ID
    var headers = new Headers {
        new Header("trace-id", Encoding.UTF8.GetBytes(Activity.Current?.TraceId.ToString() ?? ""))
    };

    // 消费时提取追踪上下文
    var traceId = result.Message.Headers?.FirstOrDefault(h => h.Key == "trace-id")?.Value;
    if (traceId != null)
    {
        Activity.Current?.SetParentId(Encoding.UTF8.GetString(traceId));
    }

    // 添加配置验证
    public class KafkaEventBusOptions : IValidatableObject
    {
        [Required] public string BootstrapServers { get; set; }
        [Required] public string Topic { get; set; }
        [Range(1, 100)] public int MaxRetryCount { get; set; } = 3;

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (!Topic.StartsWith("mediatr-"))
                yield return new ValidationResult("Topic name must start with 'mediatr-'");
        }
    }

    // 使用ObjectPool优化序列化器
    private static readonly ObjectPool<JsonSerializerOptions> _serializerPool = 
        new DefaultObjectPool<JsonSerializerOptions>(new JsonSerializerOptionsPooledPolicy());
        
    // 添加消息压缩支持
    private static readonly ObjectPool<BrotliEncoder> _compressorPool = 
        new DefaultObjectPool<BrotliEncoder>(new BrotliEncoderPooledPolicy());
        
    // 批量处理缓冲区
    private readonly Channel<Message<Null, byte[]>> _batchChannel = Channel.CreateBounded<Message<Null, byte[]>>(
        new BoundedChannelOptions(1000) { SingleWriter = true, FullMode = BoundedChannelFullMode.Wait });

    // 在序列化/反序列化时借用对象
    var options = _serializerPool.Get();
    try {
        JsonSerializer.Serialize(value, options);
    } finally {
        _serializerPool.Return(options);
    }
    
    // 批量发布方法
    public async Task PublishBatch(IEnumerable<T> notifications, CancellationToken ct = default)
    {
        var batch = notifications.Select(n => new Message<Null, byte[]> 
        { 
            Value = Compress(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(n))
        });
        
        await _producer.ProduceAsync(_options.Topic, batch, ct);
    }
    
    // 消息压缩方法
    private byte[] Compress(byte[] data)
    {
        using var output = new MemoryStream();
        using var compressor = _compressorPool.Get();
        using var compressionStream = new BrotliStream(output, compressor, leaveOpen: true);
        compressionStream.Write(data);
        compressionStream.Flush();
        return output.ToArray();
    }
    
    // 死信队列处理
    private readonly Channel<Message<Null, byte[]>> _deadLetterChannel = Channel.CreateUnbounded<Message<Null, byte[]>>();
    
    private async Task ProcessDeadLetters(CancellationToken ct)
    {
        await foreach (var msg in _deadLetterChannel.Reader.ReadAllAsync(ct))
        {
            await _producer.ProduceAsync($"{_options.Topic}-deadletter", msg, ct);
        }
    }

    // 实现健康检查
    public class KafkaHealthCheck : IHealthCheck
    {
        private readonly IProducer<Null, byte[]> _producer;
        
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken ct = default)
        {
            try {
                await _producer.ProduceAsync("health-check", null, ct);
                return HealthCheckResult.Healthy();
            } catch {
                return HealthCheckResult.Unhealthy();
            }
        }
    }

    // 使用结构化日志记录关键事件
    _logger.LogInformation("Processed Kafka message {Topic}/{Partition}/{Offset}", 
    result.Topic, result.Partition, result.Offset);
}

public static class KafkaEventBusExtensions
{
    public static IServiceCollection AddKafkaEventBus<T>(this IServiceCollection services, Action<KafkaEventBusOptions> configure) 
        where T : INotification
    {
        services.Configure(configure);
        services.AddSingleton<IDistributedEventBus<T>, KafkaEventBus<T>>();
        services.AddHostedService<KafkaEventBusBackgroundService<T>>();
        
        // 注册死信队列处理器
        services.AddHostedService<DeadLetterProcessor>();
        
        // 注册批量处理器
        services.AddHostedService<BatchProcessor>();
        
        // 注册健康检查
        services.AddHealthChecks()
            .AddCheck<KafkaHealthCheck>("kafka");
            
        return services;
    }
}

public class KafkaEventBusBackgroundService<T> : BackgroundService where T : INotification
{
    private readonly IDistributedEventBus<T> _eventBus;

    public KafkaEventBusBackgroundService(IDistributedEventBus<T> eventBus)
    {
        _eventBus = eventBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _eventBus.StartConsuming(stoppingToken);
    }
}

// 使用事列
services.AddKafkaEventBus<MyNotification>(options => {
    options.BootstrapServers = "localhost:9092";
    options.Topic = "my-topic";
    options.GroupId = "my-group";
});