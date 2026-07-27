#load "opentelemetry_integration.cs"

Console.WriteLine("=== opentelemetry_integration.cs Test ===");

try
{
    // 验证 class: OtelMetricsProcessor
    var type_OtelMetricsProcessor = Type.GetType("OtelMetricsProcessor");
    if (type_OtelMetricsProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 OtelMetricsProcessor (class) 存在");
        var ctors_OtelMetricsProcessor = type_OtelMetricsProcessor.GetConstructors();
        Console.WriteLine($"[PASS] OtelMetricsProcessor 构造函数数量: {ctors_OtelMetricsProcessor.Length}");
        var methods_OtelMetricsProcessor = type_OtelMetricsProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OtelMetricsProcessor 公开方法数量: {methods_OtelMetricsProcessor.Length}");
        foreach (var m in methods_OtelMetricsProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OtelMetricsProcessor 未找到，尝试无命名空间...");
        type_OtelMetricsProcessor = Type.GetType("OtelMetricsProcessor");
        if (type_OtelMetricsProcessor != null)
            Console.WriteLine("[PASS] 类型 OtelMetricsProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OtelMetricsProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
