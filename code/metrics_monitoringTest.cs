#load "metrics_monitoring.cs"

Console.WriteLine("=== metrics_monitoring.cs Test ===");

try
{
    // 验证 class: CarterMetrics
    var type_CarterMetrics = Type.GetType("CarterMetrics");
    if (type_CarterMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 CarterMetrics (class) 存在");
        var ctors_CarterMetrics = type_CarterMetrics.GetConstructors();
        Console.WriteLine($"[PASS] CarterMetrics 构造函数数量: {ctors_CarterMetrics.Length}");
        var methods_CarterMetrics = type_CarterMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CarterMetrics 公开方法数量: {methods_CarterMetrics.Length}");
        foreach (var m in methods_CarterMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CarterMetrics 未找到，尝试无命名空间...");
        type_CarterMetrics = Type.GetType("CarterMetrics");
        if (type_CarterMetrics != null)
            Console.WriteLine("[PASS] 类型 CarterMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CarterMetrics 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
