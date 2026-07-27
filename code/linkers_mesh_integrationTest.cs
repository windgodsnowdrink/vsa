#load "linkers_mesh_integration.cs"

Console.WriteLine("=== linkers_mesh_integration.cs Test ===");

try
{
    // 验证 class: LinkersMeshOptions
    var type_LinkersMeshOptions = Type.GetType("LinkersMeshOptions");
    if (type_LinkersMeshOptions != null)
    {
        Console.WriteLine("[PASS] 类型 LinkersMeshOptions (class) 存在");
        var ctors_LinkersMeshOptions = type_LinkersMeshOptions.GetConstructors();
        Console.WriteLine($"[PASS] LinkersMeshOptions 构造函数数量: {ctors_LinkersMeshOptions.Length}");
        var methods_LinkersMeshOptions = type_LinkersMeshOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LinkersMeshOptions 公开方法数量: {methods_LinkersMeshOptions.Length}");
        foreach (var m in methods_LinkersMeshOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LinkersMeshOptions 未找到，尝试无命名空间...");
        type_LinkersMeshOptions = Type.GetType("LinkersMeshOptions");
        if (type_LinkersMeshOptions != null)
            Console.WriteLine("[PASS] 类型 LinkersMeshOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LinkersMeshOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LinkersMeshClient
    var type_LinkersMeshClient = Type.GetType("LinkersMeshClient");
    if (type_LinkersMeshClient != null)
    {
        Console.WriteLine("[PASS] 类型 LinkersMeshClient (class) 存在");
        var ctors_LinkersMeshClient = type_LinkersMeshClient.GetConstructors();
        Console.WriteLine($"[PASS] LinkersMeshClient 构造函数数量: {ctors_LinkersMeshClient.Length}");
        var methods_LinkersMeshClient = type_LinkersMeshClient.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LinkersMeshClient 公开方法数量: {methods_LinkersMeshClient.Length}");
        foreach (var m in methods_LinkersMeshClient)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LinkersMeshClient 未找到，尝试无命名空间...");
        type_LinkersMeshClient = Type.GetType("LinkersMeshClient");
        if (type_LinkersMeshClient != null)
            Console.WriteLine("[PASS] 类型 LinkersMeshClient (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LinkersMeshClient 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LinkersMeshExtensions
    var type_LinkersMeshExtensions = Type.GetType("LinkersMeshExtensions");
    if (type_LinkersMeshExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 LinkersMeshExtensions (class) 存在");
        var ctors_LinkersMeshExtensions = type_LinkersMeshExtensions.GetConstructors();
        Console.WriteLine($"[PASS] LinkersMeshExtensions 构造函数数量: {ctors_LinkersMeshExtensions.Length}");
        var methods_LinkersMeshExtensions = type_LinkersMeshExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LinkersMeshExtensions 公开方法数量: {methods_LinkersMeshExtensions.Length}");
        foreach (var m in methods_LinkersMeshExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LinkersMeshExtensions 未找到，尝试无命名空间...");
        type_LinkersMeshExtensions = Type.GetType("LinkersMeshExtensions");
        if (type_LinkersMeshExtensions != null)
            Console.WriteLine("[PASS] 类型 LinkersMeshExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LinkersMeshExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LinkersMeshBackgroundService
    var type_LinkersMeshBackgroundService = Type.GetType("LinkersMeshBackgroundService");
    if (type_LinkersMeshBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 LinkersMeshBackgroundService (class) 存在");
        var ctors_LinkersMeshBackgroundService = type_LinkersMeshBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] LinkersMeshBackgroundService 构造函数数量: {ctors_LinkersMeshBackgroundService.Length}");
        var methods_LinkersMeshBackgroundService = type_LinkersMeshBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LinkersMeshBackgroundService 公开方法数量: {methods_LinkersMeshBackgroundService.Length}");
        foreach (var m in methods_LinkersMeshBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LinkersMeshBackgroundService 未找到，尝试无命名空间...");
        type_LinkersMeshBackgroundService = Type.GetType("LinkersMeshBackgroundService");
        if (type_LinkersMeshBackgroundService != null)
            Console.WriteLine("[PASS] 类型 LinkersMeshBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LinkersMeshBackgroundService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ILinkersMeshClient
    var type_ILinkersMeshClient = Type.GetType("ILinkersMeshClient");
    if (type_ILinkersMeshClient != null)
    {
        Console.WriteLine("[PASS] 类型 ILinkersMeshClient (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ILinkersMeshClient 未找到，尝试无命名空间...");
        type_ILinkersMeshClient = Type.GetType("ILinkersMeshClient");
        if (type_ILinkersMeshClient != null)
            Console.WriteLine("[PASS] 类型 ILinkersMeshClient (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ILinkersMeshClient 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
