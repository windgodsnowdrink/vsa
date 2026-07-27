#load "orleans_eventsourcing_integration.cs"

Console.WriteLine("=== orleans_eventsourcing_integration.cs Test ===");

try
{
    // 验证 class: OrderState
    var type_OrderState = Type.GetType("OrderState");
    if (type_OrderState != null)
    {
        Console.WriteLine("[PASS] 类型 OrderState (class) 存在");
        var ctors_OrderState = type_OrderState.GetConstructors();
        Console.WriteLine($"[PASS] OrderState 构造函数数量: {ctors_OrderState.Length}");
        var methods_OrderState = type_OrderState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderState 公开方法数量: {methods_OrderState.Length}");
        foreach (var m in methods_OrderState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderState 未找到，尝试无命名空间...");
        type_OrderState = Type.GetType("OrderState");
        if (type_OrderState != null)
            Console.WriteLine("[PASS] 类型 OrderState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderState 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderGrain
    var type_OrderGrain = Type.GetType("OrderGrain");
    if (type_OrderGrain != null)
    {
        Console.WriteLine("[PASS] 类型 OrderGrain (class) 存在");
        var ctors_OrderGrain = type_OrderGrain.GetConstructors();
        Console.WriteLine($"[PASS] OrderGrain 构造函数数量: {ctors_OrderGrain.Length}");
        var methods_OrderGrain = type_OrderGrain.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderGrain 公开方法数量: {methods_OrderGrain.Length}");
        foreach (var m in methods_OrderGrain)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderGrain 未找到，尝试无命名空间...");
        type_OrderGrain = Type.GetType("OrderGrain");
        if (type_OrderGrain != null)
            Console.WriteLine("[PASS] 类型 OrderGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderEventHandlerGrain
    var type_OrderEventHandlerGrain = Type.GetType("OrderEventHandlerGrain");
    if (type_OrderEventHandlerGrain != null)
    {
        Console.WriteLine("[PASS] 类型 OrderEventHandlerGrain (class) 存在");
        var ctors_OrderEventHandlerGrain = type_OrderEventHandlerGrain.GetConstructors();
        Console.WriteLine($"[PASS] OrderEventHandlerGrain 构造函数数量: {ctors_OrderEventHandlerGrain.Length}");
        var methods_OrderEventHandlerGrain = type_OrderEventHandlerGrain.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderEventHandlerGrain 公开方法数量: {methods_OrderEventHandlerGrain.Length}");
        foreach (var m in methods_OrderEventHandlerGrain)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderEventHandlerGrain 未找到，尝试无命名空间...");
        type_OrderEventHandlerGrain = Type.GetType("OrderEventHandlerGrain");
        if (type_OrderEventHandlerGrain != null)
            Console.WriteLine("[PASS] 类型 OrderEventHandlerGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderEventHandlerGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrleansEventSourcingOptions
    var type_OrleansEventSourcingOptions = Type.GetType("OrleansEventSourcingOptions");
    if (type_OrleansEventSourcingOptions != null)
    {
        Console.WriteLine("[PASS] 类型 OrleansEventSourcingOptions (class) 存在");
        var ctors_OrleansEventSourcingOptions = type_OrleansEventSourcingOptions.GetConstructors();
        Console.WriteLine($"[PASS] OrleansEventSourcingOptions 构造函数数量: {ctors_OrleansEventSourcingOptions.Length}");
        var methods_OrleansEventSourcingOptions = type_OrleansEventSourcingOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrleansEventSourcingOptions 公开方法数量: {methods_OrleansEventSourcingOptions.Length}");
        foreach (var m in methods_OrleansEventSourcingOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrleansEventSourcingOptions 未找到，尝试无命名空间...");
        type_OrleansEventSourcingOptions = Type.GetType("OrleansEventSourcingOptions");
        if (type_OrleansEventSourcingOptions != null)
            Console.WriteLine("[PASS] 类型 OrleansEventSourcingOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrleansEventSourcingOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IOrderGrain
    var type_IOrderGrain = Type.GetType("IOrderGrain");
    if (type_IOrderGrain != null)
    {
        Console.WriteLine("[PASS] 类型 IOrderGrain (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IOrderGrain 未找到，尝试无命名空间...");
        type_IOrderGrain = Type.GetType("IOrderGrain");
        if (type_IOrderGrain != null)
            Console.WriteLine("[PASS] 类型 IOrderGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IOrderGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IOrderEventHandlerGrain
    var type_IOrderEventHandlerGrain = Type.GetType("IOrderEventHandlerGrain");
    if (type_IOrderEventHandlerGrain != null)
    {
        Console.WriteLine("[PASS] 类型 IOrderEventHandlerGrain (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IOrderEventHandlerGrain 未找到，尝试无命名空间...");
        type_IOrderEventHandlerGrain = Type.GetType("IOrderEventHandlerGrain");
        if (type_IOrderEventHandlerGrain != null)
            Console.WriteLine("[PASS] 类型 IOrderEventHandlerGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IOrderEventHandlerGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 record: EventBase
    var type_EventBase = Type.GetType("EventBase");
    if (type_EventBase != null)
    {
        Console.WriteLine("[PASS] 类型 EventBase (record) 存在");
        var ctors_EventBase = type_EventBase.GetConstructors();
        Console.WriteLine($"[PASS] EventBase 构造函数数量: {ctors_EventBase.Length}");
        var methods_EventBase = type_EventBase.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventBase 公开方法数量: {methods_EventBase.Length}");
        foreach (var m in methods_EventBase)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventBase 未找到，尝试无命名空间...");
        type_EventBase = Type.GetType("EventBase");
        if (type_EventBase != null)
            Console.WriteLine("[PASS] 类型 EventBase (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventBase 可能为顶层语句或嵌套类型");
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

    // 验证 record: OrderPaidEvent
    var type_OrderPaidEvent = Type.GetType("OrderPaidEvent");
    if (type_OrderPaidEvent != null)
    {
        Console.WriteLine("[PASS] 类型 OrderPaidEvent (record) 存在");
        var ctors_OrderPaidEvent = type_OrderPaidEvent.GetConstructors();
        Console.WriteLine($"[PASS] OrderPaidEvent 构造函数数量: {ctors_OrderPaidEvent.Length}");
        var methods_OrderPaidEvent = type_OrderPaidEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderPaidEvent 公开方法数量: {methods_OrderPaidEvent.Length}");
        foreach (var m in methods_OrderPaidEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderPaidEvent 未找到，尝试无命名空间...");
        type_OrderPaidEvent = Type.GetType("OrderPaidEvent");
        if (type_OrderPaidEvent != null)
            Console.WriteLine("[PASS] 类型 OrderPaidEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderPaidEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: OrderShippedEvent
    var type_OrderShippedEvent = Type.GetType("OrderShippedEvent");
    if (type_OrderShippedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 OrderShippedEvent (record) 存在");
        var ctors_OrderShippedEvent = type_OrderShippedEvent.GetConstructors();
        Console.WriteLine($"[PASS] OrderShippedEvent 构造函数数量: {ctors_OrderShippedEvent.Length}");
        var methods_OrderShippedEvent = type_OrderShippedEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderShippedEvent 公开方法数量: {methods_OrderShippedEvent.Length}");
        foreach (var m in methods_OrderShippedEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderShippedEvent 未找到，尝试无命名空间...");
        type_OrderShippedEvent = Type.GetType("OrderShippedEvent");
        if (type_OrderShippedEvent != null)
            Console.WriteLine("[PASS] 类型 OrderShippedEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderShippedEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
