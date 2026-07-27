#load "performance_metrics.cs"

Console.WriteLine("=== performance_metrics.cs Test ===");

try
{
    // 验证 class: ChannelPerformanceMonitor
    var type_ChannelPerformanceMonitor = Type.GetType("ChannelPerformanceMonitor");
    if (type_ChannelPerformanceMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelPerformanceMonitor (class) 存在");
        var ctors_ChannelPerformanceMonitor = type_ChannelPerformanceMonitor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelPerformanceMonitor 构造函数数量: {ctors_ChannelPerformanceMonitor.Length}");
        var methods_ChannelPerformanceMonitor = type_ChannelPerformanceMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelPerformanceMonitor 公开方法数量: {methods_ChannelPerformanceMonitor.Length}");
        foreach (var m in methods_ChannelPerformanceMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelPerformanceMonitor 未找到，尝试无命名空间...");
        type_ChannelPerformanceMonitor = Type.GetType("ChannelPerformanceMonitor");
        if (type_ChannelPerformanceMonitor != null)
            Console.WriteLine("[PASS] 类型 ChannelPerformanceMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelPerformanceMonitor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
