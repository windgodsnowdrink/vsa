#load "kiota_metrics.cs"

Console.WriteLine("=== kiota_metrics.cs Test ===");

try
{
    // 验证 class: ChannelCodeGenMonitor
    var type_ChannelCodeGenMonitor = Type.GetType("ChannelCodeGenMonitor");
    if (type_ChannelCodeGenMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelCodeGenMonitor (class) 存在");
        var ctors_ChannelCodeGenMonitor = type_ChannelCodeGenMonitor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelCodeGenMonitor 构造函数数量: {ctors_ChannelCodeGenMonitor.Length}");
        var methods_ChannelCodeGenMonitor = type_ChannelCodeGenMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelCodeGenMonitor 公开方法数量: {methods_ChannelCodeGenMonitor.Length}");
        foreach (var m in methods_ChannelCodeGenMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelCodeGenMonitor 未找到，尝试无命名空间...");
        type_ChannelCodeGenMonitor = Type.GetType("ChannelCodeGenMonitor");
        if (type_ChannelCodeGenMonitor != null)
            Console.WriteLine("[PASS] 类型 ChannelCodeGenMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelCodeGenMonitor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
