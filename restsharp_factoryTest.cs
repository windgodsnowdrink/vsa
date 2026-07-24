#load "restsharp_factory.cs"

Console.WriteLine("=== restsharp_factory Test ===");

try
{
    var t0 = typeof(ChannelRestClientFactory);
    Console.WriteLine($"[PASS] ChannelRestClientFactory 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}