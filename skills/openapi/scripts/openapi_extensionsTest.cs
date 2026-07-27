#load "openapi_extensions.cs"

Console.WriteLine("=== openapi_extensions Test ===");

try
{
    var openApiOptionsType = Type.GetType("OpenAPIOptions");
    Console.WriteLine(openApiOptionsType != null ? "[PASS] OpenAPIOptions 类型存在" : "[FAIL] OpenAPIOptions 类型未找到");

    if (openApiOptionsType != null)
    {
        Console.WriteLine(openApiOptionsType.GetProperty("Enabled") != null ? "[PASS] OpenAPIOptions.Enabled 属性存在" : "[FAIL] OpenAPIOptions.Enabled 属性未找到");
        Console.WriteLine(openApiOptionsType.GetProperty("ApiTitle") != null ? "[PASS] OpenAPIOptions.ApiTitle 属性存在" : "[FAIL] OpenAPIOptions.ApiTitle 属性未找到");
        Console.WriteLine(openApiOptionsType.GetProperty("ApiVersion") != null ? "[PASS] OpenAPIOptions.ApiVersion 属性存在" : "[FAIL] OpenAPIOptions.ApiVersion 属性未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}