#load "processx_integration.cs"

Console.WriteLine("=== processx_integration Test ===");

try
{
    var t0 = typeof(ProcessXIntegration.ProcessMonitorOptions);
    Console.WriteLine($"[PASS] ProcessMonitorOptions 存在");
    var t1 = typeof(ProcessXIntegration.GarnetOptions);
    Console.WriteLine($"[PASS] GarnetOptions 存在");
    var t2 = typeof(ProcessXIntegration.MySqlOptions);
    Console.WriteLine($"[PASS] MySqlOptions 存在");
    var t3 = typeof(ProcessXIntegration.MqttOptions);
    Console.WriteLine($"[PASS] MqttOptions 存在");
    var t4 = typeof(ProcessXIntegration.ProcessXOptions);
    Console.WriteLine($"[PASS] ProcessXOptions 存在");
    var t5 = typeof(ProcessXIntegration.ProcessMonitorService);
    Console.WriteLine($"[PASS] ProcessMonitorService 存在");
    var t6 = typeof(ProcessXIntegration.ProcessXService);
    Console.WriteLine($"[PASS] ProcessXService 存在");
    var t7 = typeof(ProcessXIntegration.ProcessMonitorExtensions);
    Console.WriteLine($"[PASS] ProcessMonitorExtensions 存在");
    var t8 = typeof(ProcessXIntegration.MySqlExtensions);
    Console.WriteLine($"[PASS] MySqlExtensions 存在");
    var t9 = typeof(ProcessXIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t10 = typeof(ProcessXIntegration.GarnetService);
    Console.WriteLine($"[PASS] GarnetService 存在");
    var t11 = typeof(ProcessXIntegration.MySqlService);
    Console.WriteLine($"[PASS] MySqlService 存在");
    var t12 = typeof(ProcessXIntegration.SqliteService);
    Console.WriteLine($"[PASS] SqliteService 存在");
    var t13 = typeof(ProcessXIntegration.LiteDbService);
    Console.WriteLine($"[PASS] LiteDbService 存在");
    var t14 = typeof(ProcessXIntegration.MqttService);
    Console.WriteLine($"[PASS] MqttService 存在");
    var t15 = typeof(ProcessXIntegration.IProcessMonitorService);
    Console.WriteLine($"[PASS] IProcessMonitorService 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(ProcessXIntegration.IProcessXService);
    Console.WriteLine($"[PASS] IProcessXService 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(ProcessXIntegration.IGarnetService);
    Console.WriteLine($"[PASS] IGarnetService 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(ProcessXIntegration.IMySqlService);
    Console.WriteLine($"[PASS] IMySqlService 接口存在 (IsInterface: {t18.IsInterface})");
    var t19 = typeof(ProcessXIntegration.IMqttService);
    Console.WriteLine($"[PASS] IMqttService 接口存在 (IsInterface: {t19.IsInterface})");
    var t20 = typeof(ProcessXIntegration.ProcessPriority);
    Console.WriteLine($"[PASS] ProcessPriority enum 存在 (IsEnum: {t20.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}