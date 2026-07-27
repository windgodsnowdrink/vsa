#load "avalonia_integration.cs"

Console.WriteLine("=== avalonia_integration Test ===");

try
{
    var t0 = typeof(AvaloniaIntegration.AvaloniaOptions);
    Console.WriteLine($"[PASS] AvaloniaOptions 存在");
    var t1 = typeof(AvaloniaIntegration.AvaloniaService);
    Console.WriteLine($"[PASS] AvaloniaService 存在");
    var t2 = typeof(AvaloniaIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(AvaloniaIntegration.App);
    Console.WriteLine($"[PASS] App 存在");
    var t4 = typeof(AvaloniaIntegration.MainViewModel);
    Console.WriteLine($"[PASS] MainViewModel 存在");
    var t5 = typeof(AvaloniaIntegration.MainWindow);
    Console.WriteLine($"[PASS] MainWindow 存在");
    var t6 = typeof(AvaloniaIntegration.IAvaloniaService);
    Console.WriteLine($"[PASS] IAvaloniaService 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}