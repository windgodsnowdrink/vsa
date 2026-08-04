#load "hotchocolate_advanced.cs"

Console.WriteLine("=== hotchocolate_advanced Test ===");

try
{
    var t0 = typeof(BatchUserDataLoader);
    Console.WriteLine($"[PASS] BatchUserDataLoader 存在");
    var t1 = typeof(CacheMiddleware);
    Console.WriteLine($"[PASS] CacheMiddleware 存在");
    var t2 = typeof(AuthorizeDirectiveType);
    Console.WriteLine($"[PASS] AuthorizeDirectiveType 存在");
    var t3 = typeof(AuthorizeDirective);
    Console.WriteLine($"[PASS] AuthorizeDirective 存在");
    var t4 = typeof(User);
    Console.WriteLine($"[PASS] User 存在");
    var t5 = typeof(UserDbContext);
    Console.WriteLine($"[PASS] UserDbContext 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}