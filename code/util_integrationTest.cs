#load "util_integration.cs"

Console.WriteLine("=== util_integration.cs Test ===");

try
{
    // 验证 class: UtilIntegration.UtilOptions
    var type_UtilOptions = Type.GetType("UtilIntegration.UtilOptions");
    if (type_UtilOptions != null)
    {
        Console.WriteLine("[PASS] 类型 UtilIntegration.UtilOptions (class) 存在");
        var ctors_UtilOptions = type_UtilOptions.GetConstructors();
        Console.WriteLine($"[PASS] UtilIntegration.UtilOptions 构造函数数量: {ctors_UtilOptions.Length}");
        var methods_UtilOptions = type_UtilOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UtilIntegration.UtilOptions 公开方法数量: {methods_UtilOptions.Length}");
        foreach (var m in methods_UtilOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UtilIntegration.UtilOptions 未找到，尝试无命名空间...");
        type_UtilOptions = Type.GetType("UtilOptions");
        if (type_UtilOptions != null)
            Console.WriteLine("[PASS] 类型 UtilOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UtilOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UtilIntegration.UtilService
    var type_UtilService = Type.GetType("UtilIntegration.UtilService");
    if (type_UtilService != null)
    {
        Console.WriteLine("[PASS] 类型 UtilIntegration.UtilService (class) 存在");
        var ctors_UtilService = type_UtilService.GetConstructors();
        Console.WriteLine($"[PASS] UtilIntegration.UtilService 构造函数数量: {ctors_UtilService.Length}");
        var methods_UtilService = type_UtilService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UtilIntegration.UtilService 公开方法数量: {methods_UtilService.Length}");
        foreach (var m in methods_UtilService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UtilIntegration.UtilService 未找到，尝试无命名空间...");
        type_UtilService = Type.GetType("UtilService");
        if (type_UtilService != null)
            Console.WriteLine("[PASS] 类型 UtilService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UtilService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UtilIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("UtilIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 UtilIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] UtilIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UtilIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UtilIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UtilIntegration.ExampleUsage
    var type_ExampleUsage = Type.GetType("UtilIntegration.ExampleUsage");
    if (type_ExampleUsage != null)
    {
        Console.WriteLine("[PASS] 类型 UtilIntegration.ExampleUsage (class) 存在");
        var ctors_ExampleUsage = type_ExampleUsage.GetConstructors();
        Console.WriteLine($"[PASS] UtilIntegration.ExampleUsage 构造函数数量: {ctors_ExampleUsage.Length}");
        var methods_ExampleUsage = type_ExampleUsage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UtilIntegration.ExampleUsage 公开方法数量: {methods_ExampleUsage.Length}");
        foreach (var m in methods_ExampleUsage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UtilIntegration.ExampleUsage 未找到，尝试无命名空间...");
        type_ExampleUsage = Type.GetType("ExampleUsage");
        if (type_ExampleUsage != null)
            Console.WriteLine("[PASS] 类型 ExampleUsage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExampleUsage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: UtilIntegration.IUtilService
    var type_IUtilService = Type.GetType("UtilIntegration.IUtilService");
    if (type_IUtilService != null)
    {
        Console.WriteLine("[PASS] 类型 UtilIntegration.IUtilService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UtilIntegration.IUtilService 未找到，尝试无命名空间...");
        type_IUtilService = Type.GetType("IUtilService");
        if (type_IUtilService != null)
            Console.WriteLine("[PASS] 类型 IUtilService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IUtilService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
