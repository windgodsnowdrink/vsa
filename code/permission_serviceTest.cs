#load "permission_service.cs"

Console.WriteLine("=== permission_service.cs Test ===");

try
{
    // 验证 class: PermissionService
    var type_PermissionService = Type.GetType("PermissionService");
    if (type_PermissionService != null)
    {
        Console.WriteLine("[PASS] 类型 PermissionService (class) 存在");
        var ctors_PermissionService = type_PermissionService.GetConstructors();
        Console.WriteLine($"[PASS] PermissionService 构造函数数量: {ctors_PermissionService.Length}");
        var methods_PermissionService = type_PermissionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PermissionService 公开方法数量: {methods_PermissionService.Length}");
        foreach (var m in methods_PermissionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PermissionService 未找到，尝试无命名空间...");
        type_PermissionService = Type.GetType("PermissionService");
        if (type_PermissionService != null)
            Console.WriteLine("[PASS] 类型 PermissionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PermissionService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PermissionRequirement
    var type_PermissionRequirement = Type.GetType("PermissionRequirement");
    if (type_PermissionRequirement != null)
    {
        Console.WriteLine("[PASS] 类型 PermissionRequirement (class) 存在");
        var ctors_PermissionRequirement = type_PermissionRequirement.GetConstructors();
        Console.WriteLine($"[PASS] PermissionRequirement 构造函数数量: {ctors_PermissionRequirement.Length}");
        var methods_PermissionRequirement = type_PermissionRequirement.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PermissionRequirement 公开方法数量: {methods_PermissionRequirement.Length}");
        foreach (var m in methods_PermissionRequirement)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PermissionRequirement 未找到，尝试无命名空间...");
        type_PermissionRequirement = Type.GetType("PermissionRequirement");
        if (type_PermissionRequirement != null)
            Console.WriteLine("[PASS] 类型 PermissionRequirement (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PermissionRequirement 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PermissionHandler
    var type_PermissionHandler = Type.GetType("PermissionHandler");
    if (type_PermissionHandler != null)
    {
        Console.WriteLine("[PASS] 类型 PermissionHandler (class) 存在");
        var ctors_PermissionHandler = type_PermissionHandler.GetConstructors();
        Console.WriteLine($"[PASS] PermissionHandler 构造函数数量: {ctors_PermissionHandler.Length}");
        var methods_PermissionHandler = type_PermissionHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PermissionHandler 公开方法数量: {methods_PermissionHandler.Length}");
        foreach (var m in methods_PermissionHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PermissionHandler 未找到，尝试无命名空间...");
        type_PermissionHandler = Type.GetType("PermissionHandler");
        if (type_PermissionHandler != null)
            Console.WriteLine("[PASS] 类型 PermissionHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PermissionHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPermissionService
    var type_IPermissionService = Type.GetType("IPermissionService");
    if (type_IPermissionService != null)
    {
        Console.WriteLine("[PASS] 类型 IPermissionService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPermissionService 未找到，尝试无命名空间...");
        type_IPermissionService = Type.GetType("IPermissionService");
        if (type_IPermissionService != null)
            Console.WriteLine("[PASS] 类型 IPermissionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPermissionService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
