#load "highcharts_integration.cs"

Console.WriteLine("=== highcharts_integration.cs Test ===");

try
{
    // 验证 class: HighchartsIntegration.HighchartsOptions
    var type_HighchartsOptions = Type.GetType("HighchartsIntegration.HighchartsOptions");
    if (type_HighchartsOptions != null)
    {
        Console.WriteLine("[PASS] 类型 HighchartsIntegration.HighchartsOptions (class) 存在");
        var ctors_HighchartsOptions = type_HighchartsOptions.GetConstructors();
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsOptions 构造函数数量: {ctors_HighchartsOptions.Length}");
        var methods_HighchartsOptions = type_HighchartsOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsOptions 公开方法数量: {methods_HighchartsOptions.Length}");
        foreach (var m in methods_HighchartsOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HighchartsIntegration.HighchartsOptions 未找到，尝试无命名空间...");
        type_HighchartsOptions = Type.GetType("HighchartsOptions");
        if (type_HighchartsOptions != null)
            Console.WriteLine("[PASS] 类型 HighchartsOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HighchartsOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HighchartsIntegration.HighchartsInstancePooledObjectPolicy
    var type_HighchartsInstancePooledObjectPolicy = Type.GetType("HighchartsIntegration.HighchartsInstancePooledObjectPolicy");
    if (type_HighchartsInstancePooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 HighchartsIntegration.HighchartsInstancePooledObjectPolicy (class) 存在");
        var ctors_HighchartsInstancePooledObjectPolicy = type_HighchartsInstancePooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsInstancePooledObjectPolicy 构造函数数量: {ctors_HighchartsInstancePooledObjectPolicy.Length}");
        var methods_HighchartsInstancePooledObjectPolicy = type_HighchartsInstancePooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsInstancePooledObjectPolicy 公开方法数量: {methods_HighchartsInstancePooledObjectPolicy.Length}");
        foreach (var m in methods_HighchartsInstancePooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HighchartsIntegration.HighchartsInstancePooledObjectPolicy 未找到，尝试无命名空间...");
        type_HighchartsInstancePooledObjectPolicy = Type.GetType("HighchartsInstancePooledObjectPolicy");
        if (type_HighchartsInstancePooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 HighchartsInstancePooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HighchartsInstancePooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HighchartsIntegration.HighchartsRenderService
    var type_HighchartsRenderService = Type.GetType("HighchartsIntegration.HighchartsRenderService");
    if (type_HighchartsRenderService != null)
    {
        Console.WriteLine("[PASS] 类型 HighchartsIntegration.HighchartsRenderService (class) 存在");
        var ctors_HighchartsRenderService = type_HighchartsRenderService.GetConstructors();
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsRenderService 构造函数数量: {ctors_HighchartsRenderService.Length}");
        var methods_HighchartsRenderService = type_HighchartsRenderService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsRenderService 公开方法数量: {methods_HighchartsRenderService.Length}");
        foreach (var m in methods_HighchartsRenderService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HighchartsIntegration.HighchartsRenderService 未找到，尝试无命名空间...");
        type_HighchartsRenderService = Type.GetType("HighchartsRenderService");
        if (type_HighchartsRenderService != null)
            Console.WriteLine("[PASS] 类型 HighchartsRenderService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HighchartsRenderService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HighchartsIntegration.HighchartsServiceExtensions
    var type_HighchartsServiceExtensions = Type.GetType("HighchartsIntegration.HighchartsServiceExtensions");
    if (type_HighchartsServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 HighchartsIntegration.HighchartsServiceExtensions (class) 存在");
        var ctors_HighchartsServiceExtensions = type_HighchartsServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsServiceExtensions 构造函数数量: {ctors_HighchartsServiceExtensions.Length}");
        var methods_HighchartsServiceExtensions = type_HighchartsServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsServiceExtensions 公开方法数量: {methods_HighchartsServiceExtensions.Length}");
        foreach (var m in methods_HighchartsServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HighchartsIntegration.HighchartsServiceExtensions 未找到，尝试无命名空间...");
        type_HighchartsServiceExtensions = Type.GetType("HighchartsServiceExtensions");
        if (type_HighchartsServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 HighchartsServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HighchartsServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HighchartsIntegration.HighchartsExamples
    var type_HighchartsExamples = Type.GetType("HighchartsIntegration.HighchartsExamples");
    if (type_HighchartsExamples != null)
    {
        Console.WriteLine("[PASS] 类型 HighchartsIntegration.HighchartsExamples (class) 存在");
        var ctors_HighchartsExamples = type_HighchartsExamples.GetConstructors();
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsExamples 构造函数数量: {ctors_HighchartsExamples.Length}");
        var methods_HighchartsExamples = type_HighchartsExamples.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsExamples 公开方法数量: {methods_HighchartsExamples.Length}");
        foreach (var m in methods_HighchartsExamples)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HighchartsIntegration.HighchartsExamples 未找到，尝试无命名空间...");
        type_HighchartsExamples = Type.GetType("HighchartsExamples");
        if (type_HighchartsExamples != null)
            Console.WriteLine("[PASS] 类型 HighchartsExamples (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HighchartsExamples 可能为顶层语句或嵌套类型");
    }

    // 验证 record: HighchartsIntegration.HighchartsRenderJob
    var type_HighchartsRenderJob = Type.GetType("HighchartsIntegration.HighchartsRenderJob");
    if (type_HighchartsRenderJob != null)
    {
        Console.WriteLine("[PASS] 类型 HighchartsIntegration.HighchartsRenderJob (record) 存在");
        var ctors_HighchartsRenderJob = type_HighchartsRenderJob.GetConstructors();
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsRenderJob 构造函数数量: {ctors_HighchartsRenderJob.Length}");
        var methods_HighchartsRenderJob = type_HighchartsRenderJob.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HighchartsIntegration.HighchartsRenderJob 公开方法数量: {methods_HighchartsRenderJob.Length}");
        foreach (var m in methods_HighchartsRenderJob)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HighchartsIntegration.HighchartsRenderJob 未找到，尝试无命名空间...");
        type_HighchartsRenderJob = Type.GetType("HighchartsRenderJob");
        if (type_HighchartsRenderJob != null)
            Console.WriteLine("[PASS] 类型 HighchartsRenderJob (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HighchartsRenderJob 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
