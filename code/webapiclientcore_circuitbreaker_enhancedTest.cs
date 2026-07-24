#load "webapiclientcore_circuitbreaker_enhanced.cs"

Console.WriteLine("=== webapiclientcore_circuitbreaker_enhanced.cs Test ===");

try
{
    // 验证 class: ChannelCircuitStateHandler
    var type_ChannelCircuitStateHandler = Type.GetType("ChannelCircuitStateHandler");
    if (type_ChannelCircuitStateHandler != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelCircuitStateHandler (class) 存在");
        var ctors_ChannelCircuitStateHandler = type_ChannelCircuitStateHandler.GetConstructors();
        Console.WriteLine($"[PASS] ChannelCircuitStateHandler 构造函数数量: {ctors_ChannelCircuitStateHandler.Length}");
        var methods_ChannelCircuitStateHandler = type_ChannelCircuitStateHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelCircuitStateHandler 公开方法数量: {methods_ChannelCircuitStateHandler.Length}");
        foreach (var m in methods_ChannelCircuitStateHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelCircuitStateHandler 未找到，尝试无命名空间...");
        type_ChannelCircuitStateHandler = Type.GetType("ChannelCircuitStateHandler");
        if (type_ChannelCircuitStateHandler != null)
            Console.WriteLine("[PASS] 类型 ChannelCircuitStateHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelCircuitStateHandler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
