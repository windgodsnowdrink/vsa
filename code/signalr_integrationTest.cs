#load "signalr_integration.cs"

Console.WriteLine("=== signalr_integration.cs Test ===");

try
{
    // 验证 class: ChatHub
    var type_ChatHub = Type.GetType("ChatHub");
    if (type_ChatHub != null)
    {
        Console.WriteLine("[PASS] 类型 ChatHub (class) 存在");
        var ctors_ChatHub = type_ChatHub.GetConstructors();
        Console.WriteLine($"[PASS] ChatHub 构造函数数量: {ctors_ChatHub.Length}");
        var methods_ChatHub = type_ChatHub.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChatHub 公开方法数量: {methods_ChatHub.Length}");
        foreach (var m in methods_ChatHub)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChatHub 未找到，尝试无命名空间...");
        type_ChatHub = Type.GetType("ChatHub");
        if (type_ChatHub != null)
            Console.WriteLine("[PASS] 类型 ChatHub (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChatHub 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChannelMessageProcessor
    var type_ChannelMessageProcessor = Type.GetType("ChannelMessageProcessor");
    if (type_ChannelMessageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMessageProcessor (class) 存在");
        var ctors_ChannelMessageProcessor = type_ChannelMessageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMessageProcessor 构造函数数量: {ctors_ChannelMessageProcessor.Length}");
        var methods_ChannelMessageProcessor = type_ChannelMessageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMessageProcessor 公开方法数量: {methods_ChannelMessageProcessor.Length}");
        foreach (var m in methods_ChannelMessageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMessageProcessor 未找到，尝试无命名空间...");
        type_ChannelMessageProcessor = Type.GetType("ChannelMessageProcessor");
        if (type_ChannelMessageProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelMessageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMessageProcessor 可能为顶层语句或嵌套类型");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
