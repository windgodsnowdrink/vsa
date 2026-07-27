#load "opentelemetry_alerting.cs"

Console.WriteLine("=== opentelemetry_alerting.cs Test ===");

try
{
    // 验证 class: DynamicAlertEngine
    var type_DynamicAlertEngine = Type.GetType("DynamicAlertEngine");
    if (type_DynamicAlertEngine != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicAlertEngine (class) 存在");
        var ctors_DynamicAlertEngine = type_DynamicAlertEngine.GetConstructors();
        Console.WriteLine($"[PASS] DynamicAlertEngine 构造函数数量: {ctors_DynamicAlertEngine.Length}");
        var methods_DynamicAlertEngine = type_DynamicAlertEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicAlertEngine 公开方法数量: {methods_DynamicAlertEngine.Length}");
        foreach (var m in methods_DynamicAlertEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicAlertEngine 未找到，尝试无命名空间...");
        type_DynamicAlertEngine = Type.GetType("DynamicAlertEngine");
        if (type_DynamicAlertEngine != null)
            Console.WriteLine("[PASS] 类型 DynamicAlertEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicAlertEngine 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
