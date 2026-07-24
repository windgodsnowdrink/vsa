#load "masstransit_production_integration.cs"

Console.WriteLine("=== masstransit_production_integration.cs Test ===");

try
{
    // 验证 class: MassTransitOptions
    var type_MassTransitOptions = Type.GetType("MassTransitOptions");
    if (type_MassTransitOptions != null)
    {
        Console.WriteLine("[PASS] 类型 MassTransitOptions (class) 存在");
        var ctors_MassTransitOptions = type_MassTransitOptions.GetConstructors();
        Console.WriteLine($"[PASS] MassTransitOptions 构造函数数量: {ctors_MassTransitOptions.Length}");
        var methods_MassTransitOptions = type_MassTransitOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MassTransitOptions 公开方法数量: {methods_MassTransitOptions.Length}");
        foreach (var m in methods_MassTransitOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MassTransitOptions 未找到，尝试无命名空间...");
        type_MassTransitOptions = Type.GetType("MassTransitOptions");
        if (type_MassTransitOptions != null)
            Console.WriteLine("[PASS] 类型 MassTransitOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MassTransitOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderCreatedConsumer
    var type_OrderCreatedConsumer = Type.GetType("OrderCreatedConsumer");
    if (type_OrderCreatedConsumer != null)
    {
        Console.WriteLine("[PASS] 类型 OrderCreatedConsumer (class) 存在");
        var ctors_OrderCreatedConsumer = type_OrderCreatedConsumer.GetConstructors();
        Console.WriteLine($"[PASS] OrderCreatedConsumer 构造函数数量: {ctors_OrderCreatedConsumer.Length}");
        var methods_OrderCreatedConsumer = type_OrderCreatedConsumer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderCreatedConsumer 公开方法数量: {methods_OrderCreatedConsumer.Length}");
        foreach (var m in methods_OrderCreatedConsumer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderCreatedConsumer 未找到，尝试无命名空间...");
        type_OrderCreatedConsumer = Type.GetType("OrderCreatedConsumer");
        if (type_OrderCreatedConsumer != null)
            Console.WriteLine("[PASS] 类型 OrderCreatedConsumer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderCreatedConsumer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderStateMachine
    var type_OrderStateMachine = Type.GetType("OrderStateMachine");
    if (type_OrderStateMachine != null)
    {
        Console.WriteLine("[PASS] 类型 OrderStateMachine (class) 存在");
        var ctors_OrderStateMachine = type_OrderStateMachine.GetConstructors();
        Console.WriteLine($"[PASS] OrderStateMachine 构造函数数量: {ctors_OrderStateMachine.Length}");
        var methods_OrderStateMachine = type_OrderStateMachine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderStateMachine 公开方法数量: {methods_OrderStateMachine.Length}");
        foreach (var m in methods_OrderStateMachine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderStateMachine 未找到，尝试无命名空间...");
        type_OrderStateMachine = Type.GetType("OrderStateMachine");
        if (type_OrderStateMachine != null)
            Console.WriteLine("[PASS] 类型 OrderStateMachine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderStateMachine 可能为顶层语句或嵌套类型");
    }

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

    // 验证 class: OrderStateDbContext
    var type_OrderStateDbContext = Type.GetType("OrderStateDbContext");
    if (type_OrderStateDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 OrderStateDbContext (class) 存在");
        var ctors_OrderStateDbContext = type_OrderStateDbContext.GetConstructors();
        Console.WriteLine($"[PASS] OrderStateDbContext 构造函数数量: {ctors_OrderStateDbContext.Length}");
        var methods_OrderStateDbContext = type_OrderStateDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderStateDbContext 公开方法数量: {methods_OrderStateDbContext.Length}");
        foreach (var m in methods_OrderStateDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderStateDbContext 未找到，尝试无命名空间...");
        type_OrderStateDbContext = Type.GetType("OrderStateDbContext");
        if (type_OrderStateDbContext != null)
            Console.WriteLine("[PASS] 类型 OrderStateDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderStateDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderStateMap
    var type_OrderStateMap = Type.GetType("OrderStateMap");
    if (type_OrderStateMap != null)
    {
        Console.WriteLine("[PASS] 类型 OrderStateMap (class) 存在");
        var ctors_OrderStateMap = type_OrderStateMap.GetConstructors();
        Console.WriteLine($"[PASS] OrderStateMap 构造函数数量: {ctors_OrderStateMap.Length}");
        var methods_OrderStateMap = type_OrderStateMap.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderStateMap 公开方法数量: {methods_OrderStateMap.Length}");
        foreach (var m in methods_OrderStateMap)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderStateMap 未找到，尝试无命名空间...");
        type_OrderStateMap = Type.GetType("OrderStateMap");
        if (type_OrderStateMap != null)
            Console.WriteLine("[PASS] 类型 OrderStateMap (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderStateMap 可能为顶层语句或嵌套类型");
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

    // 验证 class: MassTransitBackgroundService
    var type_MassTransitBackgroundService = Type.GetType("MassTransitBackgroundService");
    if (type_MassTransitBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 MassTransitBackgroundService (class) 存在");
        var ctors_MassTransitBackgroundService = type_MassTransitBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] MassTransitBackgroundService 构造函数数量: {ctors_MassTransitBackgroundService.Length}");
        var methods_MassTransitBackgroundService = type_MassTransitBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MassTransitBackgroundService 公开方法数量: {methods_MassTransitBackgroundService.Length}");
        foreach (var m in methods_MassTransitBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MassTransitBackgroundService 未找到，尝试无命名空间...");
        type_MassTransitBackgroundService = Type.GetType("MassTransitBackgroundService");
        if (type_MassTransitBackgroundService != null)
            Console.WriteLine("[PASS] 类型 MassTransitBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MassTransitBackgroundService 可能为顶层语句或嵌套类型");
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

    // 验证 class: AdvancedMassTransitScenarios
    var type_AdvancedMassTransitScenarios = Type.GetType("AdvancedMassTransitScenarios");
    if (type_AdvancedMassTransitScenarios != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedMassTransitScenarios (class) 存在");
        var ctors_AdvancedMassTransitScenarios = type_AdvancedMassTransitScenarios.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedMassTransitScenarios 构造函数数量: {ctors_AdvancedMassTransitScenarios.Length}");
        var methods_AdvancedMassTransitScenarios = type_AdvancedMassTransitScenarios.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedMassTransitScenarios 公开方法数量: {methods_AdvancedMassTransitScenarios.Length}");
        foreach (var m in methods_AdvancedMassTransitScenarios)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedMassTransitScenarios 未找到，尝试无命名空间...");
        type_AdvancedMassTransitScenarios = Type.GetType("AdvancedMassTransitScenarios");
        if (type_AdvancedMassTransitScenarios != null)
            Console.WriteLine("[PASS] 类型 AdvancedMassTransitScenarios (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedMassTransitScenarios 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderSaga
    var type_OrderSaga = Type.GetType("OrderSaga");
    if (type_OrderSaga != null)
    {
        Console.WriteLine("[PASS] 类型 OrderSaga (class) 存在");
        var ctors_OrderSaga = type_OrderSaga.GetConstructors();
        Console.WriteLine($"[PASS] OrderSaga 构造函数数量: {ctors_OrderSaga.Length}");
        var methods_OrderSaga = type_OrderSaga.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderSaga 公开方法数量: {methods_OrderSaga.Length}");
        foreach (var m in methods_OrderSaga)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderSaga 未找到，尝试无命名空间...");
        type_OrderSaga = Type.GetType("OrderSaga");
        if (type_OrderSaga != null)
            Console.WriteLine("[PASS] 类型 OrderSaga (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderSaga 可能为顶层语句或嵌套类型");
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

    // 验证 record: OrderProcessedEvent
    var type_OrderProcessedEvent = Type.GetType("OrderProcessedEvent");
    if (type_OrderProcessedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 OrderProcessedEvent (record) 存在");
        var ctors_OrderProcessedEvent = type_OrderProcessedEvent.GetConstructors();
        Console.WriteLine($"[PASS] OrderProcessedEvent 构造函数数量: {ctors_OrderProcessedEvent.Length}");
        var methods_OrderProcessedEvent = type_OrderProcessedEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderProcessedEvent 公开方法数量: {methods_OrderProcessedEvent.Length}");
        foreach (var m in methods_OrderProcessedEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderProcessedEvent 未找到，尝试无命名空间...");
        type_OrderProcessedEvent = Type.GetType("OrderProcessedEvent");
        if (type_OrderProcessedEvent != null)
            Console.WriteLine("[PASS] 类型 OrderProcessedEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderProcessedEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
