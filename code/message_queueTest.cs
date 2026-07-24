#load "message_queue.cs"

Console.WriteLine("=== message_queue.cs Test ===");

try
{
    // 验证 class: ChannelMessageQueueProcessor
    var type_ChannelMessageQueueProcessor = Type.GetType("ChannelMessageQueueProcessor");
    if (type_ChannelMessageQueueProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMessageQueueProcessor (class) 存在");
        var ctors_ChannelMessageQueueProcessor = type_ChannelMessageQueueProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMessageQueueProcessor 构造函数数量: {ctors_ChannelMessageQueueProcessor.Length}");
        var methods_ChannelMessageQueueProcessor = type_ChannelMessageQueueProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMessageQueueProcessor 公开方法数量: {methods_ChannelMessageQueueProcessor.Length}");
        foreach (var m in methods_ChannelMessageQueueProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMessageQueueProcessor 未找到，尝试无命名空间...");
        type_ChannelMessageQueueProcessor = Type.GetType("ChannelMessageQueueProcessor");
        if (type_ChannelMessageQueueProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelMessageQueueProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMessageQueueProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QueueMessage
    var type_QueueMessage = Type.GetType("QueueMessage");
    if (type_QueueMessage != null)
    {
        Console.WriteLine("[PASS] 类型 QueueMessage (class) 存在");
        var ctors_QueueMessage = type_QueueMessage.GetConstructors();
        Console.WriteLine($"[PASS] QueueMessage 构造函数数量: {ctors_QueueMessage.Length}");
        var methods_QueueMessage = type_QueueMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QueueMessage 公开方法数量: {methods_QueueMessage.Length}");
        foreach (var m in methods_QueueMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QueueMessage 未找到，尝试无命名空间...");
        type_QueueMessage = Type.GetType("QueueMessage");
        if (type_QueueMessage != null)
            Console.WriteLine("[PASS] 类型 QueueMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QueueMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
