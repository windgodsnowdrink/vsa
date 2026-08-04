#:sdk Microsoft.NET.Sdk.Web
#:package DeltaCompressionDotNet@1.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using System.IO.MemoryMappedFiles;
using DeltaCompression;

// 版本差异分析器（基于内存映射文件）
public sealed class VersionDiffAnalyzer
{
    private readonly ThreadLocal<DeltaCreator> _deltaCreator;
    private readonly ThreadLocal<DeltaApplier> _deltaApplier;

    public VersionDiffAnalyzer()
    {
        _deltaCreator = new(() => new DeltaCreator());
        _deltaApplier = new(() => new DeltaApplier());
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe byte[] CreateDelta(string oldVersionPath, string newVersionPath)
    {
        using var mmfOld = MemoryMappedFile.CreateFromFile(oldVersionPath);
        using var mmfNew = MemoryMappedFile.CreateFromFile(newVersionPath);
        return _deltaCreator.Value.CreateDelta(mmfOld, mmfNew);
    }
}