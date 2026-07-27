#:sdk Microsoft.NET.Sdk.Web
#:package MessagePack@2.5.122
#:package Microsoft.IO.RecyclableMemoryStream@3.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using MessagePack;
using Microsoft.IO;

// 进程内高性能MessagePack序列化处理器
public class InProcMessagePackSerializer : IDisposable
{
    private readonly RecyclableMemoryStreamManager _memoryManager;
    private readonly ObjectPool<MemoryStream> _streamPool;
    
    // 构造函数初始化内存管理组件
    public InProcMessagePackSerializer()
    {
        _memoryManager = new RecyclableMemoryStreamManager();
        _streamPool = new DefaultObjectPool<MemoryStream>(
            new MemoryStreamPooledObjectPolicy(_memoryManager), 
            maxCapacity: Environment.ProcessorCount * 2);
    }

    // 高性能序列化方法
    public byte[] Serialize<T>(T value)
    {
        using var memoryStream = _memoryManager.GetStream();
        MessagePackSerializer.Serialize(memoryStream, value);
        return memoryStream.ToArray();
    }

    // 零拷贝序列化方法
    public void Serialize<T>(IBufferWriter<byte> writer, T value)
    {
        var mpWriter = new MessagePackWriter(writer);
        MessagePackSerializer.Serialize(ref mpWriter, value);
        mpWriter.Flush();
    }

    // 高性能反序列化方法
    public T? Deserialize<T>(ReadOnlySpan<byte> data)
    {
        return MessagePackSerializer.Deserialize<T>(data);
    }

    // 池化反序列化方法
    public T? DeserializePooled<T>(ReadOnlyMemory<byte> data)
    {
        var stream = _streamPool.Get();
        try
        {
            stream.Write(data.Span);
            stream.Position = 0;
            return MessagePackSerializer.Deserialize<T>(stream);
        }
        finally
        {
            stream.SetLength(0);
            _streamPool.Return(stream);
        }
    }

    public void Dispose()
    {
        // 清理资源
    }
}

// 内存流池策略
internal class MemoryStreamPooledObjectPolicy : IPooledObjectPolicy<MemoryStream>
{
    private readonly RecyclableMemoryStreamManager _memoryManager;

    public MemoryStreamPooledObjectPolicy(RecyclableMemoryStreamManager memoryManager)
    {
        _memoryManager = memoryManager;
    }

    public MemoryStream Create()
    {
        return _memoryManager.GetStream();
    }

    public bool Return(MemoryStream obj)
    {
        if (obj.CanWrite)
        {
            obj.SetLength(0);
            return true;
        }
        return false;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<InProcMessagePackSerializer>();

var app = builder.Build();
app.MapGet("/", () => "In-Proc MessagePack Serializer Ready");
app.Run();