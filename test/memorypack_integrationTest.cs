#load "memorypack_integration.cs"

Console.WriteLine("=== memorypack_integration Test ===");

try
{
    var t0 = typeof(MemoryPackIntegration);
    Console.WriteLine($"[PASS] MemoryPackIntegration 存在");
    var t1 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t2 = typeof(MemoryOwnerPooledPolicy);
    Console.WriteLine($"[PASS] MemoryOwnerPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}