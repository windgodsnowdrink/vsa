#load "sshnet_blazor_integration.cs"

Console.WriteLine("=== sshnet_blazor_integration Test ===");

try
{
    var t0 = typeof(TerminalHub);
    Console.WriteLine($"[PASS] TerminalHub 存在");
    var t1 = typeof(TerminalComponent);
    Console.WriteLine($"[PASS] TerminalComponent 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}