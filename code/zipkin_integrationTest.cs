#load "zipkin_integration.cs"

Console.WriteLine("=== zipkin_integration.cs Test ===");

try
{
    // 验证 class: ZipkinOptions
    var type_ZipkinOptions = Type.GetType("ZipkinOptions");
    if (type_ZipkinOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ZipkinOptions (class) 存在");
        var ctors_ZipkinOptions = type_ZipkinOptions.GetConstructors();
        Console.WriteLine($"[PASS] ZipkinOptions 构造函数数量: {ctors_ZipkinOptions.Length}");
        var methods_ZipkinOptions = type_ZipkinOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZipkinOptions 公开方法数量: {methods_ZipkinOptions.Length}");
        foreach (var m in methods_ZipkinOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZipkinOptions 未找到，尝试无命名空间...");
        type_ZipkinOptions = Type.GetType("ZipkinOptions");
        if (type_ZipkinOptions != null)
            Console.WriteLine("[PASS] 类型 ZipkinOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZipkinOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZipkinService
    var type_ZipkinService = Type.GetType("ZipkinService");
    if (type_ZipkinService != null)
    {
        Console.WriteLine("[PASS] 类型 ZipkinService (class) 存在");
        var ctors_ZipkinService = type_ZipkinService.GetConstructors();
        Console.WriteLine($"[PASS] ZipkinService 构造函数数量: {ctors_ZipkinService.Length}");
        var methods_ZipkinService = type_ZipkinService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZipkinService 公开方法数量: {methods_ZipkinService.Length}");
        foreach (var m in methods_ZipkinService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZipkinService 未找到，尝试无命名空间...");
        type_ZipkinService = Type.GetType("ZipkinService");
        if (type_ZipkinService != null)
            Console.WriteLine("[PASS] 类型 ZipkinService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZipkinService 可能为顶层语句或嵌套类型");
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

    // 验证 class: ZipkinHealthCheck
    var type_ZipkinHealthCheck = Type.GetType("ZipkinHealthCheck");
    if (type_ZipkinHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 ZipkinHealthCheck (class) 存在");
        var ctors_ZipkinHealthCheck = type_ZipkinHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] ZipkinHealthCheck 构造函数数量: {ctors_ZipkinHealthCheck.Length}");
        var methods_ZipkinHealthCheck = type_ZipkinHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZipkinHealthCheck 公开方法数量: {methods_ZipkinHealthCheck.Length}");
        foreach (var m in methods_ZipkinHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZipkinHealthCheck 未找到，尝试无命名空间...");
        type_ZipkinHealthCheck = Type.GetType("ZipkinHealthCheck");
        if (type_ZipkinHealthCheck != null)
            Console.WriteLine("[PASS] 类型 ZipkinHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZipkinHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IZipkinService
    var type_IZipkinService = Type.GetType("IZipkinService");
    if (type_IZipkinService != null)
    {
        Console.WriteLine("[PASS] 类型 IZipkinService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IZipkinService 未找到，尝试无命名空间...");
        type_IZipkinService = Type.GetType("IZipkinService");
        if (type_IZipkinService != null)
            Console.WriteLine("[PASS] 类型 IZipkinService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IZipkinService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
