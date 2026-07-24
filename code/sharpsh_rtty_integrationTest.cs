#load "sharpsh_rtty_integration.cs"

Console.WriteLine("=== sharpsh_rtty_integration.cs Test ===");

try
{
    // 验证 class: RttyOptions
    var type_RttyOptions = Type.GetType("RttyOptions");
    if (type_RttyOptions != null)
    {
        Console.WriteLine("[PASS] 类型 RttyOptions (class) 存在");
        var ctors_RttyOptions = type_RttyOptions.GetConstructors();
        Console.WriteLine($"[PASS] RttyOptions 构造函数数量: {ctors_RttyOptions.Length}");
        var methods_RttyOptions = type_RttyOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RttyOptions 公开方法数量: {methods_RttyOptions.Length}");
        foreach (var m in methods_RttyOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RttyOptions 未找到，尝试无命名空间...");
        type_RttyOptions = Type.GetType("RttyOptions");
        if (type_RttyOptions != null)
            Console.WriteLine("[PASS] 类型 RttyOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RttyOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RttyService
    var type_RttyService = Type.GetType("RttyService");
    if (type_RttyService != null)
    {
        Console.WriteLine("[PASS] 类型 RttyService (class) 存在");
        var ctors_RttyService = type_RttyService.GetConstructors();
        Console.WriteLine($"[PASS] RttyService 构造函数数量: {ctors_RttyService.Length}");
        var methods_RttyService = type_RttyService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RttyService 公开方法数量: {methods_RttyService.Length}");
        foreach (var m in methods_RttyService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RttyService 未找到，尝试无命名空间...");
        type_RttyService = Type.GetType("RttyService");
        if (type_RttyService != null)
            Console.WriteLine("[PASS] 类型 RttyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RttyService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IRttyService
    var type_IRttyService = Type.GetType("IRttyService");
    if (type_IRttyService != null)
    {
        Console.WriteLine("[PASS] 类型 IRttyService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IRttyService 未找到，尝试无命名空间...");
        type_IRttyService = Type.GetType("IRttyService");
        if (type_IRttyService != null)
            Console.WriteLine("[PASS] 类型 IRttyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IRttyService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
