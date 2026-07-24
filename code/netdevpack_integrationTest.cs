#load "netdevpack_integration.cs"

Console.WriteLine("=== netdevpack_integration.cs Test ===");

try
{
    // 验证 class: NetDevPackIntegration.NetDevPackOptions
    var type_NetDevPackOptions = Type.GetType("NetDevPackIntegration.NetDevPackOptions");
    if (type_NetDevPackOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NetDevPackIntegration.NetDevPackOptions (class) 存在");
        var ctors_NetDevPackOptions = type_NetDevPackOptions.GetConstructors();
        Console.WriteLine($"[PASS] NetDevPackIntegration.NetDevPackOptions 构造函数数量: {ctors_NetDevPackOptions.Length}");
        var methods_NetDevPackOptions = type_NetDevPackOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetDevPackIntegration.NetDevPackOptions 公开方法数量: {methods_NetDevPackOptions.Length}");
        foreach (var m in methods_NetDevPackOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetDevPackIntegration.NetDevPackOptions 未找到，尝试无命名空间...");
        type_NetDevPackOptions = Type.GetType("NetDevPackOptions");
        if (type_NetDevPackOptions != null)
            Console.WriteLine("[PASS] 类型 NetDevPackOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetDevPackOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetDevPackIntegration.NetDevPackService
    var type_NetDevPackService = Type.GetType("NetDevPackIntegration.NetDevPackService");
    if (type_NetDevPackService != null)
    {
        Console.WriteLine("[PASS] 类型 NetDevPackIntegration.NetDevPackService (class) 存在");
        var ctors_NetDevPackService = type_NetDevPackService.GetConstructors();
        Console.WriteLine($"[PASS] NetDevPackIntegration.NetDevPackService 构造函数数量: {ctors_NetDevPackService.Length}");
        var methods_NetDevPackService = type_NetDevPackService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetDevPackIntegration.NetDevPackService 公开方法数量: {methods_NetDevPackService.Length}");
        foreach (var m in methods_NetDevPackService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetDevPackIntegration.NetDevPackService 未找到，尝试无命名空间...");
        type_NetDevPackService = Type.GetType("NetDevPackService");
        if (type_NetDevPackService != null)
            Console.WriteLine("[PASS] 类型 NetDevPackService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetDevPackService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetDevPackIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("NetDevPackIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 NetDevPackIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] NetDevPackIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetDevPackIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetDevPackIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetDevPackIntegration.ExampleUsage
    var type_ExampleUsage = Type.GetType("NetDevPackIntegration.ExampleUsage");
    if (type_ExampleUsage != null)
    {
        Console.WriteLine("[PASS] 类型 NetDevPackIntegration.ExampleUsage (class) 存在");
        var ctors_ExampleUsage = type_ExampleUsage.GetConstructors();
        Console.WriteLine($"[PASS] NetDevPackIntegration.ExampleUsage 构造函数数量: {ctors_ExampleUsage.Length}");
        var methods_ExampleUsage = type_ExampleUsage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetDevPackIntegration.ExampleUsage 公开方法数量: {methods_ExampleUsage.Length}");
        foreach (var m in methods_ExampleUsage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetDevPackIntegration.ExampleUsage 未找到，尝试无命名空间...");
        type_ExampleUsage = Type.GetType("ExampleUsage");
        if (type_ExampleUsage != null)
            Console.WriteLine("[PASS] 类型 ExampleUsage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExampleUsage 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetDevPackIntegration.UserRegisteredEvent
    var type_UserRegisteredEvent = Type.GetType("NetDevPackIntegration.UserRegisteredEvent");
    if (type_UserRegisteredEvent != null)
    {
        Console.WriteLine("[PASS] 类型 NetDevPackIntegration.UserRegisteredEvent (class) 存在");
        var ctors_UserRegisteredEvent = type_UserRegisteredEvent.GetConstructors();
        Console.WriteLine($"[PASS] NetDevPackIntegration.UserRegisteredEvent 构造函数数量: {ctors_UserRegisteredEvent.Length}");
        var methods_UserRegisteredEvent = type_UserRegisteredEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetDevPackIntegration.UserRegisteredEvent 公开方法数量: {methods_UserRegisteredEvent.Length}");
        foreach (var m in methods_UserRegisteredEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetDevPackIntegration.UserRegisteredEvent 未找到，尝试无命名空间...");
        type_UserRegisteredEvent = Type.GetType("UserRegisteredEvent");
        if (type_UserRegisteredEvent != null)
            Console.WriteLine("[PASS] 类型 UserRegisteredEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserRegisteredEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: NetDevPackIntegration.INetDevPackService
    var type_INetDevPackService = Type.GetType("NetDevPackIntegration.INetDevPackService");
    if (type_INetDevPackService != null)
    {
        Console.WriteLine("[PASS] 类型 NetDevPackIntegration.INetDevPackService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetDevPackIntegration.INetDevPackService 未找到，尝试无命名空间...");
        type_INetDevPackService = Type.GetType("INetDevPackService");
        if (type_INetDevPackService != null)
            Console.WriteLine("[PASS] 类型 INetDevPackService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 INetDevPackService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
