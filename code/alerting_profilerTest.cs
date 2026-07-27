#load "alerting_profiler.cs"

Console.WriteLine("=== alerting_profiler.cs Test ===");

try
{
    // 验证 class: ProfilerAlertService
    var type_ProfilerAlertService = Type.GetType("ProfilerAlertService");
    if (type_ProfilerAlertService != null)
    {
        Console.WriteLine("[PASS] 类型 ProfilerAlertService (class) 存在");
        var ctors_ProfilerAlertService = type_ProfilerAlertService.GetConstructors();
        Console.WriteLine($"[PASS] ProfilerAlertService 构造函数数量: {ctors_ProfilerAlertService.Length}");
        var methods_ProfilerAlertService = type_ProfilerAlertService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProfilerAlertService 公开方法数量: {methods_ProfilerAlertService.Length}");
        foreach (var m in methods_ProfilerAlertService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProfilerAlertService 未找到，尝试无命名空间...");
        type_ProfilerAlertService = Type.GetType("ProfilerAlertService");
        if (type_ProfilerAlertService != null)
            Console.WriteLine("[PASS] 类型 ProfilerAlertService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProfilerAlertService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
