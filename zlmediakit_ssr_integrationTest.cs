#load "zlmediakit_ssr_integration.cs"

Console.WriteLine("=== zlmediakit_ssr_integration Test ===");

try
{
    var t0 = typeof(ZlmSsrProcessor);
    Console.WriteLine($"[PASS] ZlmSsrProcessor 存在");
    var t1 = typeof(ZlmServiceExtensions);
    Console.WriteLine($"[PASS] ZlmServiceExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}