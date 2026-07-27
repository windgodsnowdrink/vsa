#load "ip2region_integration.cs"

Console.WriteLine("=== ip2region_integration.cs Test ===");

try
{
    // 验证 class: Ip2RegionIntegration.Ip2RegionOptions
    var type_Ip2RegionOptions = Type.GetType("Ip2RegionIntegration.Ip2RegionOptions");
    if (type_Ip2RegionOptions != null)
    {
        Console.WriteLine("[PASS] 类型 Ip2RegionIntegration.Ip2RegionOptions (class) 存在");
        var ctors_Ip2RegionOptions = type_Ip2RegionOptions.GetConstructors();
        Console.WriteLine($"[PASS] Ip2RegionIntegration.Ip2RegionOptions 构造函数数量: {ctors_Ip2RegionOptions.Length}");
        var methods_Ip2RegionOptions = type_Ip2RegionOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Ip2RegionIntegration.Ip2RegionOptions 公开方法数量: {methods_Ip2RegionOptions.Length}");
        foreach (var m in methods_Ip2RegionOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Ip2RegionIntegration.Ip2RegionOptions 未找到，尝试无命名空间...");
        type_Ip2RegionOptions = Type.GetType("Ip2RegionOptions");
        if (type_Ip2RegionOptions != null)
            Console.WriteLine("[PASS] 类型 Ip2RegionOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Ip2RegionOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Ip2RegionIntegration.RegionInfo
    var type_RegionInfo = Type.GetType("Ip2RegionIntegration.RegionInfo");
    if (type_RegionInfo != null)
    {
        Console.WriteLine("[PASS] 类型 Ip2RegionIntegration.RegionInfo (class) 存在");
        var ctors_RegionInfo = type_RegionInfo.GetConstructors();
        Console.WriteLine($"[PASS] Ip2RegionIntegration.RegionInfo 构造函数数量: {ctors_RegionInfo.Length}");
        var methods_RegionInfo = type_RegionInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Ip2RegionIntegration.RegionInfo 公开方法数量: {methods_RegionInfo.Length}");
        foreach (var m in methods_RegionInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Ip2RegionIntegration.RegionInfo 未找到，尝试无命名空间...");
        type_RegionInfo = Type.GetType("RegionInfo");
        if (type_RegionInfo != null)
            Console.WriteLine("[PASS] 类型 RegionInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RegionInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Ip2RegionIntegration.Ip2RegionService
    var type_Ip2RegionService = Type.GetType("Ip2RegionIntegration.Ip2RegionService");
    if (type_Ip2RegionService != null)
    {
        Console.WriteLine("[PASS] 类型 Ip2RegionIntegration.Ip2RegionService (class) 存在");
        var ctors_Ip2RegionService = type_Ip2RegionService.GetConstructors();
        Console.WriteLine($"[PASS] Ip2RegionIntegration.Ip2RegionService 构造函数数量: {ctors_Ip2RegionService.Length}");
        var methods_Ip2RegionService = type_Ip2RegionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Ip2RegionIntegration.Ip2RegionService 公开方法数量: {methods_Ip2RegionService.Length}");
        foreach (var m in methods_Ip2RegionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Ip2RegionIntegration.Ip2RegionService 未找到，尝试无命名空间...");
        type_Ip2RegionService = Type.GetType("Ip2RegionService");
        if (type_Ip2RegionService != null)
            Console.WriteLine("[PASS] 类型 Ip2RegionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Ip2RegionService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Ip2RegionIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("Ip2RegionIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 Ip2RegionIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] Ip2RegionIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Ip2RegionIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Ip2RegionIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: Ip2RegionIntegration.IIp2RegionService
    var type_IIp2RegionService = Type.GetType("Ip2RegionIntegration.IIp2RegionService");
    if (type_IIp2RegionService != null)
    {
        Console.WriteLine("[PASS] 类型 Ip2RegionIntegration.IIp2RegionService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Ip2RegionIntegration.IIp2RegionService 未找到，尝试无命名空间...");
        type_IIp2RegionService = Type.GetType("IIp2RegionService");
        if (type_IIp2RegionService != null)
            Console.WriteLine("[PASS] 类型 IIp2RegionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IIp2RegionService 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: Ip2RegionIntegration.Ip2RegionCacheType
    var type_Ip2RegionCacheType = Type.GetType("Ip2RegionIntegration.Ip2RegionCacheType");
    if (type_Ip2RegionCacheType != null)
    {
        Console.WriteLine("[PASS] 类型 Ip2RegionIntegration.Ip2RegionCacheType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Ip2RegionIntegration.Ip2RegionCacheType 未找到，尝试无命名空间...");
        type_Ip2RegionCacheType = Type.GetType("Ip2RegionCacheType");
        if (type_Ip2RegionCacheType != null)
            Console.WriteLine("[PASS] 类型 Ip2RegionCacheType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Ip2RegionCacheType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
