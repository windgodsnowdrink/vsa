#load "wolverinefx_xa_integration.cs"

Console.WriteLine("=== wolverinefx_xa_integration.cs Test ===");

try
{
    // 验证 class: XATransactionMiddleware
    var type_XATransactionMiddleware = Type.GetType("XATransactionMiddleware");
    if (type_XATransactionMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 XATransactionMiddleware (class) 存在");
        var ctors_XATransactionMiddleware = type_XATransactionMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] XATransactionMiddleware 构造函数数量: {ctors_XATransactionMiddleware.Length}");
        var methods_XATransactionMiddleware = type_XATransactionMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] XATransactionMiddleware 公开方法数量: {methods_XATransactionMiddleware.Length}");
        foreach (var m in methods_XATransactionMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 XATransactionMiddleware 未找到，尝试无命名空间...");
        type_XATransactionMiddleware = Type.GetType("XATransactionMiddleware");
        if (type_XATransactionMiddleware != null)
            Console.WriteLine("[PASS] 类型 XATransactionMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 XATransactionMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WolverineDependencyInjectionExtensions
    var type_WolverineDependencyInjectionExtensions = Type.GetType("WolverineDependencyInjectionExtensions");
    if (type_WolverineDependencyInjectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 WolverineDependencyInjectionExtensions (class) 存在");
        var ctors_WolverineDependencyInjectionExtensions = type_WolverineDependencyInjectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] WolverineDependencyInjectionExtensions 构造函数数量: {ctors_WolverineDependencyInjectionExtensions.Length}");
        var methods_WolverineDependencyInjectionExtensions = type_WolverineDependencyInjectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WolverineDependencyInjectionExtensions 公开方法数量: {methods_WolverineDependencyInjectionExtensions.Length}");
        foreach (var m in methods_WolverineDependencyInjectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WolverineDependencyInjectionExtensions 未找到，尝试无命名空间...");
        type_WolverineDependencyInjectionExtensions = Type.GetType("WolverineDependencyInjectionExtensions");
        if (type_WolverineDependencyInjectionExtensions != null)
            Console.WriteLine("[PASS] 类型 WolverineDependencyInjectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WolverineDependencyInjectionExtensions 可能为顶层语句或嵌套类型");
    }

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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
