#load "memorypack_demo.cs"

Console.WriteLine("=== memorypack_demo Test ===");

try
{
    var t0 = typeof(MemoryPackProcessor);
    Console.WriteLine($"[PASS] MemoryPackProcessor 存在");
    var t1 = typeof(TodoEvent);
    Console.WriteLine($"[PASS] TodoEvent 存在");
    var t2 = typeof(VersionTolerantModel);
    Console.WriteLine($"[PASS] VersionTolerantModel 存在");
    var t3 = typeof(CustomFormatterModel);
    Console.WriteLine($"[PASS] CustomFormatterModel 存在");
    var t4 = typeof(DateTimeOffsetFormatter);
    Console.WriteLine($"[PASS] DateTimeOffsetFormatter 存在");
    var t5 = typeof(MemoryPackMetrics);
    Console.WriteLine($"[PASS] MemoryPackMetrics 存在");
    var t6 = typeof(ITodoEvent);
    Console.WriteLine($"[PASS] ITodoEvent 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}