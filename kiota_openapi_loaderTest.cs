#load "kiota_openapi_loader.cs"

Console.WriteLine("=== kiota_openapi_loader Test ===");

try
{
    var t0 = typeof(ChannelOpenApiLoader);
    Console.WriteLine($"[PASS] ChannelOpenApiLoader 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}