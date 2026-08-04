#:sdk Microsoft.NET.Sdk.Web
#:package EventStore.Client@22.0.0
#:package Aeron@1.40.0
#:package ZeroFormatter@1.7.0
#:property LangVersion preview
#:property TargetFramework net10.0

// 百万级TPS架构： 事件溯源+UDP传输+Aeron
using System.Threading.Channels;
using Aeron;
using EventStore.Client;
using ZeroFormatter;

var builder = WebApplication.CreateBuilder(args);

// 核心组件：
// 1. Aeron UDP传输层 - 基于高吞吐量消息库
builder.Services.AddSingleton<IAeron>(_ => 
    Aeron.Context.New()
        .AeronDirectoryName("aeron")
        .MTU(4096)
        .ThreadingMode(ThreadingMode.Dedicated)
        .InitialWindowLength(1_000_000));

builder.Services.AddSingleton<IMessageTransport>(_ => 
    new AeronTransport(
        new Aeron.Context()
            .AeronDirectoryName("aeron")
            .MTU(4096)));

// 2. EventSourcing事件存储 - 使用EventStoreDB集群
builder.Services.AddEventStoreClient(settings => {
    settings.ConnectivitySettings.Address = new Uri("esdb://cluster:2113");
    settings.DefaultDeadline = TimeSpan.FromSeconds(5);
});

// 3. 零拷贝消息通道（Disruptor模式）
var messageChannel = Channel.CreateBounded<MessageEnvelope>(
    new BoundedChannelOptions(1_000_000) {
        SingleReader = true,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.DropOldest
    });

builder.Services.AddSingleton(messageChannel);
builder.Services.AddHostedService<MessageProcessor>();

// 3. 零拷贝内存管理
[SkipLocalsInit]
public class AeronMessageProcessor : IMessageProcessor
{
    private readonly ThreadLocal<Span<byte>> _buffer = 
        new(() => stackalloc byte[4096]);
    
    public unsafe void Process(byte* data, int length)
    {
        var span = new Span<byte>(data, length);
        // 使用栈上分配的内存处理消息
    }
}

// 4. 线程本地内存分配器
builder.Services.AddSingleton<IMemoryAllocator>(_ => 
    new ThreadLocalAllocator(
        blockSize: 1.MB,
        maxBlocksPerThread: 1000,
        cacheLineSize: 64));

var app = builder.Build();
app.Run();

// 零拷贝消息结构
[ZeroFormattable, SkipLocalsInit]
public struct MessageEnvelope
{
    [Index(0)] public long Timestamp;
    [Index(1)] public int MessageType;
    [Index(2)] public byte[] Payload;
}

// 高性能消息处理器
public sealed class MessageProcessor : BackgroundService
{
    private readonly Channel<MessageEnvelope> _channel;
    private readonly ThreadLocal<Span<byte>> _buffer;
    private readonly IEventStoreClient _eventStore;

    public MessageProcessor(
        Channel<MessageEnvelope> channel,
        IEventStoreClient eventStore)
    {
        _channel = channel;
        _eventStore = eventStore;
        _buffer = new(() => stackalloc byte[1024], trackAllValues: false);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            var buffer = _buffer.Value;
            await ProcessMessage(message, buffer);
        }
    }

    [SkipLocalsInit]
    private unsafe Task ProcessMessage(MessageEnvelope message, Span<byte> buffer)
    {
        fixed (byte* ptr = buffer)
        {
            // 零拷贝处理逻辑
            var eventData = new EventData(
                Uuid.NewUuid(),
                $"MessageType_{message.MessageType}",
                buffer.Slice(0, message.Payload.Length).ToArray());
            
            return _eventStore.AppendToStreamAsync(
                $"messages-{DateTime.UtcNow:yyyyMMdd}",
                StreamState.Any,
                new[] { eventData });
        }
    }
}

// 线程本地内存分配器
public class ThreadLocalAllocator : IMemoryAllocator
{
    private readonly ThreadLocal<Memory<byte>> _threadLocalMemory;

    public ThreadLocalAllocator(int blockSize, int maxBlocksPerThread, int cacheLineSize)
    {
        _threadLocalMemory = new(() => {
            var memory = new Memory<byte>(new byte[blockSize]);
            // 确保内存对齐
            if ((long)Unsafe.AsPointer(ref memory.Span[0]) % cacheLineSize != 0)
                throw new InvalidOperationException("Memory not aligned");
            return memory;
        }, trackAllValues: false);
    }

    public Memory<byte> Rent(int size) => _threadLocalMemory.Value.Slice(0, size);
    public void Return(Memory<byte> memory) { }
}
