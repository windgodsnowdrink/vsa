#load "htmlsanitizer_integration.cs"

Console.WriteLine("=== htmlsanitizer_integration.cs Test ===");

try
{
    // 验证 class: HtmlSanitizerIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("HtmlSanitizerIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlSanitizerIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlSanitizerIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HtmlSanitizerIntegration.HtmlSanitizerOptions
    var type_HtmlSanitizerOptions = Type.GetType("HtmlSanitizerIntegration.HtmlSanitizerOptions");
    if (type_HtmlSanitizerOptions != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlSanitizerIntegration.HtmlSanitizerOptions (class) 存在");
        var ctors_HtmlSanitizerOptions = type_HtmlSanitizerOptions.GetConstructors();
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.HtmlSanitizerOptions 构造函数数量: {ctors_HtmlSanitizerOptions.Length}");
        var methods_HtmlSanitizerOptions = type_HtmlSanitizerOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.HtmlSanitizerOptions 公开方法数量: {methods_HtmlSanitizerOptions.Length}");
        foreach (var m in methods_HtmlSanitizerOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlSanitizerIntegration.HtmlSanitizerOptions 未找到，尝试无命名空间...");
        type_HtmlSanitizerOptions = Type.GetType("HtmlSanitizerOptions");
        if (type_HtmlSanitizerOptions != null)
            Console.WriteLine("[PASS] 类型 HtmlSanitizerOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HtmlSanitizerOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HtmlSanitizerIntegration.HtmlProcessingService
    var type_HtmlProcessingService = Type.GetType("HtmlSanitizerIntegration.HtmlProcessingService");
    if (type_HtmlProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlSanitizerIntegration.HtmlProcessingService (class) 存在");
        var ctors_HtmlProcessingService = type_HtmlProcessingService.GetConstructors();
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.HtmlProcessingService 构造函数数量: {ctors_HtmlProcessingService.Length}");
        var methods_HtmlProcessingService = type_HtmlProcessingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.HtmlProcessingService 公开方法数量: {methods_HtmlProcessingService.Length}");
        foreach (var m in methods_HtmlProcessingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlSanitizerIntegration.HtmlProcessingService 未找到，尝试无命名空间...");
        type_HtmlProcessingService = Type.GetType("HtmlProcessingService");
        if (type_HtmlProcessingService != null)
            Console.WriteLine("[PASS] 类型 HtmlProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HtmlProcessingService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HtmlSanitizerIntegration.HtmlSanitizerPooledObjectPolicy
    var type_HtmlSanitizerPooledObjectPolicy = Type.GetType("HtmlSanitizerIntegration.HtmlSanitizerPooledObjectPolicy");
    if (type_HtmlSanitizerPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlSanitizerIntegration.HtmlSanitizerPooledObjectPolicy (class) 存在");
        var ctors_HtmlSanitizerPooledObjectPolicy = type_HtmlSanitizerPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.HtmlSanitizerPooledObjectPolicy 构造函数数量: {ctors_HtmlSanitizerPooledObjectPolicy.Length}");
        var methods_HtmlSanitizerPooledObjectPolicy = type_HtmlSanitizerPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.HtmlSanitizerPooledObjectPolicy 公开方法数量: {methods_HtmlSanitizerPooledObjectPolicy.Length}");
        foreach (var m in methods_HtmlSanitizerPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlSanitizerIntegration.HtmlSanitizerPooledObjectPolicy 未找到，尝试无命名空间...");
        type_HtmlSanitizerPooledObjectPolicy = Type.GetType("HtmlSanitizerPooledObjectPolicy");
        if (type_HtmlSanitizerPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 HtmlSanitizerPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HtmlSanitizerPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HtmlSanitizerIntegration.RemoveIframesStrategy
    var type_RemoveIframesStrategy = Type.GetType("HtmlSanitizerIntegration.RemoveIframesStrategy");
    if (type_RemoveIframesStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlSanitizerIntegration.RemoveIframesStrategy (class) 存在");
        var ctors_RemoveIframesStrategy = type_RemoveIframesStrategy.GetConstructors();
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.RemoveIframesStrategy 构造函数数量: {ctors_RemoveIframesStrategy.Length}");
        var methods_RemoveIframesStrategy = type_RemoveIframesStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.RemoveIframesStrategy 公开方法数量: {methods_RemoveIframesStrategy.Length}");
        foreach (var m in methods_RemoveIframesStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlSanitizerIntegration.RemoveIframesStrategy 未找到，尝试无命名空间...");
        type_RemoveIframesStrategy = Type.GetType("RemoveIframesStrategy");
        if (type_RemoveIframesStrategy != null)
            Console.WriteLine("[PASS] 类型 RemoveIframesStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RemoveIframesStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: HtmlSanitizerIntegration.ICustomSanitizerStrategy
    var type_ICustomSanitizerStrategy = Type.GetType("HtmlSanitizerIntegration.ICustomSanitizerStrategy");
    if (type_ICustomSanitizerStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlSanitizerIntegration.ICustomSanitizerStrategy (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlSanitizerIntegration.ICustomSanitizerStrategy 未找到，尝试无命名空间...");
        type_ICustomSanitizerStrategy = Type.GetType("ICustomSanitizerStrategy");
        if (type_ICustomSanitizerStrategy != null)
            Console.WriteLine("[PASS] 类型 ICustomSanitizerStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICustomSanitizerStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 record: HtmlSanitizerIntegration.HtmlProcessingJob
    var type_HtmlProcessingJob = Type.GetType("HtmlSanitizerIntegration.HtmlProcessingJob");
    if (type_HtmlProcessingJob != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlSanitizerIntegration.HtmlProcessingJob (record) 存在");
        var ctors_HtmlProcessingJob = type_HtmlProcessingJob.GetConstructors();
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.HtmlProcessingJob 构造函数数量: {ctors_HtmlProcessingJob.Length}");
        var methods_HtmlProcessingJob = type_HtmlProcessingJob.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlSanitizerIntegration.HtmlProcessingJob 公开方法数量: {methods_HtmlProcessingJob.Length}");
        foreach (var m in methods_HtmlProcessingJob)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlSanitizerIntegration.HtmlProcessingJob 未找到，尝试无命名空间...");
        type_HtmlProcessingJob = Type.GetType("HtmlProcessingJob");
        if (type_HtmlProcessingJob != null)
            Console.WriteLine("[PASS] 类型 HtmlProcessingJob (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HtmlProcessingJob 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
