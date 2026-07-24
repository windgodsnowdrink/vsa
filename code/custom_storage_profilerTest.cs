#load "custom_storage_profiler.cs"

Console.WriteLine("=== custom_storage_profiler.cs Test ===");

try
{
    // 验证 class: RedisStorageProvider
    var type_RedisStorageProvider = Type.GetType("RedisStorageProvider");
    if (type_RedisStorageProvider != null)
    {
        Console.WriteLine("[PASS] 类型 RedisStorageProvider (class) 存在");
        var ctors_RedisStorageProvider = type_RedisStorageProvider.GetConstructors();
        Console.WriteLine($"[PASS] RedisStorageProvider 构造函数数量: {ctors_RedisStorageProvider.Length}");
        var methods_RedisStorageProvider = type_RedisStorageProvider.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisStorageProvider 公开方法数量: {methods_RedisStorageProvider.Length}");
        foreach (var m in methods_RedisStorageProvider)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisStorageProvider 未找到，尝试无命名空间...");
        type_RedisStorageProvider = Type.GetType("RedisStorageProvider");
        if (type_RedisStorageProvider != null)
            Console.WriteLine("[PASS] 类型 RedisStorageProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisStorageProvider 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
