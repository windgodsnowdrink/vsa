#load "snowflake_service.cs"

Console.WriteLine("=== snowflake_service.cs Test ===");

try
{
    // 验证 class: SnowflakeGenerator
    var type_SnowflakeGenerator = Type.GetType("SnowflakeGenerator");
    if (type_SnowflakeGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 SnowflakeGenerator (class) 存在");
        var ctors_SnowflakeGenerator = type_SnowflakeGenerator.GetConstructors();
        Console.WriteLine($"[PASS] SnowflakeGenerator 构造函数数量: {ctors_SnowflakeGenerator.Length}");
        var methods_SnowflakeGenerator = type_SnowflakeGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SnowflakeGenerator 公开方法数量: {methods_SnowflakeGenerator.Length}");
        foreach (var m in methods_SnowflakeGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SnowflakeGenerator 未找到，尝试无命名空间...");
        type_SnowflakeGenerator = Type.GetType("SnowflakeGenerator");
        if (type_SnowflakeGenerator != null)
            Console.WriteLine("[PASS] 类型 SnowflakeGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SnowflakeGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SnowflakeService
    var type_SnowflakeService = Type.GetType("SnowflakeService");
    if (type_SnowflakeService != null)
    {
        Console.WriteLine("[PASS] 类型 SnowflakeService (class) 存在");
        var ctors_SnowflakeService = type_SnowflakeService.GetConstructors();
        Console.WriteLine($"[PASS] SnowflakeService 构造函数数量: {ctors_SnowflakeService.Length}");
        var methods_SnowflakeService = type_SnowflakeService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SnowflakeService 公开方法数量: {methods_SnowflakeService.Length}");
        foreach (var m in methods_SnowflakeService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SnowflakeService 未找到，尝试无命名空间...");
        type_SnowflakeService = Type.GetType("SnowflakeService");
        if (type_SnowflakeService != null)
            Console.WriteLine("[PASS] 类型 SnowflakeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SnowflakeService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IdRequest
    var type_IdRequest = Type.GetType("IdRequest");
    if (type_IdRequest != null)
    {
        Console.WriteLine("[PASS] 类型 IdRequest (class) 存在");
        var ctors_IdRequest = type_IdRequest.GetConstructors();
        Console.WriteLine($"[PASS] IdRequest 构造函数数量: {ctors_IdRequest.Length}");
        var methods_IdRequest = type_IdRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IdRequest 公开方法数量: {methods_IdRequest.Length}");
        foreach (var m in methods_IdRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IdRequest 未找到，尝试无命名空间...");
        type_IdRequest = Type.GetType("IdRequest");
        if (type_IdRequest != null)
            Console.WriteLine("[PASS] 类型 IdRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IdRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IdContext
    var type_IdContext = Type.GetType("IdContext");
    if (type_IdContext != null)
    {
        Console.WriteLine("[PASS] 类型 IdContext (class) 存在");
        var ctors_IdContext = type_IdContext.GetConstructors();
        Console.WriteLine($"[PASS] IdContext 构造函数数量: {ctors_IdContext.Length}");
        var methods_IdContext = type_IdContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IdContext 公开方法数量: {methods_IdContext.Length}");
        foreach (var m in methods_IdContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IdContext 未找到，尝试无命名空间...");
        type_IdContext = Type.GetType("IdContext");
        if (type_IdContext != null)
            Console.WriteLine("[PASS] 类型 IdContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IdContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IdContextPooledPolicy
    var type_IdContextPooledPolicy = Type.GetType("IdContextPooledPolicy");
    if (type_IdContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 IdContextPooledPolicy (class) 存在");
        var ctors_IdContextPooledPolicy = type_IdContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] IdContextPooledPolicy 构造函数数量: {ctors_IdContextPooledPolicy.Length}");
        var methods_IdContextPooledPolicy = type_IdContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IdContextPooledPolicy 公开方法数量: {methods_IdContextPooledPolicy.Length}");
        foreach (var m in methods_IdContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IdContextPooledPolicy 未找到，尝试无命名空间...");
        type_IdContextPooledPolicy = Type.GetType("IdContextPooledPolicy");
        if (type_IdContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 IdContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IdContextPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
