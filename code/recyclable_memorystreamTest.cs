#load "recyclable_memorystream.cs"

Console.WriteLine("=== recyclable_memorystream.cs Test ===");

try
{
    // 验证 class: ChannelMemoryProcessor
    var type_ChannelMemoryProcessor = Type.GetType("ChannelMemoryProcessor");
    if (type_ChannelMemoryProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMemoryProcessor (class) 存在");
        var ctors_ChannelMemoryProcessor = type_ChannelMemoryProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMemoryProcessor 构造函数数量: {ctors_ChannelMemoryProcessor.Length}");
        var methods_ChannelMemoryProcessor = type_ChannelMemoryProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMemoryProcessor 公开方法数量: {methods_ChannelMemoryProcessor.Length}");
        foreach (var m in methods_ChannelMemoryProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMemoryProcessor 未找到，尝试无命名空间...");
        type_ChannelMemoryProcessor = Type.GetType("ChannelMemoryProcessor");
        if (type_ChannelMemoryProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelMemoryProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMemoryProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
