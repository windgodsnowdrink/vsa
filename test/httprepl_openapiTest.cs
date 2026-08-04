#load "httprepl_openapi.cs"

Console.WriteLine("=== httprepl_openapi Test ===");

try
{
    var t0 = typeof(ChannelOpenApiProcessor);
    Console.WriteLine($"[PASS] ChannelOpenApiProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}