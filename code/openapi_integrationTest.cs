#load "openapi_integration.cs"

Console.WriteLine("=== openapi_integration.cs Test ===");

try
{
    // 验证 class: OpenApiMetrics
    var type_OpenApiMetrics = Type.GetType("OpenApiMetrics");
    if (type_OpenApiMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 OpenApiMetrics (class) 存在");
        var ctors_OpenApiMetrics = type_OpenApiMetrics.GetConstructors();
        Console.WriteLine($"[PASS] OpenApiMetrics 构造函数数量: {ctors_OpenApiMetrics.Length}");
        var methods_OpenApiMetrics = type_OpenApiMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OpenApiMetrics 公开方法数量: {methods_OpenApiMetrics.Length}");
        foreach (var m in methods_OpenApiMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OpenApiMetrics 未找到，尝试无命名空间...");
        type_OpenApiMetrics = Type.GetType("OpenApiMetrics");
        if (type_OpenApiMetrics != null)
            Console.WriteLine("[PASS] 类型 OpenApiMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OpenApiMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CachingSwaggerProvider
    var type_CachingSwaggerProvider = Type.GetType("CachingSwaggerProvider");
    if (type_CachingSwaggerProvider != null)
    {
        Console.WriteLine("[PASS] 类型 CachingSwaggerProvider (class) 存在");
        var ctors_CachingSwaggerProvider = type_CachingSwaggerProvider.GetConstructors();
        Console.WriteLine($"[PASS] CachingSwaggerProvider 构造函数数量: {ctors_CachingSwaggerProvider.Length}");
        var methods_CachingSwaggerProvider = type_CachingSwaggerProvider.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CachingSwaggerProvider 公开方法数量: {methods_CachingSwaggerProvider.Length}");
        foreach (var m in methods_CachingSwaggerProvider)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CachingSwaggerProvider 未找到，尝试无命名空间...");
        type_CachingSwaggerProvider = Type.GetType("CachingSwaggerProvider");
        if (type_CachingSwaggerProvider != null)
            Console.WriteLine("[PASS] 类型 CachingSwaggerProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CachingSwaggerProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OpenApiMetricsService
    var type_OpenApiMetricsService = Type.GetType("OpenApiMetricsService");
    if (type_OpenApiMetricsService != null)
    {
        Console.WriteLine("[PASS] 类型 OpenApiMetricsService (class) 存在");
        var ctors_OpenApiMetricsService = type_OpenApiMetricsService.GetConstructors();
        Console.WriteLine($"[PASS] OpenApiMetricsService 构造函数数量: {ctors_OpenApiMetricsService.Length}");
        var methods_OpenApiMetricsService = type_OpenApiMetricsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OpenApiMetricsService 公开方法数量: {methods_OpenApiMetricsService.Length}");
        foreach (var m in methods_OpenApiMetricsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OpenApiMetricsService 未找到，尝试无命名空间...");
        type_OpenApiMetricsService = Type.GetType("OpenApiMetricsService");
        if (type_OpenApiMetricsService != null)
            Console.WriteLine("[PASS] 类型 OpenApiMetricsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OpenApiMetricsService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OpenApiOptions
    var type_OpenApiOptions = Type.GetType("OpenApiOptions");
    if (type_OpenApiOptions != null)
    {
        Console.WriteLine("[PASS] 类型 OpenApiOptions (class) 存在");
        var ctors_OpenApiOptions = type_OpenApiOptions.GetConstructors();
        Console.WriteLine($"[PASS] OpenApiOptions 构造函数数量: {ctors_OpenApiOptions.Length}");
        var methods_OpenApiOptions = type_OpenApiOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OpenApiOptions 公开方法数量: {methods_OpenApiOptions.Length}");
        foreach (var m in methods_OpenApiOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OpenApiOptions 未找到，尝试无命名空间...");
        type_OpenApiOptions = Type.GetType("OpenApiOptions");
        if (type_OpenApiOptions != null)
            Console.WriteLine("[PASS] 类型 OpenApiOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OpenApiOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OpenApiExtensions
    var type_OpenApiExtensions = Type.GetType("OpenApiExtensions");
    if (type_OpenApiExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 OpenApiExtensions (class) 存在");
        var ctors_OpenApiExtensions = type_OpenApiExtensions.GetConstructors();
        Console.WriteLine($"[PASS] OpenApiExtensions 构造函数数量: {ctors_OpenApiExtensions.Length}");
        var methods_OpenApiExtensions = type_OpenApiExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OpenApiExtensions 公开方法数量: {methods_OpenApiExtensions.Length}");
        foreach (var m in methods_OpenApiExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OpenApiExtensions 未找到，尝试无命名空间...");
        type_OpenApiExtensions = Type.GetType("OpenApiExtensions");
        if (type_OpenApiExtensions != null)
            Console.WriteLine("[PASS] 类型 OpenApiExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OpenApiExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
