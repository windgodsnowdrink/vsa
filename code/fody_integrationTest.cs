#load "fody_integration.cs"

Console.WriteLine("=== fody_integration.cs Test ===");

try
{
    // 验证 class: LogAttribute
    var type_LogAttribute = Type.GetType("LogAttribute");
    if (type_LogAttribute != null)
    {
        Console.WriteLine("[PASS] 类型 LogAttribute (class) 存在");
        var ctors_LogAttribute = type_LogAttribute.GetConstructors();
        Console.WriteLine($"[PASS] LogAttribute 构造函数数量: {ctors_LogAttribute.Length}");
        var methods_LogAttribute = type_LogAttribute.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogAttribute 公开方法数量: {methods_LogAttribute.Length}");
        foreach (var m in methods_LogAttribute)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogAttribute 未找到，尝试无命名空间...");
        type_LogAttribute = Type.GetType("LogAttribute");
        if (type_LogAttribute != null)
            Console.WriteLine("[PASS] 类型 LogAttribute (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogAttribute 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LogContext
    var type_LogContext = Type.GetType("LogContext");
    if (type_LogContext != null)
    {
        Console.WriteLine("[PASS] 类型 LogContext (class) 存在");
        var ctors_LogContext = type_LogContext.GetConstructors();
        Console.WriteLine($"[PASS] LogContext 构造函数数量: {ctors_LogContext.Length}");
        var methods_LogContext = type_LogContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogContext 公开方法数量: {methods_LogContext.Length}");
        foreach (var m in methods_LogContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogContext 未找到，尝试无命名空间...");
        type_LogContext = Type.GetType("LogContext");
        if (type_LogContext != null)
            Console.WriteLine("[PASS] 类型 LogContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LogProcessor
    var type_LogProcessor = Type.GetType("LogProcessor");
    if (type_LogProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 LogProcessor (class) 存在");
        var ctors_LogProcessor = type_LogProcessor.GetConstructors();
        Console.WriteLine($"[PASS] LogProcessor 构造函数数量: {ctors_LogProcessor.Length}");
        var methods_LogProcessor = type_LogProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogProcessor 公开方法数量: {methods_LogProcessor.Length}");
        foreach (var m in methods_LogProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogProcessor 未找到，尝试无命名空间...");
        type_LogProcessor = Type.GetType("LogProcessor");
        if (type_LogProcessor != null)
            Console.WriteLine("[PASS] 类型 LogProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MethodTimerAttribute
    var type_MethodTimerAttribute = Type.GetType("MethodTimerAttribute");
    if (type_MethodTimerAttribute != null)
    {
        Console.WriteLine("[PASS] 类型 MethodTimerAttribute (class) 存在");
        var ctors_MethodTimerAttribute = type_MethodTimerAttribute.GetConstructors();
        Console.WriteLine($"[PASS] MethodTimerAttribute 构造函数数量: {ctors_MethodTimerAttribute.Length}");
        var methods_MethodTimerAttribute = type_MethodTimerAttribute.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MethodTimerAttribute 公开方法数量: {methods_MethodTimerAttribute.Length}");
        foreach (var m in methods_MethodTimerAttribute)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MethodTimerAttribute 未找到，尝试无命名空间...");
        type_MethodTimerAttribute = Type.GetType("MethodTimerAttribute");
        if (type_MethodTimerAttribute != null)
            Console.WriteLine("[PASS] 类型 MethodTimerAttribute (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MethodTimerAttribute 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AotFriendlyProxy
    var type_AotFriendlyProxy = Type.GetType("AotFriendlyProxy");
    if (type_AotFriendlyProxy != null)
    {
        Console.WriteLine("[PASS] 类型 AotFriendlyProxy (class) 存在");
        var ctors_AotFriendlyProxy = type_AotFriendlyProxy.GetConstructors();
        Console.WriteLine($"[PASS] AotFriendlyProxy 构造函数数量: {ctors_AotFriendlyProxy.Length}");
        var methods_AotFriendlyProxy = type_AotFriendlyProxy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AotFriendlyProxy 公开方法数量: {methods_AotFriendlyProxy.Length}");
        foreach (var m in methods_AotFriendlyProxy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AotFriendlyProxy 未找到，尝试无命名空间...");
        type_AotFriendlyProxy = Type.GetType("AotFriendlyProxy");
        if (type_AotFriendlyProxy != null)
            Console.WriteLine("[PASS] 类型 AotFriendlyProxy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AotFriendlyProxy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DynamicCompilationInterceptor
    var type_DynamicCompilationInterceptor = Type.GetType("DynamicCompilationInterceptor");
    if (type_DynamicCompilationInterceptor != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicCompilationInterceptor (class) 存在");
        var ctors_DynamicCompilationInterceptor = type_DynamicCompilationInterceptor.GetConstructors();
        Console.WriteLine($"[PASS] DynamicCompilationInterceptor 构造函数数量: {ctors_DynamicCompilationInterceptor.Length}");
        var methods_DynamicCompilationInterceptor = type_DynamicCompilationInterceptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicCompilationInterceptor 公开方法数量: {methods_DynamicCompilationInterceptor.Length}");
        foreach (var m in methods_DynamicCompilationInterceptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicCompilationInterceptor 未找到，尝试无命名空间...");
        type_DynamicCompilationInterceptor = Type.GetType("DynamicCompilationInterceptor");
        if (type_DynamicCompilationInterceptor != null)
            Console.WriteLine("[PASS] 类型 DynamicCompilationInterceptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicCompilationInterceptor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
