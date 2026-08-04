#load "zstd_compression_integration.cs"

Console.WriteLine("=== zstd_compression_integration Test ===");

try
{
    var t0 = typeof(ZstdCompression.ZstdCompressionOptions);
    Console.WriteLine($"[PASS] ZstdCompressionOptions 存在");
    var t1 = typeof(ZstdCompression.ZstdCompressor);
    Console.WriteLine($"[PASS] ZstdCompressor 存在");
    var t2 = typeof(ZstdCompression.ZstdCompressionExtensions);
    Console.WriteLine($"[PASS] ZstdCompressionExtensions 存在");
    var t3 = typeof(ZstdCompression.IZstdCompressor);
    Console.WriteLine($"[PASS] IZstdCompressor 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}