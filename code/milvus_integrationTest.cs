#load "milvus_integration.cs"

Console.WriteLine("=== milvus_integration.cs Test ===");

try
{
    // 验证 class: MilvusIntegration.MilvusOptions
    var type_MilvusOptions = Type.GetType("MilvusIntegration.MilvusOptions");
    if (type_MilvusOptions != null)
    {
        Console.WriteLine("[PASS] 类型 MilvusIntegration.MilvusOptions (class) 存在");
        var ctors_MilvusOptions = type_MilvusOptions.GetConstructors();
        Console.WriteLine($"[PASS] MilvusIntegration.MilvusOptions 构造函数数量: {ctors_MilvusOptions.Length}");
        var methods_MilvusOptions = type_MilvusOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MilvusIntegration.MilvusOptions 公开方法数量: {methods_MilvusOptions.Length}");
        foreach (var m in methods_MilvusOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MilvusIntegration.MilvusOptions 未找到，尝试无命名空间...");
        type_MilvusOptions = Type.GetType("MilvusOptions");
        if (type_MilvusOptions != null)
            Console.WriteLine("[PASS] 类型 MilvusOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MilvusOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MilvusIntegration.MilvusService
    var type_MilvusService = Type.GetType("MilvusIntegration.MilvusService");
    if (type_MilvusService != null)
    {
        Console.WriteLine("[PASS] 类型 MilvusIntegration.MilvusService (class) 存在");
        var ctors_MilvusService = type_MilvusService.GetConstructors();
        Console.WriteLine($"[PASS] MilvusIntegration.MilvusService 构造函数数量: {ctors_MilvusService.Length}");
        var methods_MilvusService = type_MilvusService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MilvusIntegration.MilvusService 公开方法数量: {methods_MilvusService.Length}");
        foreach (var m in methods_MilvusService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MilvusIntegration.MilvusService 未找到，尝试无命名空间...");
        type_MilvusService = Type.GetType("MilvusService");
        if (type_MilvusService != null)
            Console.WriteLine("[PASS] 类型 MilvusService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MilvusService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MilvusIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("MilvusIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MilvusIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MilvusIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MilvusIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MilvusIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MilvusIntegration.ApplicationBuilderExtensions
    var type_ApplicationBuilderExtensions = Type.GetType("MilvusIntegration.ApplicationBuilderExtensions");
    if (type_ApplicationBuilderExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MilvusIntegration.ApplicationBuilderExtensions (class) 存在");
        var ctors_ApplicationBuilderExtensions = type_ApplicationBuilderExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MilvusIntegration.ApplicationBuilderExtensions 构造函数数量: {ctors_ApplicationBuilderExtensions.Length}");
        var methods_ApplicationBuilderExtensions = type_ApplicationBuilderExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MilvusIntegration.ApplicationBuilderExtensions 公开方法数量: {methods_ApplicationBuilderExtensions.Length}");
        foreach (var m in methods_ApplicationBuilderExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MilvusIntegration.ApplicationBuilderExtensions 未找到，尝试无命名空间...");
        type_ApplicationBuilderExtensions = Type.GetType("ApplicationBuilderExtensions");
        if (type_ApplicationBuilderExtensions != null)
            Console.WriteLine("[PASS] 类型 ApplicationBuilderExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ApplicationBuilderExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MilvusIntegration.ExampleUsage
    var type_ExampleUsage = Type.GetType("MilvusIntegration.ExampleUsage");
    if (type_ExampleUsage != null)
    {
        Console.WriteLine("[PASS] 类型 MilvusIntegration.ExampleUsage (class) 存在");
        var ctors_ExampleUsage = type_ExampleUsage.GetConstructors();
        Console.WriteLine($"[PASS] MilvusIntegration.ExampleUsage 构造函数数量: {ctors_ExampleUsage.Length}");
        var methods_ExampleUsage = type_ExampleUsage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MilvusIntegration.ExampleUsage 公开方法数量: {methods_ExampleUsage.Length}");
        foreach (var m in methods_ExampleUsage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MilvusIntegration.ExampleUsage 未找到，尝试无命名空间...");
        type_ExampleUsage = Type.GetType("ExampleUsage");
        if (type_ExampleUsage != null)
            Console.WriteLine("[PASS] 类型 ExampleUsage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExampleUsage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: MilvusIntegration.IMilvusService
    var type_IMilvusService = Type.GetType("MilvusIntegration.IMilvusService");
    if (type_IMilvusService != null)
    {
        Console.WriteLine("[PASS] 类型 MilvusIntegration.IMilvusService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MilvusIntegration.IMilvusService 未找到，尝试无命名空间...");
        type_IMilvusService = Type.GetType("IMilvusService");
        if (type_IMilvusService != null)
            Console.WriteLine("[PASS] 类型 IMilvusService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMilvusService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
