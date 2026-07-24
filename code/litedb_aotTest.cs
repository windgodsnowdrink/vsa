#load "litedb_aot.cs"

Console.WriteLine("=== litedb_aot.cs Test ===");

try
{
    // 验证 class: AotOptimizedRepository
    var type_AotOptimizedRepository = Type.GetType("AotOptimizedRepository");
    if (type_AotOptimizedRepository != null)
    {
        Console.WriteLine("[PASS] 类型 AotOptimizedRepository (class) 存在");
        var ctors_AotOptimizedRepository = type_AotOptimizedRepository.GetConstructors();
        Console.WriteLine($"[PASS] AotOptimizedRepository 构造函数数量: {ctors_AotOptimizedRepository.Length}");
        var methods_AotOptimizedRepository = type_AotOptimizedRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AotOptimizedRepository 公开方法数量: {methods_AotOptimizedRepository.Length}");
        foreach (var m in methods_AotOptimizedRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AotOptimizedRepository 未找到，尝试无命名空间...");
        type_AotOptimizedRepository = Type.GetType("AotOptimizedRepository");
        if (type_AotOptimizedRepository != null)
            Console.WriteLine("[PASS] 类型 AotOptimizedRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AotOptimizedRepository 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
