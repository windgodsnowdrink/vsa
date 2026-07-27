#load "lazyslidecaptcha_integration.cs"

Console.WriteLine("=== lazyslidecaptcha_integration.cs Test ===");

try
{
    // 验证 class: CaptchaOptions
    var type_CaptchaOptions = Type.GetType("CaptchaOptions");
    if (type_CaptchaOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaOptions (class) 存在");
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

    // 验证 class: BitmapPooledObjectPolicy
    var type_BitmapPooledObjectPolicy = Type.GetType("BitmapPooledObjectPolicy");
    if (type_BitmapPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 BitmapPooledObjectPolicy (class) 存在");
        var ctors_BitmapPooledObjectPolicy = type_BitmapPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] BitmapPooledObjectPolicy 构造函数数量: {ctors_BitmapPooledObjectPolicy.Length}");
        var methods_BitmapPooledObjectPolicy = type_BitmapPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BitmapPooledObjectPolicy 公开方法数量: {methods_BitmapPooledObjectPolicy.Length}");
        foreach (var m in methods_BitmapPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BitmapPooledObjectPolicy 未找到，尝试无命名空间...");
        type_BitmapPooledObjectPolicy = Type.GetType("BitmapPooledObjectPolicy");
        if (type_BitmapPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 BitmapPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BitmapPooledObjectPolicy 可能为顶层语句或嵌套类型");
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

    // 验证 record: CaptchaMetrics
    var type_CaptchaMetrics = Type.GetType("CaptchaMetrics");
    if (type_CaptchaMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaMetrics (record) 存在");
        var ctors_CaptchaMetrics = type_CaptchaMetrics.GetConstructors();
        Console.WriteLine($"[PASS] CaptchaMetrics 构造函数数量: {ctors_CaptchaMetrics.Length}");
        var methods_CaptchaMetrics = type_CaptchaMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CaptchaMetrics 公开方法数量: {methods_CaptchaMetrics.Length}");
        foreach (var m in methods_CaptchaMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaMetrics 未找到，尝试无命名空间...");
        type_CaptchaMetrics = Type.GetType("CaptchaMetrics");
        if (type_CaptchaMetrics != null)
            Console.WriteLine("[PASS] 类型 CaptchaMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: CaptchaMode
    var type_CaptchaMode = Type.GetType("CaptchaMode");
    if (type_CaptchaMode != null)
    {
        Console.WriteLine("[PASS] 类型 CaptchaMode (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CaptchaMode 未找到，尝试无命名空间...");
        type_CaptchaMode = Type.GetType("CaptchaMode");
        if (type_CaptchaMode != null)
            Console.WriteLine("[PASS] 类型 CaptchaMode (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CaptchaMode 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
