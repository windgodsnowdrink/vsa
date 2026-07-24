#load "Todo.cs"

Console.WriteLine("=== Todo Test ===");

try
{
    var t0 = typeof(PathEntryPointExtensions);
    Console.WriteLine($"[PASS] PathEntryPointExtensions 存在");
    var t1 = typeof(AppContextExtensions);
    Console.WriteLine($"[PASS] AppContextExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}