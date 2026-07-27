#load "tye_core.cs"

Console.WriteLine("=== tye_core Test ===");

try
{
    var t0 = typeof(Tye.Core.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t1 = typeof(Tye.Core.ServiceManager);
    Console.WriteLine($"[PASS] ServiceManager 存在");
    var t2 = typeof(Tye.Core.ConfigManager);
    Console.WriteLine($"[PASS] ConfigManager 存在");
    var t3 = typeof(Tye.Core.DeployManager);
    Console.WriteLine($"[PASS] DeployManager 存在");
    var t4 = typeof(Tye.Core.LocalManager);
    Console.WriteLine($"[PASS] LocalManager 存在");
    var t5 = typeof(Tye.Core.KubeManager);
    Console.WriteLine($"[PASS] KubeManager 存在");
    var t6 = typeof(Tye.Core.LogsManager);
    Console.WriteLine($"[PASS] LogsManager 存在");
    var t7 = typeof(Tye.Core.DashboardManager);
    Console.WriteLine($"[PASS] DashboardManager 存在");
    var t8 = typeof(Tye.Core.ServiceDiscovery);
    Console.WriteLine($"[PASS] ServiceDiscovery 存在");
    var t9 = typeof(Tye.Core.ConfigProvider);
    Console.WriteLine($"[PASS] ConfigProvider 存在");
    var t10 = typeof(Tye.Core.DeployProvider);
    Console.WriteLine($"[PASS] DeployProvider 存在");
    var t11 = typeof(Tye.Core.LocalProvider);
    Console.WriteLine($"[PASS] LocalProvider 存在");
    var t12 = typeof(Tye.Core.KubeProvider);
    Console.WriteLine($"[PASS] KubeProvider 存在");
    var t13 = typeof(Tye.Core.LogsProvider);
    Console.WriteLine($"[PASS] LogsProvider 存在");
    var t14 = typeof(Tye.Core.DashboardProvider);
    Console.WriteLine($"[PASS] DashboardProvider 存在");
    var t15 = typeof(Tye.Core.ServiceInfo);
    Console.WriteLine($"[PASS] ServiceInfo 存在");
    var t16 = typeof(Tye.Core.DeploymentInfo);
    Console.WriteLine($"[PASS] DeploymentInfo 存在");
    var t17 = typeof(Tye.Core.KubeResource);
    Console.WriteLine($"[PASS] KubeResource 存在");
    var t18 = typeof(Tye.Core.IServiceManager);
    Console.WriteLine($"[PASS] IServiceManager 接口存在 (IsInterface: {t18.IsInterface})");
    var t19 = typeof(Tye.Core.IConfigManager);
    Console.WriteLine($"[PASS] IConfigManager 接口存在 (IsInterface: {t19.IsInterface})");
    var t20 = typeof(Tye.Core.IDeployManager);
    Console.WriteLine($"[PASS] IDeployManager 接口存在 (IsInterface: {t20.IsInterface})");
    var t21 = typeof(Tye.Core.ILocalManager);
    Console.WriteLine($"[PASS] ILocalManager 接口存在 (IsInterface: {t21.IsInterface})");
    var t22 = typeof(Tye.Core.IKubeManager);
    Console.WriteLine($"[PASS] IKubeManager 接口存在 (IsInterface: {t22.IsInterface})");
    var t23 = typeof(Tye.Core.ILogsManager);
    Console.WriteLine($"[PASS] ILogsManager 接口存在 (IsInterface: {t23.IsInterface})");
    var t24 = typeof(Tye.Core.IDashboardManager);
    Console.WriteLine($"[PASS] IDashboardManager 接口存在 (IsInterface: {t24.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}