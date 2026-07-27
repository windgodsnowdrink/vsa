#load "distributed_history_store.cs"

Console.WriteLine("=== distributed_history_store Test ===");

try
{
    var t0 = typeof(DistributedHistoryWriter);
    Console.WriteLine($"[PASS] DistributedHistoryWriter 存在");
    var t1 = typeof(HistoryEvent);
    Console.WriteLine($"[PASS] HistoryEvent record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}