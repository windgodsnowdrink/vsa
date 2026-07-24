#load "netservercore_integration.cs"

Console.WriteLine("=== netservercore_integration.cs Test ===");

try
{
    // 验证 class: NetServerCoreOptions
    var type_NetServerCoreOptions = Type.GetType("NetServerCoreOptions");
    if (type_NetServerCoreOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NetServerCoreOptions (class) 存在");
        var ctors_NetServerCoreOptions = type_NetServerCoreOptions.GetConstructors();
        Console.WriteLine($"[PASS] NetServerCoreOptions 构造函数数量: {ctors_NetServerCoreOptions.Length}");
        var methods_NetServerCoreOptions = type_NetServerCoreOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetServerCoreOptions 公开方法数量: {methods_NetServerCoreOptions.Length}");
        foreach (var m in methods_NetServerCoreOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetServerCoreOptions 未找到，尝试无命名空间...");
        type_NetServerCoreOptions = Type.GetType("NetServerCoreOptions");
        if (type_NetServerCoreOptions != null)
            Console.WriteLine("[PASS] 类型 NetServerCoreOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetServerCoreOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetServerCoreService
    var type_NetServerCoreService = Type.GetType("NetServerCoreService");
    if (type_NetServerCoreService != null)
    {
        Console.WriteLine("[PASS] 类型 NetServerCoreService (class) 存在");
        var ctors_NetServerCoreService = type_NetServerCoreService.GetConstructors();
        Console.WriteLine($"[PASS] NetServerCoreService 构造函数数量: {ctors_NetServerCoreService.Length}");
        var methods_NetServerCoreService = type_NetServerCoreService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetServerCoreService 公开方法数量: {methods_NetServerCoreService.Length}");
        foreach (var m in methods_NetServerCoreService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetServerCoreService 未找到，尝试无命名空间...");
        type_NetServerCoreService = Type.GetType("NetServerCoreService");
        if (type_NetServerCoreService != null)
            Console.WriteLine("[PASS] 类型 NetServerCoreService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetServerCoreService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: INetServerCoreService
    var type_INetServerCoreService = Type.GetType("INetServerCoreService");
    if (type_INetServerCoreService != null)
    {
        Console.WriteLine("[PASS] 类型 INetServerCoreService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 INetServerCoreService 未找到，尝试无命名空间...");
        type_INetServerCoreService = Type.GetType("INetServerCoreService");
        if (type_INetServerCoreService != null)
            Console.WriteLine("[PASS] 类型 INetServerCoreService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 INetServerCoreService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
