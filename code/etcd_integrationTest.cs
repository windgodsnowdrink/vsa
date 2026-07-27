#load "etcd_integration.cs"

Console.WriteLine("=== etcd_integration.cs Test ===");

try
{
    // 验证 class: EtcdIntegration.EtcdOptions
    var type_EtcdOptions = Type.GetType("EtcdIntegration.EtcdOptions");
    if (type_EtcdOptions != null)
    {
        Console.WriteLine("[PASS] 类型 EtcdIntegration.EtcdOptions (class) 存在");
        var ctors_EtcdOptions = type_EtcdOptions.GetConstructors();
        Console.WriteLine($"[PASS] EtcdIntegration.EtcdOptions 构造函数数量: {ctors_EtcdOptions.Length}");
        var methods_EtcdOptions = type_EtcdOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EtcdIntegration.EtcdOptions 公开方法数量: {methods_EtcdOptions.Length}");
        foreach (var m in methods_EtcdOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EtcdIntegration.EtcdOptions 未找到，尝试无命名空间...");
        type_EtcdOptions = Type.GetType("EtcdOptions");
        if (type_EtcdOptions != null)
            Console.WriteLine("[PASS] 类型 EtcdOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EtcdOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EtcdIntegration.EtcdService
    var type_EtcdService = Type.GetType("EtcdIntegration.EtcdService");
    if (type_EtcdService != null)
    {
        Console.WriteLine("[PASS] 类型 EtcdIntegration.EtcdService (class) 存在");
        var ctors_EtcdService = type_EtcdService.GetConstructors();
        Console.WriteLine($"[PASS] EtcdIntegration.EtcdService 构造函数数量: {ctors_EtcdService.Length}");
        var methods_EtcdService = type_EtcdService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EtcdIntegration.EtcdService 公开方法数量: {methods_EtcdService.Length}");
        foreach (var m in methods_EtcdService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EtcdIntegration.EtcdService 未找到，尝试无命名空间...");
        type_EtcdService = Type.GetType("EtcdService");
        if (type_EtcdService != null)
            Console.WriteLine("[PASS] 类型 EtcdService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EtcdService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EtcdIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("EtcdIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 EtcdIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] EtcdIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EtcdIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EtcdIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: EtcdIntegration.IEtcdService
    var type_IEtcdService = Type.GetType("EtcdIntegration.IEtcdService");
    if (type_IEtcdService != null)
    {
        Console.WriteLine("[PASS] 类型 EtcdIntegration.IEtcdService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EtcdIntegration.IEtcdService 未找到，尝试无命名空间...");
        type_IEtcdService = Type.GetType("IEtcdService");
        if (type_IEtcdService != null)
            Console.WriteLine("[PASS] 类型 IEtcdService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IEtcdService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
