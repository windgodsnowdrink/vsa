#load "util_enhanced_integration.cs"

Console.WriteLine("=== util_enhanced_integration.cs Test ===");

try
{
    // 验证 class: UtilEnhancedIntegration.UtilEnhancedOptions
    var type_UtilEnhancedOptions = Type.GetType("UtilEnhancedIntegration.UtilEnhancedOptions");
    if (type_UtilEnhancedOptions != null)
    {
        Console.WriteLine("[PASS] 类型 UtilEnhancedIntegration.UtilEnhancedOptions (class) 存在");
        var ctors_UtilEnhancedOptions = type_UtilEnhancedOptions.GetConstructors();
        Console.WriteLine($"[PASS] UtilEnhancedIntegration.UtilEnhancedOptions 构造函数数量: {ctors_UtilEnhancedOptions.Length}");
        var methods_UtilEnhancedOptions = type_UtilEnhancedOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UtilEnhancedIntegration.UtilEnhancedOptions 公开方法数量: {methods_UtilEnhancedOptions.Length}");
        foreach (var m in methods_UtilEnhancedOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UtilEnhancedIntegration.UtilEnhancedOptions 未找到，尝试无命名空间...");
        type_UtilEnhancedOptions = Type.GetType("UtilEnhancedOptions");
        if (type_UtilEnhancedOptions != null)
            Console.WriteLine("[PASS] 类型 UtilEnhancedOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UtilEnhancedOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UtilEnhancedIntegration.UtilEnhancedService
    var type_UtilEnhancedService = Type.GetType("UtilEnhancedIntegration.UtilEnhancedService");
    if (type_UtilEnhancedService != null)
    {
        Console.WriteLine("[PASS] 类型 UtilEnhancedIntegration.UtilEnhancedService (class) 存在");
        var ctors_UtilEnhancedService = type_UtilEnhancedService.GetConstructors();
        Console.WriteLine($"[PASS] UtilEnhancedIntegration.UtilEnhancedService 构造函数数量: {ctors_UtilEnhancedService.Length}");
        var methods_UtilEnhancedService = type_UtilEnhancedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UtilEnhancedIntegration.UtilEnhancedService 公开方法数量: {methods_UtilEnhancedService.Length}");
        foreach (var m in methods_UtilEnhancedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UtilEnhancedIntegration.UtilEnhancedService 未找到，尝试无命名空间...");
        type_UtilEnhancedService = Type.GetType("UtilEnhancedService");
        if (type_UtilEnhancedService != null)
            Console.WriteLine("[PASS] 类型 UtilEnhancedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UtilEnhancedService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UtilEnhancedIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("UtilEnhancedIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 UtilEnhancedIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] UtilEnhancedIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UtilEnhancedIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UtilEnhancedIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UtilEnhancedIntegration.ExampleEnhancedUsage
    var type_ExampleEnhancedUsage = Type.GetType("UtilEnhancedIntegration.ExampleEnhancedUsage");
    if (type_ExampleEnhancedUsage != null)
    {
        Console.WriteLine("[PASS] 类型 UtilEnhancedIntegration.ExampleEnhancedUsage (class) 存在");
        var ctors_ExampleEnhancedUsage = type_ExampleEnhancedUsage.GetConstructors();
        Console.WriteLine($"[PASS] UtilEnhancedIntegration.ExampleEnhancedUsage 构造函数数量: {ctors_ExampleEnhancedUsage.Length}");
        var methods_ExampleEnhancedUsage = type_ExampleEnhancedUsage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UtilEnhancedIntegration.ExampleEnhancedUsage 公开方法数量: {methods_ExampleEnhancedUsage.Length}");
        foreach (var m in methods_ExampleEnhancedUsage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UtilEnhancedIntegration.ExampleEnhancedUsage 未找到，尝试无命名空间...");
        type_ExampleEnhancedUsage = Type.GetType("ExampleEnhancedUsage");
        if (type_ExampleEnhancedUsage != null)
            Console.WriteLine("[PASS] 类型 ExampleEnhancedUsage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExampleEnhancedUsage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: UtilEnhancedIntegration.IUtilEnhancedService
    var type_IUtilEnhancedService = Type.GetType("UtilEnhancedIntegration.IUtilEnhancedService");
    if (type_IUtilEnhancedService != null)
    {
        Console.WriteLine("[PASS] 类型 UtilEnhancedIntegration.IUtilEnhancedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UtilEnhancedIntegration.IUtilEnhancedService 未找到，尝试无命名空间...");
        type_IUtilEnhancedService = Type.GetType("IUtilEnhancedService");
        if (type_IUtilEnhancedService != null)
            Console.WriteLine("[PASS] 类型 IUtilEnhancedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IUtilEnhancedService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
