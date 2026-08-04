#load "supersocket_integration.cs"

Console.WriteLine("=== supersocket_integration Test ===");

try
{
    var t0 = typeof(SuperSocketOptions);
    Console.WriteLine($"[PASS] SuperSocketOptions 存在");
    var t1 = typeof(TieredMemoryConfig);
    Console.WriteLine($"[PASS] TieredMemoryConfig 存在");
    var t2 = typeof(SuperSocketService);
    Console.WriteLine($"[PASS] SuperSocketService 存在");
    var t3 = typeof(SuperSocketExtensions);
    Console.WriteLine($"[PASS] SuperSocketExtensions 存在");
    var t4 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t5 = typeof(TieredMemoryService);
    Console.WriteLine($"[PASS] TieredMemoryService 存在");
    var t6 = typeof(SizedMemoryPool);
    Console.WriteLine($"[PASS] SizedMemoryPool 存在");
    var t7 = typeof(ArrayMemoryOwner);
    Console.WriteLine($"[PASS] ArrayMemoryOwner 存在");
    var t8 = typeof(TieredMemoryPooledObjectPolicy);
    Console.WriteLine($"[PASS] TieredMemoryPooledObjectPolicy 存在");
    var t9 = typeof(TokenRingBuffer);
    Console.WriteLine($"[PASS] TokenRingBuffer 存在");
    var t10 = typeof(Utf8StringPackageInfo);
    Console.WriteLine($"[PASS] Utf8StringPackageInfo 存在");
    var t11 = typeof(SuperSocketExample);
    Console.WriteLine($"[PASS] SuperSocketExample 存在");
    var t12 = typeof(ISuperSocketService);
    Console.WriteLine($"[PASS] ISuperSocketService 接口存在 (IsInterface: {t12.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}