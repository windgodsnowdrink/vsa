#load "canalsharp_integration.cs"

Console.WriteLine("=== canalsharp_integration Test ===");

try
{
    var t0 = typeof(CanalWorker);
    Console.WriteLine($"[PASS] CanalWorker 存在");
    var t1 = typeof(EntryProcessor);
    Console.WriteLine($"[PASS] EntryProcessor 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(CanalOptions);
    Console.WriteLine($"[PASS] CanalOptions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}