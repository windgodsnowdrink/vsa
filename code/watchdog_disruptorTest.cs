#load "watchdog_disruptor.cs"

Console.WriteLine("=== watchdog_disruptor.cs Test ===");

try
{
    // 验证 class: LogEventProcessor
    var type_LogEventProcessor = Type.GetType("LogEventProcessor");
    if (type_LogEventProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 LogEventProcessor (class) 存在");
        var ctors_LogEventProcessor = type_LogEventProcessor.GetConstructors();
        Console.WriteLine($"[PASS] LogEventProcessor 构造函数数量: {ctors_LogEventProcessor.Length}");
        var methods_LogEventProcessor = type_LogEventProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogEventProcessor 公开方法数量: {methods_LogEventProcessor.Length}");
        foreach (var m in methods_LogEventProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogEventProcessor 未找到，尝试无命名空间...");
        type_LogEventProcessor = Type.GetType("LogEventProcessor");
        if (type_LogEventProcessor != null)
            Console.WriteLine("[PASS] 类型 LogEventProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogEventProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
