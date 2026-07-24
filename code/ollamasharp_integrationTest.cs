#load "ollamasharp_integration.cs"

Console.WriteLine("=== ollamasharp_integration.cs Test ===");

try
{
    // 验证 class: OllamaOptions
    var type_OllamaOptions = Type.GetType("OllamaOptions");
    if (type_OllamaOptions != null)
    {
        Console.WriteLine("[PASS] 类型 OllamaOptions (class) 存在");
        var ctors_OllamaOptions = type_OllamaOptions.GetConstructors();
        Console.WriteLine($"[PASS] OllamaOptions 构造函数数量: {ctors_OllamaOptions.Length}");
        var methods_OllamaOptions = type_OllamaOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OllamaOptions 公开方法数量: {methods_OllamaOptions.Length}");
        foreach (var m in methods_OllamaOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OllamaOptions 未找到，尝试无命名空间...");
        type_OllamaOptions = Type.GetType("OllamaOptions");
        if (type_OllamaOptions != null)
            Console.WriteLine("[PASS] 类型 OllamaOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OllamaOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OllamaService
    var type_OllamaService = Type.GetType("OllamaService");
    if (type_OllamaService != null)
    {
        Console.WriteLine("[PASS] 类型 OllamaService (class) 存在");
        var ctors_OllamaService = type_OllamaService.GetConstructors();
        Console.WriteLine($"[PASS] OllamaService 构造函数数量: {ctors_OllamaService.Length}");
        var methods_OllamaService = type_OllamaService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OllamaService 公开方法数量: {methods_OllamaService.Length}");
        foreach (var m in methods_OllamaService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OllamaService 未找到，尝试无命名空间...");
        type_OllamaService = Type.GetType("OllamaService");
        if (type_OllamaService != null)
            Console.WriteLine("[PASS] 类型 OllamaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OllamaService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IOllamaService
    var type_IOllamaService = Type.GetType("IOllamaService");
    if (type_IOllamaService != null)
    {
        Console.WriteLine("[PASS] 类型 IOllamaService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IOllamaService 未找到，尝试无命名空间...");
        type_IOllamaService = Type.GetType("IOllamaService");
        if (type_IOllamaService != null)
            Console.WriteLine("[PASS] 类型 IOllamaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IOllamaService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
