#load "publicapi_integration.cs"

Console.WriteLine("=== publicapi_integration.cs Test ===");

try
{
    // 验证 class: PublicApiOptions
    var type_PublicApiOptions = Type.GetType("PublicApiOptions");
    if (type_PublicApiOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PublicApiOptions (class) 存在");
        var ctors_PublicApiOptions = type_PublicApiOptions.GetConstructors();
        Console.WriteLine($"[PASS] PublicApiOptions 构造函数数量: {ctors_PublicApiOptions.Length}");
        var methods_PublicApiOptions = type_PublicApiOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PublicApiOptions 公开方法数量: {methods_PublicApiOptions.Length}");
        foreach (var m in methods_PublicApiOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PublicApiOptions 未找到，尝试无命名空间...");
        type_PublicApiOptions = Type.GetType("PublicApiOptions");
        if (type_PublicApiOptions != null)
            Console.WriteLine("[PASS] 类型 PublicApiOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PublicApiOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PublicApiClient
    var type_PublicApiClient = Type.GetType("PublicApiClient");
    if (type_PublicApiClient != null)
    {
        Console.WriteLine("[PASS] 类型 PublicApiClient (class) 存在");
        var ctors_PublicApiClient = type_PublicApiClient.GetConstructors();
        Console.WriteLine($"[PASS] PublicApiClient 构造函数数量: {ctors_PublicApiClient.Length}");
        var methods_PublicApiClient = type_PublicApiClient.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PublicApiClient 公开方法数量: {methods_PublicApiClient.Length}");
        foreach (var m in methods_PublicApiClient)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PublicApiClient 未找到，尝试无命名空间...");
        type_PublicApiClient = Type.GetType("PublicApiClient");
        if (type_PublicApiClient != null)
            Console.WriteLine("[PASS] 类型 PublicApiClient (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PublicApiClient 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PublicApiJsonContext
    var type_PublicApiJsonContext = Type.GetType("PublicApiJsonContext");
    if (type_PublicApiJsonContext != null)
    {
        Console.WriteLine("[PASS] 类型 PublicApiJsonContext (class) 存在");
        var ctors_PublicApiJsonContext = type_PublicApiJsonContext.GetConstructors();
        Console.WriteLine($"[PASS] PublicApiJsonContext 构造函数数量: {ctors_PublicApiJsonContext.Length}");
        var methods_PublicApiJsonContext = type_PublicApiJsonContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PublicApiJsonContext 公开方法数量: {methods_PublicApiJsonContext.Length}");
        foreach (var m in methods_PublicApiJsonContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PublicApiJsonContext 未找到，尝试无命名空间...");
        type_PublicApiJsonContext = Type.GetType("PublicApiJsonContext");
        if (type_PublicApiJsonContext != null)
            Console.WriteLine("[PASS] 类型 PublicApiJsonContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PublicApiJsonContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
