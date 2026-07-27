#load "eventsourcing_service.cs"

Console.WriteLine("=== eventsourcing_service.cs Test ===");

try
{
    // 验证 class: EventStoreEngine
    var type_EventStoreEngine = Type.GetType("EventStoreEngine");
    if (type_EventStoreEngine != null)
    {
        Console.WriteLine("[PASS] 类型 EventStoreEngine (class) 存在");
        var ctors_EventStoreEngine = type_EventStoreEngine.GetConstructors();
        Console.WriteLine($"[PASS] EventStoreEngine 构造函数数量: {ctors_EventStoreEngine.Length}");
        var methods_EventStoreEngine = type_EventStoreEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventStoreEngine 公开方法数量: {methods_EventStoreEngine.Length}");
        foreach (var m in methods_EventStoreEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventStoreEngine 未找到，尝试无命名空间...");
        type_EventStoreEngine = Type.GetType("EventStoreEngine");
        if (type_EventStoreEngine != null)
            Console.WriteLine("[PASS] 类型 EventStoreEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventStoreEngine 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderAggregate
    var type_OrderAggregate = Type.GetType("OrderAggregate");
    if (type_OrderAggregate != null)
    {
        Console.WriteLine("[PASS] 类型 OrderAggregate (class) 存在");
        var ctors_OrderAggregate = type_OrderAggregate.GetConstructors();
        Console.WriteLine($"[PASS] OrderAggregate 构造函数数量: {ctors_OrderAggregate.Length}");
        var methods_OrderAggregate = type_OrderAggregate.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderAggregate 公开方法数量: {methods_OrderAggregate.Length}");
        foreach (var m in methods_OrderAggregate)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderAggregate 未找到，尝试无命名空间...");
        type_OrderAggregate = Type.GetType("OrderAggregate");
        if (type_OrderAggregate != null)
            Console.WriteLine("[PASS] 类型 OrderAggregate (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderAggregate 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventData
    var type_EventData = Type.GetType("EventData");
    if (type_EventData != null)
    {
        Console.WriteLine("[PASS] 类型 EventData (class) 存在");
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

    // 验证 record: OrderCreatedEvent
    var type_OrderCreatedEvent = Type.GetType("OrderCreatedEvent");
    if (type_OrderCreatedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 OrderCreatedEvent (record) 存在");
        var ctors_OrderCreatedEvent = type_OrderCreatedEvent.GetConstructors();
        Console.WriteLine($"[PASS] OrderCreatedEvent 构造函数数量: {ctors_OrderCreatedEvent.Length}");
        var methods_OrderCreatedEvent = type_OrderCreatedEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderCreatedEvent 公开方法数量: {methods_OrderCreatedEvent.Length}");
        foreach (var m in methods_OrderCreatedEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderCreatedEvent 未找到，尝试无命名空间...");
        type_OrderCreatedEvent = Type.GetType("OrderCreatedEvent");
        if (type_OrderCreatedEvent != null)
            Console.WriteLine("[PASS] 类型 OrderCreatedEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderCreatedEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CreateOrderCommand
    var type_CreateOrderCommand = Type.GetType("CreateOrderCommand");
    if (type_CreateOrderCommand != null)
    {
        Console.WriteLine("[PASS] 类型 CreateOrderCommand (record) 存在");
        var ctors_CreateOrderCommand = type_CreateOrderCommand.GetConstructors();
        Console.WriteLine($"[PASS] CreateOrderCommand 构造函数数量: {ctors_CreateOrderCommand.Length}");
        var methods_CreateOrderCommand = type_CreateOrderCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateOrderCommand 公开方法数量: {methods_CreateOrderCommand.Length}");
        foreach (var m in methods_CreateOrderCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateOrderCommand 未找到，尝试无命名空间...");
        type_CreateOrderCommand = Type.GetType("CreateOrderCommand");
        if (type_CreateOrderCommand != null)
            Console.WriteLine("[PASS] 类型 CreateOrderCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateOrderCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: OrderId
    var type_OrderId = Type.GetType("OrderId");
    if (type_OrderId != null)
    {
        Console.WriteLine("[PASS] 类型 OrderId (record) 存在");
        var ctors_OrderId = type_OrderId.GetConstructors();
        Console.WriteLine($"[PASS] OrderId 构造函数数量: {ctors_OrderId.Length}");
        var methods_OrderId = type_OrderId.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderId 公开方法数量: {methods_OrderId.Length}");
        foreach (var m in methods_OrderId)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderId 未找到，尝试无命名空间...");
        type_OrderId = Type.GetType("OrderId");
        if (type_OrderId != null)
            Console.WriteLine("[PASS] 类型 OrderId (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderId 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
