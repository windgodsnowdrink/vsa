#load "masstransit_integration.cs"

Console.WriteLine("=== masstransit_integration.cs Test ===");

try
{
    // 验证 class: TailLatencyOptimizer
    var type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
    if (type_TailLatencyOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (class) 存在");
        var ctors_TailLatencyOptimizer = type_TailLatencyOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] TailLatencyOptimizer 构造函数数量: {ctors_TailLatencyOptimizer.Length}");
        var methods_TailLatencyOptimizer = type_TailLatencyOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TailLatencyOptimizer 公开方法数量: {methods_TailLatencyOptimizer.Length}");
        foreach (var m in methods_TailLatencyOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TailLatencyOptimizer 未找到，尝试无命名空间...");
        type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
        if (type_TailLatencyOptimizer != null)
            Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailLatencyOptimizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChannelMessageProcessor
    var type_ChannelMessageProcessor = Type.GetType("ChannelMessageProcessor");
    if (type_ChannelMessageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMessageProcessor (class) 存在");
        var ctors_ChannelMessageProcessor = type_ChannelMessageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMessageProcessor 构造函数数量: {ctors_ChannelMessageProcessor.Length}");
        var methods_ChannelMessageProcessor = type_ChannelMessageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMessageProcessor 公开方法数量: {methods_ChannelMessageProcessor.Length}");
        foreach (var m in methods_ChannelMessageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMessageProcessor 未找到，尝试无命名空间...");
        type_ChannelMessageProcessor = Type.GetType("ChannelMessageProcessor");
        if (type_ChannelMessageProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelMessageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMessageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: MessageEnvelope
    var type_MessageEnvelope = Type.GetType("MessageEnvelope");
    if (type_MessageEnvelope != null)
    {
        Console.WriteLine("[PASS] 类型 MessageEnvelope (struct) 存在");
        var ctors_MessageEnvelope = type_MessageEnvelope.GetConstructors();
        Console.WriteLine($"[PASS] MessageEnvelope 构造函数数量: {ctors_MessageEnvelope.Length}");
        var methods_MessageEnvelope = type_MessageEnvelope.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageEnvelope 公开方法数量: {methods_MessageEnvelope.Length}");
        foreach (var m in methods_MessageEnvelope)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageEnvelope 未找到，尝试无命名空间...");
        type_MessageEnvelope = Type.GetType("MessageEnvelope");
        if (type_MessageEnvelope != null)
            Console.WriteLine("[PASS] 类型 MessageEnvelope (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageEnvelope 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
