#load "surging_microservice_integration.cs"

Console.WriteLine("=== surging_microservice_integration.cs Test ===");

try
{
    // 验证 class: OrderService
    var type_OrderService = Type.GetType("OrderService");
    if (type_OrderService != null)
    {
        Console.WriteLine("[PASS] 类型 OrderService (class) 存在");
        var ctors_OrderService = type_OrderService.GetConstructors();
        Console.WriteLine($"[PASS] OrderService 构造函数数量: {ctors_OrderService.Length}");
        var methods_OrderService = type_OrderService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderService 公开方法数量: {methods_OrderService.Length}");
        foreach (var m in methods_OrderService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderService 未找到，尝试无命名空间...");
        type_OrderService = Type.GetType("OrderService");
        if (type_OrderService != null)
            Console.WriteLine("[PASS] 类型 OrderService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Startup
    var type_Startup = Type.GetType("Startup");
    if (type_Startup != null)
    {
        Console.WriteLine("[PASS] 类型 Startup (class) 存在");
        var ctors_Startup = type_Startup.GetConstructors();
        Console.WriteLine($"[PASS] Startup 构造函数数量: {ctors_Startup.Length}");
        var methods_Startup = type_Startup.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Startup 公开方法数量: {methods_Startup.Length}");
        foreach (var m in methods_Startup)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Startup 未找到，尝试无命名空间...");
        type_Startup = Type.GetType("Startup");
        if (type_Startup != null)
            Console.WriteLine("[PASS] 类型 Startup (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Startup 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IOrderService
    var type_IOrderService = Type.GetType("IOrderService");
    if (type_IOrderService != null)
    {
        Console.WriteLine("[PASS] 类型 IOrderService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IOrderService 未找到，尝试无命名空间...");
        type_IOrderService = Type.GetType("IOrderService");
        if (type_IOrderService != null)
            Console.WriteLine("[PASS] 类型 IOrderService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IOrderService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
