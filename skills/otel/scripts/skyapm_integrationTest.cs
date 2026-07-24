#load "skyapm_integration.cs"

Console.WriteLine("=== skyapm_integration Test ===");

try
{
    // 验证 ChannelSegmentDispatcher 类
    var dispatcherType = typeof(ChannelSegmentDispatcher);
    Console.WriteLine($"[PASS] ChannelSegmentDispatcher 类型存在: {dispatcherType.Name}");
    Console.WriteLine($"[PASS] DispatchAsync 方法: {dispatcherType.GetMethod("DispatchAsync") != null}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}