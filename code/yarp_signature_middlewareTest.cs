#load "yarp_signature_middleware.cs"

Console.WriteLine("=== yarp_signature_middleware.cs Test ===");

try
{
    // 验证 class: SignatureMiddleware
    var type_SignatureMiddleware = Type.GetType("SignatureMiddleware");
    if (type_SignatureMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 SignatureMiddleware (class) 存在");
        var ctors_SignatureMiddleware = type_SignatureMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] SignatureMiddleware 构造函数数量: {ctors_SignatureMiddleware.Length}");
        var methods_SignatureMiddleware = type_SignatureMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SignatureMiddleware 公开方法数量: {methods_SignatureMiddleware.Length}");
        foreach (var m in methods_SignatureMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SignatureMiddleware 未找到，尝试无命名空间...");
        type_SignatureMiddleware = Type.GetType("SignatureMiddleware");
        if (type_SignatureMiddleware != null)
            Console.WriteLine("[PASS] 类型 SignatureMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SignatureMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SignatureOptions
    var type_SignatureOptions = Type.GetType("SignatureOptions");
    if (type_SignatureOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SignatureOptions (class) 存在");
        var ctors_SignatureOptions = type_SignatureOptions.GetConstructors();
        Console.WriteLine($"[PASS] SignatureOptions 构造函数数量: {ctors_SignatureOptions.Length}");
        var methods_SignatureOptions = type_SignatureOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SignatureOptions 公开方法数量: {methods_SignatureOptions.Length}");
        foreach (var m in methods_SignatureOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SignatureOptions 未找到，尝试无命名空间...");
        type_SignatureOptions = Type.GetType("SignatureOptions");
        if (type_SignatureOptions != null)
            Console.WriteLine("[PASS] 类型 SignatureOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SignatureOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
