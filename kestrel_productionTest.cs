#load "kestrel_production.cs"

Console.WriteLine("=== kestrel_production Test ===");

try
{
    var t0 = typeof(KestrelProductionServer);
    Console.WriteLine($"[PASS] KestrelProductionServer 存在");
    var t1 = typeof(MemoryPooledPolicy);
    Console.WriteLine($"[PASS] MemoryPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}