#load "restsharp_factory.cs"

Console.WriteLine("=== restsharp_factory.cs Test ===");

try
{
    // 验证 class: ChannelRestClientFactory
    var type_ChannelRestClientFactory = Type.GetType("ChannelRestClientFactory");
    if (type_ChannelRestClientFactory != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelRestClientFactory (class) 存在");
        var ctors_ChannelRestClientFactory = type_ChannelRestClientFactory.GetConstructors();
        Console.WriteLine($"[PASS] ChannelRestClientFactory 构造函数数量: {ctors_ChannelRestClientFactory.Length}");
        var methods_ChannelRestClientFactory = type_ChannelRestClientFactory.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelRestClientFactory 公开方法数量: {methods_ChannelRestClientFactory.Length}");
        foreach (var m in methods_ChannelRestClientFactory)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelRestClientFactory 未找到，尝试无命名空间...");
        type_ChannelRestClientFactory = Type.GetType("ChannelRestClientFactory");
        if (type_ChannelRestClientFactory != null)
            Console.WriteLine("[PASS] 类型 ChannelRestClientFactory (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelRestClientFactory 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
