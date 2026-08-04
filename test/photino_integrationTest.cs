#load "photino_integration.cs"

Console.WriteLine("=== photino_integration Test ===");

try
{
    var t0 = typeof(PhotinoOptions);
    Console.WriteLine($"[PASS] PhotinoOptions 存在");
    var t1 = typeof(PhotinoService);
    Console.WriteLine($"[PASS] PhotinoService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(MainViewModel);
    Console.WriteLine($"[PASS] MainViewModel 存在");
    var t4 = typeof(LocalStorageContext);
    Console.WriteLine($"[PASS] LocalStorageContext 存在");
    var t5 = typeof(LocalStorageItem);
    Console.WriteLine($"[PASS] LocalStorageItem 存在");
    var t6 = typeof(IPhotinoService);
    Console.WriteLine($"[PASS] IPhotinoService 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}