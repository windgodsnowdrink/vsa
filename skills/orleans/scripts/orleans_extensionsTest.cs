#load "orleans_extensions.cs"

Console.WriteLine("=== orleans_extensions Test ===");

try
{
    var t0 = typeof(OrleansOptions);
    Console.WriteLine($"[PASS] OrleansOptions 存在");
    var t1 = typeof(OrleansConfiguration);
    Console.WriteLine($"[PASS] OrleansConfiguration 存在");
    var t2 = typeof(OrleansClusterNode);
    Console.WriteLine($"[PASS] OrleansClusterNode 存在");
    var t3 = typeof(OrleansClusterStatus);
    Console.WriteLine($"[PASS] OrleansClusterStatus 存在");
    var t4 = typeof(OrleansService);
    Console.WriteLine($"[PASS] OrleansService 存在");
    var t5 = typeof(OrleansEventSourcingService);
    Console.WriteLine($"[PASS] OrleansEventSourcingService 存在");
    var t6 = typeof(OrleansStateMachineService);
    Console.WriteLine($"[PASS] OrleansStateMachineService 存在");
    var t7 = typeof(OrleansClusterService);
    Console.WriteLine($"[PASS] OrleansClusterService 存在");
    var t8 = typeof(OrleansConfigurationService);
    Console.WriteLine($"[PASS] OrleansConfigurationService 存在");
    var t9 = typeof(OrleansDeploymentService);
    Console.WriteLine($"[PASS] OrleansDeploymentService 存在");
    var t10 = typeof(OrleansServiceCollectionExtensions);
    Console.WriteLine($"[PASS] OrleansServiceCollectionExtensions 存在");
    var t11 = typeof(IOrleansService);
    Console.WriteLine($"[PASS] IOrleansService 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(IOrleansEventSourcingService);
    Console.WriteLine($"[PASS] IOrleansEventSourcingService 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(IOrleansStateMachineService);
    Console.WriteLine($"[PASS] IOrleansStateMachineService 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(IOrleansClusterService);
    Console.WriteLine($"[PASS] IOrleansClusterService 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(IOrleansConfigurationService);
    Console.WriteLine($"[PASS] IOrleansConfigurationService 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(IOrleansDeploymentService);
    Console.WriteLine($"[PASS] IOrleansDeploymentService 接口存在 (IsInterface: {t16.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}