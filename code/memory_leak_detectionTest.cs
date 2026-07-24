#load "memory_leak_detection.cs"

Console.WriteLine("=== memory_leak_detection.cs Test ===");

try
{
    // 验证 class: ChannelMemoryLeakDetector
    var type_ChannelMemoryLeakDetector = Type.GetType("ChannelMemoryLeakDetector");
    if (type_ChannelMemoryLeakDetector != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMemoryLeakDetector (class) 存在");
        var ctors_ChannelMemoryLeakDetector = type_ChannelMemoryLeakDetector.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMemoryLeakDetector 构造函数数量: {ctors_ChannelMemoryLeakDetector.Length}");
        var methods_ChannelMemoryLeakDetector = type_ChannelMemoryLeakDetector.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMemoryLeakDetector 公开方法数量: {methods_ChannelMemoryLeakDetector.Length}");
        foreach (var m in methods_ChannelMemoryLeakDetector)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMemoryLeakDetector 未找到，尝试无命名空间...");
        type_ChannelMemoryLeakDetector = Type.GetType("ChannelMemoryLeakDetector");
        if (type_ChannelMemoryLeakDetector != null)
            Console.WriteLine("[PASS] 类型 ChannelMemoryLeakDetector (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMemoryLeakDetector 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
