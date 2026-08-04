#:sdk Microsoft.NET.Sdk.Web
#:package SpanJson@4.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using Microsoft.Extensions.ObjectPool;
using SpanJson;

public class ThreadSafeMemoryPoolOptimized : IDisposable
{
    private readonly ObjectPool<byte[]> _bufferPool;
    private readonly ObjectPool<Utf8JsonWriter> _writerPool;

    public ThreadSafeMemoryPoolOptimized()
    {
        var bufferPolicy = new BufferPooledObjectPolicy();
        _bufferPool = new DefaultObjectPool<byte[]>(bufferPolicy, Environment.ProcessorCount * 4);

        var writerPolicy = new JsonWriterPooledObjectPolicy();
        _writerPool = new DefaultObjectPool<Utf8JsonWriter>(writerPolicy);
    }

    public byte[] Serialize<T>(T value)
    {
        var buffer = _bufferPool.Get();
        try
        {
            var writer = _writerPool.Get();
            try
            {
                writer.Reset(buffer);
                JsonSerializer.Generic.Utf8.Serialize(ref writer, value);
                return buffer.AsSpan(0, (int)writer.BytesWritten).ToArray();
            }
            finally
            {
                _writerPool.Return(writer);
            }
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    public void Dispose()
    {
        // 清理资源
    }
}

internal class BufferPooledObjectPolicy : IPooledObjectPolicy<byte[]>
{
    public byte[] Create() => ArrayPool<byte>.Shared.Rent(4096);
    
    public bool Return(byte[] obj)
    {
        ArrayPool<byte>.Shared.Return(obj);
        return true;
    }
}

internal class JsonWriterPooledObjectPolicy : IPooledObjectPolicy<Utf8JsonWriter>
{
    public Utf8JsonWriter Create() => new(new MemoryStream());
    
    public bool Return(Utf8JsonWriter obj)
    {
        obj.Reset();
        return true;
    }
}