#load "fleck_websocket.cs"

Console.WriteLine("=== fleck_websocket Test ===");

try
{
    var t0 = typeof(RedisMessagePersistence);
    Console.WriteLine($"[PASS] RedisMessagePersistence 存在");
    var t1 = typeof(FleckMessageProcessor);
    Console.WriteLine($"[PASS] FleckMessageProcessor 存在");
    var t2 = typeof(WebSocketMessage);
    Console.WriteLine($"[PASS] WebSocketMessage 存在");
    var t3 = typeof(IMessagePersistence);
    Console.WriteLine($"[PASS] IMessagePersistence 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}