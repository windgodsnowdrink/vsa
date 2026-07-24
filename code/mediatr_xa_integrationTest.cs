#load "mediatr_xa_integration.cs"

Console.WriteLine("=== mediatr_xa_integration.cs Test ===");

try
{
    // 验证 class: XATransactionBehavior
    var type_XATransactionBehavior = Type.GetType("XATransactionBehavior");
    if (type_XATransactionBehavior != null)
    {
        Console.WriteLine("[PASS] 类型 XATransactionBehavior (class) 存在");
        var ctors_XATransactionBehavior = type_XATransactionBehavior.GetConstructors();
        Console.WriteLine($"[PASS] XATransactionBehavior 构造函数数量: {ctors_XATransactionBehavior.Length}");
        var methods_XATransactionBehavior = type_XATransactionBehavior.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] XATransactionBehavior 公开方法数量: {methods_XATransactionBehavior.Length}");
        foreach (var m in methods_XATransactionBehavior)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 XATransactionBehavior 未找到，尝试无命名空间...");
        type_XATransactionBehavior = Type.GetType("XATransactionBehavior");
        if (type_XATransactionBehavior != null)
            Console.WriteLine("[PASS] 类型 XATransactionBehavior (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 XATransactionBehavior 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MediatRDependencyInjectionExtensions
    var type_MediatRDependencyInjectionExtensions = Type.GetType("MediatRDependencyInjectionExtensions");
    if (type_MediatRDependencyInjectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MediatRDependencyInjectionExtensions (class) 存在");
        var ctors_MediatRDependencyInjectionExtensions = type_MediatRDependencyInjectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MediatRDependencyInjectionExtensions 构造函数数量: {ctors_MediatRDependencyInjectionExtensions.Length}");
        var methods_MediatRDependencyInjectionExtensions = type_MediatRDependencyInjectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediatRDependencyInjectionExtensions 公开方法数量: {methods_MediatRDependencyInjectionExtensions.Length}");
        foreach (var m in methods_MediatRDependencyInjectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediatRDependencyInjectionExtensions 未找到，尝试无命名空间...");
        type_MediatRDependencyInjectionExtensions = Type.GetType("MediatRDependencyInjectionExtensions");
        if (type_MediatRDependencyInjectionExtensions != null)
            Console.WriteLine("[PASS] 类型 MediatRDependencyInjectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediatRDependencyInjectionExtensions 可能为顶层语句或嵌套类型");
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
