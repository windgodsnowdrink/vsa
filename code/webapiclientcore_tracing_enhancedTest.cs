#load "webapiclientcore_tracing_enhanced.cs"

Console.WriteLine("=== webapiclientcore_tracing_enhanced.cs Test ===");

try
{
    // 验证 class: ChannelTraceProcessor
    var type_ChannelTraceProcessor = Type.GetType("ChannelTraceProcessor");
    if (type_ChannelTraceProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelTraceProcessor (class) 存在");
        var ctors_ChannelTraceProcessor = type_ChannelTraceProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelTraceProcessor 构造函数数量: {ctors_ChannelTraceProcessor.Length}");
        var methods_ChannelTraceProcessor = type_ChannelTraceProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelTraceProcessor 公开方法数量: {methods_ChannelTraceProcessor.Length}");
        foreach (var m in methods_ChannelTraceProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelTraceProcessor 未找到，尝试无命名空间...");
        type_ChannelTraceProcessor = Type.GetType("ChannelTraceProcessor");
        if (type_ChannelTraceProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelTraceProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelTraceProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
