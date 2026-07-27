#load "webapiclientcore_circuitbreaker.cs"

Console.WriteLine("=== webapiclientcore_circuitbreaker.cs Test ===");

try
{
    // 验证 class: ChannelCircuitEventHandler
    var type_ChannelCircuitEventHandler = Type.GetType("ChannelCircuitEventHandler");
    if (type_ChannelCircuitEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelCircuitEventHandler (class) 存在");
        var ctors_ChannelCircuitEventHandler = type_ChannelCircuitEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] ChannelCircuitEventHandler 构造函数数量: {ctors_ChannelCircuitEventHandler.Length}");
        var methods_ChannelCircuitEventHandler = type_ChannelCircuitEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelCircuitEventHandler 公开方法数量: {methods_ChannelCircuitEventHandler.Length}");
        foreach (var m in methods_ChannelCircuitEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelCircuitEventHandler 未找到，尝试无命名空间...");
        type_ChannelCircuitEventHandler = Type.GetType("ChannelCircuitEventHandler");
        if (type_ChannelCircuitEventHandler != null)
            Console.WriteLine("[PASS] 类型 ChannelCircuitEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelCircuitEventHandler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
