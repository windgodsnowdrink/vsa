#load "csgo_integration.cs"

Console.WriteLine("=== csgo_integration.cs Test ===");

try
{
    // 验证 class: CsGoIntegration.CsGoOptions
    var type_CsGoOptions = Type.GetType("CsGoIntegration.CsGoOptions");
    if (type_CsGoOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CsGoIntegration.CsGoOptions (class) 存在");
        var ctors_CsGoOptions = type_CsGoOptions.GetConstructors();
        Console.WriteLine($"[PASS] CsGoIntegration.CsGoOptions 构造函数数量: {ctors_CsGoOptions.Length}");
        var methods_CsGoOptions = type_CsGoOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CsGoIntegration.CsGoOptions 公开方法数量: {methods_CsGoOptions.Length}");
        foreach (var m in methods_CsGoOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CsGoIntegration.CsGoOptions 未找到，尝试无命名空间...");
        type_CsGoOptions = Type.GetType("CsGoOptions");
        if (type_CsGoOptions != null)
            Console.WriteLine("[PASS] 类型 CsGoOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CsGoOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CsGoIntegration.CsGoService
    var type_CsGoService = Type.GetType("CsGoIntegration.CsGoService");
    if (type_CsGoService != null)
    {
        Console.WriteLine("[PASS] 类型 CsGoIntegration.CsGoService (class) 存在");
        var ctors_CsGoService = type_CsGoService.GetConstructors();
        Console.WriteLine($"[PASS] CsGoIntegration.CsGoService 构造函数数量: {ctors_CsGoService.Length}");
        var methods_CsGoService = type_CsGoService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CsGoIntegration.CsGoService 公开方法数量: {methods_CsGoService.Length}");
        foreach (var m in methods_CsGoService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CsGoIntegration.CsGoService 未找到，尝试无命名空间...");
        type_CsGoService = Type.GetType("CsGoService");
        if (type_CsGoService != null)
            Console.WriteLine("[PASS] 类型 CsGoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CsGoService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CsGoIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("CsGoIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 CsGoIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] CsGoIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CsGoIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CsGoIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: CsGoIntegration.ICsGoService
    var type_ICsGoService = Type.GetType("CsGoIntegration.ICsGoService");
    if (type_ICsGoService != null)
    {
        Console.WriteLine("[PASS] 类型 CsGoIntegration.ICsGoService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CsGoIntegration.ICsGoService 未找到，尝试无命名空间...");
        type_ICsGoService = Type.GetType("ICsGoService");
        if (type_ICsGoService != null)
            Console.WriteLine("[PASS] 类型 ICsGoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICsGoService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
