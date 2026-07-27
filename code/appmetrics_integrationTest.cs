#load "appmetrics_integration.cs"

Console.WriteLine("=== appmetrics_integration.cs Test ===");

try
{
    // 验证 class: AppMetricsService
    var type_AppMetricsService = Type.GetType("AppMetricsService");
    if (type_AppMetricsService != null)
    {
        Console.WriteLine("[PASS] 类型 AppMetricsService (class) 存在");
        var ctors_AppMetricsService = type_AppMetricsService.GetConstructors();
        Console.WriteLine($"[PASS] AppMetricsService 构造函数数量: {ctors_AppMetricsService.Length}");
        var methods_AppMetricsService = type_AppMetricsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AppMetricsService 公开方法数量: {methods_AppMetricsService.Length}");
        foreach (var m in methods_AppMetricsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AppMetricsService 未找到，尝试无命名空间...");
        type_AppMetricsService = Type.GetType("AppMetricsService");
        if (type_AppMetricsService != null)
            Console.WriteLine("[PASS] 类型 AppMetricsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AppMetricsService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
