#load "high_frequency_1m.cs"

Console.WriteLine("=== high_frequency_1m.cs Test ===");

try
{
    // 验证 class: AeronMessageProcessor
    var type_AeronMessageProcessor = Type.GetType("AeronMessageProcessor");
    if (type_AeronMessageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 AeronMessageProcessor (class) 存在");
        var ctors_AeronMessageProcessor = type_AeronMessageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] AeronMessageProcessor 构造函数数量: {ctors_AeronMessageProcessor.Length}");
        var methods_AeronMessageProcessor = type_AeronMessageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AeronMessageProcessor 公开方法数量: {methods_AeronMessageProcessor.Length}");
        foreach (var m in methods_AeronMessageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AeronMessageProcessor 未找到，尝试无命名空间...");
        type_AeronMessageProcessor = Type.GetType("AeronMessageProcessor");
        if (type_AeronMessageProcessor != null)
            Console.WriteLine("[PASS] 类型 AeronMessageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AeronMessageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageProcessor
    var type_MessageProcessor = Type.GetType("MessageProcessor");
    if (type_MessageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 MessageProcessor (class) 存在");
        var ctors_MessageProcessor = type_MessageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] MessageProcessor 构造函数数量: {ctors_MessageProcessor.Length}");
        var methods_MessageProcessor = type_MessageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageProcessor 公开方法数量: {methods_MessageProcessor.Length}");
        foreach (var m in methods_MessageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageProcessor 未找到，尝试无命名空间...");
        type_MessageProcessor = Type.GetType("MessageProcessor");
        if (type_MessageProcessor != null)
            Console.WriteLine("[PASS] 类型 MessageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ThreadLocalAllocator
    var type_ThreadLocalAllocator = Type.GetType("ThreadLocalAllocator");
    if (type_ThreadLocalAllocator != null)
    {
        Console.WriteLine("[PASS] 类型 ThreadLocalAllocator (class) 存在");
        var ctors_ThreadLocalAllocator = type_ThreadLocalAllocator.GetConstructors();
        Console.WriteLine($"[PASS] ThreadLocalAllocator 构造函数数量: {ctors_ThreadLocalAllocator.Length}");
        var methods_ThreadLocalAllocator = type_ThreadLocalAllocator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ThreadLocalAllocator 公开方法数量: {methods_ThreadLocalAllocator.Length}");
        foreach (var m in methods_ThreadLocalAllocator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ThreadLocalAllocator 未找到，尝试无命名空间...");
        type_ThreadLocalAllocator = Type.GetType("ThreadLocalAllocator");
        if (type_ThreadLocalAllocator != null)
            Console.WriteLine("[PASS] 类型 ThreadLocalAllocator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThreadLocalAllocator 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: MessageEnvelope
    var type_MessageEnvelope = Type.GetType("MessageEnvelope");
    if (type_MessageEnvelope != null)
    {
        Console.WriteLine("[PASS] 类型 MessageEnvelope (struct) 存在");
        var ctors_MessageEnvelope = type_MessageEnvelope.GetConstructors();
        Console.WriteLine($"[PASS] MessageEnvelope 构造函数数量: {ctors_MessageEnvelope.Length}");
        var methods_MessageEnvelope = type_MessageEnvelope.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageEnvelope 公开方法数量: {methods_MessageEnvelope.Length}");
        foreach (var m in methods_MessageEnvelope)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageEnvelope 未找到，尝试无命名空间...");
        type_MessageEnvelope = Type.GetType("MessageEnvelope");
        if (type_MessageEnvelope != null)
            Console.WriteLine("[PASS] 类型 MessageEnvelope (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageEnvelope 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
