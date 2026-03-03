#:sdk Microsoft.NET.Sdk.Web
#:package System.Text.Json@8.0.0
#:package MessagePack@2.5.122
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Text.Json;
using MessagePack;

public class HybridSerializer
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        DefaultBufferSize = 4096
    };

    public byte[] SerializeToJson<T>(T value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, _jsonOptions);
    }

    public byte[] SerializeToBinary<T>(T value)
    {
        return MessagePackSerializer.Serialize(value);
    }

    public T? DeserializeFromJson<T>(ReadOnlySpan<byte> data)
    {
        return JsonSerializer.Deserialize<T>(data, _jsonOptions);
    }

    public T DeserializeFromBinary<T>(ReadOnlySpan<byte> data)
    {
        return MessagePackSerializer.Deserialize<T>(data);
    }
}