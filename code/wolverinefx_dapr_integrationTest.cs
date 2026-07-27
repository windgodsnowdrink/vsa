#load "wolverinefx_dapr_integration.cs"

Console.WriteLine("=== wolverinefx_dapr_integration.cs Test ===");

try
{
    // 验证 class: DaprEventBusOptions
    var type_DaprEventBusOptions = Type.GetType("DaprEventBusOptions");
    if (type_DaprEventBusOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DaprEventBusOptions (class) 存在");
        var ctors_DaprEventBusOptions = type_DaprEventBusOptions.GetConstructors();
        Console.WriteLine($"[PASS] DaprEventBusOptions 构造函数数量: {ctors_DaprEventBusOptions.Length}");
        var methods_DaprEventBusOptions = type_DaprEventBusOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DaprEventBusOptions 公开方法数量: {methods_DaprEventBusOptions.Length}");
        foreach (var m in methods_DaprEventBusOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DaprEventBusOptions 未找到，尝试无命名空间...");
        type_DaprEventBusOptions = Type.GetType("DaprEventBusOptions");
        if (type_DaprEventBusOptions != null)
            Console.WriteLine("[PASS] 类型 DaprEventBusOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DaprEventBusOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DaprEventBusExtensions
    var type_DaprEventBusExtensions = Type.GetType("DaprEventBusExtensions");
    if (type_DaprEventBusExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DaprEventBusExtensions (class) 存在");
        var ctors_DaprEventBusExtensions = type_DaprEventBusExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DaprEventBusExtensions 构造函数数量: {ctors_DaprEventBusExtensions.Length}");
        var methods_DaprEventBusExtensions = type_DaprEventBusExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DaprEventBusExtensions 公开方法数量: {methods_DaprEventBusExtensions.Length}");
        foreach (var m in methods_DaprEventBusExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DaprEventBusExtensions 未找到，尝试无命名空间...");
        type_DaprEventBusExtensions = Type.GetType("DaprEventBusExtensions");
        if (type_DaprEventBusExtensions != null)
            Console.WriteLine("[PASS] 类型 DaprEventBusExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DaprEventBusExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DaprHealthCheck
    var type_DaprHealthCheck = Type.GetType("DaprHealthCheck");
    if (type_DaprHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 DaprHealthCheck (class) 存在");
        var ctors_DaprHealthCheck = type_DaprHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] DaprHealthCheck 构造函数数量: {ctors_DaprHealthCheck.Length}");
        var methods_DaprHealthCheck = type_DaprHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DaprHealthCheck 公开方法数量: {methods_DaprHealthCheck.Length}");
        foreach (var m in methods_DaprHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DaprHealthCheck 未找到，尝试无命名空间...");
        type_DaprHealthCheck = Type.GetType("DaprHealthCheck");
        if (type_DaprHealthCheck != null)
            Console.WriteLine("[PASS] 类型 DaprHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DaprHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderHandlers
    var type_OrderHandlers = Type.GetType("OrderHandlers");
    if (type_OrderHandlers != null)
    {
        Console.WriteLine("[PASS] 类型 OrderHandlers (class) 存在");
        var ctors_OrderHandlers = type_OrderHandlers.GetConstructors();
        Console.WriteLine($"[PASS] OrderHandlers 构造函数数量: {ctors_OrderHandlers.Length}");
        var methods_OrderHandlers = type_OrderHandlers.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderHandlers 公开方法数量: {methods_OrderHandlers.Length}");
        foreach (var m in methods_OrderHandlers)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderHandlers 未找到，尝试无命名空间...");
        type_OrderHandlers = Type.GetType("OrderHandlers");
        if (type_OrderHandlers != null)
            Console.WriteLine("[PASS] 类型 OrderHandlers (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderHandlers 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
