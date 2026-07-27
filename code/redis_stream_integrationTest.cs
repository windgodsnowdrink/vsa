#load "redis_stream_integration.cs"

Console.WriteLine("=== redis_stream_integration.cs Test ===");

try
{
    // 验证 class: RedisStreamIntegration.RedisStreamOptions
    var type_RedisStreamOptions = Type.GetType("RedisStreamIntegration.RedisStreamOptions");
    if (type_RedisStreamOptions != null)
    {
        Console.WriteLine("[PASS] 类型 RedisStreamIntegration.RedisStreamOptions (class) 存在");
        var ctors_RedisStreamOptions = type_RedisStreamOptions.GetConstructors();
        Console.WriteLine($"[PASS] RedisStreamIntegration.RedisStreamOptions 构造函数数量: {ctors_RedisStreamOptions.Length}");
        var methods_RedisStreamOptions = type_RedisStreamOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisStreamIntegration.RedisStreamOptions 公开方法数量: {methods_RedisStreamOptions.Length}");
        foreach (var m in methods_RedisStreamOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisStreamIntegration.RedisStreamOptions 未找到，尝试无命名空间...");
        type_RedisStreamOptions = Type.GetType("RedisStreamOptions");
        if (type_RedisStreamOptions != null)
            Console.WriteLine("[PASS] 类型 RedisStreamOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisStreamOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisStreamIntegration.RedisStreamService
    var type_RedisStreamService = Type.GetType("RedisStreamIntegration.RedisStreamService");
    if (type_RedisStreamService != null)
    {
        Console.WriteLine("[PASS] 类型 RedisStreamIntegration.RedisStreamService (class) 存在");
        var ctors_RedisStreamService = type_RedisStreamService.GetConstructors();
        Console.WriteLine($"[PASS] RedisStreamIntegration.RedisStreamService 构造函数数量: {ctors_RedisStreamService.Length}");
        var methods_RedisStreamService = type_RedisStreamService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisStreamIntegration.RedisStreamService 公开方法数量: {methods_RedisStreamService.Length}");
        foreach (var m in methods_RedisStreamService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisStreamIntegration.RedisStreamService 未找到，尝试无命名空间...");
        type_RedisStreamService = Type.GetType("RedisStreamService");
        if (type_RedisStreamService != null)
            Console.WriteLine("[PASS] 类型 RedisStreamService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisStreamService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisStreamIntegration.RedisStreamExtensions
    var type_RedisStreamExtensions = Type.GetType("RedisStreamIntegration.RedisStreamExtensions");
    if (type_RedisStreamExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 RedisStreamIntegration.RedisStreamExtensions (class) 存在");
        var ctors_RedisStreamExtensions = type_RedisStreamExtensions.GetConstructors();
        Console.WriteLine($"[PASS] RedisStreamIntegration.RedisStreamExtensions 构造函数数量: {ctors_RedisStreamExtensions.Length}");
        var methods_RedisStreamExtensions = type_RedisStreamExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisStreamIntegration.RedisStreamExtensions 公开方法数量: {methods_RedisStreamExtensions.Length}");
        foreach (var m in methods_RedisStreamExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisStreamIntegration.RedisStreamExtensions 未找到，尝试无命名空间...");
        type_RedisStreamExtensions = Type.GetType("RedisStreamExtensions");
        if (type_RedisStreamExtensions != null)
            Console.WriteLine("[PASS] 类型 RedisStreamExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisStreamExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisStreamIntegration.Program
    var type_Program = Type.GetType("RedisStreamIntegration.Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 RedisStreamIntegration.Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] RedisStreamIntegration.Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisStreamIntegration.Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisStreamIntegration.Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: RedisStreamIntegration.IRedisStreamService
    var type_IRedisStreamService = Type.GetType("RedisStreamIntegration.IRedisStreamService");
    if (type_IRedisStreamService != null)
    {
        Console.WriteLine("[PASS] 类型 RedisStreamIntegration.IRedisStreamService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisStreamIntegration.IRedisStreamService 未找到，尝试无命名空间...");
        type_IRedisStreamService = Type.GetType("IRedisStreamService");
        if (type_IRedisStreamService != null)
            Console.WriteLine("[PASS] 类型 IRedisStreamService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IRedisStreamService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
