#load "genvue_integration.cs"

Console.WriteLine("=== genvue_integration Test ===");

try
{
    var t0 = typeof(GenVueIntegration.GenVueOptions);
    Console.WriteLine($"[PASS] GenVueOptions 存在");
    var t1 = typeof(GenVueIntegration.GenVueService);
    Console.WriteLine($"[PASS] GenVueService 存在");
    var t2 = typeof(GenVueIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(GenVueIntegration.IGenVueService);
    Console.WriteLine($"[PASS] IGenVueService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}