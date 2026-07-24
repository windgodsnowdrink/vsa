#load "brotli_compression_integration.cs"

Console.WriteLine("=== brotli_compression_integration Test ===");

try
{
    var t0 = typeof(BrotliCompressionOptions);
    Console.WriteLine($"[PASS] BrotliCompressionOptions 存在");
    var t1 = typeof(BrotliMessageCompressor);
    Console.WriteLine($"[PASS] BrotliMessageCompressor 存在");
    var t2 = typeof(BrotliCompressionExtensions);
    Console.WriteLine($"[PASS] BrotliCompressionExtensions 存在");
    var t3 = typeof(IMessageCompressor);
    Console.WriteLine($"[PASS] IMessageCompressor 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}