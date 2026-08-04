using Disruptor;
using Disruptor.Dsl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using MqttAdvancedIntegration;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.EventBus;
using MQTTnet.Protocol;
using OpenTelemetry.Metrics;
using System.ApplicationModel;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO.Pipelines;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Channels;

// 高级内存管理配置
[assembly: TieredMemoryConfiguration(EnableTieredMemory = true, HotPoolSizeMB = 100, WarmPoolSizeMB = 500, ColdPoolSizeMB = 1000)]

namespace MqttAdvancedIntegration;

#region 配置项
public class MqttConnectionOptions
{
    public string Host { get; set; } = "172.100.61.43";
    public int Port { get; set; } = 1883;
    public string ClientId { get; set; } = "mqtt_service";
    public string Username { get; set; } = "emqx_test";
    public string Password { get; set; } = "anb*&nnm2266";
    public int ConnectionPoolSize { get; set; } = 50;
    public int MaxRetryAttempts { get; set; } = 5;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);
}

public class TransformOptions
{
    public int RingBufferSize { get; set; } = 32 * 1024;
    public int ParallelismDegree { get; set; } = Environment.ProcessorCount;
    public int BatchSize { get; set; } = 100;
    public TimeSpan FlushInterval { get; set; } = TimeSpan.FromMilliseconds(500);
    public long MaxQueueSize { get; set; } = 100_000;
}
#endregion

#region 持久化模型
[Index(nameof(Topic))]
[Index(nameof(Timestamp))]
public class PersistedMessage
{
    public long Id { get; set; }
    public required string Topic { get; set; }
    public required byte[] Payload { get; set; }
    public MqttQualityOfServiceLevel QosLevel { get; set; }
    public bool Retain { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string? DeadReason { get; set; }
    public string? CorrelationId { get; set; }
}

public class MqttMessageDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<PersistedMessage> Messages { get; set; }

    public MqttMessageDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 使用配置从IConfiguration注入
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = _configuration.GetConnectionString("MqttMessageDB");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
#endregion

#region DDD领域模型
public class Device
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; }
    public DeviceStatus Status { get; set; }
    public DateTime LastSeen { get; set; }
}

public enum DeviceStatus { Online, Offline, Degraded, Unauthorized }

public class DeviceEvent : IEvent
{
    public string DeviceId { get; set; }
    public DeviceStatus NewStatus { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? Metadata { get; set; }
}
#endregion

#region CQRS实现
// 命令
public class UpdateDeviceStatusCommand : ICommand
{
    public string DeviceId { get; set; }
    public DeviceStatus Status { get; set; }
}

// 查询
public class GetDeviceStatusQuery : IQuery<DeviceStatus>
{
    public string DeviceId { get; set; }
}

// DTO
public record DeviceStatusDto(string DeviceId, DeviceStatus Status, DateTime LastUpdated);

// 命令处理器
[AotCompatible]
public class DeviceCommandHandler : ICommandHandler<UpdateDeviceStatusCommand>
{
    // 实现处理逻辑
    private readonly IDeviceRepository _repository;
    private readonly ITieredMemoryManager _memoryManager;
    private readonly ILogger<DeviceCommandHandler> _logger;

    public DeviceCommandHandler(
        IDeviceRepository repository,
        ITieredMemoryManager memoryManager,
        ILogger<DeviceCommandHandler> logger)
    {
        _repository = repository;
        _memoryManager = memoryManager;
        _logger = logger;
    }

    public async Task Handle(UpdateDeviceStatusCommand command, CancellationToken cancellationToken)
    {
        using var lease = _memoryManager.RentHotMemory(128);
        Span<byte> buffer = lease.Memory.Span;

        // 使用AOT友好的序列化
        var written = JsonSerializer.Serialize(command, typeof(UpdateDeviceStatusCommand),
            MqttAotJsonContext.Default, buffer);

        _logger.LogProcessedCommand(command);

        var device = await _deviceRepository.GetAsync(command.DeviceId);
        if (device == null)
        {
            _logger.LogWarning("未找到设备: {DeviceId}", command.DeviceId);
            return;
        }

        device.Status = command.Status;
        device.LastSeen = DateTime.UtcNow;

        await _deviceRepository.UpdateAsync(device);

        // 使用线程安全更新
        //return _deviceRepository.UpdateStatus(
        //    command.DeviceId,
        //    command.Status,
        //    new ReadOnlyMemory<byte>(buffer.Slice(0, written)));

        _logger.LogInformation("设备 {DeviceId} 状态更新为 {Status}",
            command.DeviceId, command.Status);
    }
}

// 查询处理器
public class DeviceQueryHandler : IQueryHandler<GetDeviceStatusQuery, DeviceStatus>
{
    // 实现处理逻辑
    private readonly IDeviceRepository _deviceRepository;

    public DeviceQueryHandler(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<DeviceStatus?> Handle(GetDeviceStatusQuery query, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetAsync(query.DeviceId);
        return device?.Status;
    }
}
#endregion

#region 内存管理技术组件
[TieredMemory(RetentionTime = 20, Tier = MemoryTier.Hot)]
public struct HotMessage
{
    public long Sequence { get; set; }
    public Timestamp Timestamp { get; set; }
    public SpanManager<byte> Payload { get; set; }
    public MqttApplicationMessage Original { get; set; }
}

[TieredMemory(RetentionTime = 300, Tier = MemoryTier.Warm)]
public class WarmMessageCache
{
    public Dictionary<long, PersistedMessage> Messages { get; } = new();
}

[TieredMemory(RetentionTime = 3600, Tier = MemoryTier.Cold)]
public class ColdStorageProxy
{
    // 冷存储访问逻辑
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ObjectPool<MemoryStream> _memoryStreamPool;

    public ColdStorageProxy(
        BlobServiceClient blobServiceClient,
        ObjectPoolProvider poolProvider)
    {
        _blobServiceClient = blobServiceClient;
        _memoryStreamPool = poolProvider.Create(new MemoryStreamPoolPolicy());
    }

    public async Task UploadMessageAsync(PersistedMessage message)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("mqtt-messages");
        await containerClient.CreateIfNotExistsAsync();

        var blobName = $"{message.Timestamp:yyyyMMddHHmmss}-{message.Id}";
        var stream = _memoryStreamPool.Get();

        try
        {
            await JsonSerializer.SerializeAsync(stream, message);
            stream.Position = 0;
            await containerClient.UploadBlobAsync(blobName, stream);
        }
        finally
        {
            stream.SetLength(0); // 清空流
            _memoryStreamPool.Return(stream);
        }
    }
}

// 冷存储集成实现
public class MqttArchiveService : BackgroundService
{
    private readonly Channel<PersistedMessage> _archiveChannel;
    private readonly IColdStorageProvider _coldStorage;
    private readonly ObjectPool<PersistedMessage> _messagePool;
    private readonly IAotCompressor _compressor;

    public MqttArchiveService(
        IColdStorageProvider coldStorage,
        ObjectPoolProvider poolProvider,
        IAotCompressor compressor)
    {
        _coldStorage = coldStorage;
        _compressor = compressor;
        _archiveChannel = Channel.CreateBounded<PersistedMessage>(new BoundedChannelOptions(10_000)
        {
            SingleWriter = true,
            SingleReader = true,
            // 环形缓冲策略
            FullMode = BoundedChannelFullMode.Wait
        });

        _messagePool = poolProvider.Create(new MessageObjectPolicy());
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var message in _archiveChannel.Reader.ReadAllAsync(ct))
        {
            using var compressed = await _compressor.CompressAsync(message.Payload);
            await _coldStorage.ArchiveMessageAsync(
                message.Topic,
                compressed.Memory,
                message.Timestamp);

            // 返还对象池
            _messagePool.Return(message);
        }
    }

    public ValueTask QueueForArchive(PersistedMessage message)
    {
        return _archiveChannel.Writer.WriteAsync(message);
    }
}


public class MemoryStreamPoolPolicy : PooledObjectPolicy<MemoryStream>
{
    public override MemoryStream Create() => new MemoryStream(8192);
    public override bool Return(MemoryStream obj) => true;
}

public sealed class PoolingManager : MemoryPool<byte>
{
    // 高级内存池实现
    private const int MaxBufferSize = 1024 * 1024; // 最大1MB
    private readonly ObjectPool<byte[]> _smallBuffers;   // 小对象池 (4KB)
    private readonly ObjectPool<byte[]> _mediumBuffers;  // 中对象池 (16KB)
    private readonly ObjectPool<byte[]> _largeBuffers;   // 大对象池 (64KB)
    private readonly ConcurrentQueue<byte[]> _oversizedBuffers = new();

    public override int MaxBufferSize => MaxBufferSize;

    public AdvancedMemoryPool()
    {
        _smallBuffers = CreateFixedSizeBufferPool(4096, 1000);
        _mediumBuffers = CreateFixedSizeBufferPool(16384, 500);
        _largeBuffers = CreateFixedSizeBufferPool(65536, 100);
    }

    private ArrayMemoryPool CreateFixedSizeBufferPool(int size, int maxRetained)
    {
        return new ArrayMemoryPool(new ArrayPoolBucketConfiguration(size), maxRetained);
    }

    public override IMemoryOwner<byte> Rent(int size)
    {
        if (size > MaxBufferSize)
            throw new ArgumentOutOfRangeException(nameof(size));

        byte[]? buffer = null;

        if (size <= 4096)
            buffer = _smallBuffers.Get();
        else if (size <= 16384)
            buffer = _mediumBuffers.Get();
        else if (size <= 65536)
            buffer = _largeBuffers.Get();
        else
        {
            buffer = ArrayPool<byte>.Shared.Rent(size);
            _oversizedBuffers.Enqueue(buffer);
        }

        return new AdvancedMemoryOwner(this, buffer);
    }

    private void ReturnBuffer(byte[] buffer)
    {
        int size = buffer.Length;

        if (size <= 4096)
        {
            Array.Clear(buffer, 0, size);
            _smallBuffers.Return(buffer);
        }
        else if (size <= 16384)
        {
            Array.Clear(buffer, 0, size);
            _mediumBuffers.Return(buffer);
        }
        else if (size <= 65536)
        {
            Array.Clear(buffer, 0, size);
            _largeBuffers.Return(buffer);
        }
        else
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private sealed class AdvancedMemoryOwner : IMemoryOwner<byte>
    {
        private readonly AdvancedMemoryPool _pool;
        private byte[]? _buffer;

        public AdvancedMemoryOwner(AdvancedMemoryPool pool, byte[] buffer)
        {
            _pool = pool;
            _buffer = buffer;
            Memory = new Memory<byte>(buffer);
        }

        public Memory<byte> Memory { get; private set; }

        public void Dispose()
        {
            if (_buffer is null) return;

            var buffer = _buffer;
            Memory = default;
            _buffer = null;

            _pool.ReturnBuffer(buffer);
        }
    }
}

[StackAlloc(maxSize: 1024)]
public ref struct MqttBufferWriter
{
    private Span<byte> _buffer;
    private int _written;

    public MqttBufferWriter(Span<byte> buffer)
    {
        _buffer = buffer;
        _written = 0;
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        if (_written + data.Length > _buffer.Length)
            throw new InvalidOperationException("Buffer overflow");

        data.CopyTo(_buffer.Slice(_written));
        _written += data.Length;
    }

    public Span<byte> WrittenSpan => _buffer.Slice(0, _written);
}
#endregion

#region 状态机实现
public class DeviceConnectionStateMachine
{
    private State _currentState = State.Disconnected;
    private readonly IMeter _meter;

    private enum State { Disconnected, Connecting, Connected, Degraded, Disconnecting }

    public DeviceConnectionStateMachine(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create("Device.StateMachine");
    }

    public void Transition(DeviceEvent @event)
    {
        switch (_currentState)
        {
            case State.Disconnected:
                if (@event.NewStatus == DeviceStatus.Online)
                {
                    _currentState = State.Connecting;
                    LogTransition(State.Connecting, @event);
                }
                break;
            case State.Connecting:
                if (@event.NewStatus == DeviceStatus.Online)
                {
                    _currentState = State.Connected;
                    LogTransition(State.Connected, @event);
                }
                else if (@event.NewStatus == DeviceStatus.Unauthorized)
                {
                    _currentState = State.Disconnected;
                    LogTransition(State.Disconnected, @event);
                }
                break;
            case State.Connected:
                if (@event.NewStatus == DeviceStatus.Degraded)
                {
                    _currentState = State.Degraded;
                    LogTransition(State.Degraded, @event);
                }
                else if (@event.NewStatus == DeviceStatus.Offline)
                {
                    _currentState = State.Disconnecting;
                    LogTransition(State.Disconnecting, @event);
                }
                break;
            case State.Degraded:
                if (@event.NewStatus == DeviceStatus.Online)
                {
                    _currentState = State.Connected;
                    LogTransition(State.Connected, @event);
                }
                else if (@event.NewStatus == DeviceStatus.Offline)
                {
                    _currentState = State.Disconnecting;
                    LogTransition(State.Disconnecting, @event);
                }
                break;
            case State.Disconnecting:
                if (@event.NewStatus == DeviceStatus.Offline)
                {
                    _currentState = State.Disconnected;
                    LogTransition(State.Disconnected, @event);
                    NotifyDeviceDisconnected(@event.DeviceId);
                }
                break;
        }
    }

    private void LogTransition(State newState, DeviceEvent @event)
    {
        _logger.LogTransition(
            deviceId: @event.DeviceId,
            fromState: _currentState,
            toState: newState,
            eventType: @event.GetType().Name);
    }
}
// 状态机集成
public class DeviceIntegrationService : BackgroundService
{
    private readonly DeviceConnectionStateMachine _stateMachine;
    private readonly Channel<MqttApplicationMessage> _messageChannel;
    private readonly IDeviceRepository _repository;
    private readonly IHubContext<DeviceHub> _hub;

    public DeviceIntegrationService(
        DeviceConnectionStateMachine stateMachine,
        IDeviceRepository repository,
        IHubContext<DeviceHub> hub)
    {
        _stateMachine = stateMachine;
        _repository = repository;
        _hub = hub;
        _messageChannel = Channel.CreateUnbounded<MqttApplicationMessage>(
            new UnboundedChannelOptions { SingleWriter = true });
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var msg in _messageChannel.Reader.ReadAllAsync(ct))
        {
            using var scope = ServiceActivator.GetScope();
            var message = JsonSerializer.Deserialize(
                msg.Payload,
                typeof(DeviceEvent),
                DeviceContext.Default) as DeviceEvent;

            if (message != null)
            {
                // 更新状态机
                var device = await _repository.GetAsync(message.DeviceId);
                _stateMachine.Transition(message, device);

                // 更新数据库
                await _repository.UpdateWithStateMachineAsync(
                    device,
                    _stateMachine.CurrentState);

                // 推送实时通知
                await _hub.Clients.All.SendAsync("DeviceUpdated",
                    new DeviceStatusDto(device.Id, device.Status));
            }
        }
    }

    public ValueTask QueueDeviceMessage(MqttApplicationMessage message)
    {
        return _messageChannel.Writer.WriteAsync(message);
    }
}

#endregion

#region 高性能消息处理
[SkipLocalsInit, MethodImpl(MethodImplOptions.AggressiveOptimization)]
public class MessageTransformBlock : IMessageProcessor
{
    private readonly RingBuffer<DeviceEvent> _ringBuffer;
    private readonly SequenceBarrier _sequenceBarrier;
    private readonly IEventProcessor _eventProcessor;
    private readonly ObjectPool<MqttApplicationMessage> _messagePool;
    private readonly IOptions<TransformOptions> _options;
    private readonly Meter _meter;
    private Histogram<double> _transformDuration;
    private readonly ConcurrentDictionary<int, ThreadLocalMemory> _threadLocalBuffers;
    private readonly Stopwatch _batchSw = Stopwatch.StartNew();
    private int _batchCount;

    public MessageTransformBlock(
        ObjectPool<MqttApplicationMessage> messagePool,
        IOptions<TransformOptions> options,
        IMeterFactory meterFactory)
    {
        // 初始化Disruptor
        var disruptor = new Disruptor.Dsl.Disruptor<DeviceEvent>(
            () => new DeviceEvent(),
            options.Value.RingBufferSize,
            TaskScheduler.Default,
            ProducerType.Multi,
            new BlockingWaitStrategy());

        disruptor
            .HandleEventsWith(new[] { new EventProcessor(this) })
            .Then(new CleanupHandler());

        _ringBuffer = disruptor.Start();
        _sequenceBarrier = _ringBuffer.NewBarrier();
        _eventProcessor = new BatchEventProcessor(
            _ringBuffer,
            _sequenceBarrier,
            new EventHandler());

        // 指标初始化
        _meter = meterFactory.Create("MessageTransform");
        _transformDuration = _meter.CreateHistogram<double>("transform_duration_ms", "ms");
    }

    public void Process(MqttApplicationMessage message)
    {
        using var handle = new ProcessorHandle(RecordProcessingStart);
        try
        {
            long sequence = _ringBuffer.Next();
            var deviceEvent = _ringBuffer[sequence];
            // 复制数据到ring buffer条目
            _ringBuffer.Publish(sequence);
        }
        catch (Exception ex)
        {
            handle.OnException(ex);
            throw;
        }
    }

    private class EventProcessor : IEventHandler<DeviceEvent>
    {
        private readonly MessageTransformBlock _parent;

        public EventProcessor(MessageTransformBlock parent) => _parent = parent;

        public void OnEvent(DeviceEvent data, long sequence, bool endOfBatch)
        {
            using var eventHandle = new EventHandle(_parent);
            // 处理业务逻辑
            if (endOfBatch || _parent._batchSw.Elapsed >= _parent._options.Value.FlushInterval)
            {
                _parent._batchCount++;
                CompleteBatch();
            }
        }

        private void CompleteBatch()
        {
            _parent._batchSw.Restart();
        }
    }

    private class CleanupHandler : IEventHandler<DeviceEvent>
    {
        private readonly ObjectPool<DeviceEvent> _eventPool;

        public CleanupHandler(ObjectPool<DeviceEvent> eventPool)
        {
            _eventPool = eventPool;
        }

        // 清理资源
        public void OnEvent(DeviceEvent data, long sequence, bool endOfBatch)
        {
            // 设备事件重置
            data.DeviceId = null;
            data.NewStatus = default;
            data.Metadata?.Clear();
            data.Timestamp = default;

            // 返回对象池
            _eventPool.Return(data);
        }
    }
}
#endregion

#region 核心服务实现
public sealed class MqttNetworkServer : BackgroundService
{
    private const string MetricsEndpoint = "/metrics";
    private const string HealthEndpoint = "/health";
    private const int StreamBufferSize = 4096;

    private readonly IMqttEventBus _eventBus;
    private readonly MqttConnectionPool _connectionPool;
    private readonly MqttRecoveryService _recoveryService;
    private readonly IOptions<MqttConnectionOptions> _options;
    private readonly ITopicRegistry _topicRegistry;
    private readonly MessageTransformBlock _transformBlock;
    private readonly MqttPerformanceMonitor _monitor;
    private readonly WebSocketNotificationService _notificationService;
    private readonly IMemoryCache _cache;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MqttNetworkServer> _logger;
    private readonly MqttTlsCertificateManager _certManager;
    private readonly MqttClusterManager _clusterManager;
    private readonly IDeadLetterQueue _deadLetterQueue;
    private readonly Channel<MqttApplicationMessage> _pendingRetryQueue;
    private IMqttServer? _mqttServer;
    private Timer? _stateAggregator;
    private bool _isLeaderNode;
    private CancellationTokenSource _cts = new CancellationTokenSource();

    // 注入所有依赖
    public MqttNetworkServer(
        IMqttEventBus eventBus,
        MqttConnectionPool connectionPool,
        MqttRecoveryService recoveryService,
        IOptions<MqttConnectionOptions> options,
        ITopicRegistry topicRegistry,
        MessageTransformBlock transformBlock,
        MqttPerformanceMonitor monitor,
        WebSocketNotificationService notificationService,
        IMemoryCache cache,
        IServiceProvider serviceProvider,
        ILogger<MqttNetworkServer> logger,
        MqttTlsCertificateManager certManager,
        MqttClusterManager clusterManager,
        IDeadLetterQueue deadLetterQueue
        )
    {
        // 初始化字段
        _eventBus = eventBus;
        _connectionPool = connectionPool;
        _recoveryService = recoveryService;
        _options = options;
        _topicRegistry = topicRegistry;
        _transformBlock = transformBlock;
        _monitor = monitor;
        _notificationService = notificationService;
        _cache = cache;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _certManager = certManager;
        _clusterManager = clusterManager;
        _deadLetterQueue = deadLetterQueue;
        _pendingRetryQueue = Channel.CreateUnbounded<MqttApplicationMessage>(new UnboundedChannelOptions
        {
            SingleWriter = false,
            SingleReader = true
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeClusterRole();
        _stateAggregator = new Timer(AggregateClusterState, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

        var optionsBuilder = new MqttServerOptionsBuilder()
            .WithConnectionBacklog(1000)
            .WithDefaultEndpointPort(_options.Value.Port)
            .WithDefaultEndpoint()
            .ApplicationMessageInterceptor = OnApplicationMessageReceived;

        // 配置WebSocket
        optionsBuilder.WithWebSocketEndpoint("/mqtt")
            .WithWebSocketKeepAliveInterval(TimeSpan.FromSeconds(30));

        // TLS配置
        optionsBuilder.WithEncryptedEndpoint()
            .WithEncryptedEndpointPort(8883)
            .WithEncryptionCertificate(_certManager.GetCertificate().Export(X509ContentType.Pfx));

        _mqttServer = new MqttFactory().CreateMqttServer();

        // 注册事件处理
        _mqttServer.ValidatingConnectionAsync += OnValidateConnection;
        _mqttServer.ClientDisconnectedAsync += OnClientDisconnected;
        _mqttServer.ClientSubscribedTopicAsync += OnTopicSubscribed;

        await _mqttServer.StartAsync(optionsBuilder.Build());
        await _recoveryService.RecoverMessagesAsync();
        await SubscribeSystemTopics();
    }

    private async Task SubscribeSystemTopics()
    {
        await _eventBus.SubscribeAsync("$SYS/#", new EventHandlerContext
        {
            QoSLevel = MqttQualityOfServiceLevel.AtLeastOnce,
            Handler = HandleSystemEvent
        });
    }

    private Task HandleSystemEvent(MqttApplicationMessage message)
    {
        var topic = message.Topic;
        var payload = Encoding.UTF8.GetString(message.Payload);

        if (topic.StartsWith("$SYS/cluster/"))
            return _clusterManager.HandleClusterEvent(payload);

        if (topic.StartsWith("$SYS/health/"))
            UpdateNodeHealth(payload);

        return Task.CompletedTask;
    }

    // 实现扇入扇出
    [FanIn(SourceTopics = new[] { "sensor/temperature", "sensor/humidity" })]
    [FanOut(DestinationTopics = new[] { "dashboard/sensors", "api/sensors" })]
    public Task ProcessSensorData(MqttApplicationMessage message)
    {
        // 处理并转发消息
        return Task.CompletedTask;
    }

    // 死信队列处理器
    [DeadLetterHandler(RetryCount = 3, DeadLetterTopic = "deadletters/sensors")]
    public Task HandleSensorData(SensorData data)
    {
        try
        {
            // 处理数据
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return new DeadLetterProcessResult(false, ex.Message);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // 清理逻辑
        _cts.Cancel();

        if (_mqttServer != null)
        {
            await _mqttServer.StopAsync();
            _mqttServer.ClientDisconnectedAsync -= OnClientDisconnected;
            _mqttServer = null;
        }

        _stateAggregator?.Dispose();
        _stateAggregator = null;

        await _clusterManager.LeaveClusterAsync();

        // 优雅关闭处理中的消息
        await _transformBlock.CompleteAsync();
        await _recoveryService.SavePendingMessages();
        await _deadLetterQueue.FlushAsync();

        // 关闭连接池
        await _connectionPool.DisposeAsync();

        _logger.LogInformation("MQTT 服务已停止");
        await base.StopAsync(cancellationToken);
    }
}
#endregion

#region WebSocket集成
public class WebSocketNotificationService : IWebSocketHandler
{
    private const int MaxConnections = 10_000;
    private readonly WebSocketConnectionManager _connectionManager;
    private readonly ObjectPool<Message> _messagePool;
    private readonly ILogger<WebSocketNotificationService> _logger;
    private readonly Meter _meter;
    private readonly Counter<int> _connectionsCounter;
    private readonly PipeScheduler _scheduler = PipeScheduler.ThreadPool;
    private readonly SemaphoreSlim _connectionSemaphore = new(MaxConnections);
    private readonly SpanMemoryManager<byte> _memoryManager = new(4096);

    public WebSocketNotificationService(
        WebSocketConnectionManager connectionManager,
        ObjectPool<Message> messagePool,
        ILogger<WebSocketNotificationService> logger,
        IMeterFactory meterFactory)
    {
        // 初始化字段
        _meter = meterFactory.Create("WebSocket");
        _connectionsCounter = _meter.CreateCounter<int>("connections");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task HandleWebSocketAsync(WebSocket socket, string clientId)
    {
        if (!_connectionSemaphore.Wait(0))
        {
            // 拒绝连接
            return;
        }

        try (var lease = _memoryManager.Allocate())
        {
            var buffer = lease.Memory;
            _connectionManager.AddConnection(clientId, socket);
            _connectionsCounter.Add(1);

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, default);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, default);
                    break;
                }

                // 处理消息
                await ProcessWebSocketMessage(buffer.Slice(0, result.Count));
            }
        }
        finally
        {
            _connectionManager.RemoveConnection(clientId);
            _connectionSemaphore.Release();
            _connectionsCounter.Add(-1);
        }
    }

    [TailLatencyOptimization]
    private ValueTask ProcessWebSocketMessage(Memory<byte> message)
    {
        var asyncTask = InternalProcess(message);
        if (asyncTask.IsCompletedSuccessfully)
            return default;

        return new ValueTask(TailLatencyOptimizer.OptimizeAsync(asyncTask));
    }

    private async Task InternalProcess(Memory<byte> message)
    {
        // 消息处理逻辑
        MessageHandlerRouter(() => { Console.Write("Process"); })
    }
}

// 12. 消息处理逻辑（使用Action委托）
// ======================================================
public class MessageHandlerRouter
{
    private readonly Dictionary<string, Action<MqttApplicationMessage>> _handlers = new();
    private readonly Action<MqttApplicationMessage> _deadLetterHandler;

    public MessageHandlerRouter(Action<MqttApplicationMessage> deadLetterHandler)
    {
        _deadLetterHandler = deadLetterHandler
            ?? throw new ArgumentNullException(nameof(deadLetterHandler));
    }

    public void RegisterHandler(string topicPattern, Action<MqttApplicationMessage> handler)
    {
        _handlers[topicPattern] = handler;
    }

    public void RouteMessage(MqttApplicationMessage message)
    {
        bool handled = false;
        foreach (var (pattern, handler) in _handlers)
        {
            if (MqttTopicFilterComparer.IsMatch(message.Topic, pattern))
            {
                try
                {
                    handler(message);
                    handled = true;
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "消息处理失败 Topic: {Topic}", message.Topic);
                }
            }
        }

        if (!handled)
        {
            _deadLetterHandler(message);
        }
    }
}
#endregion

#region 认证和授权
public class MqttAuthenticationService : IMqttAuthenticationHandler
{
    private readonly IAuthenticationCacheManager _cache;
    private readonly JwtValidator _jwtValidator;

    public MqttAuthenticationService(
        IAuthenticationCacheManager cache,
        JwtValidator jwtValidator)
    {
        _cache = cache;
        _jwtValidator = jwtValidator;
    }

    public ValueTask<bool> ValidateConnectionAsync(ValidatingConnectionContext context)
    {
        if (!context.IsValidClientId(out var clientId))
            return new(false);

        // 使用缓存加速认证
        if (_cache.TryGetAuthStatus(clientId, out var authStatus))
            return new(authStatus.IsAuthenticated);

        var token = context.GetBearerToken();
        if (string.IsNullOrEmpty(token))
            return new(false);

        var validationResult = _jwtValidator.ValidateToken(token);
        _cache.CacheAuthStatus(clientId, validationResult);

        return new(validationResult.IsAuthenticated);
    }
}
#endregion

#region 高级诊断
[DiagnosticSource]
public class MqttDiagnosticSource
{
    public static MeterListener? Listener { get; private set; }

    static MqttDiagnosticSource()
    {
        Listener = new MeterListener
        {
            InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name == "MQTT")
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            }
        };

        Listener.SetMeasurementEventCallback<long>((inst, value, tags, state) =>
            LogMetrics(inst.Name, value, inst.Unit));

        Listener.Start();
    }

    private static void LogMetrics(string name, long value, string? unit)
    {
        // 实现具体日志逻辑
        using var scope = _logger.BeginScope("Metrics");

        switch (name)
        {
            case "mqtt.messages.published":
                _logger.Information("已发布消息: {Count}", value);
                break;

            case "mqtt.publish.latency_ms":
                if (value > 100)
                    _logger.Warning("高延迟警告: {Latency}ms", value);
                else
                    _logger.Debug("发布延迟: {Latency}ms", value);
                break;

            case "mqtt.connections.active":
                if (value > 10000)
                    _logger.Warning("连接数过高: {Count}", value);
                else
                    _logger.Information("活动连接: {Count}", value);
                break;

            case "mqtt.queue.backpressure":
                if (value > 90)
                    _logger.Error("队列压力过高: {Percentage}%", value);
                break;

            default:
                _logger.Debug("{Metric}: {Value}{Unit}", name, value, unit ?? "");
                break;
        }

        // 推送到监控系统
        if (_monitorAgent != null)
        {
            string fullName = unit != null ? $"{name}_{unit}" : name;
            _monitorAgent.RecordCustomMetric(fullName, value);
        }
    }
}

// 与现有监控系统集成
public class TelemetryBridgeService
{
    private readonly IMeterService _existingMeter;
    private readonly IHistogram _messageLatencyHistogram;
    private readonly ICounter _messageCounter;

    public TelemetryBridgeService(
        IMeterService legacyMeter,
        IConfiguration config)
    {
        _existingMeter = legacyMeter;

        // 映射OpenTelemetry指标到已有系统
        MeterListener listener = new();
        listener.InstrumentPublished = (instrument, meter) =>
        {
            if (instrument.Name == "mqtt.messages.received")
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };

        listener.SetMeasurementEventCallback<double>((inst, value, tags, state) =>
        {
            if (inst.Name == "mqtt.message.latency")
                _existingMeter.RecordLatency("mqtt", value);
        });

        listener.Start();
    }

    // 同时收集传统指标和现代指标
    public void RecordMessageProcessing(
        string topic,
        int size,
        double latency)
    {
        // OpenTelemetry指标
        _messageCounter.Add(1,
            new("topic", topic),
            new("size", size));

        _messageLatencyHistogram.Record(latency);

        // 传统监控系统指标
        _existingMeter.IncrementCounter("mqtt_msg");
        _existingMeter.RecordLatency("mqtt", latency);
    }
}


#endregion

public static class ModuleExtensions
{
    public static IServiceCollection AddAdvancedMqttModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置依赖注入
        services.AddOptions<MqttConnectionOptions>()
            .Bind(configuration.GetSection("Mqtt:Connection"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<TransformOptions>()
            .Bind(configuration.GetSection("Mqtt:Transformation"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<MqttRecoveryService>();
        services.AddSingleton<MqttConnectionPool>();
        services.AddSingleton<MessageTransformBlock>();
        services.AddSingleton<WebSocketNotificationService>();
        services.AddSingleton<MqttPerformanceMonitor>();
        services.AddSingleton<MqttClusterManager>();

        // 对象池配置
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton(s =>
            s.GetService<ObjectPoolProvider>().Create(new MessageObjectPolicy()));

        // DDD/事件溯源基础设施
        services.AddEventSourcing<MqttEventStore>();
        services.AddCqrsBus();
        services.AddDomainEventsHandlers(Assembly.GetExecutingAssembly());

        // 缓存
        services.AddStackExchangeRedisCache(opts =>
            opts.Configuration = configuration.GetConnectionString("Redis"));

        // AOT编译支持
        if (RuntimeFeature.IsNativeAot)
            services.AddSingleton<IMessageSerializer, NativeAotMessageSerializer>();
        else
            services.AddSingleton<IMessageSerializer, MessageSerializer>();

        // OpenTelemetry
        services.AddOpenTelemetry()
            .WithMetrics(builder => builder
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddPrometheusExporter());

        return services;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseKestrel(options =>
                {
                    var mqttOptions = options.ApplicationServices
                        .GetService<IOptions<MqttConnectionOptions>>()?.Value;

                    options.ListenAnyIP(1883, listenOptions =>
                        listenOptions.UseMqtt());

                    options.ListenAnyIP(8881, listenOptions =>
                        listenOptions.UseHttps());

                    options.ListenAnyIP(8080, listenOptions =>
                        listenOptions.UseWebSockets());
                });
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddAdvancedMqttModule(hostContext.Configuration);
                services.AddHostedService<MqttNetworkServer>();
            })
            .ConfigureLogging(builder =>
                builder.AddOpenTelemetry(options =>
                    options.AddConsoleExporter()));
}


public static class MqttAotIntegrationExtensions
{
    public static IServiceCollection AddAotCompatibleMqttService(
        this IServiceCollection services,
        Action<MqttOptions> configure)
    {
        // AOT兼容的SourceGenerated模式（.NET 8新特性）
        MqttOptions options = new();
        configure?.Invoke(options);

        // 核心服务
        services.AddSingleton(options);
        services.AddSingleton<IMqttEventBus, AotCompatibleMqttEventBus>();

        // 事件注册（AOT安全方式）
        services.AddAotCompatibleEventHandlers();

        // 添加AOT兼容Verb
        AddAotVerbs();

        return services;
    }

    [UnconditionalConditional("ENABLE_AOT_COMPILATION")]
    private static void AddAotVerbs()
    {
        // AOT编译时特殊处理
        RttiHelpers.RegisterAllEventHandlers();
        JsonSerializerContext.RegisterContext<MqttAotJsonContext>();
    }
}

// AOT兼容的命令处理器注册
public static class ServiceRegistration
{
    // AOT兼容的方式注册CQRS处理器
    [ModuleInitializer]
    public static void RegisterCommandHandlers()
    {
        // 1. 静态注册避免反射
        CommandDispatcher.Registry.Register<UpdateDeviceStatusCommand, DeviceCommandHandler>();
        QueryDispatcher.Registry.Register<GetDeviceStatusQuery, DeviceQueryHandler>();

        // 2. 属性标记（AOT兼容）
    }

    // 注册消息处理器（AOT安全）
    public static void SubscribeMessages(
        IMqttEventBus eventBus,
        IEnumerable<IMqttMessageHandler> handlers)
    {
        foreach (var handler in handlers)
        {
            // 不使用反射的动态订阅
            eventBus.SubscribeSafe(
                handler.Topic,
                handler.QosLevel,
                handler.HandleMessage);
        }
    }
}

// 传统接口适配器
public class LegacyMqttAdapter : ILegacyMessageApi
{
    private readonly IMqttEventBus _eventBus;

    public LegacyMqttAdapter(IMqttEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task PostMessage(LegacyMessage message)
    {
        // 转换为现代格式
        var newMessage = MqttMessageMapper.FromLegacy(message);

        // 发布到MQTT
        await _eventBus.PublishAsync(
            $"legacy/{message.Topic}",
            newMessage);
    }
}

// AOT兼容的JSON上下文（.NET 8源生成器）
[JsonSerializable(typeof(UpdateDeviceStatusCommand))]
[JsonSerializable(typeof(DeviceStatusDto))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultBufferSize = 128)]
internal partial class MqttAotJsonContext : JsonSerializerContext
{
}

// AOT与动态代理,AOT兼容的代理模式：使用源生成器
[GenerateInterfaceProxy]
public partial class ServiceProxy : IService
{
    private readonly IService _implementation;
    private readonly ILogger<ServiceProxy> _logger;

    public ServiceProxy(IService implementation, ILogger<ServiceProxy> logger)
    {
        _implementation = implementation;
        _logger = logger;
    }

    public void Process()
    {
        _logger.LogInformation($"开始调用 {nameof(Process)}");
        try
        {
            _implementation.Process();
        }
        finally
        {
            _logger.LogInformation($"调用完成 {nameof(Process)}");
        }
    }
}

// AOT反射,反射改进策略
// 使用源生成器替代反射[GenerateSerializer]
[JsonDerivedType(typeof(DeviceOnlineEvent))]
[JsonDerivedType(typeof(DeviceOfflineEvent))]
public interface IDeviceEvent { }

// 手动注册替代动态发现
public static class EventRegistry
{
    private static readonly Dictionary<string, Type> _types = new();

    static EventRegistry()
    {
        Register("online", typeof(DeviceOnlineEvent));
        Register("offline", typeof(DeviceOfflineEvent));
    }

    public static void Register(string typeName, Type eventType)
        => _types[typeName] = eventType;

    public static Type? GetEventType(string name)
        => _types.GetValueOrDefault(name);
}

1. 诊断工具
# 在项目文件中添加配置
//<PropertyGroup>
//  <PublishAot>true</PublishAot>
//  <TrimmerDefaultAction>link</TrimmerDefaultAction>
//  <IlcGenerateCompleteTypeMetadata>false</IlcGenerateCompleteTypeMetadata>
//  <IlcGenerateStackTraceData>false</IlcGenerateStackTraceData>
//</PropertyGroup>
# 分析工具
//dotnet publish -c Release -r win-x64 /p:IlcReportWarning=true
//2. 警告分类和处理
//警告代码    含义 解决方案
//IL2057 动态访问检测  添加[DynamicallyAccessedMembers] 特性
//IL2067 泛型参数缺失  指定where T : new () 约束
//IL2026 需要保留特定成员[DynamicDependency(DynamicallyAccessedMemberTypes.All, type)]
//IL3050 模式不兼容   使用生成器替代动态模式
//SYSLIB0018  反射方法过时 迁移到源生成方案
# 分析工具

// Program.cs
var builder = WebApplication.CreateBuilder(args);

// 现有服务注册
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddUniversalInfrastructure();
// 添加MQTT模块（与现有服务并行）
builder.Services.AddAotCompatibleMqttService(config =>
{
    config.Host = "broker.example.com";
    config.Port = 8883;
    config.UseTls = true;
});

// 共享数据库上下文
builder.Services.AddScoped<IMqttMessageRepository>(provider =>
    provider.GetRequiredService<AppDbContext>());

// 健康检查集成
builder.Services.AddHealthChecks()
    .AddCheck<MqttHealthCheck>("mqtt")
    .AddDbContextCheck<AppDbContext>();

// AOT和JIT混合部署模式
public static bool IsAotEnabled = Configuration.IsAotActive();
if (IsAotEnabled)
{
    builder.Services.AddAotModules();
    builder.Services.AddSingleton<IMessageSerializer, AotBinarySerializer>();
    // AOT模式的静态解析
    AotContainer.Resolve(typeof(T));
}
else
{
    builder.Services.AddJitModules();
    builder.Services.AddSingleton<IMessageSerializer, ReflectionSerializer>();
    // JIT模式的动态解析
    DynamicDI.Resolve<T>();
}
// 共享服务（通用接口）
builder.Services.AddScoped<IDeviceService, UnifiedDeviceService>();

// 动态扫描注册
services.Scan(scan => scan
    .FromAssemblyOf<MyService>()
    .AddClasses(classes => classes.InNamespaces("App.Services"))
    .AsImplementedInterfaces());

// AOT兼容显式注册
var assembly = typeof(Program).Assembly;
var serviceTypes = assembly.GetExportedTypes()
    .Where(t => t.Namespace == "App.Services" && t.IsClass);

foreach (var type in serviceTypes)
{
    services.AddTransient(type);
}

var app = builder.Build();

// 路由配置
app.MapControllers();
// 在启动时注册自定义处理程序
app.UseMqttMessageHandling(builder =>
{
    builder.AddHandler("sensors/temperature", msg =>
    {
        // 业务处理
        var temperature = Encoding.UTF8.GetString(msg.Payload);
        Console.WriteLine($"温度传感器: {temperature}°C");
    });

    builder.AddHandler("devices/+/status", msg =>
    {
        // 设备状态更新
        var deviceId = msg.Topic.Split('/')[1];
        var status = JsonSerializer.Deserialize<DeviceStatus>(msg.Payload);
        _deviceManager.UpdateDeviceStatus(deviceId, status);
    });
});
app.MapMqttGrapavAdapter("/metrics");
app.MapHealthChecks("/health");
app.UseWebSockets();

// 启动MQTT服务（与现有服务共存）
app.UseMqttHostedService();

app.Run();