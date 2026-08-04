#:sdk Microsoft.NET.Sdk.Web
#:package System.Text.Json@8.0.0
#:package System.Memory@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Text.Json;

public class TieredJsonProcessor
{
    private readonly MemoryPool<byte> _hotMemory;
    private readonly MemoryPool<byte> _coldMemory;
    private readonly JsonSerializerOptions _options;

    public TieredJsonProcessor()
    {
        _hotMemory = MemoryPool<byte>.Shared;
        _coldMemory = new NativeMemoryPool(1024 * 1024 * 256); // 256MB
        
        _options = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultBufferSize = 4096,
            MaxDepth = 64,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }

    public byte[] SerializeHot<T>(T value)
    {
        using var memory = _hotMemory.Rent(1024);
        var span = memory.Memory.Span;
        var writer = new Utf8JsonWriter(span);
        JsonSerializer.Serialize(writer, value, _options);
        return span[..writer.BytesWritten].ToArray();
    }

    public byte[] SerializeCold<T>(T value)
    {
        using var memory = _coldMemory.Rent(1024 * 1024); // 1MB
        var span = memory.Memory.Span;
        var writer = new Utf8JsonWriter(span);
        JsonSerializer.Serialize(writer, value, _options);
        return span[..writer.BytesWritten].ToArray();
    }
}