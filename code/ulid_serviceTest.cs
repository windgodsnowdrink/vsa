#load "ulid_service.cs"

Console.WriteLine("=== ulid_service.cs Test ===");

try
{
    // 验证 class: UlidGenerator
    var type_UlidGenerator = Type.GetType("UlidGenerator");
    if (type_UlidGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 UlidGenerator (class) 存在");
        var ctors_UlidGenerator = type_UlidGenerator.GetConstructors();
        Console.WriteLine($"[PASS] UlidGenerator 构造函数数量: {ctors_UlidGenerator.Length}");
        var methods_UlidGenerator = type_UlidGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UlidGenerator 公开方法数量: {methods_UlidGenerator.Length}");
        foreach (var m in methods_UlidGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UlidGenerator 未找到，尝试无命名空间...");
        type_UlidGenerator = Type.GetType("UlidGenerator");
        if (type_UlidGenerator != null)
            Console.WriteLine("[PASS] 类型 UlidGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UlidGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UlidRequest
    var type_UlidRequest = Type.GetType("UlidRequest");
    if (type_UlidRequest != null)
    {
        Console.WriteLine("[PASS] 类型 UlidRequest (class) 存在");
        var ctors_UlidRequest = type_UlidRequest.GetConstructors();
        Console.WriteLine($"[PASS] UlidRequest 构造函数数量: {ctors_UlidRequest.Length}");
        var methods_UlidRequest = type_UlidRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UlidRequest 公开方法数量: {methods_UlidRequest.Length}");
        foreach (var m in methods_UlidRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UlidRequest 未找到，尝试无命名空间...");
        type_UlidRequest = Type.GetType("UlidRequest");
        if (type_UlidRequest != null)
            Console.WriteLine("[PASS] 类型 UlidRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UlidRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UlidContext
    var type_UlidContext = Type.GetType("UlidContext");
    if (type_UlidContext != null)
    {
        Console.WriteLine("[PASS] 类型 UlidContext (class) 存在");
        var ctors_UlidContext = type_UlidContext.GetConstructors();
        Console.WriteLine($"[PASS] UlidContext 构造函数数量: {ctors_UlidContext.Length}");
        var methods_UlidContext = type_UlidContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UlidContext 公开方法数量: {methods_UlidContext.Length}");
        foreach (var m in methods_UlidContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UlidContext 未找到，尝试无命名空间...");
        type_UlidContext = Type.GetType("UlidContext");
        if (type_UlidContext != null)
            Console.WriteLine("[PASS] 类型 UlidContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UlidContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UlidContextPooledPolicy
    var type_UlidContextPooledPolicy = Type.GetType("UlidContextPooledPolicy");
    if (type_UlidContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 UlidContextPooledPolicy (class) 存在");
        var ctors_UlidContextPooledPolicy = type_UlidContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] UlidContextPooledPolicy 构造函数数量: {ctors_UlidContextPooledPolicy.Length}");
        var methods_UlidContextPooledPolicy = type_UlidContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UlidContextPooledPolicy 公开方法数量: {methods_UlidContextPooledPolicy.Length}");
        foreach (var m in methods_UlidContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UlidContextPooledPolicy 未找到，尝试无命名空间...");
        type_UlidContextPooledPolicy = Type.GetType("UlidContextPooledPolicy");
        if (type_UlidContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 UlidContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UlidContextPooledPolicy 可能为顶层语句或嵌套类型");
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
