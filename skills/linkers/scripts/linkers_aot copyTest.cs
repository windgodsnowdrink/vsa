#load "linkers_aot copy.cs"

Console.WriteLine("=== linkers_aot copy Test ===");

try
{
    var t0 = typeof(LinkersAot.LinkersService);
    Console.WriteLine($"[PASS] LinkersService 存在");
    var t1 = typeof(LinkersAot.NativeMethods);
    Console.WriteLine($"[PASS] NativeMethods 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}