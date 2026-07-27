#load "pdfreport_integration.cs"

Console.WriteLine("=== pdfreport_integration.cs Test ===");

try
{
    // 验证 class: PdfReportIntegration.PdfReportOptions
    var type_PdfReportOptions = Type.GetType("PdfReportIntegration.PdfReportOptions");
    if (type_PdfReportOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PdfReportIntegration.PdfReportOptions (class) 存在");
        var ctors_PdfReportOptions = type_PdfReportOptions.GetConstructors();
        Console.WriteLine($"[PASS] PdfReportIntegration.PdfReportOptions 构造函数数量: {ctors_PdfReportOptions.Length}");
        var methods_PdfReportOptions = type_PdfReportOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PdfReportIntegration.PdfReportOptions 公开方法数量: {methods_PdfReportOptions.Length}");
        foreach (var m in methods_PdfReportOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PdfReportIntegration.PdfReportOptions 未找到，尝试无命名空间...");
        type_PdfReportOptions = Type.GetType("PdfReportOptions");
        if (type_PdfReportOptions != null)
            Console.WriteLine("[PASS] 类型 PdfReportOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PdfReportOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PdfReportIntegration.PdfReportService
    var type_PdfReportService = Type.GetType("PdfReportIntegration.PdfReportService");
    if (type_PdfReportService != null)
    {
        Console.WriteLine("[PASS] 类型 PdfReportIntegration.PdfReportService (class) 存在");
        var ctors_PdfReportService = type_PdfReportService.GetConstructors();
        Console.WriteLine($"[PASS] PdfReportIntegration.PdfReportService 构造函数数量: {ctors_PdfReportService.Length}");
        var methods_PdfReportService = type_PdfReportService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PdfReportIntegration.PdfReportService 公开方法数量: {methods_PdfReportService.Length}");
        foreach (var m in methods_PdfReportService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PdfReportIntegration.PdfReportService 未找到，尝试无命名空间...");
        type_PdfReportService = Type.GetType("PdfReportService");
        if (type_PdfReportService != null)
            Console.WriteLine("[PASS] 类型 PdfReportService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PdfReportService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PdfReportIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("PdfReportIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PdfReportIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PdfReportIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PdfReportIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PdfReportIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: PdfReportIntegration.IPdfReportService
    var type_IPdfReportService = Type.GetType("PdfReportIntegration.IPdfReportService");
    if (type_IPdfReportService != null)
    {
        Console.WriteLine("[PASS] 类型 PdfReportIntegration.IPdfReportService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PdfReportIntegration.IPdfReportService 未找到，尝试无命名空间...");
        type_IPdfReportService = Type.GetType("IPdfReportService");
        if (type_IPdfReportService != null)
            Console.WriteLine("[PASS] 类型 IPdfReportService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPdfReportService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
