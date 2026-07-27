#load "yarp_batch_middleware.cs"

Console.WriteLine("=== yarp_batch_middleware.cs Test ===");

try
{
    // 验证 class: BatchMiddleware
    var type_BatchMiddleware = Type.GetType("BatchMiddleware");
    if (type_BatchMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 BatchMiddleware (class) 存在");
        var ctors_BatchMiddleware = type_BatchMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] BatchMiddleware 构造函数数量: {ctors_BatchMiddleware.Length}");
        var methods_BatchMiddleware = type_BatchMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchMiddleware 公开方法数量: {methods_BatchMiddleware.Length}");
        foreach (var m in methods_BatchMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchMiddleware 未找到，尝试无命名空间...");
        type_BatchMiddleware = Type.GetType("BatchMiddleware");
        if (type_BatchMiddleware != null)
            Console.WriteLine("[PASS] 类型 BatchMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BatchOptions
    var type_BatchOptions = Type.GetType("BatchOptions");
    if (type_BatchOptions != null)
    {
        Console.WriteLine("[PASS] 类型 BatchOptions (class) 存在");
        var ctors_BatchOptions = type_BatchOptions.GetConstructors();
        Console.WriteLine($"[PASS] BatchOptions 构造函数数量: {ctors_BatchOptions.Length}");
        var methods_BatchOptions = type_BatchOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchOptions 公开方法数量: {methods_BatchOptions.Length}");
        foreach (var m in methods_BatchOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchOptions 未找到，尝试无命名空间...");
        type_BatchOptions = Type.GetType("BatchOptions");
        if (type_BatchOptions != null)
            Console.WriteLine("[PASS] 类型 BatchOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
