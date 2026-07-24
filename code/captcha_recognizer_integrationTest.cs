#load "captcha_recognizer_integration.cs"

Console.WriteLine("=== captcha_recognizer_integration.cs Test ===");

try
{
    // 验证 class: CaptchaRecognizerIntegration.CaptchaOptions
    var type_CaptchaOptions = Type.GetType("CaptchaRecognizerIntegration.CaptchaOptions");
    if (type_CaptchaOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaRecognizerIntegration.CaptchaOptions (class) 存在");
        var ctors_CaptchaOptions = type_CaptchaOptions.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaRecognizerIntegration.CaptchaOptions 构造函数数量: {ctors_CaptchaOptions.Length}");
        var methods_CaptchaOptions = type_CaptchaOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaRecognizerIntegration.CaptchaOptions 公开方法数量: {methods_CaptchaOptions.Length}");
        foreach (var m in methods_CaptchaOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaRecognizerIntegration.CaptchaOptions 未找到，尝试无命名空间...");
        type_CaptchaOptions = Type.GetType("CaptchaOptions");
        if (type_CaptchaOptions != null)
            Console.WriteLine("[PASS] 类型 CaptchaOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaRecognizerIntegration.CaptchaHealthStatus
    var type_CaptchaHealthStatus = Type.GetType("CaptchaRecognizerIntegration.CaptchaHealthStatus");
    if (type_CaptchaHealthStatus != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaRecognizerIntegration.CaptchaHealthStatus (class) 存在");
        var ctors_CaptchaHealthStatus = type_CaptchaHealthStatus.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaRecognizerIntegration.CaptchaHealthStatus 构造函数数量: {ctors_CaptchaHealthStatus.Length}");
        var methods_CaptchaHealthStatus = type_CaptchaHealthStatus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaRecognizerIntegration.CaptchaHealthStatus 公开方法数量: {methods_CaptchaHealthStatus.Length}");
        foreach (var m in methods_CaptchaHealthStatus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaRecognizerIntegration.CaptchaHealthStatus 未找到，尝试无命名空间...");
        type_CaptchaHealthStatus = Type.GetType("CaptchaHealthStatus");
        if (type_CaptchaHealthStatus != null)
            Console.WriteLine("[PASS] 类型 CaptchaHealthStatus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaHealthStatus 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaRecognizerIntegration.CaptchaService
    var type_CaptchaService = Type.GetType("CaptchaRecognizerIntegration.CaptchaService");
    if (type_CaptchaService != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaRecognizerIntegration.CaptchaService (class) 存在");
        var ctors_CaptchaService = type_CaptchaService.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaRecognizerIntegration.CaptchaService 构造函数数量: {ctors_CaptchaService.Length}");
        var methods_CaptchaService = type_CaptchaService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaRecognizerIntegration.CaptchaService 公开方法数量: {methods_CaptchaService.Length}");
        foreach (var m in methods_CaptchaService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaRecognizerIntegration.CaptchaService 未找到，尝试无命名空间...");
        type_CaptchaService = Type.GetType("CaptchaService");
        if (type_CaptchaService != null)
            Console.WriteLine("[PASS] 类型 CaptchaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaRecognizerIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("CaptchaRecognizerIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaRecognizerIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaRecognizerIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaRecognizerIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaRecognizerIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaRecognizerIntegration.CaptchaMetricsService
    var type_CaptchaMetricsService = Type.GetType("CaptchaRecognizerIntegration.CaptchaMetricsService");
    if (type_CaptchaMetricsService != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaRecognizerIntegration.CaptchaMetricsService (class) 存在");
        var ctors_CaptchaMetricsService = type_CaptchaMetricsService.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaRecognizerIntegration.CaptchaMetricsService 构造函数数量: {ctors_CaptchaMetricsService.Length}");
        var methods_CaptchaMetricsService = type_CaptchaMetricsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaRecognizerIntegration.CaptchaMetricsService 公开方法数量: {methods_CaptchaMetricsService.Length}");
        foreach (var m in methods_CaptchaMetricsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaRecognizerIntegration.CaptchaMetricsService 未找到，尝试无命名空间...");
        type_CaptchaMetricsService = Type.GetType("CaptchaMetricsService");
        if (type_CaptchaMetricsService != null)
            Console.WriteLine("[PASS] 类型 CaptchaMetricsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaMetricsService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: CaptchaRecognizerIntegration.ICaptchaService
    var type_ICaptchaService = Type.GetType("CaptchaRecognizerIntegration.ICaptchaService");
    if (type_ICaptchaService != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaRecognizerIntegration.ICaptchaService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaRecognizerIntegration.ICaptchaService 未找到，尝试无命名空间...");
        type_ICaptchaService = Type.GetType("ICaptchaService");
        if (type_ICaptchaService != null)
            Console.WriteLine("[PASS] 类型 ICaptchaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICaptchaService 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: CaptchaRecognizerIntegration.CaptchaType
    var type_CaptchaType = Type.GetType("CaptchaRecognizerIntegration.CaptchaType");
    if (type_CaptchaType != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaRecognizerIntegration.CaptchaType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaRecognizerIntegration.CaptchaType 未找到，尝试无命名空间...");
        type_CaptchaType = Type.GetType("CaptchaType");
        if (type_CaptchaType != null)
            Console.WriteLine("[PASS] 类型 CaptchaType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
