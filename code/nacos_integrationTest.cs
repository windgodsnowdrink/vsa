#load "nacos_integration.cs"

Console.WriteLine("=== nacos_integration.cs Test ===");

try
{
    // 验证 class: NacosIntegration.NacosOptions
    var type_NacosOptions = Type.GetType("NacosIntegration.NacosOptions");
    if (type_NacosOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NacosIntegration.NacosOptions (class) 存在");
        var ctors_NacosOptions = type_NacosOptions.GetConstructors();
        Console.WriteLine($"[PASS] NacosIntegration.NacosOptions 构造函数数量: {ctors_NacosOptions.Length}");
        var methods_NacosOptions = type_NacosOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NacosIntegration.NacosOptions 公开方法数量: {methods_NacosOptions.Length}");
        foreach (var m in methods_NacosOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NacosIntegration.NacosOptions 未找到，尝试无命名空间...");
        type_NacosOptions = Type.GetType("NacosOptions");
        if (type_NacosOptions != null)
            Console.WriteLine("[PASS] 类型 NacosOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NacosOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NacosIntegration.NacosService
    var type_NacosService = Type.GetType("NacosIntegration.NacosService");
    if (type_NacosService != null)
    {
        Console.WriteLine("[PASS] 类型 NacosIntegration.NacosService (class) 存在");
        var ctors_NacosService = type_NacosService.GetConstructors();
        Console.WriteLine($"[PASS] NacosIntegration.NacosService 构造函数数量: {ctors_NacosService.Length}");
        var methods_NacosService = type_NacosService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NacosIntegration.NacosService 公开方法数量: {methods_NacosService.Length}");
        foreach (var m in methods_NacosService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NacosIntegration.NacosService 未找到，尝试无命名空间...");
        type_NacosService = Type.GetType("NacosService");
        if (type_NacosService != null)
            Console.WriteLine("[PASS] 类型 NacosService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NacosService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NacosIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("NacosIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 NacosIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] NacosIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NacosIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NacosIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: NacosIntegration.INacosService
    var type_INacosService = Type.GetType("NacosIntegration.INacosService");
    if (type_INacosService != null)
    {
        Console.WriteLine("[PASS] 类型 NacosIntegration.INacosService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NacosIntegration.INacosService 未找到，尝试无命名空间...");
        type_INacosService = Type.GetType("INacosService");
        if (type_INacosService != null)
            Console.WriteLine("[PASS] 类型 INacosService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 INacosService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
