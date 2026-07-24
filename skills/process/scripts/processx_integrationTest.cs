#load "processx_integration.cs"

Console.WriteLine("=== processx_integration Test ===");

try
{
    var t0 = typeof(ProcessXIntegration.ProcessManagementOptions);
    Console.WriteLine($"[PASS] ProcessManagementOptions 存在");
    var t1 = typeof(ProcessXIntegration.ProcessInfo);
    Console.WriteLine($"[PASS] ProcessInfo 存在");
    var t2 = typeof(ProcessXIntegration.ProcessPoolStatus);
    Console.WriteLine($"[PASS] ProcessPoolStatus 存在");
    var t3 = typeof(ProcessXIntegration.ProcessManager);
    Console.WriteLine($"[PASS] ProcessManager 存在");
    var t4 = typeof(ProcessXIntegration.ProcessPool);
    Console.WriteLine($"[PASS] ProcessPool 存在");
    var t5 = typeof(ProcessXIntegration.MockPooledProcess);
    Console.WriteLine($"[PASS] MockPooledProcess 存在");
    var t6 = typeof(ProcessXIntegration.ProcessManagementExtensions);
    Console.WriteLine($"[PASS] ProcessManagementExtensions 存在");
    var t7 = typeof(ProcessXIntegration.IProcessManager);
    Console.WriteLine($"[PASS] IProcessManager 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(ProcessXIntegration.IProcessPool);
    Console.WriteLine($"[PASS] IProcessPool 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(ProcessXIntegration.IPooledProcess);
    Console.WriteLine($"[PASS] IPooledProcess 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(ProcessXIntegration.ProcessState);
    Console.WriteLine($"[PASS] ProcessState enum 存在 (IsEnum: {t10.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}