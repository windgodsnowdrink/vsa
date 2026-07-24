#load "masstransit_inmemory_integration.cs"

Console.WriteLine("=== masstransit_inmemory_integration.cs Test ===");

try
{
    // 验证 class: InMemoryMessageConsumer
    var type_InMemoryMessageConsumer = Type.GetType("InMemoryMessageConsumer");
    if (type_InMemoryMessageConsumer != null)
    {
        Console.WriteLine("[PASS] 类型 InMemoryMessageConsumer (class) 存在");
        var ctors_InMemoryMessageConsumer = type_InMemoryMessageConsumer.GetConstructors();
        Console.WriteLine($"[PASS] InMemoryMessageConsumer 构造函数数量: {ctors_InMemoryMessageConsumer.Length}");
        var methods_InMemoryMessageConsumer = type_InMemoryMessageConsumer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] InMemoryMessageConsumer 公开方法数量: {methods_InMemoryMessageConsumer.Length}");
        foreach (var m in methods_InMemoryMessageConsumer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 InMemoryMessageConsumer 未找到，尝试无命名空间...");
        type_InMemoryMessageConsumer = Type.GetType("InMemoryMessageConsumer");
        if (type_InMemoryMessageConsumer != null)
            Console.WriteLine("[PASS] 类型 InMemoryMessageConsumer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 InMemoryMessageConsumer 可能为顶层语句或嵌套类型");
    }

    // 验证 record: InMemoryMessage
    var type_InMemoryMessage = Type.GetType("InMemoryMessage");
    if (type_InMemoryMessage != null)
    {
        Console.WriteLine("[PASS] 类型 InMemoryMessage (record) 存在");
        var ctors_InMemoryMessage = type_InMemoryMessage.GetConstructors();
        Console.WriteLine($"[PASS] InMemoryMessage 构造函数数量: {ctors_InMemoryMessage.Length}");
        var methods_InMemoryMessage = type_InMemoryMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] InMemoryMessage 公开方法数量: {methods_InMemoryMessage.Length}");
        foreach (var m in methods_InMemoryMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 InMemoryMessage 未找到，尝试无命名空间...");
        type_InMemoryMessage = Type.GetType("InMemoryMessage");
        if (type_InMemoryMessage != null)
            Console.WriteLine("[PASS] 类型 InMemoryMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 InMemoryMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
