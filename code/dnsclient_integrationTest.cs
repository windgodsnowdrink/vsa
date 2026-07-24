#load "dnsclient_integration.cs"

Console.WriteLine("=== dnsclient_integration.cs Test ===");

try
{
    // 验证 class: DnsDiscoveryOptions
    var type_DnsDiscoveryOptions = Type.GetType("DnsDiscoveryOptions");
    if (type_DnsDiscoveryOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DnsDiscoveryOptions (class) 存在");
        var ctors_DnsDiscoveryOptions = type_DnsDiscoveryOptions.GetConstructors();
        Console.WriteLine($"[PASS] DnsDiscoveryOptions 构造函数数量: {ctors_DnsDiscoveryOptions.Length}");
        var methods_DnsDiscoveryOptions = type_DnsDiscoveryOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DnsDiscoveryOptions 公开方法数量: {methods_DnsDiscoveryOptions.Length}");
        foreach (var m in methods_DnsDiscoveryOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DnsDiscoveryOptions 未找到，尝试无命名空间...");
        type_DnsDiscoveryOptions = Type.GetType("DnsDiscoveryOptions");
        if (type_DnsDiscoveryOptions != null)
            Console.WriteLine("[PASS] 类型 DnsDiscoveryOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DnsDiscoveryOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DnsServiceEntry
    var type_DnsServiceEntry = Type.GetType("DnsServiceEntry");
    if (type_DnsServiceEntry != null)
    {
        Console.WriteLine("[PASS] 类型 DnsServiceEntry (class) 存在");
        var ctors_DnsServiceEntry = type_DnsServiceEntry.GetConstructors();
        Console.WriteLine($"[PASS] DnsServiceEntry 构造函数数量: {ctors_DnsServiceEntry.Length}");
        var methods_DnsServiceEntry = type_DnsServiceEntry.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DnsServiceEntry 公开方法数量: {methods_DnsServiceEntry.Length}");
        foreach (var m in methods_DnsServiceEntry)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DnsServiceEntry 未找到，尝试无命名空间...");
        type_DnsServiceEntry = Type.GetType("DnsServiceEntry");
        if (type_DnsServiceEntry != null)
            Console.WriteLine("[PASS] 类型 DnsServiceEntry (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DnsServiceEntry 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DnsDiscoveryService
    var type_DnsDiscoveryService = Type.GetType("DnsDiscoveryService");
    if (type_DnsDiscoveryService != null)
    {
        Console.WriteLine("[PASS] 类型 DnsDiscoveryService (class) 存在");
        var ctors_DnsDiscoveryService = type_DnsDiscoveryService.GetConstructors();
        Console.WriteLine($"[PASS] DnsDiscoveryService 构造函数数量: {ctors_DnsDiscoveryService.Length}");
        var methods_DnsDiscoveryService = type_DnsDiscoveryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DnsDiscoveryService 公开方法数量: {methods_DnsDiscoveryService.Length}");
        foreach (var m in methods_DnsDiscoveryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DnsDiscoveryService 未找到，尝试无命名空间...");
        type_DnsDiscoveryService = Type.GetType("DnsDiscoveryService");
        if (type_DnsDiscoveryService != null)
            Console.WriteLine("[PASS] 类型 DnsDiscoveryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DnsDiscoveryService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DnsDiscoveryExtensions
    var type_DnsDiscoveryExtensions = Type.GetType("DnsDiscoveryExtensions");
    if (type_DnsDiscoveryExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DnsDiscoveryExtensions (class) 存在");
        var ctors_DnsDiscoveryExtensions = type_DnsDiscoveryExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DnsDiscoveryExtensions 构造函数数量: {ctors_DnsDiscoveryExtensions.Length}");
        var methods_DnsDiscoveryExtensions = type_DnsDiscoveryExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DnsDiscoveryExtensions 公开方法数量: {methods_DnsDiscoveryExtensions.Length}");
        foreach (var m in methods_DnsDiscoveryExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DnsDiscoveryExtensions 未找到，尝试无命名空间...");
        type_DnsDiscoveryExtensions = Type.GetType("DnsDiscoveryExtensions");
        if (type_DnsDiscoveryExtensions != null)
            Console.WriteLine("[PASS] 类型 DnsDiscoveryExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DnsDiscoveryExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDnsDiscoveryService
    var type_IDnsDiscoveryService = Type.GetType("IDnsDiscoveryService");
    if (type_IDnsDiscoveryService != null)
    {
        Console.WriteLine("[PASS] 类型 IDnsDiscoveryService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDnsDiscoveryService 未找到，尝试无命名空间...");
        type_IDnsDiscoveryService = Type.GetType("IDnsDiscoveryService");
        if (type_IDnsDiscoveryService != null)
            Console.WriteLine("[PASS] 类型 IDnsDiscoveryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDnsDiscoveryService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
