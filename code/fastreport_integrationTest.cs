#load "fastreport_integration.cs"

Console.WriteLine("=== fastreport_integration.cs Test ===");

try
{
    // 验证 class: FastReportOptions
    var type_FastReportOptions = Type.GetType("FastReportOptions");
    if (type_FastReportOptions != null)
    {
        Console.WriteLine("[PASS] 类型 FastReportOptions (class) 存在");
        var ctors_FastReportOptions = type_FastReportOptions.GetConstructors();
        Console.WriteLine($"[PASS] FastReportOptions 构造函数数量: {ctors_FastReportOptions.Length}");
        var methods_FastReportOptions = type_FastReportOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FastReportOptions 公开方法数量: {methods_FastReportOptions.Length}");
        foreach (var m in methods_FastReportOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FastReportOptions 未找到，尝试无命名空间...");
        type_FastReportOptions = Type.GetType("FastReportOptions");
        if (type_FastReportOptions != null)
            Console.WriteLine("[PASS] 类型 FastReportOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FastReportOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ReportService
    var type_ReportService = Type.GetType("ReportService");
    if (type_ReportService != null)
    {
        Console.WriteLine("[PASS] 类型 ReportService (class) 存在");
        var ctors_ReportService = type_ReportService.GetConstructors();
        Console.WriteLine($"[PASS] ReportService 构造函数数量: {ctors_ReportService.Length}");
        var methods_ReportService = type_ReportService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReportService 公开方法数量: {methods_ReportService.Length}");
        foreach (var m in methods_ReportService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReportService 未找到，尝试无命名空间...");
        type_ReportService = Type.GetType("ReportService");
        if (type_ReportService != null)
            Console.WriteLine("[PASS] 类型 ReportService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReportService 可能为顶层语句或嵌套类型");
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

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IReportService
    var type_IReportService = Type.GetType("IReportService");
    if (type_IReportService != null)
    {
        Console.WriteLine("[PASS] 类型 IReportService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IReportService 未找到，尝试无命名空间...");
        type_IReportService = Type.GetType("IReportService");
        if (type_IReportService != null)
            Console.WriteLine("[PASS] 类型 IReportService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IReportService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
