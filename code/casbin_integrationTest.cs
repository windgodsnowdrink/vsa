#load "casbin_integration.cs"

Console.WriteLine("=== casbin_integration.cs Test ===");

try
{
    // 验证 class: ApplicationUser
    var type_ApplicationUser = Type.GetType("ApplicationUser");
    if (type_ApplicationUser != null)
    {
        Console.WriteLine("[PASS] 类型 ApplicationUser (class) 存在");
        var ctors_ApplicationUser = type_ApplicationUser.GetConstructors();
        Console.WriteLine($"[PASS] ApplicationUser 构造函数数量: {ctors_ApplicationUser.Length}");
        var methods_ApplicationUser = type_ApplicationUser.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ApplicationUser 公开方法数量: {methods_ApplicationUser.Length}");
        foreach (var m in methods_ApplicationUser)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ApplicationUser 未找到，尝试无命名空间...");
        type_ApplicationUser = Type.GetType("ApplicationUser");
        if (type_ApplicationUser != null)
            Console.WriteLine("[PASS] 类型 ApplicationUser (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ApplicationUser 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ApplicationRole
    var type_ApplicationRole = Type.GetType("ApplicationRole");
    if (type_ApplicationRole != null)
    {
        Console.WriteLine("[PASS] 类型 ApplicationRole (class) 存在");
        var ctors_ApplicationRole = type_ApplicationRole.GetConstructors();
        Console.WriteLine($"[PASS] ApplicationRole 构造函数数量: {ctors_ApplicationRole.Length}");
        var methods_ApplicationRole = type_ApplicationRole.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ApplicationRole 公开方法数量: {methods_ApplicationRole.Length}");
        foreach (var m in methods_ApplicationRole)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ApplicationRole 未找到，尝试无命名空间...");
        type_ApplicationRole = Type.GetType("ApplicationRole");
        if (type_ApplicationRole != null)
            Console.WriteLine("[PASS] 类型 ApplicationRole (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ApplicationRole 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MenuPermission
    var type_MenuPermission = Type.GetType("MenuPermission");
    if (type_MenuPermission != null)
    {
        Console.WriteLine("[PASS] 类型 MenuPermission (class) 存在");
        var ctors_MenuPermission = type_MenuPermission.GetConstructors();
        Console.WriteLine($"[PASS] MenuPermission 构造函数数量: {ctors_MenuPermission.Length}");
        var methods_MenuPermission = type_MenuPermission.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MenuPermission 公开方法数量: {methods_MenuPermission.Length}");
        foreach (var m in methods_MenuPermission)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MenuPermission 未找到，尝试无命名空间...");
        type_MenuPermission = Type.GetType("MenuPermission");
        if (type_MenuPermission != null)
            Console.WriteLine("[PASS] 类型 MenuPermission (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MenuPermission 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RolePermission
    var type_RolePermission = Type.GetType("RolePermission");
    if (type_RolePermission != null)
    {
        Console.WriteLine("[PASS] 类型 RolePermission (class) 存在");
        var ctors_RolePermission = type_RolePermission.GetConstructors();
        Console.WriteLine($"[PASS] RolePermission 构造函数数量: {ctors_RolePermission.Length}");
        var methods_RolePermission = type_RolePermission.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RolePermission 公开方法数量: {methods_RolePermission.Length}");
        foreach (var m in methods_RolePermission)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RolePermission 未找到，尝试无命名空间...");
        type_RolePermission = Type.GetType("RolePermission");
        if (type_RolePermission != null)
            Console.WriteLine("[PASS] 类型 RolePermission (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RolePermission 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CasbinIdentityAdapter
    var type_CasbinIdentityAdapter = Type.GetType("CasbinIdentityAdapter");
    if (type_CasbinIdentityAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 CasbinIdentityAdapter (class) 存在");
        var ctors_CasbinIdentityAdapter = type_CasbinIdentityAdapter.GetConstructors();
        Console.WriteLine($"[PASS] CasbinIdentityAdapter 构造函数数量: {ctors_CasbinIdentityAdapter.Length}");
        var methods_CasbinIdentityAdapter = type_CasbinIdentityAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CasbinIdentityAdapter 公开方法数量: {methods_CasbinIdentityAdapter.Length}");
        foreach (var m in methods_CasbinIdentityAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CasbinIdentityAdapter 未找到，尝试无命名空间...");
        type_CasbinIdentityAdapter = Type.GetType("CasbinIdentityAdapter");
        if (type_CasbinIdentityAdapter != null)
            Console.WriteLine("[PASS] 类型 CasbinIdentityAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CasbinIdentityAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CasbinContextPooledPolicy
    var type_CasbinContextPooledPolicy = Type.GetType("CasbinContextPooledPolicy");
    if (type_CasbinContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 CasbinContextPooledPolicy (class) 存在");
        var ctors_CasbinContextPooledPolicy = type_CasbinContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] CasbinContextPooledPolicy 构造函数数量: {ctors_CasbinContextPooledPolicy.Length}");
        var methods_CasbinContextPooledPolicy = type_CasbinContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CasbinContextPooledPolicy 公开方法数量: {methods_CasbinContextPooledPolicy.Length}");
        foreach (var m in methods_CasbinContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CasbinContextPooledPolicy 未找到，尝试无命名空间...");
        type_CasbinContextPooledPolicy = Type.GetType("CasbinContextPooledPolicy");
        if (type_CasbinContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 CasbinContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CasbinContextPooledPolicy 可能为顶层语句或嵌套类型");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
