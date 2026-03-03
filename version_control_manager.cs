#:sdk Microsoft.NET.Sdk.Web
#:package ZstdNet@1.4.5
#:package Microsoft.IO.RecyclableMemoryStream@2.3.2
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using System.Buffers;
using ZstdNet;
using Microsoft.IO;

public sealed class VersionControlManager
{
    private readonly ThreadLocal<Compressor> _compressor;
    private readonly ThreadLocal<Decompressor> _decompressor;

    public VersionControlManager()
    {
        _compressor = new(() => new Compressor(new CompressionOptions(3)));
        _decompressor = new(() => new Decompressor());
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte[] CompressSnapshot(byte[] data) => _compressor.Value.Wrap(data);

    [SkipLocalsInit]
    public unsafe Span<byte> DecompressSnapshot(Span<byte> compressed)
    {
        fixed (byte* ptr = compressed)
        {
            return _decompressor.Value.Unwrap(ptr, compressed.Length);
        }
    }
}

public sealed class SnapshotCompressionEngine
{
    private readonly ThreadLocal<Compressor> _compressor;
    private readonly ThreadLocal<Decompressor> _decompressor;
    private readonly RecyclableMemoryStreamManager _memoryManager;

    public SnapshotCompressionEngine(RecyclableMemoryStreamManager memoryManager)
    {
        _memoryManager = memoryManager;
        _compressor = new(() => new Compressor(new CompressionOptions(3)));
        _decompressor = new(() => new Decompressor());
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public Memory<byte> CompressSnapshot(ReadOnlySpan<byte> data)
    {
        using var stream = _memoryManager.GetStream();
        var compressed = _compressor.Value.Wrap(data.ToArray());
        stream.Write(compressed);
        return stream.GetBuffer().AsMemory(0, (int)stream.Length);
    }

    [SkipLocalsInit]
    public unsafe Span<byte> DecompressSnapshot(ReadOnlySpan<byte> compressed)
    {
        fixed (byte* ptr = compressed)
        {
            return _decompressor.Value.Unwrap(ptr, compressed.Length);
        }
    }
}