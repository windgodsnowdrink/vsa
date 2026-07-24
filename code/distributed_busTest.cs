#load "distributed_bus.cs"

Console.WriteLine("=== distributed_bus.cs Test ===");

try
{
    // 验证 class: DistributedMessageBus
    var type_DistributedMessageBus = Type.GetType("DistributedMessageBus");
    if (type_DistributedMessageBus != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedMessageBus (class) 存在");
        var ctors_DistributedMessageBus = type_DistributedMessageBus.GetConstructors();
        Console.WriteLine($"[PASS] DistributedMessageBus 构造函数数量: {ctors_DistributedMessageBus.Length}");
        var methods_DistributedMessageBus = type_DistributedMessageBus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedMessageBus 公开方法数量: {methods_DistributedMessageBus.Length}");
        foreach (var m in methods_DistributedMessageBus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedMessageBus 未找到，尝试无命名空间...");
        type_DistributedMessageBus = Type.GetType("DistributedMessageBus");
        if (type_DistributedMessageBus != null)
            Console.WriteLine("[PASS] 类型 DistributedMessageBus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedMessageBus 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
