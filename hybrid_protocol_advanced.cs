#:sdk Microsoft.NET.Sdk.Web
#:package SpanJson@4.0.0
#:package MessagePack@2.5.122
#:package System.Text.Json@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using MessagePack;
using SpanJson;
using System.Text.Json;

public class HybridProtocolAdvanced
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        DefaultBufferSize = 4096
    };

    public byte[] SmartSerialize<T>(T value, SerializationProtocol protocol)
    {
        return protocol switch
        {
            SerializationProtocol.SpanJson => JsonSerializer.Generic.Utf8.SerializeToArray(value),
            SerializationProtocol.MessagePack => MessagePackSerializer.Serialize(value),
            SerializationProtocol.SystemJson => JsonSerializer.SerializeToUtf8Bytes(value, _jsonOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(protocol))
        };
    }

    public T? SmartDeserialize<T>(ReadOnlySpan<byte> data, SerializationProtocol? protocol = null)
    {
        if (protocol != null)
        {
            return protocol switch
            {
                SerializationProtocol.SpanJson => JsonSerializer.Generic.Utf8.Deserialize<T>(data),
                SerializationProtocol.MessagePack => MessagePackSerializer.Deserialize<T>(data),
                SerializationProtocol.SystemJson => JsonSerializer.Deserialize<T>(data, _jsonOptions),
                _ => throw new ArgumentOutOfRangeException(nameof(protocol))
            };
        }

        // 自动检测协议
        try
        {
            return JsonSerializer.Generic.Utf8.Deserialize<T>(data);
        }
        catch
        {
            try
            {
                return MessagePackSerializer.Deserialize<T>(data);
            }
            catch
            {
                return JsonSerializer.Deserialize<T>(data, _jsonOptions);
            }
        }
    }
}

public enum SerializationProtocol
{
    SpanJson,
    MessagePack,
    SystemJson
}