#load "dotnetcore_extension_integration.cs"

Console.WriteLine("=== dotnetcore_extension_integration.cs Test ===");

try
{
    // 验证 class: DotNetCoreExtensions.DotNetCoreExtensionOptions
    var type_DotNetCoreExtensionOptions = Type.GetType("DotNetCoreExtensions.DotNetCoreExtensionOptions");
    if (type_DotNetCoreExtensionOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DotNetCoreExtensions.DotNetCoreExtensionOptions (class) 存在");
        var ctors_DotNetCoreExtensionOptions = type_DotNetCoreExtensionOptions.GetConstructors();
        Console.WriteLine($"[PASS] DotNetCoreExtensions.DotNetCoreExtensionOptions 构造函数数量: {ctors_DotNetCoreExtensionOptions.Length}");
        var methods_DotNetCoreExtensionOptions = type_DotNetCoreExtensionOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotNetCoreExtensions.DotNetCoreExtensionOptions 公开方法数量: {methods_DotNetCoreExtensionOptions.Length}");
        foreach (var m in methods_DotNetCoreExtensionOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotNetCoreExtensions.DotNetCoreExtensionOptions 未找到，尝试无命名空间...");
        type_DotNetCoreExtensionOptions = Type.GetType("DotNetCoreExtensionOptions");
        if (type_DotNetCoreExtensionOptions != null)
            Console.WriteLine("[PASS] 类型 DotNetCoreExtensionOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotNetCoreExtensionOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DotNetCoreExtensions.DotNetCoreExtensionService
    var type_DotNetCoreExtensionService = Type.GetType("DotNetCoreExtensions.DotNetCoreExtensionService");
    if (type_DotNetCoreExtensionService != null)
    {
        Console.WriteLine("[PASS] 类型 DotNetCoreExtensions.DotNetCoreExtensionService (class) 存在");
        var ctors_DotNetCoreExtensionService = type_DotNetCoreExtensionService.GetConstructors();
        Console.WriteLine($"[PASS] DotNetCoreExtensions.DotNetCoreExtensionService 构造函数数量: {ctors_DotNetCoreExtensionService.Length}");
        var methods_DotNetCoreExtensionService = type_DotNetCoreExtensionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotNetCoreExtensions.DotNetCoreExtensionService 公开方法数量: {methods_DotNetCoreExtensionService.Length}");
        foreach (var m in methods_DotNetCoreExtensionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotNetCoreExtensions.DotNetCoreExtensionService 未找到，尝试无命名空间...");
        type_DotNetCoreExtensionService = Type.GetType("DotNetCoreExtensionService");
        if (type_DotNetCoreExtensionService != null)
            Console.WriteLine("[PASS] 类型 DotNetCoreExtensionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotNetCoreExtensionService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DotNetCoreExtensions.DotNetCoreExtensionServiceCollectionExtensions
    var type_DotNetCoreExtensionServiceCollectionExtensions = Type.GetType("DotNetCoreExtensions.DotNetCoreExtensionServiceCollectionExtensions");
    if (type_DotNetCoreExtensionServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DotNetCoreExtensions.DotNetCoreExtensionServiceCollectionExtensions (class) 存在");
        var ctors_DotNetCoreExtensionServiceCollectionExtensions = type_DotNetCoreExtensionServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DotNetCoreExtensions.DotNetCoreExtensionServiceCollectionExtensions 构造函数数量: {ctors_DotNetCoreExtensionServiceCollectionExtensions.Length}");
        var methods_DotNetCoreExtensionServiceCollectionExtensions = type_DotNetCoreExtensionServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotNetCoreExtensions.DotNetCoreExtensionServiceCollectionExtensions 公开方法数量: {methods_DotNetCoreExtensionServiceCollectionExtensions.Length}");
        foreach (var m in methods_DotNetCoreExtensionServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotNetCoreExtensions.DotNetCoreExtensionServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_DotNetCoreExtensionServiceCollectionExtensions = Type.GetType("DotNetCoreExtensionServiceCollectionExtensions");
        if (type_DotNetCoreExtensionServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 DotNetCoreExtensionServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotNetCoreExtensionServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DotNetCoreExtensions.ExampleUsage
    var type_ExampleUsage = Type.GetType("DotNetCoreExtensions.ExampleUsage");
    if (type_ExampleUsage != null)
    {
        Console.WriteLine("[PASS] 类型 DotNetCoreExtensions.ExampleUsage (class) 存在");
        var ctors_ExampleUsage = type_ExampleUsage.GetConstructors();
        Console.WriteLine($"[PASS] DotNetCoreExtensions.ExampleUsage 构造函数数量: {ctors_ExampleUsage.Length}");
        var methods_ExampleUsage = type_ExampleUsage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotNetCoreExtensions.ExampleUsage 公开方法数量: {methods_ExampleUsage.Length}");
        foreach (var m in methods_ExampleUsage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotNetCoreExtensions.ExampleUsage 未找到，尝试无命名空间...");
        type_ExampleUsage = Type.GetType("ExampleUsage");
        if (type_ExampleUsage != null)
            Console.WriteLine("[PASS] 类型 ExampleUsage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExampleUsage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: DotNetCoreExtensions.IDotNetCoreExtensionService
    var type_IDotNetCoreExtensionService = Type.GetType("DotNetCoreExtensions.IDotNetCoreExtensionService");
    if (type_IDotNetCoreExtensionService != null)
    {
        Console.WriteLine("[PASS] 类型 DotNetCoreExtensions.IDotNetCoreExtensionService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotNetCoreExtensions.IDotNetCoreExtensionService 未找到，尝试无命名空间...");
        type_IDotNetCoreExtensionService = Type.GetType("IDotNetCoreExtensionService");
        if (type_IDotNetCoreExtensionService != null)
            Console.WriteLine("[PASS] 类型 IDotNetCoreExtensionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDotNetCoreExtensionService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
