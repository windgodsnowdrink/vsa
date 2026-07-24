#load "webdown_integration.cs"

Console.WriteLine("=== webdown_integration.cs Test ===");

try
{
    // 验证 class: WebDownService
    var type_WebDownService = Type.GetType("WebDownService");
    if (type_WebDownService != null)
    {
        Console.WriteLine("[PASS] 类型 WebDownService (class) 存在");
        var ctors_WebDownService = type_WebDownService.GetConstructors();
        Console.WriteLine($"[PASS] WebDownService 构造函数数量: {ctors_WebDownService.Length}");
        var methods_WebDownService = type_WebDownService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WebDownService 公开方法数量: {methods_WebDownService.Length}");
        foreach (var m in methods_WebDownService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WebDownService 未找到，尝试无命名空间...");
        type_WebDownService = Type.GetType("WebDownService");
        if (type_WebDownService != null)
            Console.WriteLine("[PASS] 类型 WebDownService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WebDownService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IWebDownService
    var type_IWebDownService = Type.GetType("IWebDownService");
    if (type_IWebDownService != null)
    {
        Console.WriteLine("[PASS] 类型 IWebDownService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IWebDownService 未找到，尝试无命名空间...");
        type_IWebDownService = Type.GetType("IWebDownService");
        if (type_IWebDownService != null)
            Console.WriteLine("[PASS] 类型 IWebDownService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IWebDownService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DownloadTask
    var type_DownloadTask = Type.GetType("DownloadTask");
    if (type_DownloadTask != null)
    {
        Console.WriteLine("[PASS] 类型 DownloadTask (record) 存在");
        var ctors_DownloadTask = type_DownloadTask.GetConstructors();
        Console.WriteLine($"[PASS] DownloadTask 构造函数数量: {ctors_DownloadTask.Length}");
        var methods_DownloadTask = type_DownloadTask.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DownloadTask 公开方法数量: {methods_DownloadTask.Length}");
        foreach (var m in methods_DownloadTask)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DownloadTask 未找到，尝试无命名空间...");
        type_DownloadTask = Type.GetType("DownloadTask");
        if (type_DownloadTask != null)
            Console.WriteLine("[PASS] 类型 DownloadTask (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DownloadTask 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
