#load "kiota_openapi_loader.cs"

Console.WriteLine("=== kiota_openapi_loader Test ===");

try
{
    var channelOpenApiLoaderType = Type.GetType("ChannelOpenApiLoader");
    Console.WriteLine(channelOpenApiLoaderType != null ? "[PASS] ChannelOpenApiLoader 类型存在" : "[FAIL] ChannelOpenApiLoader 类型未找到");

    if (channelOpenApiLoaderType != null)
    {
        Console.WriteLine(channelOpenApiLoaderType.GetMethod("Load") != null ? "[PASS] ChannelOpenApiLoader.Load 方法存在" : "[FAIL] ChannelOpenApiLoader.Load 方法未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}