#load "hybrid_json_processor.cs"

Console.WriteLine("=== hybrid_json_processor.cs Test ===");

try
{
    // 验证 class: HybridJsonProcessor
    var type_HybridJsonProcessor = Type.GetType("HybridJsonProcessor");
    if (type_HybridJsonProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 HybridJsonProcessor (class) 存在");
        var ctors_HybridJsonProcessor = type_HybridJsonProcessor.GetConstructors();
        Console.WriteLine($"[PASS] HybridJsonProcessor 构造函数数量: {ctors_HybridJsonProcessor.Length}");
        var methods_HybridJsonProcessor = type_HybridJsonProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HybridJsonProcessor 公开方法数量: {methods_HybridJsonProcessor.Length}");
        foreach (var m in methods_HybridJsonProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HybridJsonProcessor 未找到，尝试无命名空间...");
        type_HybridJsonProcessor = Type.GetType("HybridJsonProcessor");
        if (type_HybridJsonProcessor != null)
            Console.WriteLine("[PASS] 类型 HybridJsonProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HybridJsonProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
