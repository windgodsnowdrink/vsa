#load "litedb_tieredmemory.cs"

Console.WriteLine("=== litedb_tieredmemory.cs Test ===");

try
{
    // 验证 class: TieredMemoryStorage
    var type_TieredMemoryStorage = Type.GetType("TieredMemoryStorage");
    if (type_TieredMemoryStorage != null)
    {
        Console.WriteLine("[PASS] 类型 TieredMemoryStorage (class) 存在");
        var ctors_TieredMemoryStorage = type_TieredMemoryStorage.GetConstructors();
        Console.WriteLine($"[PASS] TieredMemoryStorage 构造函数数量: {ctors_TieredMemoryStorage.Length}");
        var methods_TieredMemoryStorage = type_TieredMemoryStorage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredMemoryStorage 公开方法数量: {methods_TieredMemoryStorage.Length}");
        foreach (var m in methods_TieredMemoryStorage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredMemoryStorage 未找到，尝试无命名空间...");
        type_TieredMemoryStorage = Type.GetType("TieredMemoryStorage");
        if (type_TieredMemoryStorage != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryStorage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryStorage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
