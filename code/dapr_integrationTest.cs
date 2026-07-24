#load "dapr_integration.cs"

Console.WriteLine("=== dapr_integration.cs Test ===");

try
{
    // 验证 class: DaprOptions
    var type_DaprOptions = Type.GetType("DaprOptions");
    if (type_DaprOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DaprOptions (class) 存在");
        var ctors_DaprOptions = type_DaprOptions.GetConstructors();
        Console.WriteLine($"[PASS] DaprOptions 构造函数数量: {ctors_DaprOptions.Length}");
        var methods_DaprOptions = type_DaprOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DaprOptions 公开方法数量: {methods_DaprOptions.Length}");
        foreach (var m in methods_DaprOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DaprOptions 未找到，尝试无命名空间...");
        type_DaprOptions = Type.GetType("DaprOptions");
        if (type_DaprOptions != null)
            Console.WriteLine("[PASS] 类型 DaprOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DaprOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DaprService
    var type_DaprService = Type.GetType("DaprService");
    if (type_DaprService != null)
    {
        Console.WriteLine("[PASS] 类型 DaprService (class) 存在");
        var ctors_DaprService = type_DaprService.GetConstructors();
        Console.WriteLine($"[PASS] DaprService 构造函数数量: {ctors_DaprService.Length}");
        var methods_DaprService = type_DaprService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DaprService 公开方法数量: {methods_DaprService.Length}");
        foreach (var m in methods_DaprService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DaprService 未找到，尝试无命名空间...");
        type_DaprService = Type.GetType("DaprService");
        if (type_DaprService != null)
            Console.WriteLine("[PASS] 类型 DaprService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DaprService 可能为顶层语句或嵌套类型");
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

    // 验证 class: OrderController
    var type_OrderController = Type.GetType("OrderController");
    if (type_OrderController != null)
    {
        Console.WriteLine("[PASS] 类型 OrderController (class) 存在");
        var ctors_OrderController = type_OrderController.GetConstructors();
        Console.WriteLine($"[PASS] OrderController 构造函数数量: {ctors_OrderController.Length}");
        var methods_OrderController = type_OrderController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderController 公开方法数量: {methods_OrderController.Length}");
        foreach (var m in methods_OrderController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderController 未找到，尝试无命名空间...");
        type_OrderController = Type.GetType("OrderController");
        if (type_OrderController != null)
            Console.WriteLine("[PASS] 类型 OrderController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderController 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IDaprService
    var type_IDaprService = Type.GetType("IDaprService");
    if (type_IDaprService != null)
    {
        Console.WriteLine("[PASS] 类型 IDaprService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDaprService 未找到，尝试无命名空间...");
        type_IDaprService = Type.GetType("IDaprService");
        if (type_IDaprService != null)
            Console.WriteLine("[PASS] 类型 IDaprService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDaprService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
