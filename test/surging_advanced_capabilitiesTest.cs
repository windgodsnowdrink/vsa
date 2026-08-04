#load "surging_advanced_capabilities.cs"

Console.WriteLine("=== surging_advanced_capabilities Test ===");

try
{
    var t0 = typeof(ServiceGovernanceExtensions);
    Console.WriteLine($"[PASS] ServiceGovernanceExtensions 存在");
    var t1 = typeof(DistributedTracingExtensions);
    Console.WriteLine($"[PASS] DistributedTracingExtensions 存在");
    var t2 = typeof(MultiProtocolExtensions);
    Console.WriteLine($"[PASS] MultiProtocolExtensions 存在");
    var t3 = typeof(Startup);
    Console.WriteLine($"[PASS] Startup 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}