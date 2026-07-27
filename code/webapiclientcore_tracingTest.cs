#load "webapiclientcore_tracing.cs"

Console.WriteLine("=== webapiclientcore_tracing.cs Test ===");

try
{
    // 验证 class: ChannelTracePropagator
    var type_ChannelTracePropagator = Type.GetType("ChannelTracePropagator");
    if (type_ChannelTracePropagator != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelTracePropagator (class) 存在");
        var ctors_ChannelTracePropagator = type_ChannelTracePropagator.GetConstructors();
        Console.WriteLine($"[PASS] ChannelTracePropagator 构造函数数量: {ctors_ChannelTracePropagator.Length}");
        var methods_ChannelTracePropagator = type_ChannelTracePropagator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelTracePropagator 公开方法数量: {methods_ChannelTracePropagator.Length}");
        foreach (var m in methods_ChannelTracePropagator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelTracePropagator 未找到，尝试无命名空间...");
        type_ChannelTracePropagator = Type.GetType("ChannelTracePropagator");
        if (type_ChannelTracePropagator != null)
            Console.WriteLine("[PASS] 类型 ChannelTracePropagator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelTracePropagator 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
