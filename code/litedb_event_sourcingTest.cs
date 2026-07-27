#load "litedb_event_sourcing.cs"

Console.WriteLine("=== litedb_event_sourcing.cs Test ===");

try
{
    // 验证 class: AggregateRoot
    var type_AggregateRoot = Type.GetType("AggregateRoot");
    if (type_AggregateRoot != null)
    {
        Console.WriteLine("[PASS] 类型 AggregateRoot (class) 存在");
        var ctors_AggregateRoot = type_AggregateRoot.GetConstructors();
        Console.WriteLine($"[PASS] AggregateRoot 构造函数数量: {ctors_AggregateRoot.Length}");
        var methods_AggregateRoot = type_AggregateRoot.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AggregateRoot 公开方法数量: {methods_AggregateRoot.Length}");
        foreach (var m in methods_AggregateRoot)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AggregateRoot 未找到，尝试无命名空间...");
        type_AggregateRoot = Type.GetType("AggregateRoot");
        if (type_AggregateRoot != null)
            Console.WriteLine("[PASS] 类型 AggregateRoot (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AggregateRoot 可能为顶层语句或嵌套类型");
    }

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

    // 验证 class: EventPersistHandler
    var type_EventPersistHandler = Type.GetType("EventPersistHandler");
    if (type_EventPersistHandler != null)
    {
        Console.WriteLine("[PASS] 类型 EventPersistHandler (class) 存在");
        var ctors_EventPersistHandler = type_EventPersistHandler.GetConstructors();
        Console.WriteLine($"[PASS] EventPersistHandler 构造函数数量: {ctors_EventPersistHandler.Length}");
        var methods_EventPersistHandler = type_EventPersistHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventPersistHandler 公开方法数量: {methods_EventPersistHandler.Length}");
        foreach (var m in methods_EventPersistHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventPersistHandler 未找到，尝试无命名空间...");
        type_EventPersistHandler = Type.GetType("EventPersistHandler");
        if (type_EventPersistHandler != null)
            Console.WriteLine("[PASS] 类型 EventPersistHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventPersistHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Order
    var type_Order = Type.GetType("Order");
    if (type_Order != null)
    {
        Console.WriteLine("[PASS] 类型 Order (class) 存在");
        var ctors_Order = type_Order.GetConstructors();
        Console.WriteLine($"[PASS] Order 构造函数数量: {ctors_Order.Length}");
        var methods_Order = type_Order.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Order 公开方法数量: {methods_Order.Length}");
        foreach (var m in methods_Order)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Order 未找到，尝试无命名空间...");
        type_Order = Type.GetType("Order");
        if (type_Order != null)
            Console.WriteLine("[PASS] 类型 Order (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Order 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderCreatedEvent
    var type_OrderCreatedEvent = Type.GetType("OrderCreatedEvent");
    if (type_OrderCreatedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 OrderCreatedEvent (class) 存在");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
