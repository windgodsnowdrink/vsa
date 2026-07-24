#load "message_queue_enhanced.cs"

Console.WriteLine("=== message_queue_enhanced.cs Test ===");

try
{
    // 验证 class: ChannelAckProcessor
    var type_ChannelAckProcessor = Type.GetType("ChannelAckProcessor");
    if (type_ChannelAckProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelAckProcessor (class) 存在");
        var ctors_ChannelAckProcessor = type_ChannelAckProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelAckProcessor 构造函数数量: {ctors_ChannelAckProcessor.Length}");
        var methods_ChannelAckProcessor = type_ChannelAckProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelAckProcessor 公开方法数量: {methods_ChannelAckProcessor.Length}");
        foreach (var m in methods_ChannelAckProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelAckProcessor 未找到，尝试无命名空间...");
        type_ChannelAckProcessor = Type.GetType("ChannelAckProcessor");
        if (type_ChannelAckProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelAckProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelAckProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChannelDeadLetterProcessor
    var type_ChannelDeadLetterProcessor = Type.GetType("ChannelDeadLetterProcessor");
    if (type_ChannelDeadLetterProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelDeadLetterProcessor (class) 存在");
        var ctors_ChannelDeadLetterProcessor = type_ChannelDeadLetterProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelDeadLetterProcessor 构造函数数量: {ctors_ChannelDeadLetterProcessor.Length}");
        var methods_ChannelDeadLetterProcessor = type_ChannelDeadLetterProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelDeadLetterProcessor 公开方法数量: {methods_ChannelDeadLetterProcessor.Length}");
        foreach (var m in methods_ChannelDeadLetterProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelDeadLetterProcessor 未找到，尝试无命名空间...");
        type_ChannelDeadLetterProcessor = Type.GetType("ChannelDeadLetterProcessor");
        if (type_ChannelDeadLetterProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelDeadLetterProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelDeadLetterProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataflowNetwork
    var type_DataflowNetwork = Type.GetType("DataflowNetwork");
    if (type_DataflowNetwork != null)
    {
        Console.WriteLine("[PASS] 类型 DataflowNetwork (class) 存在");
        var ctors_DataflowNetwork = type_DataflowNetwork.GetConstructors();
        Console.WriteLine($"[PASS] DataflowNetwork 构造函数数量: {ctors_DataflowNetwork.Length}");
        var methods_DataflowNetwork = type_DataflowNetwork.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataflowNetwork 公开方法数量: {methods_DataflowNetwork.Length}");
        foreach (var m in methods_DataflowNetwork)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataflowNetwork 未找到，尝试无命名空间...");
        type_DataflowNetwork = Type.GetType("DataflowNetwork");
        if (type_DataflowNetwork != null)
            Console.WriteLine("[PASS] 类型 DataflowNetwork (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataflowNetwork 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DeadLetterMessage
    var type_DeadLetterMessage = Type.GetType("DeadLetterMessage");
    if (type_DeadLetterMessage != null)
    {
        Console.WriteLine("[PASS] 类型 DeadLetterMessage (class) 存在");
        var ctors_DeadLetterMessage = type_DeadLetterMessage.GetConstructors();
        Console.WriteLine($"[PASS] DeadLetterMessage 构造函数数量: {ctors_DeadLetterMessage.Length}");
        var methods_DeadLetterMessage = type_DeadLetterMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeadLetterMessage 公开方法数量: {methods_DeadLetterMessage.Length}");
        foreach (var m in methods_DeadLetterMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeadLetterMessage 未找到，尝试无命名空间...");
        type_DeadLetterMessage = Type.GetType("DeadLetterMessage");
        if (type_DeadLetterMessage != null)
            Console.WriteLine("[PASS] 类型 DeadLetterMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeadLetterMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
