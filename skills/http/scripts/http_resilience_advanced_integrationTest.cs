#load "http_resilience_advanced_integration.cs"

Console.WriteLine("=== http_resilience_advanced_integration Test ===");

try
{
    var t0 = typeof(ResilienceHealthCheck);
    Console.WriteLine($"[PASS] ResilienceHealthCheck 存在");
    var t1 = typeof(RequestBufferingFeature);
    Console.WriteLine($"[PASS] RequestBufferingFeature 存在");
    var t2 = typeof(HttpResilienceOptions);
    Console.WriteLine($"[PASS] HttpResilienceOptions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}