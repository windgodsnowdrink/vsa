#load "uploadstream_integration.cs"

Console.WriteLine("=== uploadstream_integration.cs Test ===");

try
{
    // 验证 class: UploadStreamService
    var type_UploadStreamService = Type.GetType("UploadStreamService");
    if (type_UploadStreamService != null)
    {
        Console.WriteLine("[PASS] 类型 UploadStreamService (class) 存在");
        var ctors_UploadStreamService = type_UploadStreamService.GetConstructors();
        Console.WriteLine($"[PASS] UploadStreamService 构造函数数量: {ctors_UploadStreamService.Length}");
        var methods_UploadStreamService = type_UploadStreamService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UploadStreamService 公开方法数量: {methods_UploadStreamService.Length}");
        foreach (var m in methods_UploadStreamService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UploadStreamService 未找到，尝试无命名空间...");
        type_UploadStreamService = Type.GetType("UploadStreamService");
        if (type_UploadStreamService != null)
            Console.WriteLine("[PASS] 类型 UploadStreamService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UploadStreamService 可能为顶层语句或嵌套类型");
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

    // 验证 class: UploadProgress
    var type_UploadProgress = Type.GetType("UploadProgress");
    if (type_UploadProgress != null)
    {
        Console.WriteLine("[PASS] 类型 UploadProgress (class) 存在");
        var ctors_UploadProgress = type_UploadProgress.GetConstructors();
        Console.WriteLine($"[PASS] UploadProgress 构造函数数量: {ctors_UploadProgress.Length}");
        var methods_UploadProgress = type_UploadProgress.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UploadProgress 公开方法数量: {methods_UploadProgress.Length}");
        foreach (var m in methods_UploadProgress)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UploadProgress 未找到，尝试无命名空间...");
        type_UploadProgress = Type.GetType("UploadProgress");
        if (type_UploadProgress != null)
            Console.WriteLine("[PASS] 类型 UploadProgress (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UploadProgress 可能为顶层语句或嵌套类型");
    }

    // 验证 class: UploadStreamServiceExtensions
    var type_UploadStreamServiceExtensions = Type.GetType("UploadStreamServiceExtensions");
    if (type_UploadStreamServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 UploadStreamServiceExtensions (class) 存在");
        var ctors_UploadStreamServiceExtensions = type_UploadStreamServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] UploadStreamServiceExtensions 构造函数数量: {ctors_UploadStreamServiceExtensions.Length}");
        var methods_UploadStreamServiceExtensions = type_UploadStreamServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UploadStreamServiceExtensions 公开方法数量: {methods_UploadStreamServiceExtensions.Length}");
        foreach (var m in methods_UploadStreamServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UploadStreamServiceExtensions 未找到，尝试无命名空间...");
        type_UploadStreamServiceExtensions = Type.GetType("UploadStreamServiceExtensions");
        if (type_UploadStreamServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 UploadStreamServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UploadStreamServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IUploadStreamService
    var type_IUploadStreamService = Type.GetType("IUploadStreamService");
    if (type_IUploadStreamService != null)
    {
        Console.WriteLine("[PASS] 类型 IUploadStreamService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IUploadStreamService 未找到，尝试无命名空间...");
        type_IUploadStreamService = Type.GetType("IUploadStreamService");
        if (type_IUploadStreamService != null)
            Console.WriteLine("[PASS] 类型 IUploadStreamService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IUploadStreamService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UploadTask
    var type_UploadTask = Type.GetType("UploadTask");
    if (type_UploadTask != null)
    {
        Console.WriteLine("[PASS] 类型 UploadTask (record) 存在");
        var ctors_UploadTask = type_UploadTask.GetConstructors();
        Console.WriteLine($"[PASS] UploadTask 构造函数数量: {ctors_UploadTask.Length}");
        var methods_UploadTask = type_UploadTask.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UploadTask 公开方法数量: {methods_UploadTask.Length}");
        foreach (var m in methods_UploadTask)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UploadTask 未找到，尝试无命名空间...");
        type_UploadTask = Type.GetType("UploadTask");
        if (type_UploadTask != null)
            Console.WriteLine("[PASS] 类型 UploadTask (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UploadTask 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
