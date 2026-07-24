#load "ipaddressrange_integration.cs"

Console.WriteLine("=== ipaddressrange_integration.cs Test ===");

try
{
    // 验证 class: IPAddressRangeIntegration.IpRangeOptions
    var type_IpRangeOptions = Type.GetType("IPAddressRangeIntegration.IpRangeOptions");
    if (type_IpRangeOptions != null)
    {
        Console.WriteLine("[PASS] 类型 IPAddressRangeIntegration.IpRangeOptions (class) 存在");
        var ctors_IpRangeOptions = type_IpRangeOptions.GetConstructors();
        Console.WriteLine($"[PASS] IPAddressRangeIntegration.IpRangeOptions 构造函数数量: {ctors_IpRangeOptions.Length}");
        var methods_IpRangeOptions = type_IpRangeOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IPAddressRangeIntegration.IpRangeOptions 公开方法数量: {methods_IpRangeOptions.Length}");
        foreach (var m in methods_IpRangeOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPAddressRangeIntegration.IpRangeOptions 未找到，尝试无命名空间...");
        type_IpRangeOptions = Type.GetType("IpRangeOptions");
        if (type_IpRangeOptions != null)
            Console.WriteLine("[PASS] 类型 IpRangeOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IpRangeOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IPAddressRangeIntegration.IPRangeInfo
    var type_IPRangeInfo = Type.GetType("IPAddressRangeIntegration.IPRangeInfo");
    if (type_IPRangeInfo != null)
    {
        Console.WriteLine("[PASS] 类型 IPAddressRangeIntegration.IPRangeInfo (class) 存在");
        var ctors_IPRangeInfo = type_IPRangeInfo.GetConstructors();
        Console.WriteLine($"[PASS] IPAddressRangeIntegration.IPRangeInfo 构造函数数量: {ctors_IPRangeInfo.Length}");
        var methods_IPRangeInfo = type_IPRangeInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IPAddressRangeIntegration.IPRangeInfo 公开方法数量: {methods_IPRangeInfo.Length}");
        foreach (var m in methods_IPRangeInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPAddressRangeIntegration.IPRangeInfo 未找到，尝试无命名空间...");
        type_IPRangeInfo = Type.GetType("IPRangeInfo");
        if (type_IPRangeInfo != null)
            Console.WriteLine("[PASS] 类型 IPRangeInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPRangeInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IPAddressRangeIntegration.IpRangeService
    var type_IpRangeService = Type.GetType("IPAddressRangeIntegration.IpRangeService");
    if (type_IpRangeService != null)
    {
        Console.WriteLine("[PASS] 类型 IPAddressRangeIntegration.IpRangeService (class) 存在");
        var ctors_IpRangeService = type_IpRangeService.GetConstructors();
        Console.WriteLine($"[PASS] IPAddressRangeIntegration.IpRangeService 构造函数数量: {ctors_IpRangeService.Length}");
        var methods_IpRangeService = type_IpRangeService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IPAddressRangeIntegration.IpRangeService 公开方法数量: {methods_IpRangeService.Length}");
        foreach (var m in methods_IpRangeService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPAddressRangeIntegration.IpRangeService 未找到，尝试无命名空间...");
        type_IpRangeService = Type.GetType("IpRangeService");
        if (type_IpRangeService != null)
            Console.WriteLine("[PASS] 类型 IpRangeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IpRangeService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IPAddressRangeIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("IPAddressRangeIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 IPAddressRangeIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] IPAddressRangeIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IPAddressRangeIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPAddressRangeIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IPAddressRangeIntegration.IPAddressRangePooledObjectPolicy
    var type_IPAddressRangePooledObjectPolicy = Type.GetType("IPAddressRangeIntegration.IPAddressRangePooledObjectPolicy");
    if (type_IPAddressRangePooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 IPAddressRangeIntegration.IPAddressRangePooledObjectPolicy (class) 存在");
        var ctors_IPAddressRangePooledObjectPolicy = type_IPAddressRangePooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] IPAddressRangeIntegration.IPAddressRangePooledObjectPolicy 构造函数数量: {ctors_IPAddressRangePooledObjectPolicy.Length}");
        var methods_IPAddressRangePooledObjectPolicy = type_IPAddressRangePooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IPAddressRangeIntegration.IPAddressRangePooledObjectPolicy 公开方法数量: {methods_IPAddressRangePooledObjectPolicy.Length}");
        foreach (var m in methods_IPAddressRangePooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPAddressRangeIntegration.IPAddressRangePooledObjectPolicy 未找到，尝试无命名空间...");
        type_IPAddressRangePooledObjectPolicy = Type.GetType("IPAddressRangePooledObjectPolicy");
        if (type_IPAddressRangePooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 IPAddressRangePooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPAddressRangePooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPAddressRangeIntegration.IIpRangeService
    var type_IIpRangeService = Type.GetType("IPAddressRangeIntegration.IIpRangeService");
    if (type_IIpRangeService != null)
    {
        Console.WriteLine("[PASS] 类型 IPAddressRangeIntegration.IIpRangeService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPAddressRangeIntegration.IIpRangeService 未找到，尝试无命名空间...");
        type_IIpRangeService = Type.GetType("IIpRangeService");
        if (type_IIpRangeService != null)
            Console.WriteLine("[PASS] 类型 IIpRangeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IIpRangeService 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: IPAddressRangeIntegration.IpRangeCacheType
    var type_IpRangeCacheType = Type.GetType("IPAddressRangeIntegration.IpRangeCacheType");
    if (type_IpRangeCacheType != null)
    {
        Console.WriteLine("[PASS] 类型 IPAddressRangeIntegration.IpRangeCacheType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPAddressRangeIntegration.IpRangeCacheType 未找到，尝试无命名空间...");
        type_IpRangeCacheType = Type.GetType("IpRangeCacheType");
        if (type_IpRangeCacheType != null)
            Console.WriteLine("[PASS] 类型 IpRangeCacheType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IpRangeCacheType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
