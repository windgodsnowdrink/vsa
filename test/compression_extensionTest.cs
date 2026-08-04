#load "compression_extension.cs"

Console.WriteLine("=== compression_extension Test ===");

try
{
    var t0 = typeof(MessageCompressor);
    Console.WriteLine($"[PASS] MessageCompressor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}