#load "echarts_integration.cs"

Console.WriteLine("=== echarts_integration.cs Test ===");

try
{
    // 验证 class: EChartsExtensions
    var type_EChartsExtensions = Type.GetType("EChartsExtensions");
    if (type_EChartsExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 EChartsExtensions (class) 存在");
        var ctors_EChartsExtensions = type_EChartsExtensions.GetConstructors();
        Console.WriteLine($"[PASS] EChartsExtensions 构造函数数量: {ctors_EChartsExtensions.Length}");
        var methods_EChartsExtensions = type_EChartsExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EChartsExtensions 公开方法数量: {methods_EChartsExtensions.Length}");
        foreach (var m in methods_EChartsExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EChartsExtensions 未找到，尝试无命名空间...");
        type_EChartsExtensions = Type.GetType("EChartsExtensions");
        if (type_EChartsExtensions != null)
            Console.WriteLine("[PASS] 类型 EChartsExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EChartsExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EChartsOptions
    var type_EChartsOptions = Type.GetType("EChartsOptions");
    if (type_EChartsOptions != null)
    {
        Console.WriteLine("[PASS] 类型 EChartsOptions (class) 存在");
        var ctors_EChartsOptions = type_EChartsOptions.GetConstructors();
        Console.WriteLine($"[PASS] EChartsOptions 构造函数数量: {ctors_EChartsOptions.Length}");
        var methods_EChartsOptions = type_EChartsOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EChartsOptions 公开方法数量: {methods_EChartsOptions.Length}");
        foreach (var m in methods_EChartsOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EChartsOptions 未找到，尝试无命名空间...");
        type_EChartsOptions = Type.GetType("EChartsOptions");
        if (type_EChartsOptions != null)
            Console.WriteLine("[PASS] 类型 EChartsOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EChartsOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EChartsInstancePooledObjectPolicy
    var type_EChartsInstancePooledObjectPolicy = Type.GetType("EChartsInstancePooledObjectPolicy");
    if (type_EChartsInstancePooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 EChartsInstancePooledObjectPolicy (class) 存在");
        var ctors_EChartsInstancePooledObjectPolicy = type_EChartsInstancePooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] EChartsInstancePooledObjectPolicy 构造函数数量: {ctors_EChartsInstancePooledObjectPolicy.Length}");
        var methods_EChartsInstancePooledObjectPolicy = type_EChartsInstancePooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EChartsInstancePooledObjectPolicy 公开方法数量: {methods_EChartsInstancePooledObjectPolicy.Length}");
        foreach (var m in methods_EChartsInstancePooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EChartsInstancePooledObjectPolicy 未找到，尝试无命名空间...");
        type_EChartsInstancePooledObjectPolicy = Type.GetType("EChartsInstancePooledObjectPolicy");
        if (type_EChartsInstancePooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 EChartsInstancePooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EChartsInstancePooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EChartsExamples
    var type_EChartsExamples = Type.GetType("EChartsExamples");
    if (type_EChartsExamples != null)
    {
        Console.WriteLine("[PASS] 类型 EChartsExamples (class) 存在");
        var ctors_EChartsExamples = type_EChartsExamples.GetConstructors();
        Console.WriteLine($"[PASS] EChartsExamples 构造函数数量: {ctors_EChartsExamples.Length}");
        var methods_EChartsExamples = type_EChartsExamples.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EChartsExamples 公开方法数量: {methods_EChartsExamples.Length}");
        foreach (var m in methods_EChartsExamples)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EChartsExamples 未找到，尝试无命名空间...");
        type_EChartsExamples = Type.GetType("EChartsExamples");
        if (type_EChartsExamples != null)
            Console.WriteLine("[PASS] 类型 EChartsExamples (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EChartsExamples 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EChartsRenderService
    var type_EChartsRenderService = Type.GetType("EChartsRenderService");
    if (type_EChartsRenderService != null)
    {
        Console.WriteLine("[PASS] 类型 EChartsRenderService (class) 存在");
        var ctors_EChartsRenderService = type_EChartsRenderService.GetConstructors();
        Console.WriteLine($"[PASS] EChartsRenderService 构造函数数量: {ctors_EChartsRenderService.Length}");
        var methods_EChartsRenderService = type_EChartsRenderService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EChartsRenderService 公开方法数量: {methods_EChartsRenderService.Length}");
        foreach (var m in methods_EChartsRenderService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EChartsRenderService 未找到，尝试无命名空间...");
        type_EChartsRenderService = Type.GetType("EChartsRenderService");
        if (type_EChartsRenderService != null)
            Console.WriteLine("[PASS] 类型 EChartsRenderService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EChartsRenderService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DynamicDataService
    var type_DynamicDataService = Type.GetType("DynamicDataService");
    if (type_DynamicDataService != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicDataService (class) 存在");
        var ctors_DynamicDataService = type_DynamicDataService.GetConstructors();
        Console.WriteLine($"[PASS] DynamicDataService 构造函数数量: {ctors_DynamicDataService.Length}");
        var methods_DynamicDataService = type_DynamicDataService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicDataService 公开方法数量: {methods_DynamicDataService.Length}");
        foreach (var m in methods_DynamicDataService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicDataService 未找到，尝试无命名空间...");
        type_DynamicDataService = Type.GetType("DynamicDataService");
        if (type_DynamicDataService != null)
            Console.WriteLine("[PASS] 类型 DynamicDataService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicDataService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: EChartsRenderJob
    var type_EChartsRenderJob = Type.GetType("EChartsRenderJob");
    if (type_EChartsRenderJob != null)
    {
        Console.WriteLine("[PASS] 类型 EChartsRenderJob (record) 存在");
        var ctors_EChartsRenderJob = type_EChartsRenderJob.GetConstructors();
        Console.WriteLine($"[PASS] EChartsRenderJob 构造函数数量: {ctors_EChartsRenderJob.Length}");
        var methods_EChartsRenderJob = type_EChartsRenderJob.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EChartsRenderJob 公开方法数量: {methods_EChartsRenderJob.Length}");
        foreach (var m in methods_EChartsRenderJob)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EChartsRenderJob 未找到，尝试无命名空间...");
        type_EChartsRenderJob = Type.GetType("EChartsRenderJob");
        if (type_EChartsRenderJob != null)
            Console.WriteLine("[PASS] 类型 EChartsRenderJob (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EChartsRenderJob 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
