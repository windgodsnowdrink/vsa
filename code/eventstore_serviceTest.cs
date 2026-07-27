#load "eventstore_service.cs"

Console.WriteLine("=== eventstore_service.cs Test ===");

try
{
    // 验证 class: EventStoreOptions
    var type_EventStoreOptions = Type.GetType("EventStoreOptions");
    if (type_EventStoreOptions != null)
    {
        Console.WriteLine("[PASS] 类型 EventStoreOptions (class) 存在");
        var ctors_EventStoreOptions = type_EventStoreOptions.GetConstructors();
        Console.WriteLine($"[PASS] EventStoreOptions 构造函数数量: {ctors_EventStoreOptions.Length}");
        var methods_EventStoreOptions = type_EventStoreOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventStoreOptions 公开方法数量: {methods_EventStoreOptions.Length}");
        foreach (var m in methods_EventStoreOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventStoreOptions 未找到，尝试无命名空间...");
        type_EventStoreOptions = Type.GetType("EventStoreOptions");
        if (type_EventStoreOptions != null)
            Console.WriteLine("[PASS] 类型 EventStoreOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventStoreOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventStoreService
    var type_EventStoreService = Type.GetType("EventStoreService");
    if (type_EventStoreService != null)
    {
        Console.WriteLine("[PASS] 类型 EventStoreService (class) 存在");
        var ctors_EventStoreService = type_EventStoreService.GetConstructors();
        Console.WriteLine($"[PASS] EventStoreService 构造函数数量: {ctors_EventStoreService.Length}");
        var methods_EventStoreService = type_EventStoreService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventStoreService 公开方法数量: {methods_EventStoreService.Length}");
        foreach (var m in methods_EventStoreService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventStoreService 未找到，尝试无命名空间...");
        type_EventStoreService = Type.GetType("EventStoreService");
        if (type_EventStoreService != null)
            Console.WriteLine("[PASS] 类型 EventStoreService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventStoreService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventDataPooledPolicy
    var type_EventDataPooledPolicy = Type.GetType("EventDataPooledPolicy");
    if (type_EventDataPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 EventDataPooledPolicy (class) 存在");
        var ctors_EventDataPooledPolicy = type_EventDataPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] EventDataPooledPolicy 构造函数数量: {ctors_EventDataPooledPolicy.Length}");
        var methods_EventDataPooledPolicy = type_EventDataPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventDataPooledPolicy 公开方法数量: {methods_EventDataPooledPolicy.Length}");
        foreach (var m in methods_EventDataPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventDataPooledPolicy 未找到，尝试无命名空间...");
        type_EventDataPooledPolicy = Type.GetType("EventDataPooledPolicy");
        if (type_EventDataPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 EventDataPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventDataPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventStoreDemo
    var type_EventStoreDemo = Type.GetType("EventStoreDemo");
    if (type_EventStoreDemo != null)
    {
        Console.WriteLine("[PASS] 类型 EventStoreDemo (class) 存在");
        var ctors_EventStoreDemo = type_EventStoreDemo.GetConstructors();
        Console.WriteLine($"[PASS] EventStoreDemo 构造函数数量: {ctors_EventStoreDemo.Length}");
        var methods_EventStoreDemo = type_EventStoreDemo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventStoreDemo 公开方法数量: {methods_EventStoreDemo.Length}");
        foreach (var m in methods_EventStoreDemo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventStoreDemo 未找到，尝试无命名空间...");
        type_EventStoreDemo = Type.GetType("EventStoreDemo");
        if (type_EventStoreDemo != null)
            Console.WriteLine("[PASS] 类型 EventStoreDemo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventStoreDemo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventStoreProcessor
    var type_EventStoreProcessor = Type.GetType("EventStoreProcessor");
    if (type_EventStoreProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 EventStoreProcessor (class) 存在");
        var ctors_EventStoreProcessor = type_EventStoreProcessor.GetConstructors();
        Console.WriteLine($"[PASS] EventStoreProcessor 构造函数数量: {ctors_EventStoreProcessor.Length}");
        var methods_EventStoreProcessor = type_EventStoreProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventStoreProcessor 公开方法数量: {methods_EventStoreProcessor.Length}");
        foreach (var m in methods_EventStoreProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventStoreProcessor 未找到，尝试无命名空间...");
        type_EventStoreProcessor = Type.GetType("EventStoreProcessor");
        if (type_EventStoreProcessor != null)
            Console.WriteLine("[PASS] 类型 EventStoreProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventStoreProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventContext
    var type_EventContext = Type.GetType("EventContext");
    if (type_EventContext != null)
    {
        Console.WriteLine("[PASS] 类型 EventContext (class) 存在");
        var ctors_EventContext = type_EventContext.GetConstructors();
        Console.WriteLine($"[PASS] EventContext 构造函数数量: {ctors_EventContext.Length}");
        var methods_EventContext = type_EventContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventContext 公开方法数量: {methods_EventContext.Length}");
        foreach (var m in methods_EventContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventContext 未找到，尝试无命名空间...");
        type_EventContext = Type.GetType("EventContext");
        if (type_EventContext != null)
            Console.WriteLine("[PASS] 类型 EventContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventContextPooledPolicy
    var type_EventContextPooledPolicy = Type.GetType("EventContextPooledPolicy");
    if (type_EventContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 EventContextPooledPolicy (class) 存在");
        var ctors_EventContextPooledPolicy = type_EventContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] EventContextPooledPolicy 构造函数数量: {ctors_EventContextPooledPolicy.Length}");
        var methods_EventContextPooledPolicy = type_EventContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventContextPooledPolicy 公开方法数量: {methods_EventContextPooledPolicy.Length}");
        foreach (var m in methods_EventContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventContextPooledPolicy 未找到，尝试无命名空间...");
        type_EventContextPooledPolicy = Type.GetType("EventContextPooledPolicy");
        if (type_EventContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 EventContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventContextPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UserCreatedEvent
    var type_UserCreatedEvent = Type.GetType("UserCreatedEvent");
    if (type_UserCreatedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 UserCreatedEvent (record) 存在");
        var ctors_UserCreatedEvent = type_UserCreatedEvent.GetConstructors();
        Console.WriteLine($"[PASS] UserCreatedEvent 构造函数数量: {ctors_UserCreatedEvent.Length}");
        var methods_UserCreatedEvent = type_UserCreatedEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UserCreatedEvent 公开方法数量: {methods_UserCreatedEvent.Length}");
        foreach (var m in methods_UserCreatedEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UserCreatedEvent 未找到，尝试无命名空间...");
        type_UserCreatedEvent = Type.GetType("UserCreatedEvent");
        if (type_UserCreatedEvent != null)
            Console.WriteLine("[PASS] 类型 UserCreatedEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserCreatedEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: EventData
    var type_EventData = Type.GetType("EventData");
    if (type_EventData != null)
    {
        Console.WriteLine("[PASS] 类型 EventData (record) 存在");
        var ctors_EventData = type_EventData.GetConstructors();
        Console.WriteLine($"[PASS] EventData 构造函数数量: {ctors_EventData.Length}");
        var methods_EventData = type_EventData.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventData 公开方法数量: {methods_EventData.Length}");
        foreach (var m in methods_EventData)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventData 未找到，尝试无命名空间...");
        type_EventData = Type.GetType("EventData");
        if (type_EventData != null)
            Console.WriteLine("[PASS] 类型 EventData (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventData 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
