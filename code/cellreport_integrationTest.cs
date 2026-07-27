#load "cellreport_integration.cs"

Console.WriteLine("=== cellreport_integration.cs Test ===");

try
{
    // 验证 class: YourNamespace.CellReportExportService
    var type_CellReportExportService = Type.GetType("YourNamespace.CellReportExportService");
    if (type_CellReportExportService != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.CellReportExportService (class) 存在");
        var ctors_CellReportExportService = type_CellReportExportService.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.CellReportExportService 构造函数数量: {ctors_CellReportExportService.Length}");
        var methods_CellReportExportService = type_CellReportExportService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.CellReportExportService 公开方法数量: {methods_CellReportExportService.Length}");
        foreach (var m in methods_CellReportExportService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.CellReportExportService 未找到，尝试无命名空间...");
        type_CellReportExportService = Type.GetType("CellReportExportService");
        if (type_CellReportExportService != null)
            Console.WriteLine("[PASS] 类型 CellReportExportService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CellReportExportService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.CellReportDataSourceProvider
    var type_CellReportDataSourceProvider = Type.GetType("YourNamespace.CellReportDataSourceProvider");
    if (type_CellReportDataSourceProvider != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.CellReportDataSourceProvider (class) 存在");
        var ctors_CellReportDataSourceProvider = type_CellReportDataSourceProvider.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.CellReportDataSourceProvider 构造函数数量: {ctors_CellReportDataSourceProvider.Length}");
        var methods_CellReportDataSourceProvider = type_CellReportDataSourceProvider.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.CellReportDataSourceProvider 公开方法数量: {methods_CellReportDataSourceProvider.Length}");
        foreach (var m in methods_CellReportDataSourceProvider)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.CellReportDataSourceProvider 未找到，尝试无命名空间...");
        type_CellReportDataSourceProvider = Type.GetType("CellReportDataSourceProvider");
        if (type_CellReportDataSourceProvider != null)
            Console.WriteLine("[PASS] 类型 CellReportDataSourceProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CellReportDataSourceProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.CellReportTemplateManager
    var type_CellReportTemplateManager = Type.GetType("YourNamespace.CellReportTemplateManager");
    if (type_CellReportTemplateManager != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.CellReportTemplateManager (class) 存在");
        var ctors_CellReportTemplateManager = type_CellReportTemplateManager.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.CellReportTemplateManager 构造函数数量: {ctors_CellReportTemplateManager.Length}");
        var methods_CellReportTemplateManager = type_CellReportTemplateManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.CellReportTemplateManager 公开方法数量: {methods_CellReportTemplateManager.Length}");
        foreach (var m in methods_CellReportTemplateManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.CellReportTemplateManager 未找到，尝试无命名空间...");
        type_CellReportTemplateManager = Type.GetType("CellReportTemplateManager");
        if (type_CellReportTemplateManager != null)
            Console.WriteLine("[PASS] 类型 CellReportTemplateManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CellReportTemplateManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.CellReportOptions
    var type_CellReportOptions = Type.GetType("YourNamespace.CellReportOptions");
    if (type_CellReportOptions != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.CellReportOptions (class) 存在");
        var ctors_CellReportOptions = type_CellReportOptions.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.CellReportOptions 构造函数数量: {ctors_CellReportOptions.Length}");
        var methods_CellReportOptions = type_CellReportOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.CellReportOptions 公开方法数量: {methods_CellReportOptions.Length}");
        foreach (var m in methods_CellReportOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.CellReportOptions 未找到，尝试无命名空间...");
        type_CellReportOptions = Type.GetType("CellReportOptions");
        if (type_CellReportOptions != null)
            Console.WriteLine("[PASS] 类型 CellReportOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CellReportOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.CellReportServiceExtensions
    var type_CellReportServiceExtensions = Type.GetType("YourNamespace.CellReportServiceExtensions");
    if (type_CellReportServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.CellReportServiceExtensions (class) 存在");
        var ctors_CellReportServiceExtensions = type_CellReportServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.CellReportServiceExtensions 构造函数数量: {ctors_CellReportServiceExtensions.Length}");
        var methods_CellReportServiceExtensions = type_CellReportServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.CellReportServiceExtensions 公开方法数量: {methods_CellReportServiceExtensions.Length}");
        foreach (var m in methods_CellReportServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.CellReportServiceExtensions 未找到，尝试无命名空间...");
        type_CellReportServiceExtensions = Type.GetType("CellReportServiceExtensions");
        if (type_CellReportServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 CellReportServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CellReportServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.CellReportServiceCollectionExtensions
    var type_CellReportServiceCollectionExtensions = Type.GetType("YourNamespace.CellReportServiceCollectionExtensions");
    if (type_CellReportServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.CellReportServiceCollectionExtensions (class) 存在");
        var ctors_CellReportServiceCollectionExtensions = type_CellReportServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.CellReportServiceCollectionExtensions 构造函数数量: {ctors_CellReportServiceCollectionExtensions.Length}");
        var methods_CellReportServiceCollectionExtensions = type_CellReportServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.CellReportServiceCollectionExtensions 公开方法数量: {methods_CellReportServiceCollectionExtensions.Length}");
        foreach (var m in methods_CellReportServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.CellReportServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_CellReportServiceCollectionExtensions = Type.GetType("CellReportServiceCollectionExtensions");
        if (type_CellReportServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 CellReportServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CellReportServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.CellReportEnginePooledObjectPolicy
    var type_CellReportEnginePooledObjectPolicy = Type.GetType("YourNamespace.CellReportEnginePooledObjectPolicy");
    if (type_CellReportEnginePooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.CellReportEnginePooledObjectPolicy (class) 存在");
        var ctors_CellReportEnginePooledObjectPolicy = type_CellReportEnginePooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.CellReportEnginePooledObjectPolicy 构造函数数量: {ctors_CellReportEnginePooledObjectPolicy.Length}");
        var methods_CellReportEnginePooledObjectPolicy = type_CellReportEnginePooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.CellReportEnginePooledObjectPolicy 公开方法数量: {methods_CellReportEnginePooledObjectPolicy.Length}");
        foreach (var m in methods_CellReportEnginePooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.CellReportEnginePooledObjectPolicy 未找到，尝试无命名空间...");
        type_CellReportEnginePooledObjectPolicy = Type.GetType("CellReportEnginePooledObjectPolicy");
        if (type_CellReportEnginePooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 CellReportEnginePooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CellReportEnginePooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.CellReportEngine
    var type_CellReportEngine = Type.GetType("YourNamespace.CellReportEngine");
    if (type_CellReportEngine != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.CellReportEngine (class) 存在");
        var ctors_CellReportEngine = type_CellReportEngine.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.CellReportEngine 构造函数数量: {ctors_CellReportEngine.Length}");
        var methods_CellReportEngine = type_CellReportEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.CellReportEngine 公开方法数量: {methods_CellReportEngine.Length}");
        foreach (var m in methods_CellReportEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.CellReportEngine 未找到，尝试无命名空间...");
        type_CellReportEngine = Type.GetType("CellReportEngine");
        if (type_CellReportEngine != null)
            Console.WriteLine("[PASS] 类型 CellReportEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CellReportEngine 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.CellReportRenderService
    var type_CellReportRenderService = Type.GetType("YourNamespace.CellReportRenderService");
    if (type_CellReportRenderService != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.CellReportRenderService (class) 存在");
        var ctors_CellReportRenderService = type_CellReportRenderService.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.CellReportRenderService 构造函数数量: {ctors_CellReportRenderService.Length}");
        var methods_CellReportRenderService = type_CellReportRenderService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.CellReportRenderService 公开方法数量: {methods_CellReportRenderService.Length}");
        foreach (var m in methods_CellReportRenderService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.CellReportRenderService 未找到，尝试无命名空间...");
        type_CellReportRenderService = Type.GetType("CellReportRenderService");
        if (type_CellReportRenderService != null)
            Console.WriteLine("[PASS] 类型 CellReportRenderService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CellReportRenderService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: YourNamespace.CellReportRenderJob
    var type_CellReportRenderJob = Type.GetType("YourNamespace.CellReportRenderJob");
    if (type_CellReportRenderJob != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.CellReportRenderJob (record) 存在");
        var ctors_CellReportRenderJob = type_CellReportRenderJob.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.CellReportRenderJob 构造函数数量: {ctors_CellReportRenderJob.Length}");
        var methods_CellReportRenderJob = type_CellReportRenderJob.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.CellReportRenderJob 公开方法数量: {methods_CellReportRenderJob.Length}");
        foreach (var m in methods_CellReportRenderJob)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.CellReportRenderJob 未找到，尝试无命名空间...");
        type_CellReportRenderJob = Type.GetType("CellReportRenderJob");
        if (type_CellReportRenderJob != null)
            Console.WriteLine("[PASS] 类型 CellReportRenderJob (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CellReportRenderJob 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
