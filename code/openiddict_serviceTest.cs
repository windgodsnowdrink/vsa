#load "openiddict_service.cs"

Console.WriteLine("=== openiddict_service.cs Test ===");

try
{
    // 验证 class: PasswordGrantHandler
    var type_PasswordGrantHandler = Type.GetType("PasswordGrantHandler");
    if (type_PasswordGrantHandler != null)
    {
        Console.WriteLine("[PASS] 类型 PasswordGrantHandler (class) 存在");
        var ctors_PasswordGrantHandler = type_PasswordGrantHandler.GetConstructors();
        Console.WriteLine($"[PASS] PasswordGrantHandler 构造函数数量: {ctors_PasswordGrantHandler.Length}");
        var methods_PasswordGrantHandler = type_PasswordGrantHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PasswordGrantHandler 公开方法数量: {methods_PasswordGrantHandler.Length}");
        foreach (var m in methods_PasswordGrantHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PasswordGrantHandler 未找到，尝试无命名空间...");
        type_PasswordGrantHandler = Type.GetType("PasswordGrantHandler");
        if (type_PasswordGrantHandler != null)
            Console.WriteLine("[PASS] 类型 PasswordGrantHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PasswordGrantHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuthorizationCodeGenerator
    var type_AuthorizationCodeGenerator = Type.GetType("AuthorizationCodeGenerator");
    if (type_AuthorizationCodeGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 AuthorizationCodeGenerator (class) 存在");
        var ctors_AuthorizationCodeGenerator = type_AuthorizationCodeGenerator.GetConstructors();
        Console.WriteLine($"[PASS] AuthorizationCodeGenerator 构造函数数量: {ctors_AuthorizationCodeGenerator.Length}");
        var methods_AuthorizationCodeGenerator = type_AuthorizationCodeGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuthorizationCodeGenerator 公开方法数量: {methods_AuthorizationCodeGenerator.Length}");
        foreach (var m in methods_AuthorizationCodeGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuthorizationCodeGenerator 未找到，尝试无命名空间...");
        type_AuthorizationCodeGenerator = Type.GetType("AuthorizationCodeGenerator");
        if (type_AuthorizationCodeGenerator != null)
            Console.WriteLine("[PASS] 类型 AuthorizationCodeGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuthorizationCodeGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RefreshTokenHandler
    var type_RefreshTokenHandler = Type.GetType("RefreshTokenHandler");
    if (type_RefreshTokenHandler != null)
    {
        Console.WriteLine("[PASS] 类型 RefreshTokenHandler (class) 存在");
        var ctors_RefreshTokenHandler = type_RefreshTokenHandler.GetConstructors();
        Console.WriteLine($"[PASS] RefreshTokenHandler 构造函数数量: {ctors_RefreshTokenHandler.Length}");
        var methods_RefreshTokenHandler = type_RefreshTokenHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RefreshTokenHandler 公开方法数量: {methods_RefreshTokenHandler.Length}");
        foreach (var m in methods_RefreshTokenHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RefreshTokenHandler 未找到，尝试无命名空间...");
        type_RefreshTokenHandler = Type.GetType("RefreshTokenHandler");
        if (type_RefreshTokenHandler != null)
            Console.WriteLine("[PASS] 类型 RefreshTokenHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RefreshTokenHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OpenIddictConfig
    var type_OpenIddictConfig = Type.GetType("OpenIddictConfig");
    if (type_OpenIddictConfig != null)
    {
        Console.WriteLine("[PASS] 类型 OpenIddictConfig (class) 存在");
        var ctors_OpenIddictConfig = type_OpenIddictConfig.GetConstructors();
        Console.WriteLine($"[PASS] OpenIddictConfig 构造函数数量: {ctors_OpenIddictConfig.Length}");
        var methods_OpenIddictConfig = type_OpenIddictConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OpenIddictConfig 公开方法数量: {methods_OpenIddictConfig.Length}");
        foreach (var m in methods_OpenIddictConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OpenIddictConfig 未找到，尝试无命名空间...");
        type_OpenIddictConfig = Type.GetType("OpenIddictConfig");
        if (type_OpenIddictConfig != null)
            Console.WriteLine("[PASS] 类型 OpenIddictConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OpenIddictConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AppJsonSerializerContext
    var type_AppJsonSerializerContext = Type.GetType("AppJsonSerializerContext");
    if (type_AppJsonSerializerContext != null)
    {
        Console.WriteLine("[PASS] 类型 AppJsonSerializerContext (class) 存在");
        var ctors_AppJsonSerializerContext = type_AppJsonSerializerContext.GetConstructors();
        Console.WriteLine($"[PASS] AppJsonSerializerContext 构造函数数量: {ctors_AppJsonSerializerContext.Length}");
        var methods_AppJsonSerializerContext = type_AppJsonSerializerContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AppJsonSerializerContext 公开方法数量: {methods_AppJsonSerializerContext.Length}");
        foreach (var m in methods_AppJsonSerializerContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AppJsonSerializerContext 未找到，尝试无命名空间...");
        type_AppJsonSerializerContext = Type.GetType("AppJsonSerializerContext");
        if (type_AppJsonSerializerContext != null)
            Console.WriteLine("[PASS] 类型 AppJsonSerializerContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AppJsonSerializerContext 可能为顶层语句或嵌套类型");
    }

    // 验证 record: RefreshTokenPayload
    var type_RefreshTokenPayload = Type.GetType("RefreshTokenPayload");
    if (type_RefreshTokenPayload != null)
    {
        Console.WriteLine("[PASS] 类型 RefreshTokenPayload (record) 存在");
        var ctors_RefreshTokenPayload = type_RefreshTokenPayload.GetConstructors();
        Console.WriteLine($"[PASS] RefreshTokenPayload 构造函数数量: {ctors_RefreshTokenPayload.Length}");
        var methods_RefreshTokenPayload = type_RefreshTokenPayload.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RefreshTokenPayload 公开方法数量: {methods_RefreshTokenPayload.Length}");
        foreach (var m in methods_RefreshTokenPayload)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RefreshTokenPayload 未找到，尝试无命名空间...");
        type_RefreshTokenPayload = Type.GetType("RefreshTokenPayload");
        if (type_RefreshTokenPayload != null)
            Console.WriteLine("[PASS] 类型 RefreshTokenPayload (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RefreshTokenPayload 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TokenRequestPayload
    var type_TokenRequestPayload = Type.GetType("TokenRequestPayload");
    if (type_TokenRequestPayload != null)
    {
        Console.WriteLine("[PASS] 类型 TokenRequestPayload (record) 存在");
        var ctors_TokenRequestPayload = type_TokenRequestPayload.GetConstructors();
        Console.WriteLine($"[PASS] TokenRequestPayload 构造函数数量: {ctors_TokenRequestPayload.Length}");
        var methods_TokenRequestPayload = type_TokenRequestPayload.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TokenRequestPayload 公开方法数量: {methods_TokenRequestPayload.Length}");
        foreach (var m in methods_TokenRequestPayload)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TokenRequestPayload 未找到，尝试无命名空间...");
        type_TokenRequestPayload = Type.GetType("TokenRequestPayload");
        if (type_TokenRequestPayload != null)
            Console.WriteLine("[PASS] 类型 TokenRequestPayload (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TokenRequestPayload 可能为顶层语句或嵌套类型");
    }

    // 验证 record: AuthorizationCodePayload
    var type_AuthorizationCodePayload = Type.GetType("AuthorizationCodePayload");
    if (type_AuthorizationCodePayload != null)
    {
        Console.WriteLine("[PASS] 类型 AuthorizationCodePayload (record) 存在");
        var ctors_AuthorizationCodePayload = type_AuthorizationCodePayload.GetConstructors();
        Console.WriteLine($"[PASS] AuthorizationCodePayload 构造函数数量: {ctors_AuthorizationCodePayload.Length}");
        var methods_AuthorizationCodePayload = type_AuthorizationCodePayload.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuthorizationCodePayload 公开方法数量: {methods_AuthorizationCodePayload.Length}");
        foreach (var m in methods_AuthorizationCodePayload)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuthorizationCodePayload 未找到，尝试无命名空间...");
        type_AuthorizationCodePayload = Type.GetType("AuthorizationCodePayload");
        if (type_AuthorizationCodePayload != null)
            Console.WriteLine("[PASS] 类型 AuthorizationCodePayload (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuthorizationCodePayload 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
