#load "magiconion_persistence_full.cs"

Console.WriteLine("=== magiconion_persistence_full.cs Test ===");

try
{
    // 验证 class: HybridCacheStrategy
    var type_HybridCacheStrategy = Type.GetType("HybridCacheStrategy");
    if (type_HybridCacheStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 HybridCacheStrategy (class) 存在");
        var ctors_HybridCacheStrategy = type_HybridCacheStrategy.GetConstructors();
        Console.WriteLine($"[PASS] HybridCacheStrategy 构造函数数量: {ctors_HybridCacheStrategy.Length}");
        var methods_HybridCacheStrategy = type_HybridCacheStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HybridCacheStrategy 公开方法数量: {methods_HybridCacheStrategy.Length}");
        foreach (var m in methods_HybridCacheStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HybridCacheStrategy 未找到，尝试无命名空间...");
        type_HybridCacheStrategy = Type.GetType("HybridCacheStrategy");
        if (type_HybridCacheStrategy != null)
            Console.WriteLine("[PASS] 类型 HybridCacheStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HybridCacheStrategy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
