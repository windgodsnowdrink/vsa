#load "refit_integration.cs"

Console.WriteLine("=== refit_integration Test ===");

try
{
    var t0 = typeof(ChannelApiRequestProcessor);
    Console.WriteLine($"[PASS] ChannelApiRequestProcessor 存在");
    var t1 = typeof(IMyApiService);
    Console.WriteLine($"[PASS] IMyApiService 接口存在 (IsInterface: {t1.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}