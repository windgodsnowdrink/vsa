#load "zeromq_demo.cs"

Console.WriteLine("=== zeromq_demo Test ===");

try
{
    var t0 = typeof(ZeroMqMessageProcessor);
    Console.WriteLine($"[PASS] ZeroMqMessageProcessor 存在");
    var t1 = typeof(ZeroMqMultiProtocolSupport);
    Console.WriteLine($"[PASS] ZeroMqMultiProtocolSupport 存在");
    var t2 = typeof(ZeroMqMessageSecurity);
    Console.WriteLine($"[PASS] ZeroMqMessageSecurity 存在");
    var t3 = typeof(ZeroMqClusterManager);
    Console.WriteLine($"[PASS] ZeroMqClusterManager 存在");
    var t4 = typeof(ZeroMqMetrics);
    Console.WriteLine($"[PASS] ZeroMqMetrics 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}