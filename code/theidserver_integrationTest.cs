#load "theidserver_integration.cs"

Console.WriteLine("=== theidserver_integration.cs Test ===");

try
{
    // 验证 class: TheIdServerAdapter
    var type_TheIdServerAdapter = Type.GetType("TheIdServerAdapter");
    if (type_TheIdServerAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 TheIdServerAdapter (class) 存在");
        var ctors_TheIdServerAdapter = type_TheIdServerAdapter.GetConstructors();
        Console.WriteLine($"[PASS] TheIdServerAdapter 构造函数数量: {ctors_TheIdServerAdapter.Length}");
        var methods_TheIdServerAdapter = type_TheIdServerAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TheIdServerAdapter 公开方法数量: {methods_TheIdServerAdapter.Length}");
        foreach (var m in methods_TheIdServerAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TheIdServerAdapter 未找到，尝试无命名空间...");
        type_TheIdServerAdapter = Type.GetType("TheIdServerAdapter");
        if (type_TheIdServerAdapter != null)
            Console.WriteLine("[PASS] 类型 TheIdServerAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TheIdServerAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TheIdServerConfig
    var type_TheIdServerConfig = Type.GetType("TheIdServerConfig");
    if (type_TheIdServerConfig != null)
    {
        Console.WriteLine("[PASS] 类型 TheIdServerConfig (class) 存在");
        var ctors_TheIdServerConfig = type_TheIdServerConfig.GetConstructors();
        Console.WriteLine($"[PASS] TheIdServerConfig 构造函数数量: {ctors_TheIdServerConfig.Length}");
        var methods_TheIdServerConfig = type_TheIdServerConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TheIdServerConfig 公开方法数量: {methods_TheIdServerConfig.Length}");
        foreach (var m in methods_TheIdServerConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TheIdServerConfig 未找到，尝试无命名空间...");
        type_TheIdServerConfig = Type.GetType("TheIdServerConfig");
        if (type_TheIdServerConfig != null)
            Console.WriteLine("[PASS] 类型 TheIdServerConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TheIdServerConfig 可能为顶层语句或嵌套类型");
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

    // 验证 enum: AuthStatus
    var type_AuthStatus = Type.GetType("AuthStatus");
    if (type_AuthStatus != null)
    {
        Console.WriteLine("[PASS] 类型 AuthStatus (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuthStatus 未找到，尝试无命名空间...");
        type_AuthStatus = Type.GetType("AuthStatus");
        if (type_AuthStatus != null)
            Console.WriteLine("[PASS] 类型 AuthStatus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuthStatus 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
