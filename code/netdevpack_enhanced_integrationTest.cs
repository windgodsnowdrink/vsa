#load "netdevpack_enhanced_integration.cs"

Console.WriteLine("=== netdevpack_enhanced_integration.cs Test ===");

try
{
    // 验证 class: NetDevPackIntegration.NetDevPackEnhancedOptions
    var type_NetDevPackEnhancedOptions = Type.GetType("NetDevPackIntegration.NetDevPackEnhancedOptions");
    if (type_NetDevPackEnhancedOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NetDevPackIntegration.NetDevPackEnhancedOptions (class) 存在");
        var ctors_NetDevPackEnhancedOptions = type_NetDevPackEnhancedOptions.GetConstructors();
        Console.WriteLine($"[PASS] NetDevPackIntegration.NetDevPackEnhancedOptions 构造函数数量: {ctors_NetDevPackEnhancedOptions.Length}");
        var methods_NetDevPackEnhancedOptions = type_NetDevPackEnhancedOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetDevPackIntegration.NetDevPackEnhancedOptions 公开方法数量: {methods_NetDevPackEnhancedOptions.Length}");
        foreach (var m in methods_NetDevPackEnhancedOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetDevPackIntegration.NetDevPackEnhancedOptions 未找到，尝试无命名空间...");
        type_NetDevPackEnhancedOptions = Type.GetType("NetDevPackEnhancedOptions");
        if (type_NetDevPackEnhancedOptions != null)
            Console.WriteLine("[PASS] 类型 NetDevPackEnhancedOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetDevPackEnhancedOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetDevPackIntegration.NetDevPackEnhancedService
    var type_NetDevPackEnhancedService = Type.GetType("NetDevPackIntegration.NetDevPackEnhancedService");
    if (type_NetDevPackEnhancedService != null)
    {
        Console.WriteLine("[PASS] 类型 NetDevPackIntegration.NetDevPackEnhancedService (class) 存在");
        var ctors_NetDevPackEnhancedService = type_NetDevPackEnhancedService.GetConstructors();
        Console.WriteLine($"[PASS] NetDevPackIntegration.NetDevPackEnhancedService 构造函数数量: {ctors_NetDevPackEnhancedService.Length}");
        var methods_NetDevPackEnhancedService = type_NetDevPackEnhancedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetDevPackIntegration.NetDevPackEnhancedService 公开方法数量: {methods_NetDevPackEnhancedService.Length}");
        foreach (var m in methods_NetDevPackEnhancedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetDevPackIntegration.NetDevPackEnhancedService 未找到，尝试无命名空间...");
        type_NetDevPackEnhancedService = Type.GetType("NetDevPackEnhancedService");
        if (type_NetDevPackEnhancedService != null)
            Console.WriteLine("[PASS] 类型 NetDevPackEnhancedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetDevPackEnhancedService 可能为顶层语句或嵌套类型");
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

    // 验证 class: NetDevPackIntegration.ExampleEnhancedUsage
    var type_ExampleEnhancedUsage = Type.GetType("NetDevPackIntegration.ExampleEnhancedUsage");
    if (type_ExampleEnhancedUsage != null)
    {
        Console.WriteLine("[PASS] 类型 NetDevPackIntegration.ExampleEnhancedUsage (class) 存在");
        var ctors_ExampleEnhancedUsage = type_ExampleEnhancedUsage.GetConstructors();
        Console.WriteLine($"[PASS] NetDevPackIntegration.ExampleEnhancedUsage 构造函数数量: {ctors_ExampleEnhancedUsage.Length}");
        var methods_ExampleEnhancedUsage = type_ExampleEnhancedUsage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetDevPackIntegration.ExampleEnhancedUsage 公开方法数量: {methods_ExampleEnhancedUsage.Length}");
        foreach (var m in methods_ExampleEnhancedUsage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetDevPackIntegration.ExampleEnhancedUsage 未找到，尝试无命名空间...");
        type_ExampleEnhancedUsage = Type.GetType("ExampleEnhancedUsage");
        if (type_ExampleEnhancedUsage != null)
            Console.WriteLine("[PASS] 类型 ExampleEnhancedUsage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExampleEnhancedUsage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: NetDevPackIntegration.INetDevPackEnhancedService
    var type_INetDevPackEnhancedService = Type.GetType("NetDevPackIntegration.INetDevPackEnhancedService");
    if (type_INetDevPackEnhancedService != null)
    {
        Console.WriteLine("[PASS] 类型 NetDevPackIntegration.INetDevPackEnhancedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetDevPackIntegration.INetDevPackEnhancedService 未找到，尝试无命名空间...");
        type_INetDevPackEnhancedService = Type.GetType("INetDevPackEnhancedService");
        if (type_INetDevPackEnhancedService != null)
            Console.WriteLine("[PASS] 类型 INetDevPackEnhancedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 INetDevPackEnhancedService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
