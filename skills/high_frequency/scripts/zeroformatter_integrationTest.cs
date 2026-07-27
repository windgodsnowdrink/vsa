#load "zeroformatter_integration.cs"

Console.WriteLine("=== zeroformatter_integration Test ===");

try
{
    var t0 = typeof(ZeroCopyProcessor);
    Console.WriteLine($"[PASS] ZeroCopyProcessor 存在");
    var t1 = typeof(ThreadLocalAllocator);
    Console.WriteLine($"[PASS] ThreadLocalAllocator 存在");
    var t2 = typeof(MessageEnvelope);
    Console.WriteLine($"[PASS] MessageEnvelope struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}