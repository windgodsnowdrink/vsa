#load "oneremote_integration.cs"

Console.WriteLine("=== oneremote_integration.cs Test ===");

try
{
    // 验证 class: OneRemoteIntegration.OneRemoteOptions
    var type_OneRemoteOptions = Type.GetType("OneRemoteIntegration.OneRemoteOptions");
    if (type_OneRemoteOptions != null)
    {
        Console.WriteLine("[PASS] 类型 OneRemoteIntegration.OneRemoteOptions (class) 存在");
        var ctors_OneRemoteOptions = type_OneRemoteOptions.GetConstructors();
        Console.WriteLine($"[PASS] OneRemoteIntegration.OneRemoteOptions 构造函数数量: {ctors_OneRemoteOptions.Length}");
        var methods_OneRemoteOptions = type_OneRemoteOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OneRemoteIntegration.OneRemoteOptions 公开方法数量: {methods_OneRemoteOptions.Length}");
        foreach (var m in methods_OneRemoteOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OneRemoteIntegration.OneRemoteOptions 未找到，尝试无命名空间...");
        type_OneRemoteOptions = Type.GetType("OneRemoteOptions");
        if (type_OneRemoteOptions != null)
            Console.WriteLine("[PASS] 类型 OneRemoteOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OneRemoteOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OneRemoteIntegration.OneRemoteService
    var type_OneRemoteService = Type.GetType("OneRemoteIntegration.OneRemoteService");
    if (type_OneRemoteService != null)
    {
        Console.WriteLine("[PASS] 类型 OneRemoteIntegration.OneRemoteService (class) 存在");
        var ctors_OneRemoteService = type_OneRemoteService.GetConstructors();
        Console.WriteLine($"[PASS] OneRemoteIntegration.OneRemoteService 构造函数数量: {ctors_OneRemoteService.Length}");
        var methods_OneRemoteService = type_OneRemoteService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OneRemoteIntegration.OneRemoteService 公开方法数量: {methods_OneRemoteService.Length}");
        foreach (var m in methods_OneRemoteService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OneRemoteIntegration.OneRemoteService 未找到，尝试无命名空间...");
        type_OneRemoteService = Type.GetType("OneRemoteService");
        if (type_OneRemoteService != null)
            Console.WriteLine("[PASS] 类型 OneRemoteService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OneRemoteService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OneRemoteIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("OneRemoteIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 OneRemoteIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] OneRemoteIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OneRemoteIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OneRemoteIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OneRemoteIntegration.ConnectionInfo
    var type_ConnectionInfo = Type.GetType("OneRemoteIntegration.ConnectionInfo");
    if (type_ConnectionInfo != null)
    {
        Console.WriteLine("[PASS] 类型 OneRemoteIntegration.ConnectionInfo (class) 存在");
        var ctors_ConnectionInfo = type_ConnectionInfo.GetConstructors();
        Console.WriteLine($"[PASS] OneRemoteIntegration.ConnectionInfo 构造函数数量: {ctors_ConnectionInfo.Length}");
        var methods_ConnectionInfo = type_ConnectionInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OneRemoteIntegration.ConnectionInfo 公开方法数量: {methods_ConnectionInfo.Length}");
        foreach (var m in methods_ConnectionInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OneRemoteIntegration.ConnectionInfo 未找到，尝试无命名空间...");
        type_ConnectionInfo = Type.GetType("ConnectionInfo");
        if (type_ConnectionInfo != null)
            Console.WriteLine("[PASS] 类型 ConnectionInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConnectionInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OneRemoteIntegration.SessionInfo
    var type_SessionInfo = Type.GetType("OneRemoteIntegration.SessionInfo");
    if (type_SessionInfo != null)
    {
        Console.WriteLine("[PASS] 类型 OneRemoteIntegration.SessionInfo (class) 存在");
        var ctors_SessionInfo = type_SessionInfo.GetConstructors();
        Console.WriteLine($"[PASS] OneRemoteIntegration.SessionInfo 构造函数数量: {ctors_SessionInfo.Length}");
        var methods_SessionInfo = type_SessionInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OneRemoteIntegration.SessionInfo 公开方法数量: {methods_SessionInfo.Length}");
        foreach (var m in methods_SessionInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OneRemoteIntegration.SessionInfo 未找到，尝试无命名空间...");
        type_SessionInfo = Type.GetType("SessionInfo");
        if (type_SessionInfo != null)
            Console.WriteLine("[PASS] 类型 SessionInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SessionInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OneRemoteIntegration.ServerInfo
    var type_ServerInfo = Type.GetType("OneRemoteIntegration.ServerInfo");
    if (type_ServerInfo != null)
    {
        Console.WriteLine("[PASS] 类型 OneRemoteIntegration.ServerInfo (class) 存在");
        var ctors_ServerInfo = type_ServerInfo.GetConstructors();
        Console.WriteLine($"[PASS] OneRemoteIntegration.ServerInfo 构造函数数量: {ctors_ServerInfo.Length}");
        var methods_ServerInfo = type_ServerInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OneRemoteIntegration.ServerInfo 公开方法数量: {methods_ServerInfo.Length}");
        foreach (var m in methods_ServerInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OneRemoteIntegration.ServerInfo 未找到，尝试无命名空间...");
        type_ServerInfo = Type.GetType("ServerInfo");
        if (type_ServerInfo != null)
            Console.WriteLine("[PASS] 类型 ServerInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServerInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OneRemoteIntegration.ConnectionStats
    var type_ConnectionStats = Type.GetType("OneRemoteIntegration.ConnectionStats");
    if (type_ConnectionStats != null)
    {
        Console.WriteLine("[PASS] 类型 OneRemoteIntegration.ConnectionStats (class) 存在");
        var ctors_ConnectionStats = type_ConnectionStats.GetConstructors();
        Console.WriteLine($"[PASS] OneRemoteIntegration.ConnectionStats 构造函数数量: {ctors_ConnectionStats.Length}");
        var methods_ConnectionStats = type_ConnectionStats.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OneRemoteIntegration.ConnectionStats 公开方法数量: {methods_ConnectionStats.Length}");
        foreach (var m in methods_ConnectionStats)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OneRemoteIntegration.ConnectionStats 未找到，尝试无命名空间...");
        type_ConnectionStats = Type.GetType("ConnectionStats");
        if (type_ConnectionStats != null)
            Console.WriteLine("[PASS] 类型 ConnectionStats (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConnectionStats 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OneRemoteIntegration.ThroughputStats
    var type_ThroughputStats = Type.GetType("OneRemoteIntegration.ThroughputStats");
    if (type_ThroughputStats != null)
    {
        Console.WriteLine("[PASS] 类型 OneRemoteIntegration.ThroughputStats (class) 存在");
        var ctors_ThroughputStats = type_ThroughputStats.GetConstructors();
        Console.WriteLine($"[PASS] OneRemoteIntegration.ThroughputStats 构造函数数量: {ctors_ThroughputStats.Length}");
        var methods_ThroughputStats = type_ThroughputStats.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OneRemoteIntegration.ThroughputStats 公开方法数量: {methods_ThroughputStats.Length}");
        foreach (var m in methods_ThroughputStats)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OneRemoteIntegration.ThroughputStats 未找到，尝试无命名空间...");
        type_ThroughputStats = Type.GetType("ThroughputStats");
        if (type_ThroughputStats != null)
            Console.WriteLine("[PASS] 类型 ThroughputStats (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThroughputStats 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OneRemoteIntegration.HealthCheckResult
    var type_HealthCheckResult = Type.GetType("OneRemoteIntegration.HealthCheckResult");
    if (type_HealthCheckResult != null)
    {
        Console.WriteLine("[PASS] 类型 OneRemoteIntegration.HealthCheckResult (class) 存在");
        var ctors_HealthCheckResult = type_HealthCheckResult.GetConstructors();
        Console.WriteLine($"[PASS] OneRemoteIntegration.HealthCheckResult 构造函数数量: {ctors_HealthCheckResult.Length}");
        var methods_HealthCheckResult = type_HealthCheckResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OneRemoteIntegration.HealthCheckResult 公开方法数量: {methods_HealthCheckResult.Length}");
        foreach (var m in methods_HealthCheckResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OneRemoteIntegration.HealthCheckResult 未找到，尝试无命名空间...");
        type_HealthCheckResult = Type.GetType("HealthCheckResult");
        if (type_HealthCheckResult != null)
            Console.WriteLine("[PASS] 类型 HealthCheckResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HealthCheckResult 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: OneRemoteIntegration.IOneRemoteService
    var type_IOneRemoteService = Type.GetType("OneRemoteIntegration.IOneRemoteService");
    if (type_IOneRemoteService != null)
    {
        Console.WriteLine("[PASS] 类型 OneRemoteIntegration.IOneRemoteService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OneRemoteIntegration.IOneRemoteService 未找到，尝试无命名空间...");
        type_IOneRemoteService = Type.GetType("IOneRemoteService");
        if (type_IOneRemoteService != null)
            Console.WriteLine("[PASS] 类型 IOneRemoteService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IOneRemoteService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
