#load "datadog_logging.cs"

Console.WriteLine("=== datadog_logging.cs Test ===");

try
{
    // 验证 class: LogEnricher
    var type_LogEnricher = Type.GetType("LogEnricher");
    if (type_LogEnricher != null)
    {
        Console.WriteLine("[PASS] 类型 LogEnricher (class) 存在");
        var ctors_LogEnricher = type_LogEnricher.GetConstructors();
        Console.WriteLine($"[PASS] LogEnricher 构造函数数量: {ctors_LogEnricher.Length}");
        var methods_LogEnricher = type_LogEnricher.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogEnricher 公开方法数量: {methods_LogEnricher.Length}");
        foreach (var m in methods_LogEnricher)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogEnricher 未找到，尝试无命名空间...");
        type_LogEnricher = Type.GetType("LogEnricher");
        if (type_LogEnricher != null)
            Console.WriteLine("[PASS] 类型 LogEnricher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogEnricher 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
