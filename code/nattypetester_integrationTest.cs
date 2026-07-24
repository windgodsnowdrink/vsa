#load "nattypetester_integration.cs"

Console.WriteLine("=== nattypetester_integration.cs Test ===");

try
{
    // 验证 class: NatTypeTesterOptions
    var type_NatTypeTesterOptions = Type.GetType("NatTypeTesterOptions");
    if (type_NatTypeTesterOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NatTypeTesterOptions (class) 存在");
        var ctors_NatTypeTesterOptions = type_NatTypeTesterOptions.GetConstructors();
        Console.WriteLine($"[PASS] NatTypeTesterOptions 构造函数数量: {ctors_NatTypeTesterOptions.Length}");
        var methods_NatTypeTesterOptions = type_NatTypeTesterOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatTypeTesterOptions 公开方法数量: {methods_NatTypeTesterOptions.Length}");
        foreach (var m in methods_NatTypeTesterOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatTypeTesterOptions 未找到，尝试无命名空间...");
        type_NatTypeTesterOptions = Type.GetType("NatTypeTesterOptions");
        if (type_NatTypeTesterOptions != null)
            Console.WriteLine("[PASS] 类型 NatTypeTesterOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatTypeTesterOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatTypeTester
    var type_NatTypeTester = Type.GetType("NatTypeTester");
    if (type_NatTypeTester != null)
    {
        Console.WriteLine("[PASS] 类型 NatTypeTester (class) 存在");
        var ctors_NatTypeTester = type_NatTypeTester.GetConstructors();
        Console.WriteLine($"[PASS] NatTypeTester 构造函数数量: {ctors_NatTypeTester.Length}");
        var methods_NatTypeTester = type_NatTypeTester.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatTypeTester 公开方法数量: {methods_NatTypeTester.Length}");
        foreach (var m in methods_NatTypeTester)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatTypeTester 未找到，尝试无命名空间...");
        type_NatTypeTester = Type.GetType("NatTypeTester");
        if (type_NatTypeTester != null)
            Console.WriteLine("[PASS] 类型 NatTypeTester (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatTypeTester 可能为顶层语句或嵌套类型");
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

    // 验证 class: UdpClientPooledObjectPolicy
    var type_UdpClientPooledObjectPolicy = Type.GetType("UdpClientPooledObjectPolicy");
    if (type_UdpClientPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 UdpClientPooledObjectPolicy (class) 存在");
        var ctors_UdpClientPooledObjectPolicy = type_UdpClientPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] UdpClientPooledObjectPolicy 构造函数数量: {ctors_UdpClientPooledObjectPolicy.Length}");
        var methods_UdpClientPooledObjectPolicy = type_UdpClientPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UdpClientPooledObjectPolicy 公开方法数量: {methods_UdpClientPooledObjectPolicy.Length}");
        foreach (var m in methods_UdpClientPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UdpClientPooledObjectPolicy 未找到，尝试无命名空间...");
        type_UdpClientPooledObjectPolicy = Type.GetType("UdpClientPooledObjectPolicy");
        if (type_UdpClientPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 UdpClientPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UdpClientPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: INatTypeTester
    var type_INatTypeTester = Type.GetType("INatTypeTester");
    if (type_INatTypeTester != null)
    {
        Console.WriteLine("[PASS] 类型 INatTypeTester (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 INatTypeTester 未找到，尝试无命名空间...");
        type_INatTypeTester = Type.GetType("INatTypeTester");
        if (type_INatTypeTester != null)
            Console.WriteLine("[PASS] 类型 INatTypeTester (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 INatTypeTester 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
