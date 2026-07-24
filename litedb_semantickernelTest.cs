#load "litedb_semantickernel.cs"

Console.WriteLine("=== litedb_semantickernel Test ===");

try
{
    var t0 = typeof(SemanticEventProcessor);
    Console.WriteLine($"[PASS] SemanticEventProcessor 存在");
    var t1 = typeof(MemoryPoolPolicy);
    Console.WriteLine($"[PASS] MemoryPoolPolicy 存在");
    var t2 = typeof(EventAnalysis);
    Console.WriteLine($"[PASS] EventAnalysis 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}