#load "hei_captcha_integration.cs"

Console.WriteLine("=== hei_captcha_integration.cs Test ===");

try
{
    // 验证 class: CaptchaDemo.CaptchaOptions
    var type_CaptchaOptions = Type.GetType("CaptchaDemo.CaptchaOptions");
    if (type_CaptchaOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaDemo.CaptchaOptions (class) 存在");
        var ctors_CaptchaOptions = type_CaptchaOptions.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaDemo.CaptchaOptions 构造函数数量: {ctors_CaptchaOptions.Length}");
        var methods_CaptchaOptions = type_CaptchaOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaDemo.CaptchaOptions 公开方法数量: {methods_CaptchaOptions.Length}");
        foreach (var m in methods_CaptchaOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaDemo.CaptchaOptions 未找到，尝试无命名空间...");
        type_CaptchaOptions = Type.GetType("CaptchaOptions");
        if (type_CaptchaOptions != null)
            Console.WriteLine("[PASS] 类型 CaptchaOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaDemo.CaptchaMetrics
    var type_CaptchaMetrics = Type.GetType("CaptchaDemo.CaptchaMetrics");
    if (type_CaptchaMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaDemo.CaptchaMetrics (class) 存在");
        var ctors_CaptchaMetrics = type_CaptchaMetrics.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaDemo.CaptchaMetrics 构造函数数量: {ctors_CaptchaMetrics.Length}");
        var methods_CaptchaMetrics = type_CaptchaMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaDemo.CaptchaMetrics 公开方法数量: {methods_CaptchaMetrics.Length}");
        foreach (var m in methods_CaptchaMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaDemo.CaptchaMetrics 未找到，尝试无命名空间...");
        type_CaptchaMetrics = Type.GetType("CaptchaMetrics");
        if (type_CaptchaMetrics != null)
            Console.WriteLine("[PASS] 类型 CaptchaMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaDemo.CaptchaHealthStatus
    var type_CaptchaHealthStatus = Type.GetType("CaptchaDemo.CaptchaHealthStatus");
    if (type_CaptchaHealthStatus != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaDemo.CaptchaHealthStatus (class) 存在");
        var ctors_CaptchaHealthStatus = type_CaptchaHealthStatus.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaDemo.CaptchaHealthStatus 构造函数数量: {ctors_CaptchaHealthStatus.Length}");
        var methods_CaptchaHealthStatus = type_CaptchaHealthStatus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaDemo.CaptchaHealthStatus 公开方法数量: {methods_CaptchaHealthStatus.Length}");
        foreach (var m in methods_CaptchaHealthStatus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaDemo.CaptchaHealthStatus 未找到，尝试无命名空间...");
        type_CaptchaHealthStatus = Type.GetType("CaptchaHealthStatus");
        if (type_CaptchaHealthStatus != null)
            Console.WriteLine("[PASS] 类型 CaptchaHealthStatus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaHealthStatus 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaDemo.CaptchaService
    var type_CaptchaService = Type.GetType("CaptchaDemo.CaptchaService");
    if (type_CaptchaService != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaDemo.CaptchaService (class) 存在");
        var ctors_CaptchaService = type_CaptchaService.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaDemo.CaptchaService 构造函数数量: {ctors_CaptchaService.Length}");
        var methods_CaptchaService = type_CaptchaService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaDemo.CaptchaService 公开方法数量: {methods_CaptchaService.Length}");
        foreach (var m in methods_CaptchaService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaDemo.CaptchaService 未找到，尝试无命名空间...");
        type_CaptchaService = Type.GetType("CaptchaService");
        if (type_CaptchaService != null)
            Console.WriteLine("[PASS] 类型 CaptchaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CaptchaDemo.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("CaptchaDemo.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaDemo.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaDemo.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaDemo.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaDemo.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: CaptchaDemo.ICaptchaService
    var type_ICaptchaService = Type.GetType("CaptchaDemo.ICaptchaService");
    if (type_ICaptchaService != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaDemo.ICaptchaService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaDemo.ICaptchaService 未找到，尝试无命名空间...");
        type_ICaptchaService = Type.GetType("ICaptchaService");
        if (type_ICaptchaService != null)
            Console.WriteLine("[PASS] 类型 ICaptchaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICaptchaService 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: CaptchaDemo.CaptchaType
    var type_CaptchaType = Type.GetType("CaptchaDemo.CaptchaType");
    if (type_CaptchaType != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaDemo.CaptchaType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaDemo.CaptchaType 未找到，尝试无命名空间...");
        type_CaptchaType = Type.GetType("CaptchaType");
        if (type_CaptchaType != null)
            Console.WriteLine("[PASS] 类型 CaptchaType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaType 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: CaptchaDemo.CaptchaComplexity
    var type_CaptchaComplexity = Type.GetType("CaptchaDemo.CaptchaComplexity");
    if (type_CaptchaComplexity != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaDemo.CaptchaComplexity (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaDemo.CaptchaComplexity 未找到，尝试无命名空间...");
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
