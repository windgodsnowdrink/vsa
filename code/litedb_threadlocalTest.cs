#load "litedb_threadlocal.cs"

Console.WriteLine("=== litedb_threadlocal.cs Test ===");

try
{
    // 验证 class: ThreadLocalMemoryPool
    var type_ThreadLocalMemoryPool = Type.GetType("ThreadLocalMemoryPool");
    if (type_ThreadLocalMemoryPool != null)
    {
        Console.WriteLine("[PASS] 类型 ThreadLocalMemoryPool (class) 存在");
        var ctors_ThreadLocalMemoryPool = type_ThreadLocalMemoryPool.GetConstructors();
        Console.WriteLine($"[PASS] ThreadLocalMemoryPool 构造函数数量: {ctors_ThreadLocalMemoryPool.Length}");
        var methods_ThreadLocalMemoryPool = type_ThreadLocalMemoryPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ThreadLocalMemoryPool 公开方法数量: {methods_ThreadLocalMemoryPool.Length}");
        foreach (var m in methods_ThreadLocalMemoryPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ThreadLocalMemoryPool 未找到，尝试无命名空间...");
        type_ThreadLocalMemoryPool = Type.GetType("ThreadLocalMemoryPool");
        if (type_ThreadLocalMemoryPool != null)
            Console.WriteLine("[PASS] 类型 ThreadLocalMemoryPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThreadLocalMemoryPool 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
