#load "mimekit_integration.cs"

Console.WriteLine("=== mimekit_integration Test ===");

try
{
    var t0 = typeof(MimeTypeProcessor);
    Console.WriteLine($"[PASS] MimeTypeProcessor 存在");
    var t1 = typeof(MimeKitExtensions);
    Console.WriteLine($"[PASS] MimeKitExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}