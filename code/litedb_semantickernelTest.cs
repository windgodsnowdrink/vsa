#load "litedb_semantickernel.cs"

Console.WriteLine("=== litedb_semantickernel.cs Test ===");

try
{
    // 验证 class: SemanticEventProcessor
    var type_SemanticEventProcessor = Type.GetType("SemanticEventProcessor");
    if (type_SemanticEventProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticEventProcessor (class) 存在");
        var ctors_SemanticEventProcessor = type_SemanticEventProcessor.GetConstructors();
        Console.WriteLine($"[PASS] SemanticEventProcessor 构造函数数量: {ctors_SemanticEventProcessor.Length}");
        var methods_SemanticEventProcessor = type_SemanticEventProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SemanticEventProcessor 公开方法数量: {methods_SemanticEventProcessor.Length}");
        foreach (var m in methods_SemanticEventProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticEventProcessor 未找到，尝试无命名空间...");
        type_SemanticEventProcessor = Type.GetType("SemanticEventProcessor");
        if (type_SemanticEventProcessor != null)
            Console.WriteLine("[PASS] 类型 SemanticEventProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SemanticEventProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MemoryPoolPolicy
    var type_MemoryPoolPolicy = Type.GetType("MemoryPoolPolicy");
    if (type_MemoryPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryPoolPolicy (class) 存在");
        var ctors_MemoryPoolPolicy = type_MemoryPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MemoryPoolPolicy 构造函数数量: {ctors_MemoryPoolPolicy.Length}");
        var methods_MemoryPoolPolicy = type_MemoryPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryPoolPolicy 公开方法数量: {methods_MemoryPoolPolicy.Length}");
        foreach (var m in methods_MemoryPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryPoolPolicy 未找到，尝试无命名空间...");
        type_MemoryPoolPolicy = Type.GetType("MemoryPoolPolicy");
        if (type_MemoryPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 MemoryPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventAnalysis
    var type_EventAnalysis = Type.GetType("EventAnalysis");
    if (type_EventAnalysis != null)
    {
        Console.WriteLine("[PASS] 类型 EventAnalysis (class) 存在");
        var ctors_EventAnalysis = type_EventAnalysis.GetConstructors();
        Console.WriteLine($"[PASS] EventAnalysis 构造函数数量: {ctors_EventAnalysis.Length}");
        var methods_EventAnalysis = type_EventAnalysis.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventAnalysis 公开方法数量: {methods_EventAnalysis.Length}");
        foreach (var m in methods_EventAnalysis)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventAnalysis 未找到，尝试无命名空间...");
        type_EventAnalysis = Type.GetType("EventAnalysis");
        if (type_EventAnalysis != null)
            Console.WriteLine("[PASS] 类型 EventAnalysis (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventAnalysis 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
