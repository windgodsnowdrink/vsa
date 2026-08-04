#load "netcore_encrypt_advanced.cs"

Console.WriteLine("=== netcore_encrypt_advanced Test ===");

try
{
    var t0 = typeof(EncryptChannel);
    Console.WriteLine($"[PASS] EncryptChannel 存在");
    var t1 = typeof(ColdMemoryPool);
    Console.WriteLine($"[PASS] ColdMemoryPool 存在");
    var t2 = typeof(ColdMemoryOwner);
    Console.WriteLine($"[PASS] ColdMemoryOwner 存在");
    var t3 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t4 = typeof(LatencyToken);
    Console.WriteLine($"[PASS] LatencyToken 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}