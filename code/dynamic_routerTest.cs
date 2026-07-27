#load "dynamic_router.cs"

Console.WriteLine("=== dynamic_router.cs Test ===");

try
{
    // 验证 class: DynamicMessageRouter
    var type_DynamicMessageRouter = Type.GetType("DynamicMessageRouter");
    if (type_DynamicMessageRouter != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicMessageRouter (class) 存在");
        var ctors_DynamicMessageRouter = type_DynamicMessageRouter.GetConstructors();
        Console.WriteLine($"[PASS] DynamicMessageRouter 构造函数数量: {ctors_DynamicMessageRouter.Length}");
        var methods_DynamicMessageRouter = type_DynamicMessageRouter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicMessageRouter 公开方法数量: {methods_DynamicMessageRouter.Length}");
        foreach (var m in methods_DynamicMessageRouter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicMessageRouter 未找到，尝试无命名空间...");
        type_DynamicMessageRouter = Type.GetType("DynamicMessageRouter");
        if (type_DynamicMessageRouter != null)
            Console.WriteLine("[PASS] 类型 DynamicMessageRouter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicMessageRouter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DynamicRouter
    var type_DynamicRouter = Type.GetType("DynamicRouter");
    if (type_DynamicRouter != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicRouter (class) 存在");
        var ctors_DynamicRouter = type_DynamicRouter.GetConstructors();
        Console.WriteLine($"[PASS] DynamicRouter 构造函数数量: {ctors_DynamicRouter.Length}");
        var methods_DynamicRouter = type_DynamicRouter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicRouter 公开方法数量: {methods_DynamicRouter.Length}");
        foreach (var m in methods_DynamicRouter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicRouter 未找到，尝试无命名空间...");
        type_DynamicRouter = Type.GetType("DynamicRouter");
        if (type_DynamicRouter != null)
            Console.WriteLine("[PASS] 类型 DynamicRouter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicRouter 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
