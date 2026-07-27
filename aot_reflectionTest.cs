#load "aot_reflection.cs"

Console.WriteLine("=== aot_reflection Test ===");

try
{
    var t0 = typeof(AotReflectionContext);
    Console.WriteLine($"[PASS] AotReflectionContext 存在");
    var t1 = typeof(AotReflectionService);
    Console.WriteLine($"[PASS] AotReflectionService 存在");
    var t2 = typeof(IAotReflectionMarker);
    Console.WriteLine($"[PASS] IAotReflectionMarker 接口存在 (IsInterface: {t2.IsInterface})");
    var t3 = typeof(ReflectionData);
    Console.WriteLine($"[PASS] ReflectionData record 存在");
    var t4 = typeof(ReflectionRequest);
    Console.WriteLine($"[PASS] ReflectionRequest record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}