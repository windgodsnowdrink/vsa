#load "http_resilience_integration.cs"

Console.WriteLine("=== http_resilience_integration Test ===");

try
{
    var t0 = typeof(HttpClientHealthCheck);
    Console.WriteLine($"[PASS] HttpClientHealthCheck 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}