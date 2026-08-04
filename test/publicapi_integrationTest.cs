#load "publicapi_integration.cs"

Console.WriteLine("=== publicapi_integration Test ===");

try
{
    var t0 = typeof(PublicApiOptions);
    Console.WriteLine($"[PASS] PublicApiOptions 存在");
    var t1 = typeof(PublicApiClient);
    Console.WriteLine($"[PASS] PublicApiClient 存在");
    var t2 = typeof(PublicApiJsonContext);
    Console.WriteLine($"[PASS] PublicApiJsonContext 存在");
    var t3 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}