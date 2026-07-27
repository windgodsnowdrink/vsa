#load "fluentftp_integration.cs"

Console.WriteLine("=== fluentftp_integration.cs Test ===");

try
{
    // 验证 class: FtpConnectionPool
    var type_FtpConnectionPool = Type.GetType("FtpConnectionPool");
    if (type_FtpConnectionPool != null)
    {
        Console.WriteLine("[PASS] 类型 FtpConnectionPool (class) 存在");
        var ctors_FtpConnectionPool = type_FtpConnectionPool.GetConstructors();
        Console.WriteLine($"[PASS] FtpConnectionPool 构造函数数量: {ctors_FtpConnectionPool.Length}");
        var methods_FtpConnectionPool = type_FtpConnectionPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FtpConnectionPool 公开方法数量: {methods_FtpConnectionPool.Length}");
        foreach (var m in methods_FtpConnectionPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FtpConnectionPool 未找到，尝试无命名空间...");
        type_FtpConnectionPool = Type.GetType("FtpConnectionPool");
        if (type_FtpConnectionPool != null)
            Console.WriteLine("[PASS] 类型 FtpConnectionPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FtpConnectionPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FtpClientPooledObjectPolicy
    var type_FtpClientPooledObjectPolicy = Type.GetType("FtpClientPooledObjectPolicy");
    if (type_FtpClientPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 FtpClientPooledObjectPolicy (class) 存在");
        var ctors_FtpClientPooledObjectPolicy = type_FtpClientPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] FtpClientPooledObjectPolicy 构造函数数量: {ctors_FtpClientPooledObjectPolicy.Length}");
        var methods_FtpClientPooledObjectPolicy = type_FtpClientPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FtpClientPooledObjectPolicy 公开方法数量: {methods_FtpClientPooledObjectPolicy.Length}");
        foreach (var m in methods_FtpClientPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FtpClientPooledObjectPolicy 未找到，尝试无命名空间...");
        type_FtpClientPooledObjectPolicy = Type.GetType("FtpClientPooledObjectPolicy");
        if (type_FtpClientPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 FtpClientPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FtpClientPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FtpConfig
    var type_FtpConfig = Type.GetType("FtpConfig");
    if (type_FtpConfig != null)
    {
        Console.WriteLine("[PASS] 类型 FtpConfig (class) 存在");
        var ctors_FtpConfig = type_FtpConfig.GetConstructors();
        Console.WriteLine($"[PASS] FtpConfig 构造函数数量: {ctors_FtpConfig.Length}");
        var methods_FtpConfig = type_FtpConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FtpConfig 公开方法数量: {methods_FtpConfig.Length}");
        foreach (var m in methods_FtpConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FtpConfig 未找到，尝试无命名空间...");
        type_FtpConfig = Type.GetType("FtpConfig");
        if (type_FtpConfig != null)
            Console.WriteLine("[PASS] 类型 FtpConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FtpConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FtpService
    var type_FtpService = Type.GetType("FtpService");
    if (type_FtpService != null)
    {
        Console.WriteLine("[PASS] 类型 FtpService (class) 存在");
        var ctors_FtpService = type_FtpService.GetConstructors();
        Console.WriteLine($"[PASS] FtpService 构造函数数量: {ctors_FtpService.Length}");
        var methods_FtpService = type_FtpService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FtpService 公开方法数量: {methods_FtpService.Length}");
        foreach (var m in methods_FtpService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FtpService 未找到，尝试无命名空间...");
        type_FtpService = Type.GetType("FtpService");
        if (type_FtpService != null)
            Console.WriteLine("[PASS] 类型 FtpService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FtpService 可能为顶层语句或嵌套类型");
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

    // 验证 class: MyService
    var type_MyService = Type.GetType("MyService");
    if (type_MyService != null)
    {
        Console.WriteLine("[PASS] 类型 MyService (class) 存在");
        var ctors_MyService = type_MyService.GetConstructors();
        Console.WriteLine($"[PASS] MyService 构造函数数量: {ctors_MyService.Length}");
        var methods_MyService = type_MyService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MyService 公开方法数量: {methods_MyService.Length}");
        foreach (var m in methods_MyService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MyService 未找到，尝试无命名空间...");
        type_MyService = Type.GetType("MyService");
        if (type_MyService != null)
            Console.WriteLine("[PASS] 类型 MyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MyService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IFtpService
    var type_IFtpService = Type.GetType("IFtpService");
    if (type_IFtpService != null)
    {
        Console.WriteLine("[PASS] 类型 IFtpService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IFtpService 未找到，尝试无命名空间...");
        type_IFtpService = Type.GetType("IFtpService");
        if (type_IFtpService != null)
            Console.WriteLine("[PASS] 类型 IFtpService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IFtpService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
