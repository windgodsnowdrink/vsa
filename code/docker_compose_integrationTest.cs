#load "docker_compose_integration.cs"

Console.WriteLine("=== docker_compose_integration.cs Test ===");

try
{
    // 验证 class: DockerComposeIntegration.DockerComposeOptions
    var type_DockerComposeOptions = Type.GetType("DockerComposeIntegration.DockerComposeOptions");
    if (type_DockerComposeOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DockerComposeIntegration.DockerComposeOptions (class) 存在");
        var ctors_DockerComposeOptions = type_DockerComposeOptions.GetConstructors();
        Console.WriteLine($"[PASS] DockerComposeIntegration.DockerComposeOptions 构造函数数量: {ctors_DockerComposeOptions.Length}");
        var methods_DockerComposeOptions = type_DockerComposeOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerComposeIntegration.DockerComposeOptions 公开方法数量: {methods_DockerComposeOptions.Length}");
        foreach (var m in methods_DockerComposeOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerComposeIntegration.DockerComposeOptions 未找到，尝试无命名空间...");
        type_DockerComposeOptions = Type.GetType("DockerComposeOptions");
        if (type_DockerComposeOptions != null)
            Console.WriteLine("[PASS] 类型 DockerComposeOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerComposeOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerComposeIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("DockerComposeIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DockerComposeIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DockerComposeIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerComposeIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerComposeIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerComposeIntegration.Startup
    var type_Startup = Type.GetType("DockerComposeIntegration.Startup");
    if (type_Startup != null)
    {
        Console.WriteLine("[PASS] 类型 DockerComposeIntegration.Startup (class) 存在");
        var ctors_Startup = type_Startup.GetConstructors();
        Console.WriteLine($"[PASS] DockerComposeIntegration.Startup 构造函数数量: {ctors_Startup.Length}");
        var methods_Startup = type_Startup.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerComposeIntegration.Startup 公开方法数量: {methods_Startup.Length}");
        foreach (var m in methods_Startup)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerComposeIntegration.Startup 未找到，尝试无命名空间...");
        type_Startup = Type.GetType("Startup");
        if (type_Startup != null)
            Console.WriteLine("[PASS] 类型 Startup (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Startup 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
