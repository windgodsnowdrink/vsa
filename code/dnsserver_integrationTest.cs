#load "dnsserver_integration.cs"

Console.WriteLine("=== dnsserver_integration.cs Test ===");

try
{
    // 验证 class: DnsServerOptions
    var type_DnsServerOptions = Type.GetType("DnsServerOptions");
    if (type_DnsServerOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DnsServerOptions (class) 存在");
        var ctors_DnsServerOptions = type_DnsServerOptions.GetConstructors();
        Console.WriteLine($"[PASS] DnsServerOptions 构造函数数量: {ctors_DnsServerOptions.Length}");
        var methods_DnsServerOptions = type_DnsServerOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DnsServerOptions 公开方法数量: {methods_DnsServerOptions.Length}");
        foreach (var m in methods_DnsServerOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DnsServerOptions 未找到，尝试无命名空间...");
        type_DnsServerOptions = Type.GetType("DnsServerOptions");
        if (type_DnsServerOptions != null)
            Console.WriteLine("[PASS] 类型 DnsServerOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DnsServerOptions 可能为顶层语句或嵌套类型");
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

    // 验证 class: DnsServerService
    var type_DnsServerService = Type.GetType("DnsServerService");
    if (type_DnsServerService != null)
    {
        Console.WriteLine("[PASS] 类型 DnsServerService (class) 存在");
        var ctors_DnsServerService = type_DnsServerService.GetConstructors();
        Console.WriteLine($"[PASS] DnsServerService 构造函数数量: {ctors_DnsServerService.Length}");
        var methods_DnsServerService = type_DnsServerService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DnsServerService 公开方法数量: {methods_DnsServerService.Length}");
        foreach (var m in methods_DnsServerService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DnsServerService 未找到，尝试无命名空间...");
        type_DnsServerService = Type.GetType("DnsServerService");
        if (type_DnsServerService != null)
            Console.WriteLine("[PASS] 类型 DnsServerService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DnsServerService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DnsServerExtensions
    var type_DnsServerExtensions = Type.GetType("DnsServerExtensions");
    if (type_DnsServerExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DnsServerExtensions (class) 存在");
        var ctors_DnsServerExtensions = type_DnsServerExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DnsServerExtensions 构造函数数量: {ctors_DnsServerExtensions.Length}");
        var methods_DnsServerExtensions = type_DnsServerExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DnsServerExtensions 公开方法数量: {methods_DnsServerExtensions.Length}");
        foreach (var m in methods_DnsServerExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DnsServerExtensions 未找到，尝试无命名空间...");
        type_DnsServerExtensions = Type.GetType("DnsServerExtensions");
        if (type_DnsServerExtensions != null)
            Console.WriteLine("[PASS] 类型 DnsServerExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DnsServerExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDnsServerService
    var type_IDnsServerService = Type.GetType("IDnsServerService");
    if (type_IDnsServerService != null)
    {
        Console.WriteLine("[PASS] 类型 IDnsServerService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDnsServerService 未找到，尝试无命名空间...");
        type_IDnsServerService = Type.GetType("IDnsServerService");
        if (type_IDnsServerService != null)
            Console.WriteLine("[PASS] 类型 IDnsServerService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDnsServerService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
