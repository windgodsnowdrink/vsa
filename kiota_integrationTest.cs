#load "kiota_integration.cs"

Console.WriteLine("=== kiota_integration Test ===");

try
{
    var t0 = typeof(ChannelKiotaCodeGenerator);
    Console.WriteLine($"[PASS] ChannelKiotaCodeGenerator 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}