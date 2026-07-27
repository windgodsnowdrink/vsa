#load "hybrid_serializer.cs"

Console.WriteLine("=== hybrid_serializer.cs Test ===");

try
{
    // 验证 class: HybridSerializer
    var type_HybridSerializer = Type.GetType("HybridSerializer");
    if (type_HybridSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 HybridSerializer (class) 存在");
        var ctors_HybridSerializer = type_HybridSerializer.GetConstructors();
        Console.WriteLine($"[PASS] HybridSerializer 构造函数数量: {ctors_HybridSerializer.Length}");
        var methods_HybridSerializer = type_HybridSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HybridSerializer 公开方法数量: {methods_HybridSerializer.Length}");
        foreach (var m in methods_HybridSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HybridSerializer 未找到，尝试无命名空间...");
        type_HybridSerializer = Type.GetType("HybridSerializer");
        if (type_HybridSerializer != null)
            Console.WriteLine("[PASS] 类型 HybridSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HybridSerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
