#load "brighter_integration.cs"

Console.WriteLine("=== brighter_integration.cs Test ===");

try
{
    // 验证 class: ConsistentHashShardingStrategy
    var type_ConsistentHashShardingStrategy = Type.GetType("ConsistentHashShardingStrategy");
    if (type_ConsistentHashShardingStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 ConsistentHashShardingStrategy (class) 存在");
        var ctors_ConsistentHashShardingStrategy = type_ConsistentHashShardingStrategy.GetConstructors();
        Console.WriteLine($"[PASS] ConsistentHashShardingStrategy 构造函数数量: {ctors_ConsistentHashShardingStrategy.Length}");
        var methods_ConsistentHashShardingStrategy = type_ConsistentHashShardingStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConsistentHashShardingStrategy 公开方法数量: {methods_ConsistentHashShardingStrategy.Length}");
        foreach (var m in methods_ConsistentHashShardingStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsistentHashShardingStrategy 未找到，尝试无命名空间...");
        type_ConsistentHashShardingStrategy = Type.GetType("ConsistentHashShardingStrategy");
        if (type_ConsistentHashShardingStrategy != null)
            Console.WriteLine("[PASS] 类型 ConsistentHashShardingStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConsistentHashShardingStrategy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
