#load "htmlagilitypack_integration.cs"

Console.WriteLine("=== htmlagilitypack_integration.cs Test ===");

try
{
    // 验证 class: HtmlAgilityPackIntegration
    var type_HtmlAgilityPackIntegration = Type.GetType("HtmlAgilityPackIntegration");
    if (type_HtmlAgilityPackIntegration != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlAgilityPackIntegration (class) 存在");
        var ctors_HtmlAgilityPackIntegration = type_HtmlAgilityPackIntegration.GetConstructors();
        Console.WriteLine($"[PASS] HtmlAgilityPackIntegration 构造函数数量: {ctors_HtmlAgilityPackIntegration.Length}");
        var methods_HtmlAgilityPackIntegration = type_HtmlAgilityPackIntegration.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlAgilityPackIntegration 公开方法数量: {methods_HtmlAgilityPackIntegration.Length}");
        foreach (var m in methods_HtmlAgilityPackIntegration)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlAgilityPackIntegration 未找到，尝试无命名空间...");
        type_HtmlAgilityPackIntegration = Type.GetType("HtmlAgilityPackIntegration");
        if (type_HtmlAgilityPackIntegration != null)
            Console.WriteLine("[PASS] 类型 HtmlAgilityPackIntegration (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HtmlAgilityPackIntegration 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HtmlAgilityPackService
    var type_HtmlAgilityPackService = Type.GetType("HtmlAgilityPackService");
    if (type_HtmlAgilityPackService != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlAgilityPackService (class) 存在");
        var ctors_HtmlAgilityPackService = type_HtmlAgilityPackService.GetConstructors();
        Console.WriteLine($"[PASS] HtmlAgilityPackService 构造函数数量: {ctors_HtmlAgilityPackService.Length}");
        var methods_HtmlAgilityPackService = type_HtmlAgilityPackService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlAgilityPackService 公开方法数量: {methods_HtmlAgilityPackService.Length}");
        foreach (var m in methods_HtmlAgilityPackService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlAgilityPackService 未找到，尝试无命名空间...");
        type_HtmlAgilityPackService = Type.GetType("HtmlAgilityPackService");
        if (type_HtmlAgilityPackService != null)
            Console.WriteLine("[PASS] 类型 HtmlAgilityPackService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HtmlAgilityPackService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HtmlParserOptions
    var type_HtmlParserOptions = Type.GetType("HtmlParserOptions");
    if (type_HtmlParserOptions != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlParserOptions (class) 存在");
        var ctors_HtmlParserOptions = type_HtmlParserOptions.GetConstructors();
        Console.WriteLine($"[PASS] HtmlParserOptions 构造函数数量: {ctors_HtmlParserOptions.Length}");
        var methods_HtmlParserOptions = type_HtmlParserOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlParserOptions 公开方法数量: {methods_HtmlParserOptions.Length}");
        foreach (var m in methods_HtmlParserOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlParserOptions 未找到，尝试无命名空间...");
        type_HtmlParserOptions = Type.GetType("HtmlParserOptions");
        if (type_HtmlParserOptions != null)
            Console.WriteLine("[PASS] 类型 HtmlParserOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HtmlParserOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HtmlSanitizerService
    var type_HtmlSanitizerService = Type.GetType("HtmlSanitizerService");
    if (type_HtmlSanitizerService != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlSanitizerService (class) 存在");
        var ctors_HtmlSanitizerService = type_HtmlSanitizerService.GetConstructors();
        Console.WriteLine($"[PASS] HtmlSanitizerService 构造函数数量: {ctors_HtmlSanitizerService.Length}");
        var methods_HtmlSanitizerService = type_HtmlSanitizerService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlSanitizerService 公开方法数量: {methods_HtmlSanitizerService.Length}");
        foreach (var m in methods_HtmlSanitizerService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlSanitizerService 未找到，尝试无命名空间...");
        type_HtmlSanitizerService = Type.GetType("HtmlSanitizerService");
        if (type_HtmlSanitizerService != null)
            Console.WriteLine("[PASS] 类型 HtmlSanitizerService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HtmlSanitizerService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HtmlProcessingPipeline
    var type_HtmlProcessingPipeline = Type.GetType("HtmlProcessingPipeline");
    if (type_HtmlProcessingPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlProcessingPipeline (class) 存在");
        var ctors_HtmlProcessingPipeline = type_HtmlProcessingPipeline.GetConstructors();
        Console.WriteLine($"[PASS] HtmlProcessingPipeline 构造函数数量: {ctors_HtmlProcessingPipeline.Length}");
        var methods_HtmlProcessingPipeline = type_HtmlProcessingPipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlProcessingPipeline 公开方法数量: {methods_HtmlProcessingPipeline.Length}");
        foreach (var m in methods_HtmlProcessingPipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlProcessingPipeline 未找到，尝试无命名空间...");
        type_HtmlProcessingPipeline = Type.GetType("HtmlProcessingPipeline");
        if (type_HtmlProcessingPipeline != null)
            Console.WriteLine("[PASS] 类型 HtmlProcessingPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HtmlProcessingPipeline 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HtmlPerformanceMonitor
    var type_HtmlPerformanceMonitor = Type.GetType("HtmlPerformanceMonitor");
    if (type_HtmlPerformanceMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 HtmlPerformanceMonitor (class) 存在");
        var ctors_HtmlPerformanceMonitor = type_HtmlPerformanceMonitor.GetConstructors();
        Console.WriteLine($"[PASS] HtmlPerformanceMonitor 构造函数数量: {ctors_HtmlPerformanceMonitor.Length}");
        var methods_HtmlPerformanceMonitor = type_HtmlPerformanceMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HtmlPerformanceMonitor 公开方法数量: {methods_HtmlPerformanceMonitor.Length}");
        foreach (var m in methods_HtmlPerformanceMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HtmlPerformanceMonitor 未找到，尝试无命名空间...");
        type_HtmlPerformanceMonitor = Type.GetType("HtmlPerformanceMonitor");
        if (type_HtmlPerformanceMonitor != null)
            Console.WriteLine("[PASS] 类型 HtmlPerformanceMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HtmlPerformanceMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: XPathQueryExecutor
    var type_XPathQueryExecutor = Type.GetType("XPathQueryExecutor");
    if (type_XPathQueryExecutor != null)
    {
        Console.WriteLine("[PASS] 类型 XPathQueryExecutor (class) 存在");
        var ctors_XPathQueryExecutor = type_XPathQueryExecutor.GetConstructors();
        Console.WriteLine($"[PASS] XPathQueryExecutor 构造函数数量: {ctors_XPathQueryExecutor.Length}");
        var methods_XPathQueryExecutor = type_XPathQueryExecutor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] XPathQueryExecutor 公开方法数量: {methods_XPathQueryExecutor.Length}");
        foreach (var m in methods_XPathQueryExecutor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 XPathQueryExecutor 未找到，尝试无命名空间...");
        type_XPathQueryExecutor = Type.GetType("XPathQueryExecutor");
        if (type_XPathQueryExecutor != null)
            Console.WriteLine("[PASS] 类型 XPathQueryExecutor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 XPathQueryExecutor 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IXPathQueryExecutor
    var type_IXPathQueryExecutor = Type.GetType("IXPathQueryExecutor");
    if (type_IXPathQueryExecutor != null)
    {
        Console.WriteLine("[PASS] 类型 IXPathQueryExecutor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IXPathQueryExecutor 未找到，尝试无命名空间...");
        type_IXPathQueryExecutor = Type.GetType("IXPathQueryExecutor");
        if (type_IXPathQueryExecutor != null)
            Console.WriteLine("[PASS] 类型 IXPathQueryExecutor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IXPathQueryExecutor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
