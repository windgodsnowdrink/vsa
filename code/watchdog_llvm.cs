#:sdk Microsoft.NET.Sdk.Web
#:package WatchDog.NET@3.0.0
#:package Microsoft.DotNet.ILCompiler@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

[MethodImpl(MethodImplOptions.AggressiveOptimization)]
public static unsafe void ProcessLogBuffer(Span<byte> buffer)
{
    fixed (byte* ptr = buffer)
    {
        // 确保cache-line对齐
        if ((long)ptr % 64 == 0) 
        {
            // SIMD向量化处理
            Vector128<byte>.LoadAligned(ptr);
        }
    }
}