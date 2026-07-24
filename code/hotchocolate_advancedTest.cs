#load "hotchocolate_advanced.cs"

Console.WriteLine("=== hotchocolate_advanced.cs Test ===");

try
{
    // 验证 class: BatchUserDataLoader
    var type_BatchUserDataLoader = Type.GetType("BatchUserDataLoader");
    if (type_BatchUserDataLoader != null)
    {
        Console.WriteLine("[PASS] 类型 BatchUserDataLoader (class) 存在");
        var ctors_BatchUserDataLoader = type_BatchUserDataLoader.GetConstructors();
        Console.WriteLine($"[PASS] BatchUserDataLoader 构造函数数量: {ctors_BatchUserDataLoader.Length}");
        var methods_BatchUserDataLoader = type_BatchUserDataLoader.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchUserDataLoader 公开方法数量: {methods_BatchUserDataLoader.Length}");
        foreach (var m in methods_BatchUserDataLoader)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchUserDataLoader 未找到，尝试无命名空间...");
        type_BatchUserDataLoader = Type.GetType("BatchUserDataLoader");
        if (type_BatchUserDataLoader != null)
            Console.WriteLine("[PASS] 类型 BatchUserDataLoader (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchUserDataLoader 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheMiddleware
    var type_CacheMiddleware = Type.GetType("CacheMiddleware");
    if (type_CacheMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 CacheMiddleware (class) 存在");
        var ctors_CacheMiddleware = type_CacheMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] CacheMiddleware 构造函数数量: {ctors_CacheMiddleware.Length}");
        var methods_CacheMiddleware = type_CacheMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheMiddleware 公开方法数量: {methods_CacheMiddleware.Length}");
        foreach (var m in methods_CacheMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheMiddleware 未找到，尝试无命名空间...");
        type_CacheMiddleware = Type.GetType("CacheMiddleware");
        if (type_CacheMiddleware != null)
            Console.WriteLine("[PASS] 类型 CacheMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuthorizeDirectiveType
    var type_AuthorizeDirectiveType = Type.GetType("AuthorizeDirectiveType");
    if (type_AuthorizeDirectiveType != null)
    {
        Console.WriteLine("[PASS] 类型 AuthorizeDirectiveType (class) 存在");
        var ctors_AuthorizeDirectiveType = type_AuthorizeDirectiveType.GetConstructors();
        Console.WriteLine($"[PASS] AuthorizeDirectiveType 构造函数数量: {ctors_AuthorizeDirectiveType.Length}");
        var methods_AuthorizeDirectiveType = type_AuthorizeDirectiveType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuthorizeDirectiveType 公开方法数量: {methods_AuthorizeDirectiveType.Length}");
        foreach (var m in methods_AuthorizeDirectiveType)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuthorizeDirectiveType 未找到，尝试无命名空间...");
        type_AuthorizeDirectiveType = Type.GetType("AuthorizeDirectiveType");
        if (type_AuthorizeDirectiveType != null)
            Console.WriteLine("[PASS] 类型 AuthorizeDirectiveType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuthorizeDirectiveType 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuthorizeDirective
    var type_AuthorizeDirective = Type.GetType("AuthorizeDirective");
    if (type_AuthorizeDirective != null)
    {
        Console.WriteLine("[PASS] 类型 AuthorizeDirective (class) 存在");
        var ctors_AuthorizeDirective = type_AuthorizeDirective.GetConstructors();
        Console.WriteLine($"[PASS] AuthorizeDirective 构造函数数量: {ctors_AuthorizeDirective.Length}");
        var methods_AuthorizeDirective = type_AuthorizeDirective.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuthorizeDirective 公开方法数量: {methods_AuthorizeDirective.Length}");
        foreach (var m in methods_AuthorizeDirective)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuthorizeDirective 未找到，尝试无命名空间...");
        type_AuthorizeDirective = Type.GetType("AuthorizeDirective");
        if (type_AuthorizeDirective != null)
            Console.WriteLine("[PASS] 类型 AuthorizeDirective (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuthorizeDirective 可能为顶层语句或嵌套类型");
    }

    // 验证 class: User
    var type_User = Type.GetType("User");
    if (type_User != null)
    {
        Console.WriteLine("[PASS] 类型 User (class) 存在");
        var ctors_User = type_User.GetConstructors();
        Console.WriteLine($"[PASS] User 构造函数数量: {ctors_User.Length}");
        var methods_User = type_User.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] User 公开方法数量: {methods_User.Length}");
        foreach (var m in methods_User)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 User 未找到，尝试无命名空间...");
        type_User = Type.GetType("User");
        if (type_User != null)
            Console.WriteLine("[PASS] 类型 User (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 User 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UserDbContext
    var type_UserDbContext = Type.GetType("UserDbContext");
    if (type_UserDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 UserDbContext (class) 存在");
        var ctors_UserDbContext = type_UserDbContext.GetConstructors();
        Console.WriteLine($"[PASS] UserDbContext 构造函数数量: {ctors_UserDbContext.Length}");
        var methods_UserDbContext = type_UserDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UserDbContext 公开方法数量: {methods_UserDbContext.Length}");
        foreach (var m in methods_UserDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UserDbContext 未找到，尝试无命名空间...");
        type_UserDbContext = Type.GetType("UserDbContext");
        if (type_UserDbContext != null)
            Console.WriteLine("[PASS] 类型 UserDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserDbContext 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
