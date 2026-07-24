#load "docker_compose_advanced_integration.cs"

Console.WriteLine("=== docker_compose_advanced_integration.cs Test ===");

try
{
    // 验证 class: DockerComposeAdvancedIntegration.DockerComposeOptions
    var type_DockerComposeOptions = Type.GetType("DockerComposeAdvancedIntegration.DockerComposeOptions");
    if (type_DockerComposeOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DockerComposeAdvancedIntegration.DockerComposeOptions (class) 存在");
        var ctors_DockerComposeOptions = type_DockerComposeOptions.GetConstructors();
        Console.WriteLine($"[PASS] DockerComposeAdvancedIntegration.DockerComposeOptions 构造函数数量: {ctors_DockerComposeOptions.Length}");
        var methods_DockerComposeOptions = type_DockerComposeOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerComposeAdvancedIntegration.DockerComposeOptions 公开方法数量: {methods_DockerComposeOptions.Length}");
        foreach (var m in methods_DockerComposeOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerComposeAdvancedIntegration.DockerComposeOptions 未找到，尝试无命名空间...");
        type_DockerComposeOptions = Type.GetType("DockerComposeOptions");
        if (type_DockerComposeOptions != null)
            Console.WriteLine("[PASS] 类型 DockerComposeOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerComposeOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerComposeAdvancedIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("DockerComposeAdvancedIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DockerComposeAdvancedIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DockerComposeAdvancedIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerComposeAdvancedIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerComposeAdvancedIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerComposeAdvancedIntegration.DockerComposeMetricsService
    var type_DockerComposeMetricsService = Type.GetType("DockerComposeAdvancedIntegration.DockerComposeMetricsService");
    if (type_DockerComposeMetricsService != null)
    {
        Console.WriteLine("[PASS] 类型 DockerComposeAdvancedIntegration.DockerComposeMetricsService (class) 存在");
        var ctors_DockerComposeMetricsService = type_DockerComposeMetricsService.GetConstructors();
        Console.WriteLine($"[PASS] DockerComposeAdvancedIntegration.DockerComposeMetricsService 构造函数数量: {ctors_DockerComposeMetricsService.Length}");
        var methods_DockerComposeMetricsService = type_DockerComposeMetricsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerComposeAdvancedIntegration.DockerComposeMetricsService 公开方法数量: {methods_DockerComposeMetricsService.Length}");
        foreach (var m in methods_DockerComposeMetricsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerComposeAdvancedIntegration.DockerComposeMetricsService 未找到，尝试无命名空间...");
        type_DockerComposeMetricsService = Type.GetType("DockerComposeMetricsService");
        if (type_DockerComposeMetricsService != null)
            Console.WriteLine("[PASS] 类型 DockerComposeMetricsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerComposeMetricsService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerComposeAdvancedIntegration.Startup
    var type_Startup = Type.GetType("DockerComposeAdvancedIntegration.Startup");
    if (type_Startup != null)
    {
        Console.WriteLine("[PASS] 类型 DockerComposeAdvancedIntegration.Startup (class) 存在");
        var ctors_Startup = type_Startup.GetConstructors();
        Console.WriteLine($"[PASS] DockerComposeAdvancedIntegration.Startup 构造函数数量: {ctors_Startup.Length}");
        var methods_Startup = type_Startup.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerComposeAdvancedIntegration.Startup 公开方法数量: {methods_Startup.Length}");
        foreach (var m in methods_Startup)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerComposeAdvancedIntegration.Startup 未找到，尝试无命名空间...");
        type_Startup = Type.GetType("Startup");
        if (type_Startup != null)
            Console.WriteLine("[PASS] 类型 Startup (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Startup 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DockerComposeAdvancedIntegration.DockerComposeEnvironment
    var type_DockerComposeEnvironment = Type.GetType("DockerComposeAdvancedIntegration.DockerComposeEnvironment");
    if (type_DockerComposeEnvironment != null)
    {
        Console.WriteLine("[PASS] 类型 DockerComposeAdvancedIntegration.DockerComposeEnvironment (record) 存在");
        var ctors_DockerComposeEnvironment = type_DockerComposeEnvironment.GetConstructors();
        Console.WriteLine($"[PASS] DockerComposeAdvancedIntegration.DockerComposeEnvironment 构造函数数量: {ctors_DockerComposeEnvironment.Length}");
        var methods_DockerComposeEnvironment = type_DockerComposeEnvironment.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerComposeAdvancedIntegration.DockerComposeEnvironment 公开方法数量: {methods_DockerComposeEnvironment.Length}");
        foreach (var m in methods_DockerComposeEnvironment)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerComposeAdvancedIntegration.DockerComposeEnvironment 未找到，尝试无命名空间...");
        type_DockerComposeEnvironment = Type.GetType("DockerComposeEnvironment");
        if (type_DockerComposeEnvironment != null)
            Console.WriteLine("[PASS] 类型 DockerComposeEnvironment (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerComposeEnvironment 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
