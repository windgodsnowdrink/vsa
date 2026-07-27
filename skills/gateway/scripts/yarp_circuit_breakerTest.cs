#load "yarp_circuit_breaker.cs"

Console.WriteLine("=== yarp_circuit_breaker Test ===");

try
{
    var t0 = typeof(CircuitBreakerMiddleware);
    Console.WriteLine($"[PASS] CircuitBreakerMiddleware 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}