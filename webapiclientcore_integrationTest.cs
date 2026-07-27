#load "webapiclientcore_integration.cs"

Console.WriteLine("=== webapiclientcore_integration Test ===");

try
{
    var t0 = typeof(ApiRequestProcessor);
    Console.WriteLine($"[PASS] ApiRequestProcessor 存在");
    var t1 = typeof(ApiHealthCheck);
    Console.WriteLine($"[PASS] ApiHealthCheck 存在");
    var t2 = typeof(IMyApiService);
    Console.WriteLine($"[PASS] IMyApiService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}