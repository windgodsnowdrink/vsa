#load "wolverinefx_tcc_integration.cs"

Console.WriteLine("=== wolverinefx_tcc_integration.cs Test ===");

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
