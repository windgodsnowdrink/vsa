#load "snowflake_drift_service.cs"

Console.WriteLine("=== snowflake_drift_service.cs Test ===");

try
{
    // 验证 class: SnowflakeDriftGenerator
    var type_SnowflakeDriftGenerator = Type.GetType("SnowflakeDriftGenerator");
    if (type_SnowflakeDriftGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 SnowflakeDriftGenerator (class) 存在");
        var ctors_SnowflakeDriftGenerator = type_SnowflakeDriftGenerator.GetConstructors();
        Console.WriteLine($"[PASS] SnowflakeDriftGenerator 构造函数数量: {ctors_SnowflakeDriftGenerator.Length}");
        var methods_SnowflakeDriftGenerator = type_SnowflakeDriftGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SnowflakeDriftGenerator 公开方法数量: {methods_SnowflakeDriftGenerator.Length}");
        foreach (var m in methods_SnowflakeDriftGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SnowflakeDriftGenerator 未找到，尝试无命名空间...");
        type_SnowflakeDriftGenerator = Type.GetType("SnowflakeDriftGenerator");
        if (type_SnowflakeDriftGenerator != null)
            Console.WriteLine("[PASS] 类型 SnowflakeDriftGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SnowflakeDriftGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SnowflakeDriftService
    var type_SnowflakeDriftService = Type.GetType("SnowflakeDriftService");
    if (type_SnowflakeDriftService != null)
    {
        Console.WriteLine("[PASS] 类型 SnowflakeDriftService (class) 存在");
        var ctors_SnowflakeDriftService = type_SnowflakeDriftService.GetConstructors();
        Console.WriteLine($"[PASS] SnowflakeDriftService 构造函数数量: {ctors_SnowflakeDriftService.Length}");
        var methods_SnowflakeDriftService = type_SnowflakeDriftService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SnowflakeDriftService 公开方法数量: {methods_SnowflakeDriftService.Length}");
        foreach (var m in methods_SnowflakeDriftService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SnowflakeDriftService 未找到，尝试无命名空间...");
        type_SnowflakeDriftService = Type.GetType("SnowflakeDriftService");
        if (type_SnowflakeDriftService != null)
            Console.WriteLine("[PASS] 类型 SnowflakeDriftService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SnowflakeDriftService 可能为顶层语句或嵌套类型");
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
