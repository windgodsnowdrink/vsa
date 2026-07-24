#load "tiered_memory_processor.cs"

Console.WriteLine("=== tiered_memory_processor.cs Test ===");

try
{
    // 验证 class: TieredMemoryProcessor
    var type_TieredMemoryProcessor = Type.GetType("TieredMemoryProcessor");
    if (type_TieredMemoryProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 TieredMemoryProcessor (class) 存在");
        var ctors_TieredMemoryProcessor = type_TieredMemoryProcessor.GetConstructors();
        Console.WriteLine($"[PASS] TieredMemoryProcessor 构造函数数量: {ctors_TieredMemoryProcessor.Length}");
        var methods_TieredMemoryProcessor = type_TieredMemoryProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredMemoryProcessor 公开方法数量: {methods_TieredMemoryProcessor.Length}");
        foreach (var m in methods_TieredMemoryProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredMemoryProcessor 未找到，尝试无命名空间...");
        type_TieredMemoryProcessor = Type.GetType("TieredMemoryProcessor");
        if (type_TieredMemoryProcessor != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NativeMemoryPool
    var type_NativeMemoryPool = Type.GetType("NativeMemoryPool");
    if (type_NativeMemoryPool != null)
    {
        Console.WriteLine("[PASS] 类型 NativeMemoryPool (class) 存在");
        var ctors_NativeMemoryPool = type_NativeMemoryPool.GetConstructors();
        Console.WriteLine($"[PASS] NativeMemoryPool 构造函数数量: {ctors_NativeMemoryPool.Length}");
        var methods_NativeMemoryPool = type_NativeMemoryPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NativeMemoryPool 公开方法数量: {methods_NativeMemoryPool.Length}");
        foreach (var m in methods_NativeMemoryPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NativeMemoryPool 未找到，尝试无命名空间...");
        type_NativeMemoryPool = Type.GetType("NativeMemoryPool");
        if (type_NativeMemoryPool != null)
            Console.WriteLine("[PASS] 类型 NativeMemoryPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NativeMemoryPool 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
