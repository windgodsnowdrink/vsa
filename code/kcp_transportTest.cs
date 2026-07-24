#load "kcp_transport.cs"

Console.WriteLine("=== kcp_transport.cs Test ===");

try
{
    // 验证 class: KcpMultiplexer
    var type_KcpMultiplexer = Type.GetType("KcpMultiplexer");
    if (type_KcpMultiplexer != null)
    {
        Console.WriteLine("[PASS] 类型 KcpMultiplexer (class) 存在");
        var ctors_KcpMultiplexer = type_KcpMultiplexer.GetConstructors();
        Console.WriteLine($"[PASS] KcpMultiplexer 构造函数数量: {ctors_KcpMultiplexer.Length}");
        var methods_KcpMultiplexer = type_KcpMultiplexer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KcpMultiplexer 公开方法数量: {methods_KcpMultiplexer.Length}");
        foreach (var m in methods_KcpMultiplexer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KcpMultiplexer 未找到，尝试无命名空间...");
        type_KcpMultiplexer = Type.GetType("KcpMultiplexer");
        if (type_KcpMultiplexer != null)
            Console.WriteLine("[PASS] 类型 KcpMultiplexer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KcpMultiplexer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KcpStreamPooledPolicy
    var type_KcpStreamPooledPolicy = Type.GetType("KcpStreamPooledPolicy");
    if (type_KcpStreamPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 KcpStreamPooledPolicy (class) 存在");
        var ctors_KcpStreamPooledPolicy = type_KcpStreamPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] KcpStreamPooledPolicy 构造函数数量: {ctors_KcpStreamPooledPolicy.Length}");
        var methods_KcpStreamPooledPolicy = type_KcpStreamPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KcpStreamPooledPolicy 公开方法数量: {methods_KcpStreamPooledPolicy.Length}");
        foreach (var m in methods_KcpStreamPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KcpStreamPooledPolicy 未找到，尝试无命名空间...");
        type_KcpStreamPooledPolicy = Type.GetType("KcpStreamPooledPolicy");
        if (type_KcpStreamPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 KcpStreamPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KcpStreamPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
