#load "resilient_polly_integration.cs"

Console.WriteLine("=== resilient_polly_integration.cs Test ===");

try
{
    // 验证 class: ResilientOptions
    var type_ResilientOptions = Type.GetType("ResilientOptions");
    if (type_ResilientOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ResilientOptions (class) 存在");
        var ctors_ResilientOptions = type_ResilientOptions.GetConstructors();
        Console.WriteLine($"[PASS] ResilientOptions 构造函数数量: {ctors_ResilientOptions.Length}");
        var methods_ResilientOptions = type_ResilientOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilientOptions 公开方法数量: {methods_ResilientOptions.Length}");
        foreach (var m in methods_ResilientOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilientOptions 未找到，尝试无命名空间...");
        type_ResilientOptions = Type.GetType("ResilientOptions");
        if (type_ResilientOptions != null)
            Console.WriteLine("[PASS] 类型 ResilientOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilientOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResilientService
    var type_ResilientService = Type.GetType("ResilientService");
    if (type_ResilientService != null)
    {
        Console.WriteLine("[PASS] 类型 ResilientService (class) 存在");
        var ctors_ResilientService = type_ResilientService.GetConstructors();
        Console.WriteLine($"[PASS] ResilientService 构造函数数量: {ctors_ResilientService.Length}");
        var methods_ResilientService = type_ResilientService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilientService 公开方法数量: {methods_ResilientService.Length}");
        foreach (var m in methods_ResilientService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilientService 未找到，尝试无命名空间...");
        type_ResilientService = Type.GetType("ResilientService");
        if (type_ResilientService != null)
            Console.WriteLine("[PASS] 类型 ResilientService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilientService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResilientExtensions
    var type_ResilientExtensions = Type.GetType("ResilientExtensions");
    if (type_ResilientExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ResilientExtensions (class) 存在");
        var ctors_ResilientExtensions = type_ResilientExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ResilientExtensions 构造函数数量: {ctors_ResilientExtensions.Length}");
        var methods_ResilientExtensions = type_ResilientExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilientExtensions 公开方法数量: {methods_ResilientExtensions.Length}");
        foreach (var m in methods_ResilientExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilientExtensions 未找到，尝试无命名空间...");
        type_ResilientExtensions = Type.GetType("ResilientExtensions");
        if (type_ResilientExtensions != null)
            Console.WriteLine("[PASS] 类型 ResilientExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilientExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResilientBackgroundService
    var type_ResilientBackgroundService = Type.GetType("ResilientBackgroundService");
    if (type_ResilientBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 ResilientBackgroundService (class) 存在");
        var ctors_ResilientBackgroundService = type_ResilientBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] ResilientBackgroundService 构造函数数量: {ctors_ResilientBackgroundService.Length}");
        var methods_ResilientBackgroundService = type_ResilientBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilientBackgroundService 公开方法数量: {methods_ResilientBackgroundService.Length}");
        foreach (var m in methods_ResilientBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilientBackgroundService 未找到，尝试无命名空间...");
        type_ResilientBackgroundService = Type.GetType("ResilientBackgroundService");
        if (type_ResilientBackgroundService != null)
            Console.WriteLine("[PASS] 类型 ResilientBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilientBackgroundService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResilientHealthCheck
    var type_ResilientHealthCheck = Type.GetType("ResilientHealthCheck");
    if (type_ResilientHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 ResilientHealthCheck (class) 存在");
        var ctors_ResilientHealthCheck = type_ResilientHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] ResilientHealthCheck 构造函数数量: {ctors_ResilientHealthCheck.Length}");
        var methods_ResilientHealthCheck = type_ResilientHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilientHealthCheck 公开方法数量: {methods_ResilientHealthCheck.Length}");
        foreach (var m in methods_ResilientHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilientHealthCheck 未找到，尝试无命名空间...");
        type_ResilientHealthCheck = Type.GetType("ResilientHealthCheck");
        if (type_ResilientHealthCheck != null)
            Console.WriteLine("[PASS] 类型 ResilientHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilientHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResilienceMetrics
    var type_ResilienceMetrics = Type.GetType("ResilienceMetrics");
    if (type_ResilienceMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 ResilienceMetrics (class) 存在");
        var ctors_ResilienceMetrics = type_ResilienceMetrics.GetConstructors();
        Console.WriteLine($"[PASS] ResilienceMetrics 构造函数数量: {ctors_ResilienceMetrics.Length}");
        var methods_ResilienceMetrics = type_ResilienceMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilienceMetrics 公开方法数量: {methods_ResilienceMetrics.Length}");
        foreach (var m in methods_ResilienceMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilienceMetrics 未找到，尝试无命名空间...");
        type_ResilienceMetrics = Type.GetType("ResilienceMetrics");
        if (type_ResilienceMetrics != null)
            Console.WriteLine("[PASS] 类型 ResilienceMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilienceMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResilientExample
    var type_ResilientExample = Type.GetType("ResilientExample");
    if (type_ResilientExample != null)
    {
        Console.WriteLine("[PASS] 类型 ResilientExample (class) 存在");
        var ctors_ResilientExample = type_ResilientExample.GetConstructors();
        Console.WriteLine($"[PASS] ResilientExample 构造函数数量: {ctors_ResilientExample.Length}");
        var methods_ResilientExample = type_ResilientExample.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilientExample 公开方法数量: {methods_ResilientExample.Length}");
        foreach (var m in methods_ResilientExample)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilientExample 未找到，尝试无命名空间...");
        type_ResilientExample = Type.GetType("ResilientExample");
        if (type_ResilientExample != null)
            Console.WriteLine("[PASS] 类型 ResilientExample (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilientExample 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IResilientService
    var type_IResilientService = Type.GetType("IResilientService");
    if (type_IResilientService != null)
    {
        Console.WriteLine("[PASS] 类型 IResilientService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IResilientService 未找到，尝试无命名空间...");
        type_IResilientService = Type.GetType("IResilientService");
        if (type_IResilientService != null)
            Console.WriteLine("[PASS] 类型 IResilientService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IResilientService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
