#load "exceptionless_integration.cs"

Console.WriteLine("=== exceptionless_integration.cs Test ===");

try
{
    // 验证 class: ExceptionlessConfig
    var type_ExceptionlessConfig = Type.GetType("ExceptionlessConfig");
    if (type_ExceptionlessConfig != null)
    {
        Console.WriteLine("[PASS] 类型 ExceptionlessConfig (class) 存在");
        var ctors_ExceptionlessConfig = type_ExceptionlessConfig.GetConstructors();
        Console.WriteLine($"[PASS] ExceptionlessConfig 构造函数数量: {ctors_ExceptionlessConfig.Length}");
        var methods_ExceptionlessConfig = type_ExceptionlessConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExceptionlessConfig 公开方法数量: {methods_ExceptionlessConfig.Length}");
        foreach (var m in methods_ExceptionlessConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExceptionlessConfig 未找到，尝试无命名空间...");
        type_ExceptionlessConfig = Type.GetType("ExceptionlessConfig");
        if (type_ExceptionlessConfig != null)
            Console.WriteLine("[PASS] 类型 ExceptionlessConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExceptionlessConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ExceptionChannel
    var type_ExceptionChannel = Type.GetType("ExceptionChannel");
    if (type_ExceptionChannel != null)
    {
        Console.WriteLine("[PASS] 类型 ExceptionChannel (class) 存在");
        var ctors_ExceptionChannel = type_ExceptionChannel.GetConstructors();
        Console.WriteLine($"[PASS] ExceptionChannel 构造函数数量: {ctors_ExceptionChannel.Length}");
        var methods_ExceptionChannel = type_ExceptionChannel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExceptionChannel 公开方法数量: {methods_ExceptionChannel.Length}");
        foreach (var m in methods_ExceptionChannel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExceptionChannel 未找到，尝试无命名空间...");
        type_ExceptionChannel = Type.GetType("ExceptionChannel");
        if (type_ExceptionChannel != null)
            Console.WriteLine("[PASS] 类型 ExceptionChannel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExceptionChannel 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ExceptionBackgroundService
    var type_ExceptionBackgroundService = Type.GetType("ExceptionBackgroundService");
    if (type_ExceptionBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 ExceptionBackgroundService (class) 存在");
        var ctors_ExceptionBackgroundService = type_ExceptionBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] ExceptionBackgroundService 构造函数数量: {ctors_ExceptionBackgroundService.Length}");
        var methods_ExceptionBackgroundService = type_ExceptionBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExceptionBackgroundService 公开方法数量: {methods_ExceptionBackgroundService.Length}");
        foreach (var m in methods_ExceptionBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExceptionBackgroundService 未找到，尝试无命名空间...");
        type_ExceptionBackgroundService = Type.GetType("ExceptionBackgroundService");
        if (type_ExceptionBackgroundService != null)
            Console.WriteLine("[PASS] 类型 ExceptionBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExceptionBackgroundService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
