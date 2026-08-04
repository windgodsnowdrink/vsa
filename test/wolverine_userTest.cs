#load "wolverine_user.cs"

Console.WriteLine("=== wolverine_user Test ===");

try
{
    var t0 = typeof(InMemoryUserRepository);
    Console.WriteLine($"[PASS] InMemoryUserRepository 存在");
    var t1 = typeof(User);
    Console.WriteLine($"[PASS] User 存在");
    var t2 = typeof(UserService);
    Console.WriteLine($"[PASS] UserService 存在");
    var t3 = typeof(UserHandler);
    Console.WriteLine($"[PASS] UserHandler 存在");
    var t4 = typeof(IUserRepository);
    Console.WriteLine($"[PASS] IUserRepository 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(IUserService);
    Console.WriteLine($"[PASS] IUserService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(UserDto);
    Console.WriteLine($"[PASS] UserDto record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}