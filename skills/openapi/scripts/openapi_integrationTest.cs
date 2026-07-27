#load "openapi_integration.cs"

Console.WriteLine("=== openapi_integration Test ===");

try
{
    var openApiMetricsType = Type.GetType("OpenApiMetrics");
    Console.WriteLine(openApiMetricsType != null ? "[PASS] OpenApiMetrics 类型存在" : "[FAIL] OpenApiMetrics 类型未找到");

    var cachingSwaggerProviderType = Type.GetType("CachingSwaggerProvider");
    Console.WriteLine(cachingSwaggerProviderType != null ? "[PASS] CachingSwaggerProvider 类型存在" : "[FAIL] CachingSwaggerProvider 类型未找到");

    var openApiMetricsServiceType = Type.GetType("OpenApiMetricsService");
    Console.WriteLine(openApiMetricsServiceType != null ? "[PASS] OpenApiMetricsService 类型存在" : "[FAIL] OpenApiMetricsService 类型未找到");

    var openApiOptionsType = Type.GetType("OpenApiOptions");
    Console.WriteLine(openApiOptionsType != null ? "[PASS] OpenApiOptions 类型存在" : "[FAIL] OpenApiOptions 类型未找到");

    var openApiExtensionsType = Type.GetType("OpenApiExtensions");
    Console.WriteLine(openApiExtensionsType != null ? "[PASS] OpenApiExtensions 类型存在" : "[FAIL] OpenApiExtensions 类型未找到");

    if (openApiExtensionsType != null)
    {
        Console.WriteLine(openApiExtensionsType.GetMethod("AddOpenApi") != null ? "[PASS] OpenApiExtensions.AddOpenApi 方法存在" : "[FAIL] OpenApiExtensions.AddOpenApi 方法未找到");
        Console.WriteLine(openApiExtensionsType.GetMethod("UseOpenApi") != null ? "[PASS] OpenApiExtensions.UseOpenApi 方法存在" : "[FAIL] OpenApiExtensions.UseOpenApi 方法未找到");
    }

    if (openApiOptionsType != null)
    {
        Console.WriteLine(openApiOptionsType.GetProperty("EnableMetrics") != null ? "[PASS] OpenApiOptions.EnableMetrics 属性存在" : "[FAIL] OpenApiOptions.EnableMetrics 属性未找到");
        Console.WriteLine(openApiOptionsType.GetProperty("EnableCaching") != null ? "[PASS] OpenApiOptions.EnableCaching 属性存在" : "[FAIL] OpenApiOptions.EnableCaching 属性未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}