#load "opencs_version_integration.cs"

Console.WriteLine("=== opencs_version_integration Test ===");

try
{
    var t0 = typeof(ComputerVisionService);
    Console.WriteLine($"[PASS] ComputerVisionService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(MatPooledObjectPolicy);
    Console.WriteLine($"[PASS] MatPooledObjectPolicy 存在");
    var t3 = typeof(IComputerVisionService);
    Console.WriteLine($"[PASS] IComputerVisionService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}