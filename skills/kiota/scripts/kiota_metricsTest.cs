#load "kiota_metrics.cs"

Console.WriteLine("=== kiota_metrics Test ===");

try
{
    var channelCodeGenMonitorType = Type.GetType("ChannelCodeGenMonitor");
    Console.WriteLine(channelCodeGenMonitorType != null ? "[PASS] ChannelCodeGenMonitor 类型存在" : "[FAIL] ChannelCodeGenMonitor 类型未找到");

    if (channelCodeGenMonitorType != null)
    {
        Console.WriteLine(channelCodeGenMonitorType.GetMethod("Record") != null ? "[PASS] ChannelCodeGenMonitor.Record 方法存在" : "[FAIL] ChannelCodeGenMonitor.Record 方法未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}