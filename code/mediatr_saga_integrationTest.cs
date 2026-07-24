#load "mediatr_saga_integration.cs"

Console.WriteLine("=== mediatr_saga_integration.cs Test ===");

try
{
    // 验证 class: SagaState
    var type_SagaState = Type.GetType("SagaState");
    if (type_SagaState != null)
    {
        Console.WriteLine("[PASS] 类型 SagaState (class) 存在");
        var ctors_SagaState = type_SagaState.GetConstructors();
        Console.WriteLine($"[PASS] SagaState 构造函数数量: {ctors_SagaState.Length}");
        var methods_SagaState = type_SagaState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SagaState 公开方法数量: {methods_SagaState.Length}");
        foreach (var m in methods_SagaState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SagaState 未找到，尝试无命名空间...");
        type_SagaState = Type.GetType("SagaState");
        if (type_SagaState != null)
            Console.WriteLine("[PASS] 类型 SagaState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SagaState 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SagaDbContext
    var type_SagaDbContext = Type.GetType("SagaDbContext");
    if (type_SagaDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 SagaDbContext (class) 存在");
        var ctors_SagaDbContext = type_SagaDbContext.GetConstructors();
        Console.WriteLine($"[PASS] SagaDbContext 构造函数数量: {ctors_SagaDbContext.Length}");
        var methods_SagaDbContext = type_SagaDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SagaDbContext 公开方法数量: {methods_SagaDbContext.Length}");
        foreach (var m in methods_SagaDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SagaDbContext 未找到，尝试无命名空间...");
        type_SagaDbContext = Type.GetType("SagaDbContext");
        if (type_SagaDbContext != null)
            Console.WriteLine("[PASS] 类型 SagaDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SagaDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CreateOrderStep
    var type_CreateOrderStep = Type.GetType("CreateOrderStep");
    if (type_CreateOrderStep != null)
    {
        Console.WriteLine("[PASS] 类型 CreateOrderStep (class) 存在");
        var ctors_CreateOrderStep = type_CreateOrderStep.GetConstructors();
        Console.WriteLine($"[PASS] CreateOrderStep 构造函数数量: {ctors_CreateOrderStep.Length}");
        var methods_CreateOrderStep = type_CreateOrderStep.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateOrderStep 公开方法数量: {methods_CreateOrderStep.Length}");
        foreach (var m in methods_CreateOrderStep)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateOrderStep 未找到，尝试无命名空间...");
        type_CreateOrderStep = Type.GetType("CreateOrderStep");
        if (type_CreateOrderStep != null)
            Console.WriteLine("[PASS] 类型 CreateOrderStep (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateOrderStep 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SagaCoordinator
    var type_SagaCoordinator = Type.GetType("SagaCoordinator");
    if (type_SagaCoordinator != null)
    {
        Console.WriteLine("[PASS] 类型 SagaCoordinator (class) 存在");
        var ctors_SagaCoordinator = type_SagaCoordinator.GetConstructors();
        Console.WriteLine($"[PASS] SagaCoordinator 构造函数数量: {ctors_SagaCoordinator.Length}");
        var methods_SagaCoordinator = type_SagaCoordinator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SagaCoordinator 公开方法数量: {methods_SagaCoordinator.Length}");
        foreach (var m in methods_SagaCoordinator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SagaCoordinator 未找到，尝试无命名空间...");
        type_SagaCoordinator = Type.GetType("SagaCoordinator");
        if (type_SagaCoordinator != null)
            Console.WriteLine("[PASS] 类型 SagaCoordinator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SagaCoordinator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SagaOrchestrator
    var type_SagaOrchestrator = Type.GetType("SagaOrchestrator");
    if (type_SagaOrchestrator != null)
    {
        Console.WriteLine("[PASS] 类型 SagaOrchestrator (class) 存在");
        var ctors_SagaOrchestrator = type_SagaOrchestrator.GetConstructors();
        Console.WriteLine($"[PASS] SagaOrchestrator 构造函数数量: {ctors_SagaOrchestrator.Length}");
        var methods_SagaOrchestrator = type_SagaOrchestrator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SagaOrchestrator 公开方法数量: {methods_SagaOrchestrator.Length}");
        foreach (var m in methods_SagaOrchestrator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SagaOrchestrator 未找到，尝试无命名空间...");
        type_SagaOrchestrator = Type.GetType("SagaOrchestrator");
        if (type_SagaOrchestrator != null)
            Console.WriteLine("[PASS] 类型 SagaOrchestrator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SagaOrchestrator 可能为顶层语句或嵌套类型");
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

    // 验证 class: OrderProcessingSagaState
    var type_OrderProcessingSagaState = Type.GetType("OrderProcessingSagaState");
    if (type_OrderProcessingSagaState != null)
    {
        Console.WriteLine("[PASS] 类型 OrderProcessingSagaState (class) 存在");
        var ctors_OrderProcessingSagaState = type_OrderProcessingSagaState.GetConstructors();
        Console.WriteLine($"[PASS] OrderProcessingSagaState 构造函数数量: {ctors_OrderProcessingSagaState.Length}");
        var methods_OrderProcessingSagaState = type_OrderProcessingSagaState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderProcessingSagaState 公开方法数量: {methods_OrderProcessingSagaState.Length}");
        foreach (var m in methods_OrderProcessingSagaState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderProcessingSagaState 未找到，尝试无命名空间...");
        type_OrderProcessingSagaState = Type.GetType("OrderProcessingSagaState");
        if (type_OrderProcessingSagaState != null)
            Console.WriteLine("[PASS] 类型 OrderProcessingSagaState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderProcessingSagaState 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderSagaState
    var type_OrderSagaState = Type.GetType("OrderSagaState");
    if (type_OrderSagaState != null)
    {
        Console.WriteLine("[PASS] 类型 OrderSagaState (class) 存在");
        var ctors_OrderSagaState = type_OrderSagaState.GetConstructors();
        Console.WriteLine($"[PASS] OrderSagaState 构造函数数量: {ctors_OrderSagaState.Length}");
        var methods_OrderSagaState = type_OrderSagaState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderSagaState 公开方法数量: {methods_OrderSagaState.Length}");
        foreach (var m in methods_OrderSagaState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderSagaState 未找到，尝试无命名空间...");
        type_OrderSagaState = Type.GetType("OrderSagaState");
        if (type_OrderSagaState != null)
            Console.WriteLine("[PASS] 类型 OrderSagaState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderSagaState 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderProcessingSaga
    var type_OrderProcessingSaga = Type.GetType("OrderProcessingSaga");
    if (type_OrderProcessingSaga != null)
    {
        Console.WriteLine("[PASS] 类型 OrderProcessingSaga (class) 存在");
        var ctors_OrderProcessingSaga = type_OrderProcessingSaga.GetConstructors();
        Console.WriteLine($"[PASS] OrderProcessingSaga 构造函数数量: {ctors_OrderProcessingSaga.Length}");
        var methods_OrderProcessingSaga = type_OrderProcessingSaga.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderProcessingSaga 公开方法数量: {methods_OrderProcessingSaga.Length}");
        foreach (var m in methods_OrderProcessingSaga)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderProcessingSaga 未找到，尝试无命名空间...");
        type_OrderProcessingSaga = Type.GetType("OrderProcessingSaga");
        if (type_OrderProcessingSaga != null)
            Console.WriteLine("[PASS] 类型 OrderProcessingSaga (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderProcessingSaga 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ISagaStep
    var type_ISagaStep = Type.GetType("ISagaStep");
    if (type_ISagaStep != null)
    {
        Console.WriteLine("[PASS] 类型 ISagaStep (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ISagaStep 未找到，尝试无命名空间...");
        type_ISagaStep = Type.GetType("ISagaStep");
        if (type_ISagaStep != null)
            Console.WriteLine("[PASS] 类型 ISagaStep (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISagaStep 可能为顶层语句或嵌套类型");
    }

    // 验证 record: OrderCreated
    var type_OrderCreated = Type.GetType("OrderCreated");
    if (type_OrderCreated != null)
    {
        Console.WriteLine("[PASS] 类型 OrderCreated (record) 存在");
        var ctors_OrderCreated = type_OrderCreated.GetConstructors();
        Console.WriteLine($"[PASS] OrderCreated 构造函数数量: {ctors_OrderCreated.Length}");
        var methods_OrderCreated = type_OrderCreated.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderCreated 公开方法数量: {methods_OrderCreated.Length}");
        foreach (var m in methods_OrderCreated)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderCreated 未找到，尝试无命名空间...");
        type_OrderCreated = Type.GetType("OrderCreated");
        if (type_OrderCreated != null)
            Console.WriteLine("[PASS] 类型 OrderCreated (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderCreated 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ProcessPaymentCommand
    var type_ProcessPaymentCommand = Type.GetType("ProcessPaymentCommand");
    if (type_ProcessPaymentCommand != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessPaymentCommand (record) 存在");
        var ctors_ProcessPaymentCommand = type_ProcessPaymentCommand.GetConstructors();
        Console.WriteLine($"[PASS] ProcessPaymentCommand 构造函数数量: {ctors_ProcessPaymentCommand.Length}");
        var methods_ProcessPaymentCommand = type_ProcessPaymentCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessPaymentCommand 公开方法数量: {methods_ProcessPaymentCommand.Length}");
        foreach (var m in methods_ProcessPaymentCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessPaymentCommand 未找到，尝试无命名空间...");
        type_ProcessPaymentCommand = Type.GetType("ProcessPaymentCommand");
        if (type_ProcessPaymentCommand != null)
            Console.WriteLine("[PASS] 类型 ProcessPaymentCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessPaymentCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: PaymentCompleted
    var type_PaymentCompleted = Type.GetType("PaymentCompleted");
    if (type_PaymentCompleted != null)
    {
        Console.WriteLine("[PASS] 类型 PaymentCompleted (record) 存在");
        var ctors_PaymentCompleted = type_PaymentCompleted.GetConstructors();
        Console.WriteLine($"[PASS] PaymentCompleted 构造函数数量: {ctors_PaymentCompleted.Length}");
        var methods_PaymentCompleted = type_PaymentCompleted.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PaymentCompleted 公开方法数量: {methods_PaymentCompleted.Length}");
        foreach (var m in methods_PaymentCompleted)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PaymentCompleted 未找到，尝试无命名空间...");
        type_PaymentCompleted = Type.GetType("PaymentCompleted");
        if (type_PaymentCompleted != null)
            Console.WriteLine("[PASS] 类型 PaymentCompleted (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PaymentCompleted 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ReserveInventoryCommand
    var type_ReserveInventoryCommand = Type.GetType("ReserveInventoryCommand");
    if (type_ReserveInventoryCommand != null)
    {
        Console.WriteLine("[PASS] 类型 ReserveInventoryCommand (record) 存在");
        var ctors_ReserveInventoryCommand = type_ReserveInventoryCommand.GetConstructors();
        Console.WriteLine($"[PASS] ReserveInventoryCommand 构造函数数量: {ctors_ReserveInventoryCommand.Length}");
        var methods_ReserveInventoryCommand = type_ReserveInventoryCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReserveInventoryCommand 公开方法数量: {methods_ReserveInventoryCommand.Length}");
        foreach (var m in methods_ReserveInventoryCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReserveInventoryCommand 未找到，尝试无命名空间...");
        type_ReserveInventoryCommand = Type.GetType("ReserveInventoryCommand");
        if (type_ReserveInventoryCommand != null)
            Console.WriteLine("[PASS] 类型 ReserveInventoryCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReserveInventoryCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: InventoryReserved
    var type_InventoryReserved = Type.GetType("InventoryReserved");
    if (type_InventoryReserved != null)
    {
        Console.WriteLine("[PASS] 类型 InventoryReserved (record) 存在");
        var ctors_InventoryReserved = type_InventoryReserved.GetConstructors();
        Console.WriteLine($"[PASS] InventoryReserved 构造函数数量: {ctors_InventoryReserved.Length}");
        var methods_InventoryReserved = type_InventoryReserved.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] InventoryReserved 公开方法数量: {methods_InventoryReserved.Length}");
        foreach (var m in methods_InventoryReserved)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 InventoryReserved 未找到，尝试无命名空间...");
        type_InventoryReserved = Type.GetType("InventoryReserved");
        if (type_InventoryReserved != null)
            Console.WriteLine("[PASS] 类型 InventoryReserved (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 InventoryReserved 可能为顶层语句或嵌套类型");
    }

    // 验证 record: OrderFailed
    var type_OrderFailed = Type.GetType("OrderFailed");
    if (type_OrderFailed != null)
    {
        Console.WriteLine("[PASS] 类型 OrderFailed (record) 存在");
        var ctors_OrderFailed = type_OrderFailed.GetConstructors();
        Console.WriteLine($"[PASS] OrderFailed 构造函数数量: {ctors_OrderFailed.Length}");
        var methods_OrderFailed = type_OrderFailed.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderFailed 公开方法数量: {methods_OrderFailed.Length}");
        foreach (var m in methods_OrderFailed)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderFailed 未找到，尝试无命名空间...");
        type_OrderFailed = Type.GetType("OrderFailed");
        if (type_OrderFailed != null)
            Console.WriteLine("[PASS] 类型 OrderFailed (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderFailed 可能为顶层语句或嵌套类型");
    }

    // 验证 record: RefundPaymentCommand
    var type_RefundPaymentCommand = Type.GetType("RefundPaymentCommand");
    if (type_RefundPaymentCommand != null)
    {
        Console.WriteLine("[PASS] 类型 RefundPaymentCommand (record) 存在");
        var ctors_RefundPaymentCommand = type_RefundPaymentCommand.GetConstructors();
        Console.WriteLine($"[PASS] RefundPaymentCommand 构造函数数量: {ctors_RefundPaymentCommand.Length}");
        var methods_RefundPaymentCommand = type_RefundPaymentCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RefundPaymentCommand 公开方法数量: {methods_RefundPaymentCommand.Length}");
        foreach (var m in methods_RefundPaymentCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RefundPaymentCommand 未找到，尝试无命名空间...");
        type_RefundPaymentCommand = Type.GetType("RefundPaymentCommand");
        if (type_RefundPaymentCommand != null)
            Console.WriteLine("[PASS] 类型 RefundPaymentCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RefundPaymentCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ReleaseInventoryCommand
    var type_ReleaseInventoryCommand = Type.GetType("ReleaseInventoryCommand");
    if (type_ReleaseInventoryCommand != null)
    {
        Console.WriteLine("[PASS] 类型 ReleaseInventoryCommand (record) 存在");
        var ctors_ReleaseInventoryCommand = type_ReleaseInventoryCommand.GetConstructors();
        Console.WriteLine($"[PASS] ReleaseInventoryCommand 构造函数数量: {ctors_ReleaseInventoryCommand.Length}");
        var methods_ReleaseInventoryCommand = type_ReleaseInventoryCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReleaseInventoryCommand 公开方法数量: {methods_ReleaseInventoryCommand.Length}");
        foreach (var m in methods_ReleaseInventoryCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReleaseInventoryCommand 未找到，尝试无命名空间...");
        type_ReleaseInventoryCommand = Type.GetType("ReleaseInventoryCommand");
        if (type_ReleaseInventoryCommand != null)
            Console.WriteLine("[PASS] 类型 ReleaseInventoryCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReleaseInventoryCommand 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
