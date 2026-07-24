#load "srs_ssr_integration.cs"

Console.WriteLine("=== srs_ssr_integration Test ===");

try
{
    var t0 = typeof(SrsStreamProcessor);
    Console.WriteLine($"[PASS] SrsStreamProcessor 存在");
    var t1 = typeof(SrsServiceExtensions);
    Console.WriteLine($"[PASS] SrsServiceExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}