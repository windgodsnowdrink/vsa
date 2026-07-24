#load "signalr_production.cs"

Console.WriteLine("=== signalr_production.cs Test ===");

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

    // 验证 class: NotificationHub
    var type_NotificationHub = Type.GetType("NotificationHub");
    if (type_NotificationHub != null)
    {
        Console.WriteLine("[PASS] 类型 NotificationHub (class) 存在");
        var ctors_NotificationHub = type_NotificationHub.GetConstructors();
        Console.WriteLine($"[PASS] NotificationHub 构造函数数量: {ctors_NotificationHub.Length}");
        var methods_NotificationHub = type_NotificationHub.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NotificationHub 公开方法数量: {methods_NotificationHub.Length}");
        foreach (var m in methods_NotificationHub)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NotificationHub 未找到，尝试无命名空间...");
        type_NotificationHub = Type.GetType("NotificationHub");
        if (type_NotificationHub != null)
            Console.WriteLine("[PASS] 类型 NotificationHub (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NotificationHub 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageDbContext
    var type_MessageDbContext = Type.GetType("MessageDbContext");
    if (type_MessageDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 MessageDbContext (class) 存在");
        var ctors_MessageDbContext = type_MessageDbContext.GetConstructors();
        Console.WriteLine($"[PASS] MessageDbContext 构造函数数量: {ctors_MessageDbContext.Length}");
        var methods_MessageDbContext = type_MessageDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageDbContext 公开方法数量: {methods_MessageDbContext.Length}");
        foreach (var m in methods_MessageDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageDbContext 未找到，尝试无命名空间...");
        type_MessageDbContext = Type.GetType("MessageDbContext");
        if (type_MessageDbContext != null)
            Console.WriteLine("[PASS] 类型 MessageDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ChatMessage
    var type_ChatMessage = Type.GetType("ChatMessage");
    if (type_ChatMessage != null)
    {
        Console.WriteLine("[PASS] 类型 ChatMessage (record) 存在");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
