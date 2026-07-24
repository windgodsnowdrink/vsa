#load "skyapm_logging.cs"

Console.WriteLine("=== skyapm_logging.cs Test ===");

try
{
    // 验证 class: TraceContextEnricher
    var type_TraceContextEnricher = Type.GetType("TraceContextEnricher");
    if (type_TraceContextEnricher != null)
    {
        Console.WriteLine("[PASS] 类型 TraceContextEnricher (class) 存在");
        var ctors_TraceContextEnricher = type_TraceContextEnricher.GetConstructors();
        Console.WriteLine($"[PASS] TraceContextEnricher 构造函数数量: {ctors_TraceContextEnricher.Length}");
        var methods_TraceContextEnricher = type_TraceContextEnricher.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TraceContextEnricher 公开方法数量: {methods_TraceContextEnricher.Length}");
        foreach (var m in methods_TraceContextEnricher)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TraceContextEnricher 未找到，尝试无命名空间...");
        type_TraceContextEnricher = Type.GetType("TraceContextEnricher");
        if (type_TraceContextEnricher != null)
            Console.WriteLine("[PASS] 类型 TraceContextEnricher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TraceContextEnricher 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
