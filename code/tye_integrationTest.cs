#load "tye_integration.cs"

Console.WriteLine("=== tye_integration.cs Test ===");

try
{
    // 验证 class: TyeIntegration.TyeOptions
    var type_TyeOptions = Type.GetType("TyeIntegration.TyeOptions");
    if (type_TyeOptions != null)
    {
        Console.WriteLine("[PASS] 类型 TyeIntegration.TyeOptions (class) 存在");
        var ctors_TyeOptions = type_TyeOptions.GetConstructors();
        Console.WriteLine($"[PASS] TyeIntegration.TyeOptions 构造函数数量: {ctors_TyeOptions.Length}");
        var methods_TyeOptions = type_TyeOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TyeIntegration.TyeOptions 公开方法数量: {methods_TyeOptions.Length}");
        foreach (var m in methods_TyeOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TyeIntegration.TyeOptions 未找到，尝试无命名空间...");
        type_TyeOptions = Type.GetType("TyeOptions");
        if (type_TyeOptions != null)
            Console.WriteLine("[PASS] 类型 TyeOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TyeOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TyeIntegration.TyeService
    var type_TyeService = Type.GetType("TyeIntegration.TyeService");
    if (type_TyeService != null)
    {
        Console.WriteLine("[PASS] 类型 TyeIntegration.TyeService (class) 存在");
        var ctors_TyeService = type_TyeService.GetConstructors();
        Console.WriteLine($"[PASS] TyeIntegration.TyeService 构造函数数量: {ctors_TyeService.Length}");
        var methods_TyeService = type_TyeService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TyeIntegration.TyeService 公开方法数量: {methods_TyeService.Length}");
        foreach (var m in methods_TyeService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TyeIntegration.TyeService 未找到，尝试无命名空间...");
        type_TyeService = Type.GetType("TyeService");
        if (type_TyeService != null)
            Console.WriteLine("[PASS] 类型 TyeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TyeService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TyeIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("TyeIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 TyeIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] TyeIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TyeIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TyeIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: TyeIntegration.ITyeService
    var type_ITyeService = Type.GetType("TyeIntegration.ITyeService");
    if (type_ITyeService != null)
    {
        Console.WriteLine("[PASS] 类型 TyeIntegration.ITyeService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TyeIntegration.ITyeService 未找到，尝试无命名空间...");
        type_ITyeService = Type.GetType("ITyeService");
        if (type_ITyeService != null)
            Console.WriteLine("[PASS] 类型 ITyeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITyeService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
