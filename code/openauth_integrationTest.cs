#load "openauth_integration.cs"

Console.WriteLine("=== openauth_integration.cs Test ===");

try
{
    // 验证 class: OpenAuthAdapter
    var type_OpenAuthAdapter = Type.GetType("OpenAuthAdapter");
    if (type_OpenAuthAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 OpenAuthAdapter (class) 存在");
        var ctors_OpenAuthAdapter = type_OpenAuthAdapter.GetConstructors();
        Console.WriteLine($"[PASS] OpenAuthAdapter 构造函数数量: {ctors_OpenAuthAdapter.Length}");
        var methods_OpenAuthAdapter = type_OpenAuthAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OpenAuthAdapter 公开方法数量: {methods_OpenAuthAdapter.Length}");
        foreach (var m in methods_OpenAuthAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OpenAuthAdapter 未找到，尝试无命名空间...");
        type_OpenAuthAdapter = Type.GetType("OpenAuthAdapter");
        if (type_OpenAuthAdapter != null)
            Console.WriteLine("[PASS] 类型 OpenAuthAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OpenAuthAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OpenAuthConfig
    var type_OpenAuthConfig = Type.GetType("OpenAuthConfig");
    if (type_OpenAuthConfig != null)
    {
        Console.WriteLine("[PASS] 类型 OpenAuthConfig (class) 存在");
        var ctors_OpenAuthConfig = type_OpenAuthConfig.GetConstructors();
        Console.WriteLine($"[PASS] OpenAuthConfig 构造函数数量: {ctors_OpenAuthConfig.Length}");
        var methods_OpenAuthConfig = type_OpenAuthConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OpenAuthConfig 公开方法数量: {methods_OpenAuthConfig.Length}");
        foreach (var m in methods_OpenAuthConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OpenAuthConfig 未找到，尝试无命名空间...");
        type_OpenAuthConfig = Type.GetType("OpenAuthConfig");
        if (type_OpenAuthConfig != null)
            Console.WriteLine("[PASS] 类型 OpenAuthConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OpenAuthConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 record: AuthRequest
    var type_AuthRequest = Type.GetType("AuthRequest");
    if (type_AuthRequest != null)
    {
        Console.WriteLine("[PASS] 类型 AuthRequest (record) 存在");
        var ctors_AuthRequest = type_AuthRequest.GetConstructors();
        Console.WriteLine($"[PASS] AuthRequest 构造函数数量: {ctors_AuthRequest.Length}");
        var methods_AuthRequest = type_AuthRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuthRequest 公开方法数量: {methods_AuthRequest.Length}");
        foreach (var m in methods_AuthRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuthRequest 未找到，尝试无命名空间...");
        type_AuthRequest = Type.GetType("AuthRequest");
        if (type_AuthRequest != null)
            Console.WriteLine("[PASS] 类型 AuthRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuthRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: AuthResult
    var type_AuthResult = Type.GetType("AuthResult");
    if (type_AuthResult != null)
    {
        Console.WriteLine("[PASS] 类型 AuthResult (record) 存在");
        var ctors_AuthResult = type_AuthResult.GetConstructors();
        Console.WriteLine($"[PASS] AuthResult 构造函数数量: {ctors_AuthResult.Length}");
        var methods_AuthResult = type_AuthResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuthResult 公开方法数量: {methods_AuthResult.Length}");
        foreach (var m in methods_AuthResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuthResult 未找到，尝试无命名空间...");
        type_AuthResult = Type.GetType("AuthResult");
        if (type_AuthResult != null)
            Console.WriteLine("[PASS] 类型 AuthResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuthResult 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
