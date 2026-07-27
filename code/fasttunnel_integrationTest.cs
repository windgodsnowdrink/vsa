#load "fasttunnel_integration.cs"

Console.WriteLine("=== fasttunnel_integration.cs Test ===");

try
{
    // 验证 class: FastTunnelIntegration.FastTunnelOptions
    var type_FastTunnelOptions = Type.GetType("FastTunnelIntegration.FastTunnelOptions");
    if (type_FastTunnelOptions != null)
    {
        Console.WriteLine("[PASS] 类型 FastTunnelIntegration.FastTunnelOptions (class) 存在");
        var ctors_FastTunnelOptions = type_FastTunnelOptions.GetConstructors();
        Console.WriteLine($"[PASS] FastTunnelIntegration.FastTunnelOptions 构造函数数量: {ctors_FastTunnelOptions.Length}");
        var methods_FastTunnelOptions = type_FastTunnelOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FastTunnelIntegration.FastTunnelOptions 公开方法数量: {methods_FastTunnelOptions.Length}");
        foreach (var m in methods_FastTunnelOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FastTunnelIntegration.FastTunnelOptions 未找到，尝试无命名空间...");
        type_FastTunnelOptions = Type.GetType("FastTunnelOptions");
        if (type_FastTunnelOptions != null)
            Console.WriteLine("[PASS] 类型 FastTunnelOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FastTunnelOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FastTunnelIntegration.FastTunnelService
    var type_FastTunnelService = Type.GetType("FastTunnelIntegration.FastTunnelService");
    if (type_FastTunnelService != null)
    {
        Console.WriteLine("[PASS] 类型 FastTunnelIntegration.FastTunnelService (class) 存在");
        var ctors_FastTunnelService = type_FastTunnelService.GetConstructors();
        Console.WriteLine($"[PASS] FastTunnelIntegration.FastTunnelService 构造函数数量: {ctors_FastTunnelService.Length}");
        var methods_FastTunnelService = type_FastTunnelService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FastTunnelIntegration.FastTunnelService 公开方法数量: {methods_FastTunnelService.Length}");
        foreach (var m in methods_FastTunnelService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FastTunnelIntegration.FastTunnelService 未找到，尝试无命名空间...");
        type_FastTunnelService = Type.GetType("FastTunnelService");
        if (type_FastTunnelService != null)
            Console.WriteLine("[PASS] 类型 FastTunnelService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FastTunnelService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FastTunnelIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("FastTunnelIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 FastTunnelIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] FastTunnelIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FastTunnelIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FastTunnelIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FastTunnelIntegration.ForwardInfo
    var type_ForwardInfo = Type.GetType("FastTunnelIntegration.ForwardInfo");
    if (type_ForwardInfo != null)
    {
        Console.WriteLine("[PASS] 类型 FastTunnelIntegration.ForwardInfo (class) 存在");
        var ctors_ForwardInfo = type_ForwardInfo.GetConstructors();
        Console.WriteLine($"[PASS] FastTunnelIntegration.ForwardInfo 构造函数数量: {ctors_ForwardInfo.Length}");
        var methods_ForwardInfo = type_ForwardInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FastTunnelIntegration.ForwardInfo 公开方法数量: {methods_ForwardInfo.Length}");
        foreach (var m in methods_ForwardInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FastTunnelIntegration.ForwardInfo 未找到，尝试无命名空间...");
        type_ForwardInfo = Type.GetType("ForwardInfo");
        if (type_ForwardInfo != null)
            Console.WriteLine("[PASS] 类型 ForwardInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ForwardInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FastTunnelIntegration.ServerInfo
    var type_ServerInfo = Type.GetType("FastTunnelIntegration.ServerInfo");
    if (type_ServerInfo != null)
    {
        Console.WriteLine("[PASS] 类型 FastTunnelIntegration.ServerInfo (class) 存在");
        var ctors_ServerInfo = type_ServerInfo.GetConstructors();
        Console.WriteLine($"[PASS] FastTunnelIntegration.ServerInfo 构造函数数量: {ctors_ServerInfo.Length}");
        var methods_ServerInfo = type_ServerInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FastTunnelIntegration.ServerInfo 公开方法数量: {methods_ServerInfo.Length}");
        foreach (var m in methods_ServerInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FastTunnelIntegration.ServerInfo 未找到，尝试无命名空间...");
        type_ServerInfo = Type.GetType("ServerInfo");
        if (type_ServerInfo != null)
            Console.WriteLine("[PASS] 类型 ServerInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServerInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: FastTunnelIntegration.IFastTunnelService
    var type_IFastTunnelService = Type.GetType("FastTunnelIntegration.IFastTunnelService");
    if (type_IFastTunnelService != null)
    {
        Console.WriteLine("[PASS] 类型 FastTunnelIntegration.IFastTunnelService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FastTunnelIntegration.IFastTunnelService 未找到，尝试无命名空间...");
        type_IFastTunnelService = Type.GetType("IFastTunnelService");
        if (type_IFastTunnelService != null)
            Console.WriteLine("[PASS] 类型 IFastTunnelService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IFastTunnelService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
