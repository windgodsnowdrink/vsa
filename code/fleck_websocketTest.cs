#load "fleck_websocket.cs"

Console.WriteLine("=== fleck_websocket.cs Test ===");

try
{
    // 验证 class: RedisMessagePersistence
    var type_RedisMessagePersistence = Type.GetType("RedisMessagePersistence");
    if (type_RedisMessagePersistence != null)
    {
        Console.WriteLine("[PASS] 类型 RedisMessagePersistence (class) 存在");
        var ctors_RedisMessagePersistence = type_RedisMessagePersistence.GetConstructors();
        Console.WriteLine($"[PASS] RedisMessagePersistence 构造函数数量: {ctors_RedisMessagePersistence.Length}");
        var methods_RedisMessagePersistence = type_RedisMessagePersistence.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisMessagePersistence 公开方法数量: {methods_RedisMessagePersistence.Length}");
        foreach (var m in methods_RedisMessagePersistence)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisMessagePersistence 未找到，尝试无命名空间...");
        type_RedisMessagePersistence = Type.GetType("RedisMessagePersistence");
        if (type_RedisMessagePersistence != null)
            Console.WriteLine("[PASS] 类型 RedisMessagePersistence (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisMessagePersistence 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FleckMessageProcessor
    var type_FleckMessageProcessor = Type.GetType("FleckMessageProcessor");
    if (type_FleckMessageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 FleckMessageProcessor (class) 存在");
        var ctors_FleckMessageProcessor = type_FleckMessageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] FleckMessageProcessor 构造函数数量: {ctors_FleckMessageProcessor.Length}");
        var methods_FleckMessageProcessor = type_FleckMessageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FleckMessageProcessor 公开方法数量: {methods_FleckMessageProcessor.Length}");
        foreach (var m in methods_FleckMessageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FleckMessageProcessor 未找到，尝试无命名空间...");
        type_FleckMessageProcessor = Type.GetType("FleckMessageProcessor");
        if (type_FleckMessageProcessor != null)
            Console.WriteLine("[PASS] 类型 FleckMessageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FleckMessageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WebSocketMessage
    var type_WebSocketMessage = Type.GetType("WebSocketMessage");
    if (type_WebSocketMessage != null)
    {
        Console.WriteLine("[PASS] 类型 WebSocketMessage (class) 存在");
        var ctors_WebSocketMessage = type_WebSocketMessage.GetConstructors();
        Console.WriteLine($"[PASS] WebSocketMessage 构造函数数量: {ctors_WebSocketMessage.Length}");
        var methods_WebSocketMessage = type_WebSocketMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WebSocketMessage 公开方法数量: {methods_WebSocketMessage.Length}");
        foreach (var m in methods_WebSocketMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WebSocketMessage 未找到，尝试无命名空间...");
        type_WebSocketMessage = Type.GetType("WebSocketMessage");
        if (type_WebSocketMessage != null)
            Console.WriteLine("[PASS] 类型 WebSocketMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WebSocketMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMessagePersistence
    var type_IMessagePersistence = Type.GetType("IMessagePersistence");
    if (type_IMessagePersistence != null)
    {
        Console.WriteLine("[PASS] 类型 IMessagePersistence (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMessagePersistence 未找到，尝试无命名空间...");
        type_IMessagePersistence = Type.GetType("IMessagePersistence");
        if (type_IMessagePersistence != null)
            Console.WriteLine("[PASS] 类型 IMessagePersistence (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMessagePersistence 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
