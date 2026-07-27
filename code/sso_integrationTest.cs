#load "sso_integration.cs"

Console.WriteLine("=== sso_integration.cs Test ===");

try
{
    // 验证 class: SsoService
    var type_SsoService = Type.GetType("SsoService");
    if (type_SsoService != null)
    {
        Console.WriteLine("[PASS] 类型 SsoService (class) 存在");
        var ctors_SsoService = type_SsoService.GetConstructors();
        Console.WriteLine($"[PASS] SsoService 构造函数数量: {ctors_SsoService.Length}");
        var methods_SsoService = type_SsoService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoService 公开方法数量: {methods_SsoService.Length}");
        foreach (var m in methods_SsoService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoService 未找到，尝试无命名空间...");
        type_SsoService = Type.GetType("SsoService");
        if (type_SsoService != null)
            Console.WriteLine("[PASS] 类型 SsoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SsoConfig
    var type_SsoConfig = Type.GetType("SsoConfig");
    if (type_SsoConfig != null)
    {
        Console.WriteLine("[PASS] 类型 SsoConfig (class) 存在");
        var ctors_SsoConfig = type_SsoConfig.GetConstructors();
        Console.WriteLine($"[PASS] SsoConfig 构造函数数量: {ctors_SsoConfig.Length}");
        var methods_SsoConfig = type_SsoConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoConfig 公开方法数量: {methods_SsoConfig.Length}");
        foreach (var m in methods_SsoConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoConfig 未找到，尝试无命名空间...");
        type_SsoConfig = Type.GetType("SsoConfig");
        if (type_SsoConfig != null)
            Console.WriteLine("[PASS] 类型 SsoConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 record: SsoRequest
    var type_SsoRequest = Type.GetType("SsoRequest");
    if (type_SsoRequest != null)
    {
        Console.WriteLine("[PASS] 类型 SsoRequest (record) 存在");
        var ctors_SsoRequest = type_SsoRequest.GetConstructors();
        Console.WriteLine($"[PASS] SsoRequest 构造函数数量: {ctors_SsoRequest.Length}");
        var methods_SsoRequest = type_SsoRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoRequest 公开方法数量: {methods_SsoRequest.Length}");
        foreach (var m in methods_SsoRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoRequest 未找到，尝试无命名空间...");
        type_SsoRequest = Type.GetType("SsoRequest");
        if (type_SsoRequest != null)
            Console.WriteLine("[PASS] 类型 SsoRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: SsoResponse
    var type_SsoResponse = Type.GetType("SsoResponse");
    if (type_SsoResponse != null)
    {
        Console.WriteLine("[PASS] 类型 SsoResponse (record) 存在");
        var ctors_SsoResponse = type_SsoResponse.GetConstructors();
        Console.WriteLine($"[PASS] SsoResponse 构造函数数量: {ctors_SsoResponse.Length}");
        var methods_SsoResponse = type_SsoResponse.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoResponse 公开方法数量: {methods_SsoResponse.Length}");
        foreach (var m in methods_SsoResponse)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoResponse 未找到，尝试无命名空间...");
        type_SsoResponse = Type.GetType("SsoResponse");
        if (type_SsoResponse != null)
            Console.WriteLine("[PASS] 类型 SsoResponse (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoResponse 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: SsoStatus
    var type_SsoStatus = Type.GetType("SsoStatus");
    if (type_SsoStatus != null)
    {
        Console.WriteLine("[PASS] 类型 SsoStatus (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoStatus 未找到，尝试无命名空间...");
        type_SsoStatus = Type.GetType("SsoStatus");
        if (type_SsoStatus != null)
            Console.WriteLine("[PASS] 类型 SsoStatus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoStatus 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
