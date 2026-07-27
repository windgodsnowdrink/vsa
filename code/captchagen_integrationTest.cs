#load "captchagen_integration.cs"

Console.WriteLine("=== captchagen_integration.cs Test ===");

try
{
    // 验证 class: CaptchaService
    var type_CaptchaService = Type.GetType("CaptchaService");
    if (type_CaptchaService != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaService (class) 存在");
        var ctors_CaptchaService = type_CaptchaService.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaService 构造函数数量: {ctors_CaptchaService.Length}");
        var methods_CaptchaService = type_CaptchaService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaService 公开方法数量: {methods_CaptchaService.Length}");
        foreach (var m in methods_CaptchaService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaService 未找到，尝试无命名空间...");
        type_CaptchaService = Type.GetType("CaptchaService");
        if (type_CaptchaService != null)
            Console.WriteLine("[PASS] 类型 CaptchaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaService 可能为顶层语句或嵌套类型");
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

    // 验证 class: BitmapPooledPolicy
    var type_BitmapPooledPolicy = Type.GetType("BitmapPooledPolicy");
    if (type_BitmapPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 BitmapPooledPolicy (class) 存在");
        var ctors_BitmapPooledPolicy = type_BitmapPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] BitmapPooledPolicy 构造函数数量: {ctors_BitmapPooledPolicy.Length}");
        var methods_BitmapPooledPolicy = type_BitmapPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BitmapPooledPolicy 公开方法数量: {methods_BitmapPooledPolicy.Length}");
        foreach (var m in methods_BitmapPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BitmapPooledPolicy 未找到，尝试无命名空间...");
        type_BitmapPooledPolicy = Type.GetType("BitmapPooledPolicy");
        if (type_BitmapPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 BitmapPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BitmapPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ICaptchaService
    var type_ICaptchaService = Type.GetType("ICaptchaService");
    if (type_ICaptchaService != null)
    {
        Console.WriteLine("[PASS] 类型 ICaptchaService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ICaptchaService 未找到，尝试无命名空间...");
        type_ICaptchaService = Type.GetType("ICaptchaService");
        if (type_ICaptchaService != null)
            Console.WriteLine("[PASS] 类型 ICaptchaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICaptchaService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CaptchaOptions
    var type_CaptchaOptions = Type.GetType("CaptchaOptions");
    if (type_CaptchaOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaOptions (record) 存在");
        var ctors_CaptchaOptions = type_CaptchaOptions.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaOptions 构造函数数量: {ctors_CaptchaOptions.Length}");
        var methods_CaptchaOptions = type_CaptchaOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaOptions 公开方法数量: {methods_CaptchaOptions.Length}");
        foreach (var m in methods_CaptchaOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaOptions 未找到，尝试无命名空间...");
        type_CaptchaOptions = Type.GetType("CaptchaOptions");
        if (type_CaptchaOptions != null)
            Console.WriteLine("[PASS] 类型 CaptchaOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CaptchaPerformanceMetrics
    var type_CaptchaPerformanceMetrics = Type.GetType("CaptchaPerformanceMetrics");
    if (type_CaptchaPerformanceMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaPerformanceMetrics (record) 存在");
        var ctors_CaptchaPerformanceMetrics = type_CaptchaPerformanceMetrics.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaPerformanceMetrics 构造函数数量: {ctors_CaptchaPerformanceMetrics.Length}");
        var methods_CaptchaPerformanceMetrics = type_CaptchaPerformanceMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaPerformanceMetrics 公开方法数量: {methods_CaptchaPerformanceMetrics.Length}");
        foreach (var m in methods_CaptchaPerformanceMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaPerformanceMetrics 未找到，尝试无命名空间...");
        type_CaptchaPerformanceMetrics = Type.GetType("CaptchaPerformanceMetrics");
        if (type_CaptchaPerformanceMetrics != null)
            Console.WriteLine("[PASS] 类型 CaptchaPerformanceMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaPerformanceMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: CaptchaType
    var type_CaptchaType = Type.GetType("CaptchaType");
    if (type_CaptchaType != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaType 未找到，尝试无命名空间...");
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
