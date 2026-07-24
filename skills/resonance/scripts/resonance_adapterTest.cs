#load "resonance_adapter.cs"

Console.WriteLine("=== resonance_adapter Test ===");

try
{
    var t0 = typeof(MessageOptions);
    Console.WriteLine($"[PASS] MessageOptions 存在");
    var t1 = typeof(MessageContext);
    Console.WriteLine($"[PASS] MessageContext 存在");
    var t2 = typeof(MessageResult);
    Console.WriteLine($"[PASS] MessageResult 存在");
    var t3 = typeof(MessageStats);
    Console.WriteLine($"[PASS] MessageStats 存在");
    var t4 = typeof(MessagePipeline);
    Console.WriteLine($"[PASS] MessagePipeline 存在");
    var t5 = typeof(ResonanceAdapter);
    Console.WriteLine($"[PASS] ResonanceAdapter 存在");
    var t6 = typeof(MessageMiddleware);
    Console.WriteLine($"[PASS] MessageMiddleware 存在");
    var t7 = typeof(ResonanceAdapterExtensions);
    Console.WriteLine($"[PASS] ResonanceAdapterExtensions 存在");
    var t8 = typeof(IMessagePipeline);
    Console.WriteLine($"[PASS] IMessagePipeline 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(IResonanceAdapter);
    Console.WriteLine($"[PASS] IResonanceAdapter 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(UserMessage);
    Console.WriteLine($"[PASS] UserMessage record 存在");
    var t11 = typeof(OrderMessage);
    Console.WriteLine($"[PASS] OrderMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}