#load "netcorekit_integration.cs"

Console.WriteLine("=== netcorekit_integration.cs Test ===");

try
{
    // 验证 class: NetCoreKitExtensions.NetCoreKitOptions
    var type_NetCoreKitOptions = Type.GetType("NetCoreKitExtensions.NetCoreKitOptions");
    if (type_NetCoreKitOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NetCoreKitExtensions.NetCoreKitOptions (class) 存在");
        var ctors_NetCoreKitOptions = type_NetCoreKitOptions.GetConstructors();
        Console.WriteLine($"[PASS] NetCoreKitExtensions.NetCoreKitOptions 构造函数数量: {ctors_NetCoreKitOptions.Length}");
        var methods_NetCoreKitOptions = type_NetCoreKitOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetCoreKitExtensions.NetCoreKitOptions 公开方法数量: {methods_NetCoreKitOptions.Length}");
        foreach (var m in methods_NetCoreKitOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetCoreKitExtensions.NetCoreKitOptions 未找到，尝试无命名空间...");
        type_NetCoreKitOptions = Type.GetType("NetCoreKitOptions");
        if (type_NetCoreKitOptions != null)
            Console.WriteLine("[PASS] 类型 NetCoreKitOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetCoreKitOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetCoreKitExtensions.NetCoreKitService
    var type_NetCoreKitService = Type.GetType("NetCoreKitExtensions.NetCoreKitService");
    if (type_NetCoreKitService != null)
    {
        Console.WriteLine("[PASS] 类型 NetCoreKitExtensions.NetCoreKitService (class) 存在");
        var ctors_NetCoreKitService = type_NetCoreKitService.GetConstructors();
        Console.WriteLine($"[PASS] NetCoreKitExtensions.NetCoreKitService 构造函数数量: {ctors_NetCoreKitService.Length}");
        var methods_NetCoreKitService = type_NetCoreKitService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetCoreKitExtensions.NetCoreKitService 公开方法数量: {methods_NetCoreKitService.Length}");
        foreach (var m in methods_NetCoreKitService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetCoreKitExtensions.NetCoreKitService 未找到，尝试无命名空间...");
        type_NetCoreKitService = Type.GetType("NetCoreKitService");
        if (type_NetCoreKitService != null)
            Console.WriteLine("[PASS] 类型 NetCoreKitService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetCoreKitService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetCoreKitExtensions.NetCoreKitServiceCollectionExtensions
    var type_NetCoreKitServiceCollectionExtensions = Type.GetType("NetCoreKitExtensions.NetCoreKitServiceCollectionExtensions");
    if (type_NetCoreKitServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 NetCoreKitExtensions.NetCoreKitServiceCollectionExtensions (class) 存在");
        var ctors_NetCoreKitServiceCollectionExtensions = type_NetCoreKitServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] NetCoreKitExtensions.NetCoreKitServiceCollectionExtensions 构造函数数量: {ctors_NetCoreKitServiceCollectionExtensions.Length}");
        var methods_NetCoreKitServiceCollectionExtensions = type_NetCoreKitServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetCoreKitExtensions.NetCoreKitServiceCollectionExtensions 公开方法数量: {methods_NetCoreKitServiceCollectionExtensions.Length}");
        foreach (var m in methods_NetCoreKitServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetCoreKitExtensions.NetCoreKitServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_NetCoreKitServiceCollectionExtensions = Type.GetType("NetCoreKitServiceCollectionExtensions");
        if (type_NetCoreKitServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 NetCoreKitServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetCoreKitServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetCoreKitExtensions.ExampleUsage
    var type_ExampleUsage = Type.GetType("NetCoreKitExtensions.ExampleUsage");
    if (type_ExampleUsage != null)
    {
        Console.WriteLine("[PASS] 类型 NetCoreKitExtensions.ExampleUsage (class) 存在");
        var ctors_ExampleUsage = type_ExampleUsage.GetConstructors();
        Console.WriteLine($"[PASS] NetCoreKitExtensions.ExampleUsage 构造函数数量: {ctors_ExampleUsage.Length}");
        var methods_ExampleUsage = type_ExampleUsage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetCoreKitExtensions.ExampleUsage 公开方法数量: {methods_ExampleUsage.Length}");
        foreach (var m in methods_ExampleUsage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetCoreKitExtensions.ExampleUsage 未找到，尝试无命名空间...");
        type_ExampleUsage = Type.GetType("ExampleUsage");
        if (type_ExampleUsage != null)
            Console.WriteLine("[PASS] 类型 ExampleUsage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExampleUsage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: NetCoreKitExtensions.INetCoreKitService
    var type_INetCoreKitService = Type.GetType("NetCoreKitExtensions.INetCoreKitService");
    if (type_INetCoreKitService != null)
    {
        Console.WriteLine("[PASS] 类型 NetCoreKitExtensions.INetCoreKitService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetCoreKitExtensions.INetCoreKitService 未找到，尝试无命名空间...");
        type_INetCoreKitService = Type.GetType("INetCoreKitService");
        if (type_INetCoreKitService != null)
            Console.WriteLine("[PASS] 类型 INetCoreKitService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 INetCoreKitService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
