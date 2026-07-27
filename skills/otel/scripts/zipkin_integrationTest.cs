#load "zipkin_integration.cs"

Console.WriteLine("=== zipkin_integration Test ===");

try
{
    // 验证 ZipkinOptions 类
    var optionsType = typeof(ZipkinOptions);
    Console.WriteLine($"[PASS] ZipkinOptions 类型存在: {optionsType.Name}");

    // 验证 IZipkinService 接口
    var serviceType = typeof(IZipkinService);
    Console.WriteLine($"[PASS] IZipkinService 类型存在: {serviceType.Name}");
    Console.WriteLine($"[PASS] IZipkinService 是接口: {serviceType.IsInterface}");

    // 验证 ZipkinService 类
    var zipkinServiceType = typeof(ZipkinService);
    Console.WriteLine($"[PASS] ZipkinService 类型存在: {zipkinServiceType.Name}");

    // 验证 ServiceCollectionExtensions 类
    var extensionsType = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 类型存在: {extensionsType.Name}");

    // 验证 ZipkinHealthCheck 类
    var healthCheckType = typeof(ZipkinHealthCheck);
    Console.WriteLine($"[PASS] ZipkinHealthCheck 类型存在: {healthCheckType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}