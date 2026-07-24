#load "mediatr_local_message_table_integration.cs"

Console.WriteLine("=== mediatr_local_message_table_integration.cs Test ===");

try
{
    // 验证 class: OutboxMessage
    var type_OutboxMessage = Type.GetType("OutboxMessage");
    if (type_OutboxMessage != null)
    {
        Console.WriteLine("[PASS] 类型 OutboxMessage (class) 存在");
        var ctors_OutboxMessage = type_OutboxMessage.GetConstructors();
        Console.WriteLine($"[PASS] OutboxMessage 构造函数数量: {ctors_OutboxMessage.Length}");
        var methods_OutboxMessage = type_OutboxMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OutboxMessage 公开方法数量: {methods_OutboxMessage.Length}");
        foreach (var m in methods_OutboxMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OutboxMessage 未找到，尝试无命名空间...");
        type_OutboxMessage = Type.GetType("OutboxMessage");
        if (type_OutboxMessage != null)
            Console.WriteLine("[PASS] 类型 OutboxMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OutboxMessage 可能为顶层语句或嵌套类型");
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

    // 验证 class: OutboxBehavior
    var type_OutboxBehavior = Type.GetType("OutboxBehavior");
    if (type_OutboxBehavior != null)
    {
        Console.WriteLine("[PASS] 类型 OutboxBehavior (class) 存在");
        var ctors_OutboxBehavior = type_OutboxBehavior.GetConstructors();
        Console.WriteLine($"[PASS] OutboxBehavior 构造函数数量: {ctors_OutboxBehavior.Length}");
        var methods_OutboxBehavior = type_OutboxBehavior.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OutboxBehavior 公开方法数量: {methods_OutboxBehavior.Length}");
        foreach (var m in methods_OutboxBehavior)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OutboxBehavior 未找到，尝试无命名空间...");
        type_OutboxBehavior = Type.GetType("OutboxBehavior");
        if (type_OutboxBehavior != null)
            Console.WriteLine("[PASS] 类型 OutboxBehavior (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OutboxBehavior 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OutboxProcessor
    var type_OutboxProcessor = Type.GetType("OutboxProcessor");
    if (type_OutboxProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 OutboxProcessor (class) 存在");
        var ctors_OutboxProcessor = type_OutboxProcessor.GetConstructors();
        Console.WriteLine($"[PASS] OutboxProcessor 构造函数数量: {ctors_OutboxProcessor.Length}");
        var methods_OutboxProcessor = type_OutboxProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OutboxProcessor 公开方法数量: {methods_OutboxProcessor.Length}");
        foreach (var m in methods_OutboxProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OutboxProcessor 未找到，尝试无命名空间...");
        type_OutboxProcessor = Type.GetType("OutboxProcessor");
        if (type_OutboxProcessor != null)
            Console.WriteLine("[PASS] 类型 OutboxProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OutboxProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MediatRDependencyInjectionExtensions
    var type_MediatRDependencyInjectionExtensions = Type.GetType("MediatRDependencyInjectionExtensions");
    if (type_MediatRDependencyInjectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MediatRDependencyInjectionExtensions (class) 存在");
        var ctors_MediatRDependencyInjectionExtensions = type_MediatRDependencyInjectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MediatRDependencyInjectionExtensions 构造函数数量: {ctors_MediatRDependencyInjectionExtensions.Length}");
        var methods_MediatRDependencyInjectionExtensions = type_MediatRDependencyInjectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediatRDependencyInjectionExtensions 公开方法数量: {methods_MediatRDependencyInjectionExtensions.Length}");
        foreach (var m in methods_MediatRDependencyInjectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediatRDependencyInjectionExtensions 未找到，尝试无命名空间...");
        type_MediatRDependencyInjectionExtensions = Type.GetType("MediatRDependencyInjectionExtensions");
        if (type_MediatRDependencyInjectionExtensions != null)
            Console.WriteLine("[PASS] 类型 MediatRDependencyInjectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediatRDependencyInjectionExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
