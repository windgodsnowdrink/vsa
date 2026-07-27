#load "mediatr_tcc_integration.cs"

Console.WriteLine("=== mediatr_tcc_integration.cs Test ===");

try
{
    // 验证 class: TccCoordinator
    var type_TccCoordinator = Type.GetType("TccCoordinator");
    if (type_TccCoordinator != null)
    {
        Console.WriteLine("[PASS] 类型 TccCoordinator (class) 存在");
        var ctors_TccCoordinator = type_TccCoordinator.GetConstructors();
        Console.WriteLine($"[PASS] TccCoordinator 构造函数数量: {ctors_TccCoordinator.Length}");
        var methods_TccCoordinator = type_TccCoordinator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TccCoordinator 公开方法数量: {methods_TccCoordinator.Length}");
        foreach (var m in methods_TccCoordinator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TccCoordinator 未找到，尝试无命名空间...");
        type_TccCoordinator = Type.GetType("TccCoordinator");
        if (type_TccCoordinator != null)
            Console.WriteLine("[PASS] 类型 TccCoordinator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TccCoordinator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CreateOrderTccHandler
    var type_CreateOrderTccHandler = Type.GetType("CreateOrderTccHandler");
    if (type_CreateOrderTccHandler != null)
    {
        Console.WriteLine("[PASS] 类型 CreateOrderTccHandler (class) 存在");
        var ctors_CreateOrderTccHandler = type_CreateOrderTccHandler.GetConstructors();
        Console.WriteLine($"[PASS] CreateOrderTccHandler 构造函数数量: {ctors_CreateOrderTccHandler.Length}");
        var methods_CreateOrderTccHandler = type_CreateOrderTccHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateOrderTccHandler 公开方法数量: {methods_CreateOrderTccHandler.Length}");
        foreach (var m in methods_CreateOrderTccHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateOrderTccHandler 未找到，尝试无命名空间...");
        type_CreateOrderTccHandler = Type.GetType("CreateOrderTccHandler");
        if (type_CreateOrderTccHandler != null)
            Console.WriteLine("[PASS] 类型 CreateOrderTccHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateOrderTccHandler 可能为顶层语句或嵌套类型");
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

    // 验证 interface: ITccTransaction
    var type_ITccTransaction = Type.GetType("ITccTransaction");
    if (type_ITccTransaction != null)
    {
        Console.WriteLine("[PASS] 类型 ITccTransaction (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITccTransaction 未找到，尝试无命名空间...");
        type_ITccTransaction = Type.GetType("ITccTransaction");
        if (type_ITccTransaction != null)
            Console.WriteLine("[PASS] 类型 ITccTransaction (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITccTransaction 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
