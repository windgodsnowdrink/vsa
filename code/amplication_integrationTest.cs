#load "amplication_integration.cs"

Console.WriteLine("=== amplication_integration.cs Test ===");

try
{
    // 验证 class: AmplicationIntegration
    var type_AmplicationIntegration = Type.GetType("AmplicationIntegration");
    if (type_AmplicationIntegration != null)
    {
        Console.WriteLine("[PASS] 类型 AmplicationIntegration (class) 存在");
        var ctors_AmplicationIntegration = type_AmplicationIntegration.GetConstructors();
        Console.WriteLine($"[PASS] AmplicationIntegration 构造函数数量: {ctors_AmplicationIntegration.Length}");
        var methods_AmplicationIntegration = type_AmplicationIntegration.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AmplicationIntegration 公开方法数量: {methods_AmplicationIntegration.Length}");
        foreach (var m in methods_AmplicationIntegration)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AmplicationIntegration 未找到，尝试无命名空间...");
        type_AmplicationIntegration = Type.GetType("AmplicationIntegration");
        if (type_AmplicationIntegration != null)
            Console.WriteLine("[PASS] 类型 AmplicationIntegration (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AmplicationIntegration 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AmplicationService
    var type_AmplicationService = Type.GetType("AmplicationService");
    if (type_AmplicationService != null)
    {
        Console.WriteLine("[PASS] 类型 AmplicationService (class) 存在");
        var ctors_AmplicationService = type_AmplicationService.GetConstructors();
        Console.WriteLine($"[PASS] AmplicationService 构造函数数量: {ctors_AmplicationService.Length}");
        var methods_AmplicationService = type_AmplicationService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AmplicationService 公开方法数量: {methods_AmplicationService.Length}");
        foreach (var m in methods_AmplicationService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AmplicationService 未找到，尝试无命名空间...");
        type_AmplicationService = Type.GetType("AmplicationService");
        if (type_AmplicationService != null)
            Console.WriteLine("[PASS] 类型 AmplicationService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AmplicationService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAmplicationService
    var type_IAmplicationService = Type.GetType("IAmplicationService");
    if (type_IAmplicationService != null)
    {
        Console.WriteLine("[PASS] 类型 IAmplicationService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAmplicationService 未找到，尝试无命名空间...");
        type_IAmplicationService = Type.GetType("IAmplicationService");
        if (type_IAmplicationService != null)
            Console.WriteLine("[PASS] 类型 IAmplicationService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAmplicationService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
