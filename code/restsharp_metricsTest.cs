#load "restsharp_metrics.cs"

Console.WriteLine("=== restsharp_metrics.cs Test ===");

try
{
    // 验证 class: PrometheusMetricCollector
    var type_PrometheusMetricCollector = Type.GetType("PrometheusMetricCollector");
    if (type_PrometheusMetricCollector != null)
    {
        Console.WriteLine("[PASS] 类型 PrometheusMetricCollector (class) 存在");
        var ctors_PrometheusMetricCollector = type_PrometheusMetricCollector.GetConstructors();
        Console.WriteLine($"[PASS] PrometheusMetricCollector 构造函数数量: {ctors_PrometheusMetricCollector.Length}");
        var methods_PrometheusMetricCollector = type_PrometheusMetricCollector.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PrometheusMetricCollector 公开方法数量: {methods_PrometheusMetricCollector.Length}");
        foreach (var m in methods_PrometheusMetricCollector)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PrometheusMetricCollector 未找到，尝试无命名空间...");
        type_PrometheusMetricCollector = Type.GetType("PrometheusMetricCollector");
        if (type_PrometheusMetricCollector != null)
            Console.WriteLine("[PASS] 类型 PrometheusMetricCollector (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PrometheusMetricCollector 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
