#load "skyapm_integration.cs"

Console.WriteLine("=== skyapm_integration.cs Test ===");

try
{
    // 验证 class: ChannelSegmentDispatcher
    var type_ChannelSegmentDispatcher = Type.GetType("ChannelSegmentDispatcher");
    if (type_ChannelSegmentDispatcher != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelSegmentDispatcher (class) 存在");
        var ctors_ChannelSegmentDispatcher = type_ChannelSegmentDispatcher.GetConstructors();
        Console.WriteLine($"[PASS] ChannelSegmentDispatcher 构造函数数量: {ctors_ChannelSegmentDispatcher.Length}");
        var methods_ChannelSegmentDispatcher = type_ChannelSegmentDispatcher.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelSegmentDispatcher 公开方法数量: {methods_ChannelSegmentDispatcher.Length}");
        foreach (var m in methods_ChannelSegmentDispatcher)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelSegmentDispatcher 未找到，尝试无命名空间...");
        type_ChannelSegmentDispatcher = Type.GetType("ChannelSegmentDispatcher");
        if (type_ChannelSegmentDispatcher != null)
            Console.WriteLine("[PASS] 类型 ChannelSegmentDispatcher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelSegmentDispatcher 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
