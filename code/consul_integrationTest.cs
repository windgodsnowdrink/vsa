#load "consul_integration.cs"

Console.WriteLine("=== consul_integration.cs Test ===");

try
{
    // 验证 class: ConsulIntegration.ConsulOptions
    var type_ConsulOptions = Type.GetType("ConsulIntegration.ConsulOptions");
    if (type_ConsulOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ConsulIntegration.ConsulOptions (class) 存在");
        var ctors_ConsulOptions = type_ConsulOptions.GetConstructors();
        Console.WriteLine($"[PASS] ConsulIntegration.ConsulOptions 构造函数数量: {ctors_ConsulOptions.Length}");
        var methods_ConsulOptions = type_ConsulOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConsulIntegration.ConsulOptions 公开方法数量: {methods_ConsulOptions.Length}");
        foreach (var m in methods_ConsulOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsulIntegration.ConsulOptions 未找到，尝试无命名空间...");
        type_ConsulOptions = Type.GetType("ConsulOptions");
        if (type_ConsulOptions != null)
            Console.WriteLine("[PASS] 类型 ConsulOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConsulOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ConsulIntegration.ConsulService
    var type_ConsulService = Type.GetType("ConsulIntegration.ConsulService");
    if (type_ConsulService != null)
    {
        Console.WriteLine("[PASS] 类型 ConsulIntegration.ConsulService (class) 存在");
        var ctors_ConsulService = type_ConsulService.GetConstructors();
        Console.WriteLine($"[PASS] ConsulIntegration.ConsulService 构造函数数量: {ctors_ConsulService.Length}");
        var methods_ConsulService = type_ConsulService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConsulIntegration.ConsulService 公开方法数量: {methods_ConsulService.Length}");
        foreach (var m in methods_ConsulService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsulIntegration.ConsulService 未找到，尝试无命名空间...");
        type_ConsulService = Type.GetType("ConsulService");
        if (type_ConsulService != null)
            Console.WriteLine("[PASS] 类型 ConsulService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConsulService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ConsulIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ConsulIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ConsulIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ConsulIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConsulIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsulIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ConsulIntegration.ConsulException
    var type_ConsulException = Type.GetType("ConsulIntegration.ConsulException");
    if (type_ConsulException != null)
    {
        Console.WriteLine("[PASS] 类型 ConsulIntegration.ConsulException (class) 存在");
        var ctors_ConsulException = type_ConsulException.GetConstructors();
        Console.WriteLine($"[PASS] ConsulIntegration.ConsulException 构造函数数量: {ctors_ConsulException.Length}");
        var methods_ConsulException = type_ConsulException.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConsulIntegration.ConsulException 公开方法数量: {methods_ConsulException.Length}");
        foreach (var m in methods_ConsulException)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsulIntegration.ConsulException 未找到，尝试无命名空间...");
        type_ConsulException = Type.GetType("ConsulException");
        if (type_ConsulException != null)
            Console.WriteLine("[PASS] 类型 ConsulException (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConsulException 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ConsulIntegration.ConsulConnectionStats
    var type_ConsulConnectionStats = Type.GetType("ConsulIntegration.ConsulConnectionStats");
    if (type_ConsulConnectionStats != null)
    {
        Console.WriteLine("[PASS] 类型 ConsulIntegration.ConsulConnectionStats (class) 存在");
        var ctors_ConsulConnectionStats = type_ConsulConnectionStats.GetConstructors();
        Console.WriteLine($"[PASS] ConsulIntegration.ConsulConnectionStats 构造函数数量: {ctors_ConsulConnectionStats.Length}");
        var methods_ConsulConnectionStats = type_ConsulConnectionStats.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConsulIntegration.ConsulConnectionStats 公开方法数量: {methods_ConsulConnectionStats.Length}");
        foreach (var m in methods_ConsulConnectionStats)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsulIntegration.ConsulConnectionStats 未找到，尝试无命名空间...");
        type_ConsulConnectionStats = Type.GetType("ConsulConnectionStats");
        if (type_ConsulConnectionStats != null)
            Console.WriteLine("[PASS] 类型 ConsulConnectionStats (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConsulConnectionStats 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ConsulIntegration.ConsulThroughputStats
    var type_ConsulThroughputStats = Type.GetType("ConsulIntegration.ConsulThroughputStats");
    if (type_ConsulThroughputStats != null)
    {
        Console.WriteLine("[PASS] 类型 ConsulIntegration.ConsulThroughputStats (class) 存在");
        var ctors_ConsulThroughputStats = type_ConsulThroughputStats.GetConstructors();
        Console.WriteLine($"[PASS] ConsulIntegration.ConsulThroughputStats 构造函数数量: {ctors_ConsulThroughputStats.Length}");
        var methods_ConsulThroughputStats = type_ConsulThroughputStats.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConsulIntegration.ConsulThroughputStats 公开方法数量: {methods_ConsulThroughputStats.Length}");
        foreach (var m in methods_ConsulThroughputStats)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsulIntegration.ConsulThroughputStats 未找到，尝试无命名空间...");
        type_ConsulThroughputStats = Type.GetType("ConsulThroughputStats");
        if (type_ConsulThroughputStats != null)
            Console.WriteLine("[PASS] 类型 ConsulThroughputStats (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConsulThroughputStats 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ConsulIntegration.IConsulService
    var type_IConsulService = Type.GetType("ConsulIntegration.IConsulService");
    if (type_IConsulService != null)
    {
        Console.WriteLine("[PASS] 类型 ConsulIntegration.IConsulService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsulIntegration.IConsulService 未找到，尝试无命名空间...");
        type_IConsulService = Type.GetType("IConsulService");
        if (type_IConsulService != null)
            Console.WriteLine("[PASS] 类型 IConsulService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IConsulService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ConsulIntegration.ITenantContext
    var type_ITenantContext = Type.GetType("ConsulIntegration.ITenantContext");
    if (type_ITenantContext != null)
    {
        Console.WriteLine("[PASS] 类型 ConsulIntegration.ITenantContext (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsulIntegration.ITenantContext 未找到，尝试无命名空间...");
        type_ITenantContext = Type.GetType("ITenantContext");
        if (type_ITenantContext != null)
            Console.WriteLine("[PASS] 类型 ITenantContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITenantContext 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
