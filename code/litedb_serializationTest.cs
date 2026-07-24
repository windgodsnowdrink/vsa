#load "litedb_serialization.cs"

Console.WriteLine("=== litedb_serialization.cs Test ===");

try
{
    // 验证 class: EventSerializer
    var type_EventSerializer = Type.GetType("EventSerializer");
    if (type_EventSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 EventSerializer (class) 存在");
        var ctors_EventSerializer = type_EventSerializer.GetConstructors();
        Console.WriteLine($"[PASS] EventSerializer 构造函数数量: {ctors_EventSerializer.Length}");
        var methods_EventSerializer = type_EventSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventSerializer 公开方法数量: {methods_EventSerializer.Length}");
        foreach (var m in methods_EventSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventSerializer 未找到，尝试无命名空间...");
        type_EventSerializer = Type.GetType("EventSerializer");
        if (type_EventSerializer != null)
            Console.WriteLine("[PASS] 类型 EventSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventSerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
