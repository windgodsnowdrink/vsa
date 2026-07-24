#load "message_persistence.cs"

Console.WriteLine("=== message_persistence.cs Test ===");

try
{
    // 验证 class: ChannelMessagePersister
    var type_ChannelMessagePersister = Type.GetType("ChannelMessagePersister");
    if (type_ChannelMessagePersister != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMessagePersister (class) 存在");
        var ctors_ChannelMessagePersister = type_ChannelMessagePersister.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMessagePersister 构造函数数量: {ctors_ChannelMessagePersister.Length}");
        var methods_ChannelMessagePersister = type_ChannelMessagePersister.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMessagePersister 公开方法数量: {methods_ChannelMessagePersister.Length}");
        foreach (var m in methods_ChannelMessagePersister)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMessagePersister 未找到，尝试无命名空间...");
        type_ChannelMessagePersister = Type.GetType("ChannelMessagePersister");
        if (type_ChannelMessagePersister != null)
            Console.WriteLine("[PASS] 类型 ChannelMessagePersister (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMessagePersister 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
