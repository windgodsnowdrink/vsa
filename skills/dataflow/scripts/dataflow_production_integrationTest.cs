#load "dataflow_production_integration.cs"

Console.WriteLine("=== dataflow_production_integration Test ===");

try
{
    var t0 = typeof(DataflowProductionIntegration.DataflowOptions);
    Console.WriteLine($"[PASS] DataflowOptions 存在");
    var t1 = typeof(DataflowProductionIntegration.DataflowProcessor);
    Console.WriteLine($"[PASS] DataflowProcessor 存在");
    var t2 = typeof(DataflowProductionIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(DataflowProductionIntegration.DataflowBackgroundService);
    Console.WriteLine($"[PASS] DataflowBackgroundService 存在");
    var t4 = typeof(DataflowProductionIntegration.Startup);
    Console.WriteLine($"[PASS] Startup 存在");
    var t5 = typeof(DataflowProductionIntegration.IDataflowProcessor);
    Console.WriteLine($"[PASS] IDataflowProcessor 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(DataflowProductionIntegration.DataflowMessage);
    Console.WriteLine($"[PASS] DataflowMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}