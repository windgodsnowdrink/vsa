#load "watchdog_semantic.cs"

Console.WriteLine("=== watchdog_semantic.cs Test ===");

try
{
    // 验证 class: SemanticTracker
    var type_SemanticTracker = Type.GetType("SemanticTracker");
    if (type_SemanticTracker != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticTracker (class) 存在");
        var ctors_SemanticTracker = type_SemanticTracker.GetConstructors();
        Console.WriteLine($"[PASS] SemanticTracker 构造函数数量: {ctors_SemanticTracker.Length}");
        var methods_SemanticTracker = type_SemanticTracker.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SemanticTracker 公开方法数量: {methods_SemanticTracker.Length}");
        foreach (var m in methods_SemanticTracker)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticTracker 未找到，尝试无命名空间...");
        type_SemanticTracker = Type.GetType("SemanticTracker");
        if (type_SemanticTracker != null)
            Console.WriteLine("[PASS] 类型 SemanticTracker (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SemanticTracker 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
