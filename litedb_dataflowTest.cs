#load "litedb_dataflow.cs"

Console.WriteLine("=== litedb_dataflow Test ===");

try
{
    var t0 = typeof(DataflowEventProcessor);
    Console.WriteLine($"[PASS] DataflowEventProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}