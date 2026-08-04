#:sdk Microsoft.NET.Sdk.Web
#:package MessagePack@2.5.122
#:package System.Buffers@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using MessagePack;

public class SpanZeroCopySerializer
{
    // 零拷贝序列化到现有缓冲区
    public int Serialize<T>(Span<byte> buffer, T value)
    {
        var writer = new MessagePackWriter(buffer);
        MessagePackSerializer.Serialize(ref writer, value);
        return writer.BytesWritten;
    }

    // 零拷贝反序列化
    public T? Deserialize<T>(ReadOnlySpan<byte> data)
    {
        return MessagePackSerializer.Deserialize<T>(data);
    }

    // 使用ArrayPool的优化版本
    public byte[] SerializeWithPool<T>(T value)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(1024);
        try
        {
            var span = buffer.AsSpan();
            var bytesWritten = Serialize(span, value);
            return span[..bytesWritten].ToArray();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}