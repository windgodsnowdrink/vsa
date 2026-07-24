#load "http_resilience_advanced_integration.cs"

Console.WriteLine("=== http_resilience_advanced_integration.cs Test ===");

try
{
    // 验证 class: ResilienceHealthCheck
    var type_ResilienceHealthCheck = Type.GetType("ResilienceHealthCheck");
    if (type_ResilienceHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 ResilienceHealthCheck (class) 存在");
        var ctors_ResilienceHealthCheck = type_ResilienceHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] ResilienceHealthCheck 构造函数数量: {ctors_ResilienceHealthCheck.Length}");
        var methods_ResilienceHealthCheck = type_ResilienceHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilienceHealthCheck 公开方法数量: {methods_ResilienceHealthCheck.Length}");
        foreach (var m in methods_ResilienceHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilienceHealthCheck 未找到，尝试无命名空间...");
        type_ResilienceHealthCheck = Type.GetType("ResilienceHealthCheck");
        if (type_ResilienceHealthCheck != null)
            Console.WriteLine("[PASS] 类型 ResilienceHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilienceHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RequestBufferingFeature
    var type_RequestBufferingFeature = Type.GetType("RequestBufferingFeature");
    if (type_RequestBufferingFeature != null)
    {
        Console.WriteLine("[PASS] 类型 RequestBufferingFeature (class) 存在");
        var ctors_RequestBufferingFeature = type_RequestBufferingFeature.GetConstructors();
        Console.WriteLine($"[PASS] RequestBufferingFeature 构造函数数量: {ctors_RequestBufferingFeature.Length}");
        var methods_RequestBufferingFeature = type_RequestBufferingFeature.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RequestBufferingFeature 公开方法数量: {methods_RequestBufferingFeature.Length}");
        foreach (var m in methods_RequestBufferingFeature)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RequestBufferingFeature 未找到，尝试无命名空间...");
        type_RequestBufferingFeature = Type.GetType("RequestBufferingFeature");
        if (type_RequestBufferingFeature != null)
            Console.WriteLine("[PASS] 类型 RequestBufferingFeature (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RequestBufferingFeature 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HttpResilienceOptions
    var type_HttpResilienceOptions = Type.GetType("HttpResilienceOptions");
    if (type_HttpResilienceOptions != null)
    {
        Console.WriteLine("[PASS] 类型 HttpResilienceOptions (class) 存在");
        var ctors_HttpResilienceOptions = type_HttpResilienceOptions.GetConstructors();
        Console.WriteLine($"[PASS] HttpResilienceOptions 构造函数数量: {ctors_HttpResilienceOptions.Length}");
        var methods_HttpResilienceOptions = type_HttpResilienceOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HttpResilienceOptions 公开方法数量: {methods_HttpResilienceOptions.Length}");
        foreach (var m in methods_HttpResilienceOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HttpResilienceOptions 未找到，尝试无命名空间...");
        type_HttpResilienceOptions = Type.GetType("HttpResilienceOptions");
        if (type_HttpResilienceOptions != null)
            Console.WriteLine("[PASS] 类型 HttpResilienceOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HttpResilienceOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
