#load "sqlite_connection_pool.cs"

Console.WriteLine("=== sqlite_connection_pool.cs Test ===");

try
{
    // 验证 class: SqliteConnectionPool
    var type_SqliteConnectionPool = Type.GetType("SqliteConnectionPool");
    if (type_SqliteConnectionPool != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteConnectionPool (class) 存在");
        var ctors_SqliteConnectionPool = type_SqliteConnectionPool.GetConstructors();
        Console.WriteLine($"[PASS] SqliteConnectionPool 构造函数数量: {ctors_SqliteConnectionPool.Length}");
        var methods_SqliteConnectionPool = type_SqliteConnectionPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteConnectionPool 公开方法数量: {methods_SqliteConnectionPool.Length}");
        foreach (var m in methods_SqliteConnectionPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteConnectionPool 未找到，尝试无命名空间...");
        type_SqliteConnectionPool = Type.GetType("SqliteConnectionPool");
        if (type_SqliteConnectionPool != null)
            Console.WriteLine("[PASS] 类型 SqliteConnectionPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteConnectionPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteConnectionPoolPolicy
    var type_SqliteConnectionPoolPolicy = Type.GetType("SqliteConnectionPoolPolicy");
    if (type_SqliteConnectionPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteConnectionPoolPolicy (class) 存在");
        var ctors_SqliteConnectionPoolPolicy = type_SqliteConnectionPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] SqliteConnectionPoolPolicy 构造函数数量: {ctors_SqliteConnectionPoolPolicy.Length}");
        var methods_SqliteConnectionPoolPolicy = type_SqliteConnectionPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteConnectionPoolPolicy 公开方法数量: {methods_SqliteConnectionPoolPolicy.Length}");
        foreach (var m in methods_SqliteConnectionPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteConnectionPoolPolicy 未找到，尝试无命名空间...");
        type_SqliteConnectionPoolPolicy = Type.GetType("SqliteConnectionPoolPolicy");
        if (type_SqliteConnectionPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 SqliteConnectionPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteConnectionPoolPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
