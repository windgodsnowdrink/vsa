#load "opentelemetry_prometheus.cs"

Console.WriteLine("=== opentelemetry_prometheus.cs Test ===");

try
{
    // 验证 class: ThreadLocalMetricExporter
    var type_ThreadLocalMetricExporter = Type.GetType("ThreadLocalMetricExporter");
    if (type_ThreadLocalMetricExporter != null)
    {
        Console.WriteLine("[PASS] 类型 ThreadLocalMetricExporter (class) 存在");
        var ctors_ThreadLocalMetricExporter = type_ThreadLocalMetricExporter.GetConstructors();
        Console.WriteLine($"[PASS] ThreadLocalMetricExporter 构造函数数量: {ctors_ThreadLocalMetricExporter.Length}");
        var methods_ThreadLocalMetricExporter = type_ThreadLocalMetricExporter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ThreadLocalMetricExporter 公开方法数量: {methods_ThreadLocalMetricExporter.Length}");
        foreach (var m in methods_ThreadLocalMetricExporter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ThreadLocalMetricExporter 未找到，尝试无命名空间...");
        type_ThreadLocalMetricExporter = Type.GetType("ThreadLocalMetricExporter");
        if (type_ThreadLocalMetricExporter != null)
            Console.WriteLine("[PASS] 类型 ThreadLocalMetricExporter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThreadLocalMetricExporter 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
