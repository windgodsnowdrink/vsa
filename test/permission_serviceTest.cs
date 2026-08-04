#load "permission_service.cs"

Console.WriteLine("=== permission_service Test ===");

try
{
    var t0 = typeof(PermissionService);
    Console.WriteLine($"[PASS] PermissionService 存在");
    var t1 = typeof(PermissionRequirement);
    Console.WriteLine($"[PASS] PermissionRequirement 存在");
    var t2 = typeof(PermissionHandler);
    Console.WriteLine($"[PASS] PermissionHandler 存在");
    var t3 = typeof(IPermissionService);
    Console.WriteLine($"[PASS] IPermissionService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}