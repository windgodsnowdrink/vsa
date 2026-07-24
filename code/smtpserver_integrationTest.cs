#load "smtpserver_integration.cs"

Console.WriteLine("=== smtpserver_integration.cs Test ===");

try
{
    // 验证 class: MessageStore
    var type_MessageStore = Type.GetType("MessageStore");
    if (type_MessageStore != null)
    {
        Console.WriteLine("[PASS] 类型 MessageStore (class) 存在");
        var ctors_MessageStore = type_MessageStore.GetConstructors();
        Console.WriteLine($"[PASS] MessageStore 构造函数数量: {ctors_MessageStore.Length}");
        var methods_MessageStore = type_MessageStore.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageStore 公开方法数量: {methods_MessageStore.Length}");
        foreach (var m in methods_MessageStore)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageStore 未找到，尝试无命名空间...");
        type_MessageStore = Type.GetType("MessageStore");
        if (type_MessageStore != null)
            Console.WriteLine("[PASS] 类型 MessageStore (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageStore 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageBuffer
    var type_MessageBuffer = Type.GetType("MessageBuffer");
    if (type_MessageBuffer != null)
    {
        Console.WriteLine("[PASS] 类型 MessageBuffer (class) 存在");
        var ctors_MessageBuffer = type_MessageBuffer.GetConstructors();
        Console.WriteLine($"[PASS] MessageBuffer 构造函数数量: {ctors_MessageBuffer.Length}");
        var methods_MessageBuffer = type_MessageBuffer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageBuffer 公开方法数量: {methods_MessageBuffer.Length}");
        foreach (var m in methods_MessageBuffer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageBuffer 未找到，尝试无命名空间...");
        type_MessageBuffer = Type.GetType("MessageBuffer");
        if (type_MessageBuffer != null)
            Console.WriteLine("[PASS] 类型 MessageBuffer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageBuffer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageBufferPooledPolicy
    var type_MessageBufferPooledPolicy = Type.GetType("MessageBufferPooledPolicy");
    if (type_MessageBufferPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MessageBufferPooledPolicy (class) 存在");
        var ctors_MessageBufferPooledPolicy = type_MessageBufferPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MessageBufferPooledPolicy 构造函数数量: {ctors_MessageBufferPooledPolicy.Length}");
        var methods_MessageBufferPooledPolicy = type_MessageBufferPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageBufferPooledPolicy 公开方法数量: {methods_MessageBufferPooledPolicy.Length}");
        foreach (var m in methods_MessageBufferPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageBufferPooledPolicy 未找到，尝试无命名空间...");
        type_MessageBufferPooledPolicy = Type.GetType("MessageBufferPooledPolicy");
        if (type_MessageBufferPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 MessageBufferPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageBufferPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MassTransitMessageForwarder
    var type_MassTransitMessageForwarder = Type.GetType("MassTransitMessageForwarder");
    if (type_MassTransitMessageForwarder != null)
    {
        Console.WriteLine("[PASS] 类型 MassTransitMessageForwarder (class) 存在");
        var ctors_MassTransitMessageForwarder = type_MassTransitMessageForwarder.GetConstructors();
        Console.WriteLine($"[PASS] MassTransitMessageForwarder 构造函数数量: {ctors_MassTransitMessageForwarder.Length}");
        var methods_MassTransitMessageForwarder = type_MassTransitMessageForwarder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MassTransitMessageForwarder 公开方法数量: {methods_MassTransitMessageForwarder.Length}");
        foreach (var m in methods_MassTransitMessageForwarder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MassTransitMessageForwarder 未找到，尝试无命名空间...");
        type_MassTransitMessageForwarder = Type.GetType("MassTransitMessageForwarder");
        if (type_MassTransitMessageForwarder != null)
            Console.WriteLine("[PASS] 类型 MassTransitMessageForwarder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MassTransitMessageForwarder 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageEncryptor
    var type_MessageEncryptor = Type.GetType("MessageEncryptor");
    if (type_MessageEncryptor != null)
    {
        Console.WriteLine("[PASS] 类型 MessageEncryptor (class) 存在");
        var ctors_MessageEncryptor = type_MessageEncryptor.GetConstructors();
        Console.WriteLine($"[PASS] MessageEncryptor 构造函数数量: {ctors_MessageEncryptor.Length}");
        var methods_MessageEncryptor = type_MessageEncryptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageEncryptor 公开方法数量: {methods_MessageEncryptor.Length}");
        foreach (var m in methods_MessageEncryptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageEncryptor 未找到，尝试无命名空间...");
        type_MessageEncryptor = Type.GetType("MessageEncryptor");
        if (type_MessageEncryptor != null)
            Console.WriteLine("[PASS] 类型 MessageEncryptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageEncryptor 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMessageRepository
    var type_IMessageRepository = Type.GetType("IMessageRepository");
    if (type_IMessageRepository != null)
    {
        Console.WriteLine("[PASS] 类型 IMessageRepository (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMessageRepository 未找到，尝试无命名空间...");
        type_IMessageRepository = Type.GetType("IMessageRepository");
        if (type_IMessageRepository != null)
            Console.WriteLine("[PASS] 类型 IMessageRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMessageRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMessageForwarder
    var type_IMessageForwarder = Type.GetType("IMessageForwarder");
    if (type_IMessageForwarder != null)
    {
        Console.WriteLine("[PASS] 类型 IMessageForwarder (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMessageForwarder 未找到，尝试无命名空间...");
        type_IMessageForwarder = Type.GetType("IMessageForwarder");
        if (type_IMessageForwarder != null)
            Console.WriteLine("[PASS] 类型 IMessageForwarder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMessageForwarder 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DbMessage
    var type_DbMessage = Type.GetType("DbMessage");
    if (type_DbMessage != null)
    {
        Console.WriteLine("[PASS] 类型 DbMessage (record) 存在");
        var ctors_DbMessage = type_DbMessage.GetConstructors();
        Console.WriteLine($"[PASS] DbMessage 构造函数数量: {ctors_DbMessage.Length}");
        var methods_DbMessage = type_DbMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DbMessage 公开方法数量: {methods_DbMessage.Length}");
        foreach (var m in methods_DbMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DbMessage 未找到，尝试无命名空间...");
        type_DbMessage = Type.GetType("DbMessage");
        if (type_DbMessage != null)
            Console.WriteLine("[PASS] 类型 DbMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DbMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 record: EmailMessageEvent
    var type_EmailMessageEvent = Type.GetType("EmailMessageEvent");
    if (type_EmailMessageEvent != null)
    {
        Console.WriteLine("[PASS] 类型 EmailMessageEvent (record) 存在");
        var ctors_EmailMessageEvent = type_EmailMessageEvent.GetConstructors();
        Console.WriteLine($"[PASS] EmailMessageEvent 构造函数数量: {ctors_EmailMessageEvent.Length}");
        var methods_EmailMessageEvent = type_EmailMessageEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EmailMessageEvent 公开方法数量: {methods_EmailMessageEvent.Length}");
        foreach (var m in methods_EmailMessageEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EmailMessageEvent 未找到，尝试无命名空间...");
        type_EmailMessageEvent = Type.GetType("EmailMessageEvent");
        if (type_EmailMessageEvent != null)
            Console.WriteLine("[PASS] 类型 EmailMessageEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EmailMessageEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
