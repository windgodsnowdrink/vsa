#load "kiota_cache.cs"

Console.WriteLine("=== kiota_cache Test ===");

try
{
    var channelCacheHandlerType = Type.GetType("ChannelCacheHandler");
    Console.WriteLine(channelCacheHandlerType != null ? "[PASS] ChannelCacheHandler 类型存在" : "[FAIL] ChannelCacheHandler 类型未找到");

    if (channelCacheHandlerType != null)
    {
        Console.WriteLine(channelCacheHandlerType.GetMethod("Handle") != null ? "[PASS] ChannelCacheHandler.Handle 方法存在" : "[FAIL] ChannelCacheHandler.Handle 方法未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}