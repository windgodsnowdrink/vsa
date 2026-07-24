#:sdk Microsoft.NET.Sdk.Web
#:package ZeroFormatter@1.7.0
#:package Disruptor.Net@3.4.0
#:package Microsoft.IO.RecyclableMemoryStream@2.3.2
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable

using System.Buffers;
using System.Runtime.CompilerServices;
using Disruptor;
using ZeroFormatter;

var builder = WebApplication.CreateBuilder(args);

// 1. Disruptor环形缓冲区配置
var disruptor = new Disruptor<MessageEvent>(() => new MessageEvent(), 
    ringBufferSize: 1_048_576,
    taskScheduler: TaskScheduler.Default,
    producerType: ProducerType.Multi,
    waitStrategy: new BusySpinIdleStrategy());

// 2. 零拷贝消息处理器链
disruptor.HandleEventsWith(
    new MessageValidator(),
    new MessageTransformer(),
    new MessagePersister());

builder.Services.AddSingleton(disruptor.Start());

var app = builder.Build();
app.MapGet("/", () => "ZeroFormatter MessageBus Ready");
app.Run();

// 消息事件结构（256字节对齐）
[ZeroFormattable, SkipLocalsInit]
public struct MessageEvent
{
    [Index(0)] public long Sequence;
    [Index(1)] public long Timestamp;
    [Index(2)] public byte MessageType;
    [Index(3)] public byte[] Payload;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => Payload = null;
}

// 消息验证处理器
public sealed class MessageValidator : IEventHandler<MessageEvent>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void OnEvent(MessageEvent data, long sequence, bool endOfBatch)
    {
        // 验证逻辑
    }
}

// 消息转换处理器
public sealed class MessageTransformer : IEventHandler<MessageEvent>
{
    private readonly ThreadLocal<Span<byte>> _buffer = 
        new(() => stackalloc byte[256], trackAllValues: false);
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void OnEvent(MessageEvent data, long sequence, bool endOfBatch)
    {
        var buffer = _buffer.Value;
        // 转换逻辑
    }
}

// 消息持久化处理器
public sealed class MessagePersister : IEventHandler<MessageEvent>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void OnEvent(MessageEvent data, long sequence, bool endOfBatch)
    {
        // 持久化逻辑
        var bytes = ZeroFormatterSerializer.Serialize(data);
    }
}