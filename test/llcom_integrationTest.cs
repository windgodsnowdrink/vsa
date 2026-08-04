#load "llcom_integration.cs"

Console.WriteLine("=== llcom_integration Test ===");

try
{
    var t0 = typeof(ZeroCopyCommunicationPipe);
    Console.WriteLine($"[PASS] ZeroCopyCommunicationPipe 存在");
    var t1 = typeof(LoicProtocolHandler);
    Console.WriteLine($"[PASS] LoicProtocolHandler 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(MainViewModel);
    Console.WriteLine($"[PASS] MainViewModel 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}