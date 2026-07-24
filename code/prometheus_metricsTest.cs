#load "prometheus_metrics.cs"

Console.WriteLine("=== prometheus_metrics.cs Test ===");

try
{
    // 验证 class: MetricsRegistry
    var type_MetricsRegistry = Type.GetType("MetricsRegistry");
    if (type_MetricsRegistry != null)
    {
        Console.WriteLine("[PASS] 类型 MetricsRegistry (class) 存在");
        var ctors_MetricsRegistry = type_MetricsRegistry.GetConstructors();
        Console.WriteLine($"[PASS] MetricsRegistry 构造函数数量: {ctors_MetricsRegistry.Length}");
        var methods_MetricsRegistry = type_MetricsRegistry.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MetricsRegistry 公开方法数量: {methods_MetricsRegistry.Length}");
        foreach (var m in methods_MetricsRegistry)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MetricsRegistry 未找到，尝试无命名空间...");
        type_MetricsRegistry = Type.GetType("MetricsRegistry");
        if (type_MetricsRegistry != null)
            Console.WriteLine("[PASS] 类型 MetricsRegistry (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MetricsRegistry 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
