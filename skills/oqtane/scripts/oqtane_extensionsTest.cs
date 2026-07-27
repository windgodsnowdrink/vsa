#load "oqtane_extensions.cs"

Console.WriteLine("=== oqtane_extensions Test ===");

try
{
    var t0 = typeof(OqtaneOptions);
    Console.WriteLine($"[PASS] OqtaneOptions 存在");
    var t1 = typeof(OqtaneConfiguration);
    Console.WriteLine($"[PASS] OqtaneConfiguration 存在");
    var t2 = typeof(OqtaneModule);
    Console.WriteLine($"[PASS] OqtaneModule 存在");
    var t3 = typeof(OqtaneTheme);
    Console.WriteLine($"[PASS] OqtaneTheme 存在");
    var t4 = typeof(OqtaneUser);
    Console.WriteLine($"[PASS] OqtaneUser 存在");
    var t5 = typeof(OqtaneModuleService);
    Console.WriteLine($"[PASS] OqtaneModuleService 存在");
    var t6 = typeof(OqtaneThemeService);
    Console.WriteLine($"[PASS] OqtaneThemeService 存在");
    var t7 = typeof(OqtaneUserService);
    Console.WriteLine($"[PASS] OqtaneUserService 存在");
    var t8 = typeof(OqtaneConfigurationService);
    Console.WriteLine($"[PASS] OqtaneConfigurationService 存在");
    var t9 = typeof(OqtaneDeploymentService);
    Console.WriteLine($"[PASS] OqtaneDeploymentService 存在");
    var t10 = typeof(OqtaneService);
    Console.WriteLine($"[PASS] OqtaneService 存在");
    var t11 = typeof(OqtaneServiceCollectionExtensions);
    Console.WriteLine($"[PASS] OqtaneServiceCollectionExtensions 存在");
    var t12 = typeof(IOqtaneService);
    Console.WriteLine($"[PASS] IOqtaneService 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(IOqtaneModuleService);
    Console.WriteLine($"[PASS] IOqtaneModuleService 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(IOqtaneThemeService);
    Console.WriteLine($"[PASS] IOqtaneThemeService 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(IOqtaneUserService);
    Console.WriteLine($"[PASS] IOqtaneUserService 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(IOqtaneConfigurationService);
    Console.WriteLine($"[PASS] IOqtaneConfigurationService 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(IOqtaneDeploymentService);
    Console.WriteLine($"[PASS] IOqtaneDeploymentService 接口存在 (IsInterface: {t17.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}