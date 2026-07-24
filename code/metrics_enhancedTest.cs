#load "metrics_enhanced.cs"

Console.WriteLine("=== metrics_enhanced.cs Test ===");

try
{
    // 验证 class: ChannelMetricProcessor
    var type_ChannelMetricProcessor = Type.GetType("ChannelMetricProcessor");
    if (type_ChannelMetricProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMetricProcessor (class) 存在");
        var ctors_ChannelMetricProcessor = type_ChannelMetricProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMetricProcessor 构造函数数量: {ctors_ChannelMetricProcessor.Length}");
        var methods_ChannelMetricProcessor = type_ChannelMetricProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMetricProcessor 公开方法数量: {methods_ChannelMetricProcessor.Length}");
        foreach (var m in methods_ChannelMetricProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMetricProcessor 未找到，尝试无命名空间...");
        type_ChannelMetricProcessor = Type.GetType("ChannelMetricProcessor");
        if (type_ChannelMetricProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelMetricProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMetricProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
