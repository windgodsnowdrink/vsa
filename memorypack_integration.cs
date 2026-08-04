#:sdk Microsoft.NET.Sdk.Web
#:package MemoryPack@1.9.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Runtime.CompilerServices;
using MemoryPack;
using Microsoft.Extensions.ObjectPool;

[SkipLocalsInit]
public static class MemoryPackIntegration
{
    private static readonly ObjectPool<IMemoryOwner<byte>> _bufferPool = 
        new DefaultObjectPool<IMemoryOwner<byte>>(new MemoryOwnerPooledPolicy(), 1024);

    [MemoryPackable]
    public partial class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public bool IsCompleted { get; set; }
    }

    public static byte[] Serialize<T>(T value) where T : class
    {
        var bufferOwner = _bufferPool.Get();
        try
        {
            var buffer = bufferOwner.Memory;
            MemoryPackSerializer.Serialize(buffer, value);
            return buffer.ToArray();
        }
        finally
        {
            _bufferPool.Return(bufferOwner);
        }
    }

    public static T? Deserialize<T>(ReadOnlySpan<byte> buffer) where T : class
    {
        return MemoryPackSerializer.Deserialize<T>(buffer);
    }

    // DI扩展方法
    public static IServiceCollection AddMemoryPackIntegration(this IServiceCollection services)
    {
        services.AddSingleton(_bufferPool);
        return services;
    }
}

// 高性能内存池策略
internal sealed class MemoryOwnerPooledPolicy : IPooledObjectPolicy<IMemoryOwner<byte>>
{
    public IMemoryOwner<byte> Create() => MemoryPool<byte>.Shared.Rent(4096);
    
    public bool Return(IMemoryOwner<byte> obj)
    {
        if (obj.Memory.Length >= 4096)
        {
            obj.Dispose();
            return false;
        }
        return true;
    }
}