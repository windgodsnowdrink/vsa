#:sdk Microsoft.NET.Sdk
#:package LiteDB@5.0.17
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using LiteDB;

public class ThreadLocalMemoryPool
{
    private static readonly ThreadLocal<byte[]> _threadLocalBuffer = 
        new(() => ArrayPool<byte>.Shared.Rent(4096));

    public static byte[] GetBuffer()
    {
        return _threadLocalBuffer.Value;
    }

    public static void ReturnBuffer()
    {
        ArrayPool<byte>.Shared.Return(_threadLocalBuffer.Value);
    }
}