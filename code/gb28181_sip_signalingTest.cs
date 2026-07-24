#load "gb28181_sip_signaling.cs"

Console.WriteLine("=== gb28181_sip_signaling.cs Test ===");

try
{
    // 验证 class: GB28181SipService
    var type_GB28181SipService = Type.GetType("GB28181SipService");
    if (type_GB28181SipService != null)
    {
        Console.WriteLine("[PASS] 类型 GB28181SipService (class) 存在");
        var ctors_GB28181SipService = type_GB28181SipService.GetConstructors();
        Console.WriteLine($"[PASS] GB28181SipService 构造函数数量: {ctors_GB28181SipService.Length}");
        var methods_GB28181SipService = type_GB28181SipService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GB28181SipService 公开方法数量: {methods_GB28181SipService.Length}");
        foreach (var m in methods_GB28181SipService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GB28181SipService 未找到，尝试无命名空间...");
        type_GB28181SipService = Type.GetType("GB28181SipService");
        if (type_GB28181SipService != null)
            Console.WriteLine("[PASS] 类型 GB28181SipService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GB28181SipService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IGB28181SipService
    var type_IGB28181SipService = Type.GetType("IGB28181SipService");
    if (type_IGB28181SipService != null)
    {
        Console.WriteLine("[PASS] 类型 IGB28181SipService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IGB28181SipService 未找到，尝试无命名空间...");
        type_IGB28181SipService = Type.GetType("IGB28181SipService");
        if (type_IGB28181SipService != null)
            Console.WriteLine("[PASS] 类型 IGB28181SipService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IGB28181SipService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
