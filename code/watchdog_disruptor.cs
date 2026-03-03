#:sdk Microsoft.NET.Sdk.Web
#:package WatchDog.NET@3.0.0
#:package Disruptor-net@3.4.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using Disruptor;
using System.Runtime.CompilerServices;

public sealed class LogEventProcessor : IEventHandler<LogEvent>
{
    private readonly ObjectPool<Memory<byte>> _memoryPool;
    
    public LogEventProcessor(ObjectPool<Memory<byte>> pool) => _memoryPool = pool;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnEvent(LogEvent data, long sequence, bool endOfBatch)
    {
        using var memory = _memoryPool.Get();
        data.CopyTo(memory.Span); // 零拷贝处理
    }
}

// 在WatchDog配置中添加
builder.Services.AddSingleton<ObjectPool<Memory<byte>>>(_ => 
    new DefaultObjectPool<Memory<byte>>(new MemoryPooledObjectPolicy(), 1000));