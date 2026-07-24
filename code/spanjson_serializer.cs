#:sdk Microsoft.NET.Sdk.Web
#:package SpanJson@4.0.0
#:package System.Buffers@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using SpanJson;

// 高性能SpanJson序列化处理器
public class SpanJsonSerializer : IDisposable
{
    private readonly ArrayPool<byte> _arrayPool;
    private readonly ObjectPool<byte[]> _bufferPool;
    
    public SpanJsonSerializer()
    {
        _arrayPool = ArrayPool<byte>.Shared;
        _bufferPool = new DefaultObjectPool<byte[]>(
            new ArrayPooledObjectPolicy(), 
            maxCapacity: Environment.ProcessorCount * 2);
    }

    // 零拷贝序列化
    public int Serialize<T>(Span<byte> buffer, T value)
    {
        return JsonSerializer.Generic.Utf8.Serialize(value, buffer);
    }

    // 使用ArrayPool的序列化
    public byte[] SerializeWithPool<T>(T value)
    {
        var buffer = _bufferPool.Get();
        try
        {
            var bytesWritten = Serialize(buffer, value);
            return buffer.AsSpan(0, bytesWritten).ToArray();
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    // 零拷贝反序列化
    public T? Deserialize<T>(ReadOnlySpan<byte> json)
    {
        return JsonSerializer.Generic.Utf8.Deserialize<T>(json);
    }

    // 使用ArrayPool的反序列化
    public T? DeserializeWithPool<T>(byte[] json)
    {
        var buffer = _bufferPool.Get();
        try
        {
            json.CopyTo(buffer, 0);
            return Deserialize<T>(buffer.AsSpan(0, json.Length));
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

// ArrayPool策略
internal class ArrayPooledObjectPolicy : IPooledObjectPolicy<byte[]>
{
    public byte[] Create()
    {
        return ArrayPool<byte>.Shared.Rent(4096);
    }

    public bool Return(byte[] obj)
    {
        ArrayPool<byte>.Shared.Return(obj);
        return true;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<SpanJsonSerializer>();

var app = builder.Build();
app.MapGet("/", () => "SpanJson Serializer Ready");
app.Run();