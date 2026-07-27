#load "masuit_tools_integration.cs"

Console.WriteLine("=== masuit_tools_integration.cs Test ===");

try
{
    // 验证 class: MasuitToolsIntegration.MasuitToolsOptions
    var type_MasuitToolsOptions = Type.GetType("MasuitToolsIntegration.MasuitToolsOptions");
    if (type_MasuitToolsOptions != null)
    {
        Console.WriteLine("[PASS] 类型 MasuitToolsIntegration.MasuitToolsOptions (class) 存在");
        var ctors_MasuitToolsOptions = type_MasuitToolsOptions.GetConstructors();
        Console.WriteLine($"[PASS] MasuitToolsIntegration.MasuitToolsOptions 构造函数数量: {ctors_MasuitToolsOptions.Length}");
        var methods_MasuitToolsOptions = type_MasuitToolsOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MasuitToolsIntegration.MasuitToolsOptions 公开方法数量: {methods_MasuitToolsOptions.Length}");
        foreach (var m in methods_MasuitToolsOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MasuitToolsIntegration.MasuitToolsOptions 未找到，尝试无命名空间...");
        type_MasuitToolsOptions = Type.GetType("MasuitToolsOptions");
        if (type_MasuitToolsOptions != null)
            Console.WriteLine("[PASS] 类型 MasuitToolsOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MasuitToolsOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MasuitToolsIntegration.MasuitToolsService
    var type_MasuitToolsService = Type.GetType("MasuitToolsIntegration.MasuitToolsService");
    if (type_MasuitToolsService != null)
    {
        Console.WriteLine("[PASS] 类型 MasuitToolsIntegration.MasuitToolsService (class) 存在");
        var ctors_MasuitToolsService = type_MasuitToolsService.GetConstructors();
        Console.WriteLine($"[PASS] MasuitToolsIntegration.MasuitToolsService 构造函数数量: {ctors_MasuitToolsService.Length}");
        var methods_MasuitToolsService = type_MasuitToolsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MasuitToolsIntegration.MasuitToolsService 公开方法数量: {methods_MasuitToolsService.Length}");
        foreach (var m in methods_MasuitToolsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MasuitToolsIntegration.MasuitToolsService 未找到，尝试无命名空间...");
        type_MasuitToolsService = Type.GetType("MasuitToolsService");
        if (type_MasuitToolsService != null)
            Console.WriteLine("[PASS] 类型 MasuitToolsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MasuitToolsService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MasuitToolsIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("MasuitToolsIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MasuitToolsIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MasuitToolsIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MasuitToolsIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MasuitToolsIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MasuitToolsIntegration.ExampleUsage
    var type_ExampleUsage = Type.GetType("MasuitToolsIntegration.ExampleUsage");
    if (type_ExampleUsage != null)
    {
        Console.WriteLine("[PASS] 类型 MasuitToolsIntegration.ExampleUsage (class) 存在");
        var ctors_ExampleUsage = type_ExampleUsage.GetConstructors();
        Console.WriteLine($"[PASS] MasuitToolsIntegration.ExampleUsage 构造函数数量: {ctors_ExampleUsage.Length}");
        var methods_ExampleUsage = type_ExampleUsage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MasuitToolsIntegration.ExampleUsage 公开方法数量: {methods_ExampleUsage.Length}");
        foreach (var m in methods_ExampleUsage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MasuitToolsIntegration.ExampleUsage 未找到，尝试无命名空间...");
        type_ExampleUsage = Type.GetType("ExampleUsage");
        if (type_ExampleUsage != null)
            Console.WriteLine("[PASS] 类型 ExampleUsage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExampleUsage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: MasuitToolsIntegration.IMasuitToolsService
    var type_IMasuitToolsService = Type.GetType("MasuitToolsIntegration.IMasuitToolsService");
    if (type_IMasuitToolsService != null)
    {
        Console.WriteLine("[PASS] 类型 MasuitToolsIntegration.IMasuitToolsService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MasuitToolsIntegration.IMasuitToolsService 未找到，尝试无命名空间...");
        type_IMasuitToolsService = Type.GetType("IMasuitToolsService");
        if (type_IMasuitToolsService != null)
            Console.WriteLine("[PASS] 类型 IMasuitToolsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMasuitToolsService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
