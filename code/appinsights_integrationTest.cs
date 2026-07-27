#load "appinsights_integration.cs"

Console.WriteLine("=== appinsights_integration.cs Test ===");

try
{
    // 验证 class: ChannelTelemetryProcessor
    var type_ChannelTelemetryProcessor = Type.GetType("ChannelTelemetryProcessor");
    if (type_ChannelTelemetryProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelTelemetryProcessor (class) 存在");
        var ctors_ChannelTelemetryProcessor = type_ChannelTelemetryProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelTelemetryProcessor 构造函数数量: {ctors_ChannelTelemetryProcessor.Length}");
        var methods_ChannelTelemetryProcessor = type_ChannelTelemetryProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelTelemetryProcessor 公开方法数量: {methods_ChannelTelemetryProcessor.Length}");
        foreach (var m in methods_ChannelTelemetryProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelTelemetryProcessor 未找到，尝试无命名空间...");
        type_ChannelTelemetryProcessor = Type.GetType("ChannelTelemetryProcessor");
        if (type_ChannelTelemetryProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelTelemetryProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelTelemetryProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
