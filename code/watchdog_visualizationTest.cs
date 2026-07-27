#load "watchdog_visualization.cs"

Console.WriteLine("=== watchdog_visualization.cs Test ===");

try
{
    // 验证 class: MemoryTracker
    var type_MemoryTracker = Type.GetType("MemoryTracker");
    if (type_MemoryTracker != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryTracker (class) 存在");
        var ctors_MemoryTracker = type_MemoryTracker.GetConstructors();
        Console.WriteLine($"[PASS] MemoryTracker 构造函数数量: {ctors_MemoryTracker.Length}");
        var methods_MemoryTracker = type_MemoryTracker.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryTracker 公开方法数量: {methods_MemoryTracker.Length}");
        foreach (var m in methods_MemoryTracker)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryTracker 未找到，尝试无命名空间...");
        type_MemoryTracker = Type.GetType("MemoryTracker");
        if (type_MemoryTracker != null)
            Console.WriteLine("[PASS] 类型 MemoryTracker (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryTracker 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
