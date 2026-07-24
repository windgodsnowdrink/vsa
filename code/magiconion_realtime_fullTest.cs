#load "magiconion_realtime_full.cs"

Console.WriteLine("=== magiconion_realtime_full.cs Test ===");

try
{
    // 验证 class: ChannelStreamingProcessor
    var type_ChannelStreamingProcessor = Type.GetType("ChannelStreamingProcessor");
    if (type_ChannelStreamingProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelStreamingProcessor (class) 存在");
        var ctors_ChannelStreamingProcessor = type_ChannelStreamingProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelStreamingProcessor 构造函数数量: {ctors_ChannelStreamingProcessor.Length}");
        var methods_ChannelStreamingProcessor = type_ChannelStreamingProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelStreamingProcessor 公开方法数量: {methods_ChannelStreamingProcessor.Length}");
        foreach (var m in methods_ChannelStreamingProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelStreamingProcessor 未找到，尝试无命名空间...");
        type_ChannelStreamingProcessor = Type.GetType("ChannelStreamingProcessor");
        if (type_ChannelStreamingProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelStreamingProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelStreamingProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: StreamingMessage
    var type_StreamingMessage = Type.GetType("StreamingMessage");
    if (type_StreamingMessage != null)
    {
        Console.WriteLine("[PASS] 类型 StreamingMessage (class) 存在");
        var ctors_StreamingMessage = type_StreamingMessage.GetConstructors();
        Console.WriteLine($"[PASS] StreamingMessage 构造函数数量: {ctors_StreamingMessage.Length}");
        var methods_StreamingMessage = type_StreamingMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StreamingMessage 公开方法数量: {methods_StreamingMessage.Length}");
        foreach (var m in methods_StreamingMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StreamingMessage 未找到，尝试无命名空间...");
        type_StreamingMessage = Type.GetType("StreamingMessage");
        if (type_StreamingMessage != null)
            Console.WriteLine("[PASS] 类型 StreamingMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StreamingMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
