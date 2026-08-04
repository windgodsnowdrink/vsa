#load "restsharp_contract.cs"

Console.WriteLine("=== restsharp_contract Test ===");

try
{
    var t0 = typeof(IUserService);
    Console.WriteLine($"[PASS] IUserService 接口存在 (IsInterface: {t0.IsInterface})");
    var t1 = typeof(User);
    Console.WriteLine($"[PASS] User record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}