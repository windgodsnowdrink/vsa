#load "known_blazor_integration.cs"

Console.WriteLine("=== known_blazor_integration.cs Test ===");

try
{
    // 验证 class: KnownBlazorOptions
    var type_KnownBlazorOptions = Type.GetType("KnownBlazorOptions");
    if (type_KnownBlazorOptions != null)
    {
        Console.WriteLine("[PASS] 类型 KnownBlazorOptions (class) 存在");
        var ctors_KnownBlazorOptions = type_KnownBlazorOptions.GetConstructors();
        Console.WriteLine($"[PASS] KnownBlazorOptions 构造函数数量: {ctors_KnownBlazorOptions.Length}");
        var methods_KnownBlazorOptions = type_KnownBlazorOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KnownBlazorOptions 公开方法数量: {methods_KnownBlazorOptions.Length}");
        foreach (var m in methods_KnownBlazorOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KnownBlazorOptions 未找到，尝试无命名空间...");
        type_KnownBlazorOptions = Type.GetType("KnownBlazorOptions");
        if (type_KnownBlazorOptions != null)
            Console.WriteLine("[PASS] 类型 KnownBlazorOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KnownBlazorOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KnownBlazorService
    var type_KnownBlazorService = Type.GetType("KnownBlazorService");
    if (type_KnownBlazorService != null)
    {
        Console.WriteLine("[PASS] 类型 KnownBlazorService (class) 存在");
        var ctors_KnownBlazorService = type_KnownBlazorService.GetConstructors();
        Console.WriteLine($"[PASS] KnownBlazorService 构造函数数量: {ctors_KnownBlazorService.Length}");
        var methods_KnownBlazorService = type_KnownBlazorService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KnownBlazorService 公开方法数量: {methods_KnownBlazorService.Length}");
        foreach (var m in methods_KnownBlazorService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KnownBlazorService 未找到，尝试无命名空间...");
        type_KnownBlazorService = Type.GetType("KnownBlazorService");
        if (type_KnownBlazorService != null)
            Console.WriteLine("[PASS] 类型 KnownBlazorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KnownBlazorService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IKnownBlazorService
    var type_IKnownBlazorService = Type.GetType("IKnownBlazorService");
    if (type_IKnownBlazorService != null)
    {
        Console.WriteLine("[PASS] 类型 IKnownBlazorService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IKnownBlazorService 未找到，尝试无命名空间...");
        type_IKnownBlazorService = Type.GetType("IKnownBlazorService");
        if (type_IKnownBlazorService != null)
            Console.WriteLine("[PASS] 类型 IKnownBlazorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IKnownBlazorService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
