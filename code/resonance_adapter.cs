#:sdk Microsoft.NET.Sdk.Web
#:package SpanJson@4.0.0
#:package System.Buffers@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Grpc.Net.Client@2.62.0
#:package MQTTnet@4.1.5
#:package RabbitMQ.Client@6.7.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using SpanJson;
using Microsoft.Extensions.ObjectPool;
using Grpc.Net.Client;
using MQTTnet;
using MQTTnet.Client;
using RabbitMQ.Client;
using System.Net.Http.Json;
using Disruptor;
using Disruptor.Dsl;
using System.Runtime.InteropServices;

// 增强版消息协议适配器基类（集成零拷贝、对象池和AES-GCM加密）
// 只需注册各协议适配器并调用ProtocolConversionCenter.ConvertAsync方法即可完成协议转换

// 多协议消息转换器基类
public abstract class MultiProtocolAdapter : IResonanceAdapter
{
    private readonly ObjectPool<byte[]> _bufferPool = new DefaultObjectPool<byte[]>(
        new ArrayPooledObjectPolicy(), Environment.ProcessorCount * 2);
    
    protected readonly ThreadLocal<Span<byte>> _processingBuffer = new(() => stackalloc byte[4096]);

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    protected virtual void ProcessHeader(ReadOnlySpan<byte> data)
    {
        // 添加CPU cache-line对齐的头部处理逻辑
        if (data.Length >= 64) // 典型cache-line大小
        {
            // 使用SIMD指令优化处理
        }
    }
    
    // 添加LLVM IR优化标记
    [ModuleInitializer]
    internal static void Initialize()
    {
        RuntimeHelpers.PrepareMethod(typeof(MultiProtocolAdapter).GetMethod("ProcessHeader").MethodHandle);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ProtocolMessage))]
    [UnconditionalSuppressMessage("Trimming", "IL2026")]
    [RequiresUnreferencedCode("Uses reflection for protocol message types")]
    public abstract ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data);

    // Add AOT-friendly serialization helper
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    protected static T AotSafeDeserialize<T>(ReadOnlySpan<byte> data) where T : class
    {
        return JsonSerializer.Generic.Utf8.Deserialize<T>(data) ?? 
               throw new InvalidOperationException("Deserialization failed");
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public abstract ValueTask<ReadOnlyMemory<byte>> AdaptOutboundAsync(object message);

    protected virtual bool EnableEncryption => false;
}

public abstract class ResonanceMessageAdapter<TMessage> : MultiProtocolAdapter
{
    // Replace ThreadLocal<Span<byte>> with cache-aligned version
    private readonly ThreadLocal<CacheAlignedBuffer> _alignedProcessingBuffer = new(() => new());

    private readonly ObjectPool<byte[]> _bufferPool = new DefaultObjectPool<byte[]>(
        new ArrayPooledObjectPolicy(), Environment.ProcessorCount * 2);
    
    private readonly ThreadLocal<AesGcm> _encryptor = new(() => new AesGcm(RandomNumberGenerator.GetBytes(16)));
    private readonly ThreadLocal<Span<byte>> _processingBuffer = new(() => stackalloc byte[4096]);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override async ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data)
    {
        var buffer = _bufferPool.Get();
        try
        {
            // 使用Span进行零拷贝处理
            // Span<byte> processingSpan = _processingBuffer.Value;
            // data.Span.CopyTo(processingSpan);

            // Use cache-aligned buffer
            Span<byte> processingSpan = _alignedProcessingBuffer.Value.AsSpan();
            data.Span.CopyTo(processingSpan);
            
            // 解密处理（如果启用加密）
            if (IsEncrypted(data.Span))
            {
                var nonce = processingSpan.Slice(0, 12);
                var tag = processingSpan.Slice(12, 16);
                var ciphertext = processingSpan.Slice(28, data.Length - 28);
                
                _encryptor.Value.Decrypt(nonce, ciphertext, tag, processingSpan.Slice(28));
            }
            
            return JsonSerializer.Generic.Utf8.Deserialize<TMessage>(processingSpan.Slice(0, data.Length));
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask<ReadOnlyMemory<byte>> AdaptOutboundAsync(object message)
    {
        if (message is not TMessage typedMessage)
            throw new ResonanceException($"消息类型不匹配: {message.GetType()}");

        var buffer = _bufferPool.Get();
        try
        {
            // 使用Span进行零拷贝序列化
            Span<byte> outputSpan = buffer.AsSpan();
            var bytesWritten = JsonSerializer.Generic.Utf8.Serialize(typedMessage, outputSpan);
            
            // 加密处理（如果启用加密）
            if (EnableEncryption)
            {
                var nonce = RandomNumberGenerator.GetBytes(12);
                var tag = new byte[16];
                
                _encryptor.Value.Encrypt(
                    nonce, 
                    outputSpan.Slice(0, bytesWritten), 
                    outputSpan.Slice(28), 
                    tag);
                
                nonce.CopyTo(outputSpan);
                tag.CopyTo(outputSpan.Slice(12));
                bytesWritten += 28;
            }
            
            return buffer.AsMemory(0, bytesWritten);
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    protected virtual bool EnableEncryption => false;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsEncrypted(ReadOnlySpan<byte> data) => 
        data.Length > 28 && data[0] == 0x01;
}

// TCP协议适配器
public class TcpProtocolAdapter : MultiProtocolAdapter
{
    public override async ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data)
    {
        var buffer = _bufferPool.Get();
        try
        {
            Span<byte> processingSpan = _processingBuffer.Value;
            data.Span.CopyTo(processingSpan);
            return JsonSerializer.Generic.Utf8.Deserialize<ProtocolMessage>(processingSpan.Slice(0, data.Length));
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    public override async ValueTask<ReadOnlyMemory<byte>> AdaptOutboundAsync(object message)
    {
        var buffer = _bufferPool.Get();
        try
        {
            Span<byte> outputSpan = buffer.AsSpan();
            var bytesWritten = JsonSerializer.Generic.Utf8.Serialize(message, outputSpan);
            return buffer.AsMemory(0, bytesWritten);
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }
}

// gRPC协议适配器
public class GrpcProtocolAdapter : MultiProtocolAdapter
{
    private readonly DisruptorRingBuffer<GrpcMessage> _ringBuffer;
    private readonly GrpcChannel _channel;

    public GrpcProtocolAdapter(string address)
    {
        _channel = GrpcChannel.ForAddress(address);
        _ringBuffer = new DisruptorRingBuffer<GrpcMessage>(1024,
            () => new GrpcMessage(new ThreadLocal<Span<byte>>(() => stackalloc byte[4096])));
    }

    public override async ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data)
    {
        var client = new ProtocolService.ProtocolServiceClient(_channel);
        var response = await client.ReceiveAsync(new ProtocolRequest { Data = data.ToArray() });
        return JsonSerializer.Generic.Utf8.Deserialize<ProtocolMessage>(response.Data.Span);
    }

    public override async ValueTask<ReadOnlyMemory<byte>> AdaptOutboundAsync(object message)
    {
        var buffer = _bufferPool.Get();
        try
        {
            Span<byte> outputSpan = buffer.AsSpan();
            var bytesWritten = JsonSerializer.Generic.Utf8.Serialize(message, outputSpan);
            
            var client = new ProtocolService.ProtocolServiceClient(_channel);
            await client.SendAsync(new ProtocolRequest { Data = outputSpan.Slice(0, bytesWritten).ToArray() });
            
            return outputSpan.Slice(0, bytesWritten).ToArray();
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }
}

// RESTful协议适配器
public class RestProtocolAdapter : MultiProtocolAdapter
{
    private readonly HttpClient _httpClient;

    public RestProtocolAdapter(string baseAddress)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
    }

    public override async ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/messages", data);
        return await response.Content.ReadFromJsonAsync<ProtocolMessage>();
    }

    public override async ValueTask<ReadOnlyMemory<byte>> AdaptOutboundAsync(object message)
    {
        // 使用stackalloc避免堆分配
        // Span<byte> buffer = stackalloc byte[4096];
        // var bytesWritten = JsonSerializer.Generic.Utf8.Serialize(message, buffer);
        // return buffer.Slice(0, bytesWritten).ToArray();

        var response = await _httpClient.PostAsJsonAsync("/api/messages", message);
        return await response.Content.ReadAsByteArrayAsync();
    }
}

// MQTT协议适配器
public class MqttProtocolAdapter : MultiProtocolAdapter
{
    private readonly IMqttClient _mqttClient;

    public MqttProtocolAdapter(string brokerAddress)
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();
        
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerAddress)
            .Build();
            
        _mqttClient.ConnectAsync(options).Wait();
    }

    public override async ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data)
    {
        var buffer = _bufferPool.Get();
        try
        {
            Span<byte> processingSpan = _processingBuffer.Value;
            data.Span.CopyTo(processingSpan);
            return JsonSerializer.Generic.Utf8.Deserialize<ProtocolMessage>(processingSpan.Slice(0, data.Length));
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    public override async ValueTask<ReadOnlyMemory<byte>> AdaptOutboundAsync(object message)
    {
        var buffer = _bufferPool.Get();
        try
        {
            Span<byte> outputSpan = buffer.AsSpan();
            var bytesWritten = JsonSerializer.Generic.Utf8.Serialize(message, outputSpan);
            
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic("protocol/messages")
                .WithPayload(outputSpan.Slice(0, bytesWritten).ToArray())
                .Build();
                
            await _mqttClient.PublishAsync(mqttMessage);
            
            return outputSpan.Slice(0, bytesWritten).ToArray();
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }
}

// AMQP协议适配器
public class AmqpProtocolAdapter : MultiProtocolAdapter
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public AmqpProtocolAdapter(string hostName)
    {
        var factory = new ConnectionFactory { HostName = hostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare("protocol_messages", durable: true, exclusive: false);
    }

    public override async ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data)
    {
        var buffer = _bufferPool.Get();
        try
        {
            Span<byte> processingSpan = _processingBuffer.Value;
            data.Span.CopyTo(processingSpan);
            return JsonSerializer.Generic.Utf8.Deserialize<ProtocolMessage>(processingSpan.Slice(0, data.Length));
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    public override async ValueTask<ReadOnlyMemory<byte>> AdaptOutboundAsync(object message)
    {
        var buffer = _bufferPool.Get();
        try
        {
            Span<byte> outputSpan = buffer.AsSpan();
            var bytesWritten = JsonSerializer.Generic.Utf8.Serialize(message, outputSpan);
            
            _channel.BasicPublish(
                exchange: "",
                routingKey: "protocol_messages",
                basicProperties: null,
                body: outputSpan.Slice(0, bytesWritten).ToArray());
                
            return outputSpan.Slice(0, bytesWritten).ToArray();
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }
}

// 高性能JSON协议适配器（集成压缩和缓存）
public class JsonMessageAdapter : ResonanceMessageAdapter<JsonMessage>
{
    private readonly ObjectPool<MemoryStream> _streamPool;
    
    public JsonMessageAdapter()
    {
        _streamPool = new DefaultObjectPool<MemoryStream>(
            new MemoryStreamPooledPolicy(), 
            Environment.ProcessorCount * 2);
    }

    protected override async ValueTask<ReadOnlyMemory<byte>> AdaptOutboundAsync(object message)
    {
        var stream = _streamPool.Get();
        try
        {
            await using var compressionStream = new BrotliStream(stream, CompressionLevel.Optimal, leaveOpen: true);
            await JsonSerializer.Generic.Utf8.SerializeAsync(compressionStream, message);
            await compressionStream.FlushAsync();
            
            return stream.GetBuffer().AsMemory(0, (int)stream.Length);
        }
        finally
        {
            stream.SetLength(0);
            _streamPool.Return(stream);
        }
    }
}

// 增强版消息总线服务（集成Disruptor模式）
public class ResonanceMessageBus : IAsyncDisposable
{
    private readonly ResonanceTransporter _transporter;
    private readonly ObjectPool<ResonanceMessage> _messagePool;
    private readonly Channel<ResonanceMessage> _priorityChannel;
    
    public ResonanceMessageBus(ResonanceTransporter transporter)
    {
        _transporter = transporter;
        _messagePool = new DefaultObjectPool<ResonanceMessage>(
            new MessagePooledObjectPolicy(), 
            Environment.ProcessorCount * 4);
            
        _priorityChannel = Channel.CreateBounded<ResonanceMessage>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });
        
        _ = Task.Run(ProcessPriorityMessagesAsync);
    }

    public async ValueTask PublishAsync<T>(T message, MessagePriority priority = MessagePriority.Normal) where T : IResonanceMessage
    {
        var resonanceMessage = _messagePool.Get();
        try
        {
            resonanceMessage.Payload = message;
            resonanceMessage.Priority = priority;
            
            if (priority > MessagePriority.High)
                await _priorityChannel.Writer.WriteAsync(resonanceMessage);
            else
                await _transporter.SendAsync(resonanceMessage);
        }
        finally
        {
            _messagePool.Return(resonanceMessage);
        }
    }

    private async Task ProcessPriorityMessagesAsync()
    {
        await foreach (var message in _priorityChannel.Reader.ReadAllAsync())
        {
            await _transporter.SendAsync(message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _priorityChannel.Writer.Complete();
        await _transporter.DisposeAsync();
    }
}


// 协议转换中心
public class ProtocolConversionCenter
{
    private readonly Dictionary<ProtocolType, MultiProtocolAdapter> _adapters;

    public ProtocolConversionCenter()
    {
        _adapters = new Dictionary<ProtocolType, MultiProtocolAdapter>();
    }

    public void RegisterAdapter(ProtocolType protocolType, MultiProtocolAdapter adapter)
    {
        _adapters[protocolType] = adapter;
    }

    public async Task<object> ConvertAsync(ProtocolType from, ProtocolType to, object message)
    {
        if (!_adapters.TryGetValue(from, out var sourceAdapter) || 
            !_adapters.TryGetValue(to, out var targetAdapter))
        {
            throw new InvalidOperationException("Protocol adapter not registered");
        }

        var intermediateData = await sourceAdapter.AdaptOutboundAsync(message);
        return await targetAdapter.AdaptInboundAsync(intermediateData);
    }
}

// Add this class for Disruptor pattern implementation
[StructLayout(LayoutKind.Explicit, Size = 64)] // Explicit cache-line alignment
public sealed class DisruptorRingBuffer<T> : IDisposable where T : class, new()
{
    private readonly RingBuffer<MessageEvent<T>> _ringBuffer;
    private readonly Disruptor<MessageEvent<T>> _disruptor;
    private readonly ISequenceBarrier _sequenceBarrier;
    private Sequence _consumerSequence = new();
    
    public DisruptorRingBuffer(int bufferSize, Func<T> factory)
    {
        _disruptor = new Disruptor<MessageEvent<T>>(
            () => new MessageEvent<T>(factory()),
            bufferSize,
            TaskScheduler.Default,
            ProducerType.Multi,
            new BlockingWaitStrategy());
        
        _ringBuffer = _disruptor.RingBuffer;
        _sequenceBarrier = _ringBuffer.NewBarrier();
        _disruptor.HandleEventsWith(new MessageEventHandler<T>());
        _disruptor.Start();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Publish(Action<T> handler)
    {
        var sequence = _ringBuffer.Next();
        try
        {
            var evt = _ringBuffer[sequence];
            handler(evt.Message);
        }
        finally
        {
            _ringBuffer.Publish(sequence);
        }
    }

    public void Dispose() => _disruptor.Shutdown();
}

[StructLayout(LayoutKind.Explicit, Size = 64)]
public class MessageEvent<T> where T : class
{
    [FieldOffset(0)] public T Message;
    
    public MessageEvent(T message) => Message = message;
}

public class MessageEventHandler<T> : IEventHandler<MessageEvent<T>> where T : class
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void OnEvent(MessageEvent<T> data, long sequence, bool endOfBatch)
    {
        // Message processing logic here
    }
}

// Add cache-line aligned buffer helper
[StructLayout(LayoutKind.Explicit, Size = 64)] // Typical cache-line size
public unsafe struct CacheAlignedBuffer
{
    [FieldOffset(0)] public fixed byte Data[64];
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public Span<byte> AsSpan() => MemoryMarshal.CreateSpan(ref Data[0], 64);
}


// 启动配置
var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<ResonanceTransporter>();
builder.Services.AddSingleton<ResonanceMessageBus>();
builder.Services.AddSingleton<IResonanceAdapter, JsonMessageAdapter>();
builder.Services.AddSingleton<IResonanceAdapter, BinaryMessageAdapter>();
builder.Services.AddSingleton<ProtocolConversionCenter>();
builder.Services.AddSingleton<MultiProtocolAdapter, TcpProtocolAdapter>();
builder.Services.AddSingleton<MultiProtocolAdapter, GrpcProtocolAdapter>();
builder.Services.AddSingleton<MultiProtocolAdapter, RestProtocolAdapter>();
builder.Services.AddSingleton<MultiProtocolAdapter, MqttProtocolAdapter>();
builder.Services.AddSingleton<MultiProtocolAdapter, AmqpProtocolAdapter>();


var app = builder.Build();
app.MapGet("/", () => "Enhanced Resonance Message Bus Ready");
app.Run();

public enum ProtocolType { Tcp, Grpc, Rest, Mqtt, Amqp }
public record ProtocolMessage(string Id, string Content, DateTime Timestamp);

public class CompressedMessageAdapter : ResonanceMessageAdapter<CompressedMessage>
{
    private readonly ObjectPool<MemoryStream> _streamPool;
    
    public CompressedMessageAdapter()
    {
        _streamPool = new DefaultObjectPool<MemoryStream>(
            new MemoryStreamPooledPolicy(),
            Environment.ProcessorCount * 2);
    }

    protected override async ValueTask<ReadOnlyMemory<byte>> AdaptOutboundAsync(object message)
    {
        using var memoryStream = _streamPool.Get();
        await using var compressionStream = new BrotliStream(memoryStream, CompressionLevel.Optimal);
        {
            await using var compressionStream = new BrotliStream(memoryStream, CompressionLevel.Optimal);
            // ... compression logic ...
    }
}

public class EncryptedMessageAdapter : ResonanceMessageAdapter<EncryptedMessage>
{
    private readonly byte[] _encryptionKey;
    
    public EncryptedMessageAdapter(byte[] encryptionKey)
    {
        _encryptionKey = encryptionKey;
    }

    protected override async ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data)
    {
        using var aes = new AesGcm(_encryptionKey);
        // ... decryption logic ...
    }
}

public class ChunkedMessageAdapter : ResonanceMessageAdapter<ChunkedMessage>
{
    private readonly ConcurrentDictionary<Guid, List<byte[]>> _chunkStore;
    
    public async ValueTask<object> ReassembleMessage(Guid messageId)
    {
        // ... reassembly logic ...
    }
}

public class TracedMessageAdapter : ResonanceMessageAdapter<TracedMessage>
{
    private readonly ActivitySource _activitySource;
    
    public TracedMessageAdapter(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    protected override async ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data)
    {
        using var activity = _activitySource.StartActivity("MessageProcessing");
        // ... tracing logic ...
    }
}

public class PriorityMessageAdapter : ResonanceMessageAdapter<PriorityMessage>
{
    private readonly PriorityChannel<ResonanceMessage> _priorityChannel;
    
    public PriorityMessageAdapter()
    {
        _priorityChannel = new PriorityChannel<ResonanceMessage>(
            priorityComparer: (x, y) => x.Priority.CompareTo(y.Priority));
    }
}

public class PersistentMessageAdapter : ResonanceMessageAdapter<PersistentMessage>
{
    private readonly IMessageStore _messageStore;
    
    public PersistentMessageAdapter(IMessageStore messageStore)
    {
        _messageStore = messageStore;
    }

    protected override async ValueTask<object> AdaptInboundAsync(ReadOnlyMemory<byte> data)
    {
        await _messageStore.StoreAsync(data);
    }
}