#load "dotnetcore_extension_integration.cs"

Console.WriteLine("=== dotnetcore_extension_integration Test ===");

try
{
    var t0 = typeof(DotNetCoreExtensions.DotNetCoreExtensionOptions);
    Console.WriteLine($"[PASS] DotNetCoreExtensionOptions 存在");
    var t1 = typeof(DotNetCoreExtensions.DotNetCoreExtensionService);
    Console.WriteLine($"[PASS] DotNetCoreExtensionService 存在");
    var t2 = typeof(DotNetCoreExtensions.DotNetCoreExtensionServiceCollectionExtensions);
    Console.WriteLine($"[PASS] DotNetCoreExtensionServiceCollectionExtensions 存在");
    var t3 = typeof(DotNetCoreExtensions.ExampleUsage);
    Console.WriteLine($"[PASS] ExampleUsage 存在");
    var t4 = typeof(DotNetCoreExtensions.IDotNetCoreExtensionService);
    Console.WriteLine($"[PASS] IDotNetCoreExtensionService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}