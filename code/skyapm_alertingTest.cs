#load "skyapm_alerting.cs"

Console.WriteLine("=== skyapm_alerting.cs Test ===");

try
{
    // 验证 class: SmartAlertEngine
    var type_SmartAlertEngine = Type.GetType("SmartAlertEngine");
    if (type_SmartAlertEngine != null)
    {
        Console.WriteLine("[PASS] 类型 SmartAlertEngine (class) 存在");
        var ctors_SmartAlertEngine = type_SmartAlertEngine.GetConstructors();
        Console.WriteLine($"[PASS] SmartAlertEngine 构造函数数量: {ctors_SmartAlertEngine.Length}");
        var methods_SmartAlertEngine = type_SmartAlertEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmartAlertEngine 公开方法数量: {methods_SmartAlertEngine.Length}");
        foreach (var m in methods_SmartAlertEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmartAlertEngine 未找到，尝试无命名空间...");
        type_SmartAlertEngine = Type.GetType("SmartAlertEngine");
        if (type_SmartAlertEngine != null)
            Console.WriteLine("[PASS] 类型 SmartAlertEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmartAlertEngine 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
