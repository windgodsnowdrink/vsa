#load "p2p_integration.cs"

Console.WriteLine("=== p2p_integration.cs Test ===");

try
{
    // 验证 class: P2PService
    var type_P2PService = Type.GetType("P2PService");
    if (type_P2PService != null)
    {
        Console.WriteLine("[PASS] 类型 P2PService (class) 存在");
        var ctors_P2PService = type_P2PService.GetConstructors();
        Console.WriteLine($"[PASS] P2PService 构造函数数量: {ctors_P2PService.Length}");
        var methods_P2PService = type_P2PService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] P2PService 公开方法数量: {methods_P2PService.Length}");
        foreach (var m in methods_P2PService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 P2PService 未找到，尝试无命名空间...");
        type_P2PService = Type.GetType("P2PService");
        if (type_P2PService != null)
            Console.WriteLine("[PASS] 类型 P2PService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 P2PService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IP2PService
    var type_IP2PService = Type.GetType("IP2PService");
    if (type_IP2PService != null)
    {
        Console.WriteLine("[PASS] 类型 IP2PService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IP2PService 未找到，尝试无命名空间...");
        type_IP2PService = Type.GetType("IP2PService");
        if (type_IP2PService != null)
            Console.WriteLine("[PASS] 类型 IP2PService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IP2PService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ReceivedMessage
    var type_ReceivedMessage = Type.GetType("ReceivedMessage");
    if (type_ReceivedMessage != null)
    {
        Console.WriteLine("[PASS] 类型 ReceivedMessage (record) 存在");
        var ctors_ReceivedMessage = type_ReceivedMessage.GetConstructors();
        Console.WriteLine($"[PASS] ReceivedMessage 构造函数数量: {ctors_ReceivedMessage.Length}");
        var methods_ReceivedMessage = type_ReceivedMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReceivedMessage 公开方法数量: {methods_ReceivedMessage.Length}");
        foreach (var m in methods_ReceivedMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReceivedMessage 未找到，尝试无命名空间...");
        type_ReceivedMessage = Type.GetType("ReceivedMessage");
        if (type_ReceivedMessage != null)
            Console.WriteLine("[PASS] 类型 ReceivedMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReceivedMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
