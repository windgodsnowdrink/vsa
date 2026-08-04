#:sdk Microsoft.NET.Sdk.Web
#:package System.Text.Json@8.0.0
#:package Microsoft.IO.RecyclableMemoryStream@3.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Text.Json;
using Microsoft.IO;

// 进程内高性能JSON序列化处理器
public class InProcJsonSerializer : IDisposable
{
    private readonly RecyclableMemoryStreamManager _memoryManager;
    private readonly ObjectPool<MemoryStream> _streamPool;
    private readonly JsonSerializerOptions _options;

    // 构造函数初始化组件
    public InProcJsonSerializer()
    {
        _memoryManager = new RecyclableMemoryStreamManager();
        _streamPool = new DefaultObjectPool<MemoryStream>(
            new MemoryStreamPooledObjectPolicy(_memoryManager),
            maxCapacity: Environment.ProcessorCount * 2);

        _options = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultBufferSize = 4096,
            MaxDepth = 64,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }

    // 高性能序列化方法
    public byte[] Serialize<T>(T value)
    {
        using var memoryStream = _memoryManager.GetStream();
        JsonSerializer.Serialize(memoryStream, value, _options);
        return memoryStream.ToArray();
    }

    // 零拷贝序列化方法
    public void Serialize<T>(IBufferWriter<byte> writer, T value)
    {
        var jsonWriter = new Utf8JsonWriter(writer);
        JsonSerializer.Serialize(jsonWriter, value, _options);
    }

    // 高性能反序列化方法
    public T? Deserialize<T>(ReadOnlySpan<byte> data)
    {
        return JsonSerializer.Deserialize<T>(data, _options);
    }

    // 池化反序列化方法
    public T? DeserializePooled<T>(ReadOnlyMemory<byte> data)
    {
        var stream = _streamPool.Get();
        try
        {
            stream.Write(data.Span);
            stream.Position = 0;
            return JsonSerializer.Deserialize<T>(stream, _options);
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
builder.Services.AddSingleton<InProcJsonSerializer>();

var app = builder.Build();
app.MapGet("/", () => "In-Proc JSON Serializer Ready");
app.Run();