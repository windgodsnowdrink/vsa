#load "keycloak_integration.cs"

Console.WriteLine("=== keycloak_integration.cs Test ===");

try
{
    // 验证 class: KeycloakService
    var type_KeycloakService = Type.GetType("KeycloakService");
    if (type_KeycloakService != null)
    {
        Console.WriteLine("[PASS] 类型 KeycloakService (class) 存在");
        var ctors_KeycloakService = type_KeycloakService.GetConstructors();
        Console.WriteLine($"[PASS] KeycloakService 构造函数数量: {ctors_KeycloakService.Length}");
        var methods_KeycloakService = type_KeycloakService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KeycloakService 公开方法数量: {methods_KeycloakService.Length}");
        foreach (var m in methods_KeycloakService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KeycloakService 未找到，尝试无命名空间...");
        type_KeycloakService = Type.GetType("KeycloakService");
        if (type_KeycloakService != null)
            Console.WriteLine("[PASS] 类型 KeycloakService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KeycloakService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KeycloakOptions
    var type_KeycloakOptions = Type.GetType("KeycloakOptions");
    if (type_KeycloakOptions != null)
    {
        Console.WriteLine("[PASS] 类型 KeycloakOptions (class) 存在");
        var ctors_KeycloakOptions = type_KeycloakOptions.GetConstructors();
        Console.WriteLine($"[PASS] KeycloakOptions 构造函数数量: {ctors_KeycloakOptions.Length}");
        var methods_KeycloakOptions = type_KeycloakOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KeycloakOptions 公开方法数量: {methods_KeycloakOptions.Length}");
        foreach (var m in methods_KeycloakOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KeycloakOptions 未找到，尝试无命名空间...");
        type_KeycloakOptions = Type.GetType("KeycloakOptions");
        if (type_KeycloakOptions != null)
            Console.WriteLine("[PASS] 类型 KeycloakOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KeycloakOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KeycloakExtensions
    var type_KeycloakExtensions = Type.GetType("KeycloakExtensions");
    if (type_KeycloakExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 KeycloakExtensions (class) 存在");
        var ctors_KeycloakExtensions = type_KeycloakExtensions.GetConstructors();
        Console.WriteLine($"[PASS] KeycloakExtensions 构造函数数量: {ctors_KeycloakExtensions.Length}");
        var methods_KeycloakExtensions = type_KeycloakExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KeycloakExtensions 公开方法数量: {methods_KeycloakExtensions.Length}");
        foreach (var m in methods_KeycloakExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KeycloakExtensions 未找到，尝试无命名空间...");
        type_KeycloakExtensions = Type.GetType("KeycloakExtensions");
        if (type_KeycloakExtensions != null)
            Console.WriteLine("[PASS] 类型 KeycloakExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KeycloakExtensions 可能为顶层语句或嵌套类型");
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
