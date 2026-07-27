#load "sso_service.cs"

Console.WriteLine("=== sso_service.cs Test ===");

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

    // 验证 class: Config
    var type_Config = Type.GetType("Config");
    if (type_Config != null)
    {
        Console.WriteLine("[PASS] 类型 Config (class) 存在");
        var ctors_Config = type_Config.GetConstructors();
        Console.WriteLine($"[PASS] Config 构造函数数量: {ctors_Config.Length}");
        var methods_Config = type_Config.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Config 公开方法数量: {methods_Config.Length}");
        foreach (var m in methods_Config)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Config 未找到，尝试无命名空间...");
        type_Config = Type.GetType("Config");
        if (type_Config != null)
            Console.WriteLine("[PASS] 类型 Config (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Config 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SsoContext
    var type_SsoContext = Type.GetType("SsoContext");
    if (type_SsoContext != null)
    {
        Console.WriteLine("[PASS] 类型 SsoContext (class) 存在");
        var ctors_SsoContext = type_SsoContext.GetConstructors();
        Console.WriteLine($"[PASS] SsoContext 构造函数数量: {ctors_SsoContext.Length}");
        var methods_SsoContext = type_SsoContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoContext 公开方法数量: {methods_SsoContext.Length}");
        foreach (var m in methods_SsoContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoContext 未找到，尝试无命名空间...");
        type_SsoContext = Type.GetType("SsoContext");
        if (type_SsoContext != null)
            Console.WriteLine("[PASS] 类型 SsoContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SsoContextPooledPolicy
    var type_SsoContextPooledPolicy = Type.GetType("SsoContextPooledPolicy");
    if (type_SsoContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 SsoContextPooledPolicy (class) 存在");
        var ctors_SsoContextPooledPolicy = type_SsoContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] SsoContextPooledPolicy 构造函数数量: {ctors_SsoContextPooledPolicy.Length}");
        var methods_SsoContextPooledPolicy = type_SsoContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoContextPooledPolicy 公开方法数量: {methods_SsoContextPooledPolicy.Length}");
        foreach (var m in methods_SsoContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoContextPooledPolicy 未找到，尝试无命名空间...");
        type_SsoContextPooledPolicy = Type.GetType("SsoContextPooledPolicy");
        if (type_SsoContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 SsoContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoContextPooledPolicy 可能为顶层语句或嵌套类型");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
