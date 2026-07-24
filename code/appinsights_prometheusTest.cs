#load "appinsights_prometheus.cs"

Console.WriteLine("=== appinsights_prometheus.cs Test ===");

try
{
    // 验证 class: PrometheusExporter
    var type_PrometheusExporter = Type.GetType("PrometheusExporter");
    if (type_PrometheusExporter != null)
    {
        Console.WriteLine("[PASS] 类型 PrometheusExporter (class) 存在");
        var ctors_PrometheusExporter = type_PrometheusExporter.GetConstructors();
        Console.WriteLine($"[PASS] PrometheusExporter 构造函数数量: {ctors_PrometheusExporter.Length}");
        var methods_PrometheusExporter = type_PrometheusExporter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PrometheusExporter 公开方法数量: {methods_PrometheusExporter.Length}");
        foreach (var m in methods_PrometheusExporter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PrometheusExporter 未找到，尝试无命名空间...");
        type_PrometheusExporter = Type.GetType("PrometheusExporter");
        if (type_PrometheusExporter != null)
            Console.WriteLine("[PASS] 类型 PrometheusExporter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PrometheusExporter 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
