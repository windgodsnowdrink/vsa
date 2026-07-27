#load "loic_integration.cs"

Console.WriteLine("=== loic_integration Test ===");

try
{
    var t0 = typeof(SerialConfig);
    Console.WriteLine($"[PASS] SerialConfig 存在");
    var t1 = typeof(SerialChannel);
    Console.WriteLine($"[PASS] SerialChannel 存在");
    var t2 = typeof(LoicProtocol);
    Console.WriteLine($"[PASS] LoicProtocol 存在");
    var t3 = typeof(LoicExtensions);
    Console.WriteLine($"[PASS] LoicExtensions 存在");
    var t4 = typeof(LoicBackgroundService);
    Console.WriteLine($"[PASS] LoicBackgroundService 存在");
    var t5 = typeof(MainWindow);
    Console.WriteLine($"[PASS] MainWindow 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}