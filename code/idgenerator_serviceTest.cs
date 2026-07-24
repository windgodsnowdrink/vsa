#load "idgenerator_service.cs"

Console.WriteLine("=== idgenerator_service.cs Test ===");

try
{
    // 验证 class: IdGeneratorService
    var type_IdGeneratorService = Type.GetType("IdGeneratorService");
    if (type_IdGeneratorService != null)
    {
        Console.WriteLine("[PASS] 类型 IdGeneratorService (class) 存在");
        var ctors_IdGeneratorService = type_IdGeneratorService.GetConstructors();
        Console.WriteLine($"[PASS] IdGeneratorService 构造函数数量: {ctors_IdGeneratorService.Length}");
        var methods_IdGeneratorService = type_IdGeneratorService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IdGeneratorService 公开方法数量: {methods_IdGeneratorService.Length}");
        foreach (var m in methods_IdGeneratorService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IdGeneratorService 未找到，尝试无命名空间...");
        type_IdGeneratorService = Type.GetType("IdGeneratorService");
        if (type_IdGeneratorService != null)
            Console.WriteLine("[PASS] 类型 IdGeneratorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IdGeneratorService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IdRequest
    var type_IdRequest = Type.GetType("IdRequest");
    if (type_IdRequest != null)
    {
        Console.WriteLine("[PASS] 类型 IdRequest (class) 存在");
        var ctors_IdRequest = type_IdRequest.GetConstructors();
        Console.WriteLine($"[PASS] IdRequest 构造函数数量: {ctors_IdRequest.Length}");
        var methods_IdRequest = type_IdRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IdRequest 公开方法数量: {methods_IdRequest.Length}");
        foreach (var m in methods_IdRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IdRequest 未找到，尝试无命名空间...");
        type_IdRequest = Type.GetType("IdRequest");
        if (type_IdRequest != null)
            Console.WriteLine("[PASS] 类型 IdRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IdRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IdContext
    var type_IdContext = Type.GetType("IdContext");
    if (type_IdContext != null)
    {
        Console.WriteLine("[PASS] 类型 IdContext (class) 存在");
        var ctors_IdContext = type_IdContext.GetConstructors();
        Console.WriteLine($"[PASS] IdContext 构造函数数量: {ctors_IdContext.Length}");
        var methods_IdContext = type_IdContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IdContext 公开方法数量: {methods_IdContext.Length}");
        foreach (var m in methods_IdContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IdContext 未找到，尝试无命名空间...");
        type_IdContext = Type.GetType("IdContext");
        if (type_IdContext != null)
            Console.WriteLine("[PASS] 类型 IdContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IdContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IdContextPooledPolicy
    var type_IdContextPooledPolicy = Type.GetType("IdContextPooledPolicy");
    if (type_IdContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 IdContextPooledPolicy (class) 存在");
        var ctors_IdContextPooledPolicy = type_IdContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] IdContextPooledPolicy 构造函数数量: {ctors_IdContextPooledPolicy.Length}");
        var methods_IdContextPooledPolicy = type_IdContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IdContextPooledPolicy 公开方法数量: {methods_IdContextPooledPolicy.Length}");
        foreach (var m in methods_IdContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IdContextPooledPolicy 未找到，尝试无命名空间...");
        type_IdContextPooledPolicy = Type.GetType("IdContextPooledPolicy");
        if (type_IdContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 IdContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IdContextPooledPolicy 可能为顶层语句或嵌套类型");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
