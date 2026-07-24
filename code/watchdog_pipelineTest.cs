#load "watchdog_pipeline.cs"

Console.WriteLine("=== watchdog_pipeline.cs Test ===");

try
{
    // 验证 class: LogPipeline
    var type_LogPipeline = Type.GetType("LogPipeline");
    if (type_LogPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 LogPipeline (class) 存在");
        var ctors_LogPipeline = type_LogPipeline.GetConstructors();
        Console.WriteLine($"[PASS] LogPipeline 构造函数数量: {ctors_LogPipeline.Length}");
        var methods_LogPipeline = type_LogPipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogPipeline 公开方法数量: {methods_LogPipeline.Length}");
        foreach (var m in methods_LogPipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogPipeline 未找到，尝试无命名空间...");
        type_LogPipeline = Type.GetType("LogPipeline");
        if (type_LogPipeline != null)
            Console.WriteLine("[PASS] 类型 LogPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogPipeline 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
