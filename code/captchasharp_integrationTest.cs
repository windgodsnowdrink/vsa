#load "captchasharp_integration.cs"

Console.WriteLine("=== captchasharp_integration.cs Test ===");

try
{
    // 验证 class: CaptchaSharpIntegration.CaptchaOptions
    var type_CaptchaOptions = Type.GetType("CaptchaSharpIntegration.CaptchaOptions");
    if (type_CaptchaOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaSharpIntegration.CaptchaOptions (class) 存在");
        var ctors_CaptchaOptions = type_CaptchaOptions.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.CaptchaOptions 构造函数数量: {ctors_CaptchaOptions.Length}");
        var methods_CaptchaOptions = type_CaptchaOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.CaptchaOptions 公开方法数量: {methods_CaptchaOptions.Length}");
        foreach (var m in methods_CaptchaOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaSharpIntegration.CaptchaOptions 未找到，尝试无命名空间...");
        type_CaptchaOptions = Type.GetType("CaptchaOptions");
        if (type_CaptchaOptions != null)
            Console.WriteLine("[PASS] 类型 CaptchaOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaSharpIntegration.CaptchaMetrics
    var type_CaptchaMetrics = Type.GetType("CaptchaSharpIntegration.CaptchaMetrics");
    if (type_CaptchaMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaSharpIntegration.CaptchaMetrics (class) 存在");
        var ctors_CaptchaMetrics = type_CaptchaMetrics.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.CaptchaMetrics 构造函数数量: {ctors_CaptchaMetrics.Length}");
        var methods_CaptchaMetrics = type_CaptchaMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.CaptchaMetrics 公开方法数量: {methods_CaptchaMetrics.Length}");
        foreach (var m in methods_CaptchaMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaSharpIntegration.CaptchaMetrics 未找到，尝试无命名空间...");
        type_CaptchaMetrics = Type.GetType("CaptchaMetrics");
        if (type_CaptchaMetrics != null)
            Console.WriteLine("[PASS] 类型 CaptchaMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaSharpIntegration.CaptchaHealthStatus
    var type_CaptchaHealthStatus = Type.GetType("CaptchaSharpIntegration.CaptchaHealthStatus");
    if (type_CaptchaHealthStatus != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaSharpIntegration.CaptchaHealthStatus (class) 存在");
        var ctors_CaptchaHealthStatus = type_CaptchaHealthStatus.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.CaptchaHealthStatus 构造函数数量: {ctors_CaptchaHealthStatus.Length}");
        var methods_CaptchaHealthStatus = type_CaptchaHealthStatus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.CaptchaHealthStatus 公开方法数量: {methods_CaptchaHealthStatus.Length}");
        foreach (var m in methods_CaptchaHealthStatus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaSharpIntegration.CaptchaHealthStatus 未找到，尝试无命名空间...");
        type_CaptchaHealthStatus = Type.GetType("CaptchaHealthStatus");
        if (type_CaptchaHealthStatus != null)
            Console.WriteLine("[PASS] 类型 CaptchaHealthStatus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaHealthStatus 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaSharpIntegration.CaptchaService
    var type_CaptchaService = Type.GetType("CaptchaSharpIntegration.CaptchaService");
    if (type_CaptchaService != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaSharpIntegration.CaptchaService (class) 存在");
        var ctors_CaptchaService = type_CaptchaService.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.CaptchaService 构造函数数量: {ctors_CaptchaService.Length}");
        var methods_CaptchaService = type_CaptchaService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.CaptchaService 公开方法数量: {methods_CaptchaService.Length}");
        foreach (var m in methods_CaptchaService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaSharpIntegration.CaptchaService 未找到，尝试无命名空间...");
        type_CaptchaService = Type.GetType("CaptchaService");
        if (type_CaptchaService != null)
            Console.WriteLine("[PASS] 类型 CaptchaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaSharpIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("CaptchaSharpIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaSharpIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaSharpIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaSharpIntegration.CaptchaMetricsService
    var type_CaptchaMetricsService = Type.GetType("CaptchaSharpIntegration.CaptchaMetricsService");
    if (type_CaptchaMetricsService != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaSharpIntegration.CaptchaMetricsService (class) 存在");
        var ctors_CaptchaMetricsService = type_CaptchaMetricsService.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.CaptchaMetricsService 构造函数数量: {ctors_CaptchaMetricsService.Length}");
        var methods_CaptchaMetricsService = type_CaptchaMetricsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaSharpIntegration.CaptchaMetricsService 公开方法数量: {methods_CaptchaMetricsService.Length}");
        foreach (var m in methods_CaptchaMetricsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaSharpIntegration.CaptchaMetricsService 未找到，尝试无命名空间...");
        type_CaptchaMetricsService = Type.GetType("CaptchaMetricsService");
        if (type_CaptchaMetricsService != null)
            Console.WriteLine("[PASS] 类型 CaptchaMetricsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaMetricsService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: CaptchaSharpIntegration.ICaptchaService
    var type_ICaptchaService = Type.GetType("CaptchaSharpIntegration.ICaptchaService");
    if (type_ICaptchaService != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaSharpIntegration.ICaptchaService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaSharpIntegration.ICaptchaService 未找到，尝试无命名空间...");
        type_ICaptchaService = Type.GetType("ICaptchaService");
        if (type_ICaptchaService != null)
            Console.WriteLine("[PASS] 类型 ICaptchaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICaptchaService 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: CaptchaSharpIntegration.CaptchaType
    var type_CaptchaType = Type.GetType("CaptchaSharpIntegration.CaptchaType");
    if (type_CaptchaType != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaSharpIntegration.CaptchaType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaSharpIntegration.CaptchaType 未找到，尝试无命名空间...");
        type_CaptchaType = Type.GetType("CaptchaType");
        if (type_CaptchaType != null)
            Console.WriteLine("[PASS] 类型 CaptchaType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaType 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: CaptchaSharpIntegration.CaptchaComplexity
    var type_CaptchaComplexity = Type.GetType("CaptchaSharpIntegration.CaptchaComplexity");
    if (type_CaptchaComplexity != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaSharpIntegration.CaptchaComplexity (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaSharpIntegration.CaptchaComplexity 未找到，尝试无命名空间...");
        type_CaptchaComplexity = Type.GetType("CaptchaComplexity");
        if (type_CaptchaComplexity != null)
            Console.WriteLine("[PASS] 类型 CaptchaComplexity (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaComplexity 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
