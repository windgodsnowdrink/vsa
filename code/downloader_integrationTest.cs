#load "downloader_integration.cs"

Console.WriteLine("=== downloader_integration.cs Test ===");

try
{
    // 验证 class: DownloaderIntegration.DownloadService
    var type_DownloadService = Type.GetType("DownloaderIntegration.DownloadService");
    if (type_DownloadService != null)
    {
        Console.WriteLine("[PASS] 类型 DownloaderIntegration.DownloadService (class) 存在");
        var ctors_DownloadService = type_DownloadService.GetConstructors();
        Console.WriteLine($"[PASS] DownloaderIntegration.DownloadService 构造函数数量: {ctors_DownloadService.Length}");
        var methods_DownloadService = type_DownloadService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DownloaderIntegration.DownloadService 公开方法数量: {methods_DownloadService.Length}");
        foreach (var m in methods_DownloadService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DownloaderIntegration.DownloadService 未找到，尝试无命名空间...");
        type_DownloadService = Type.GetType("DownloadService");
        if (type_DownloadService != null)
            Console.WriteLine("[PASS] 类型 DownloadService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DownloadService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DownloaderIntegration.DownloadProgress
    var type_DownloadProgress = Type.GetType("DownloaderIntegration.DownloadProgress");
    if (type_DownloadProgress != null)
    {
        Console.WriteLine("[PASS] 类型 DownloaderIntegration.DownloadProgress (class) 存在");
        var ctors_DownloadProgress = type_DownloadProgress.GetConstructors();
        Console.WriteLine($"[PASS] DownloaderIntegration.DownloadProgress 构造函数数量: {ctors_DownloadProgress.Length}");
        var methods_DownloadProgress = type_DownloadProgress.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DownloaderIntegration.DownloadProgress 公开方法数量: {methods_DownloadProgress.Length}");
        foreach (var m in methods_DownloadProgress)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DownloaderIntegration.DownloadProgress 未找到，尝试无命名空间...");
        type_DownloadProgress = Type.GetType("DownloadProgress");
        if (type_DownloadProgress != null)
            Console.WriteLine("[PASS] 类型 DownloadProgress (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DownloadProgress 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DownloaderIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("DownloaderIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DownloaderIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DownloaderIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DownloaderIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DownloaderIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: DownloaderIntegration.IDownloadService
    var type_IDownloadService = Type.GetType("DownloaderIntegration.IDownloadService");
    if (type_IDownloadService != null)
    {
        Console.WriteLine("[PASS] 类型 DownloaderIntegration.IDownloadService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DownloaderIntegration.IDownloadService 未找到，尝试无命名空间...");
        type_IDownloadService = Type.GetType("IDownloadService");
        if (type_IDownloadService != null)
            Console.WriteLine("[PASS] 类型 IDownloadService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDownloadService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
