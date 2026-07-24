#load "casbin_integration.cs"

Console.WriteLine("=== casbin_integration Test ===");

try
{
    var t0 = typeof(ApplicationUser);
    Console.WriteLine($"[PASS] ApplicationUser 存在");
    var t1 = typeof(ApplicationRole);
    Console.WriteLine($"[PASS] ApplicationRole 存在");
    var t2 = typeof(MenuPermission);
    Console.WriteLine($"[PASS] MenuPermission 存在");
    var t3 = typeof(RolePermission);
    Console.WriteLine($"[PASS] RolePermission 存在");
    var t4 = typeof(CasbinIdentityAdapter);
    Console.WriteLine($"[PASS] CasbinIdentityAdapter 存在");
    var t5 = typeof(CasbinContextPooledPolicy);
    Console.WriteLine($"[PASS] CasbinContextPooledPolicy 存在");
    var t6 = typeof(AuthRequest);
    Console.WriteLine($"[PASS] AuthRequest record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}