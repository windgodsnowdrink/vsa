#:sdk Microsoft.NET.Sdk
#:package LiteDB@5.0.17
#:package System.Buffers@4.5.1
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;

public static class EventSerializer
{
    private static readonly ArrayPool<byte> _pool = ArrayPool<byte>.Shared;

    public static byte[] Serialize(OutOfBandEvent @event)
    {
        var buffer = _pool.Rent(4096);
        try
        {
            var span = buffer.AsSpan();
            // 零拷贝序列化逻辑...
            return span.ToArray();
        }
        finally
        {
            _pool.Return(buffer);
        }
    }

    public static OutOfBandEvent Deserialize(ReadOnlySpan<byte> data)
    {
        // 零拷贝反序列化逻辑...
        return new OutOfBandEvent();
    }
}