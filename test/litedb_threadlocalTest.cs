#load "litedb_threadlocal.cs"

Console.WriteLine("=== litedb_threadlocal Test ===");

try
{
    var t0 = typeof(ThreadLocalMemoryPool);
    Console.WriteLine($"[PASS] ThreadLocalMemoryPool 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}