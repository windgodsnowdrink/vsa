#load "xunit_service_testing.cs"

Console.WriteLine("=== xunit_service_testing Test ===");

try
{
    var t0 = typeof(UserService);
    Console.WriteLine($"[PASS] UserService 存在");
    var t1 = typeof(UserServiceTests);
    Console.WriteLine($"[PASS] UserServiceTests 存在");
    var t2 = typeof(TestCollectionDefinition);
    Console.WriteLine($"[PASS] TestCollectionDefinition 存在");
    var t3 = typeof(ServiceTestCollection);
    Console.WriteLine($"[PASS] ServiceTestCollection 存在");
    var t4 = typeof(TestFixture);
    Console.WriteLine($"[PASS] TestFixture 存在");
    var t5 = typeof(IUserService);
    Console.WriteLine($"[PASS] IUserService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(User);
    Console.WriteLine($"[PASS] User record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}