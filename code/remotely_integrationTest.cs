#load "remotely_integration.cs"

Console.WriteLine("=== remotely_integration.cs Test ===");

try
{
    // 验证 class: Remotely.Integration.RemotelyOptions
    var type_RemotelyOptions = Type.GetType("Remotely.Integration.RemotelyOptions");
    if (type_RemotelyOptions != null)
    {
        Console.WriteLine("[PASS] 类型 Remotely.Integration.RemotelyOptions (class) 存在");
        var ctors_RemotelyOptions = type_RemotelyOptions.GetConstructors();
        Console.WriteLine($"[PASS] Remotely.Integration.RemotelyOptions 构造函数数量: {ctors_RemotelyOptions.Length}");
        var methods_RemotelyOptions = type_RemotelyOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Remotely.Integration.RemotelyOptions 公开方法数量: {methods_RemotelyOptions.Length}");
        foreach (var m in methods_RemotelyOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Remotely.Integration.RemotelyOptions 未找到，尝试无命名空间...");
        type_RemotelyOptions = Type.GetType("RemotelyOptions");
        if (type_RemotelyOptions != null)
            Console.WriteLine("[PASS] 类型 RemotelyOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RemotelyOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Remotely.Integration.RemotelyService
    var type_RemotelyService = Type.GetType("Remotely.Integration.RemotelyService");
    if (type_RemotelyService != null)
    {
        Console.WriteLine("[PASS] 类型 Remotely.Integration.RemotelyService (class) 存在");
        var ctors_RemotelyService = type_RemotelyService.GetConstructors();
        Console.WriteLine($"[PASS] Remotely.Integration.RemotelyService 构造函数数量: {ctors_RemotelyService.Length}");
        var methods_RemotelyService = type_RemotelyService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Remotely.Integration.RemotelyService 公开方法数量: {methods_RemotelyService.Length}");
        foreach (var m in methods_RemotelyService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Remotely.Integration.RemotelyService 未找到，尝试无命名空间...");
        type_RemotelyService = Type.GetType("RemotelyService");
        if (type_RemotelyService != null)
            Console.WriteLine("[PASS] 类型 RemotelyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RemotelyService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Remotely.Integration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("Remotely.Integration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 Remotely.Integration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] Remotely.Integration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Remotely.Integration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Remotely.Integration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Remotely.Integration.RemotelyHostedService
    var type_RemotelyHostedService = Type.GetType("Remotely.Integration.RemotelyHostedService");
    if (type_RemotelyHostedService != null)
    {
        Console.WriteLine("[PASS] 类型 Remotely.Integration.RemotelyHostedService (class) 存在");
        var ctors_RemotelyHostedService = type_RemotelyHostedService.GetConstructors();
        Console.WriteLine($"[PASS] Remotely.Integration.RemotelyHostedService 构造函数数量: {ctors_RemotelyHostedService.Length}");
        var methods_RemotelyHostedService = type_RemotelyHostedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Remotely.Integration.RemotelyHostedService 公开方法数量: {methods_RemotelyHostedService.Length}");
        foreach (var m in methods_RemotelyHostedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Remotely.Integration.RemotelyHostedService 未找到，尝试无命名空间...");
        type_RemotelyHostedService = Type.GetType("RemotelyHostedService");
        if (type_RemotelyHostedService != null)
            Console.WriteLine("[PASS] 类型 RemotelyHostedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RemotelyHostedService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: Remotely.Integration.IRemotelyService
    var type_IRemotelyService = Type.GetType("Remotely.Integration.IRemotelyService");
    if (type_IRemotelyService != null)
    {
        Console.WriteLine("[PASS] 类型 Remotely.Integration.IRemotelyService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Remotely.Integration.IRemotelyService 未找到，尝试无命名空间...");
        type_IRemotelyService = Type.GetType("IRemotelyService");
        if (type_IRemotelyService != null)
            Console.WriteLine("[PASS] 类型 IRemotelyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IRemotelyService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
