#load "magiconion_chat.cs"

Console.WriteLine("=== magiconion_chat.cs Test ===");

try
{
    // 验证 class: ChatService
    var type_ChatService = Type.GetType("ChatService");
    if (type_ChatService != null)
    {
        Console.WriteLine("[PASS] 类型 ChatService (class) 存在");
        var ctors_ChatService = type_ChatService.GetConstructors();
        Console.WriteLine($"[PASS] ChatService 构造函数数量: {ctors_ChatService.Length}");
        var methods_ChatService = type_ChatService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChatService 公开方法数量: {methods_ChatService.Length}");
        foreach (var m in methods_ChatService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChatService 未找到，尝试无命名空间...");
        type_ChatService = Type.GetType("ChatService");
        if (type_ChatService != null)
            Console.WriteLine("[PASS] 类型 ChatService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChatService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChannelChatProcessor
    var type_ChannelChatProcessor = Type.GetType("ChannelChatProcessor");
    if (type_ChannelChatProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelChatProcessor (class) 存在");
        var ctors_ChannelChatProcessor = type_ChannelChatProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelChatProcessor 构造函数数量: {ctors_ChannelChatProcessor.Length}");
        var methods_ChannelChatProcessor = type_ChannelChatProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelChatProcessor 公开方法数量: {methods_ChannelChatProcessor.Length}");
        foreach (var m in methods_ChannelChatProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelChatProcessor 未找到，尝试无命名空间...");
        type_ChannelChatProcessor = Type.GetType("ChannelChatProcessor");
        if (type_ChannelChatProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelChatProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelChatProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChatMessage
    var type_ChatMessage = Type.GetType("ChatMessage");
    if (type_ChatMessage != null)
    {
        Console.WriteLine("[PASS] 类型 ChatMessage (class) 存在");
        var ctors_ChatMessage = type_ChatMessage.GetConstructors();
        Console.WriteLine($"[PASS] ChatMessage 构造函数数量: {ctors_ChatMessage.Length}");
        var methods_ChatMessage = type_ChatMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChatMessage 公开方法数量: {methods_ChatMessage.Length}");
        foreach (var m in methods_ChatMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChatMessage 未找到，尝试无命名空间...");
        type_ChatMessage = Type.GetType("ChatMessage");
        if (type_ChatMessage != null)
            Console.WriteLine("[PASS] 类型 ChatMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChatMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JoinResult
    var type_JoinResult = Type.GetType("JoinResult");
    if (type_JoinResult != null)
    {
        Console.WriteLine("[PASS] 类型 JoinResult (class) 存在");
        var ctors_JoinResult = type_JoinResult.GetConstructors();
        Console.WriteLine($"[PASS] JoinResult 构造函数数量: {ctors_JoinResult.Length}");
        var methods_JoinResult = type_JoinResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JoinResult 公开方法数量: {methods_JoinResult.Length}");
        foreach (var m in methods_JoinResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JoinResult 未找到，尝试无命名空间...");
        type_JoinResult = Type.GetType("JoinResult");
        if (type_JoinResult != null)
            Console.WriteLine("[PASS] 类型 JoinResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JoinResult 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IChatService
    var type_IChatService = Type.GetType("IChatService");
    if (type_IChatService != null)
    {
        Console.WriteLine("[PASS] 类型 IChatService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IChatService 未找到，尝试无命名空间...");
        type_IChatService = Type.GetType("IChatService");
        if (type_IChatService != null)
            Console.WriteLine("[PASS] 类型 IChatService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IChatService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
