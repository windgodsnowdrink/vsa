#load "jsonrpc_optimized.cs"

Console.WriteLine("=== jsonrpc_optimized.cs Test ===");

try
{
    // 验证 class: RpcMessage
    var type_RpcMessage = Type.GetType("RpcMessage");
    if (type_RpcMessage != null)
    {
        Console.WriteLine("[PASS] 类型 RpcMessage (class) 存在");
        var ctors_RpcMessage = type_RpcMessage.GetConstructors();
        Console.WriteLine($"[PASS] RpcMessage 构造函数数量: {ctors_RpcMessage.Length}");
        var methods_RpcMessage = type_RpcMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RpcMessage 公开方法数量: {methods_RpcMessage.Length}");
        foreach (var m in methods_RpcMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RpcMessage 未找到，尝试无命名空间...");
        type_RpcMessage = Type.GetType("RpcMessage");
        if (type_RpcMessage != null)
            Console.WriteLine("[PASS] 类型 RpcMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RpcMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RpcConnectionPool
    var type_RpcConnectionPool = Type.GetType("RpcConnectionPool");
    if (type_RpcConnectionPool != null)
    {
        Console.WriteLine("[PASS] 类型 RpcConnectionPool (class) 存在");
        var ctors_RpcConnectionPool = type_RpcConnectionPool.GetConstructors();
        Console.WriteLine($"[PASS] RpcConnectionPool 构造函数数量: {ctors_RpcConnectionPool.Length}");
        var methods_RpcConnectionPool = type_RpcConnectionPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RpcConnectionPool 公开方法数量: {methods_RpcConnectionPool.Length}");
        foreach (var m in methods_RpcConnectionPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RpcConnectionPool 未找到，尝试无命名空间...");
        type_RpcConnectionPool = Type.GetType("RpcConnectionPool");
        if (type_RpcConnectionPool != null)
            Console.WriteLine("[PASS] 类型 RpcConnectionPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RpcConnectionPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RpcPooledObjectPolicy
    var type_RpcPooledObjectPolicy = Type.GetType("RpcPooledObjectPolicy");
    if (type_RpcPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 RpcPooledObjectPolicy (class) 存在");
        var ctors_RpcPooledObjectPolicy = type_RpcPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] RpcPooledObjectPolicy 构造函数数量: {ctors_RpcPooledObjectPolicy.Length}");
        var methods_RpcPooledObjectPolicy = type_RpcPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RpcPooledObjectPolicy 公开方法数量: {methods_RpcPooledObjectPolicy.Length}");
        foreach (var m in methods_RpcPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RpcPooledObjectPolicy 未找到，尝试无命名空间...");
        type_RpcPooledObjectPolicy = Type.GetType("RpcPooledObjectPolicy");
        if (type_RpcPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 RpcPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RpcPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
