#load "mediatr_local_message_table_integration.cs"

Console.WriteLine("=== mediatr_local_message_table_integration Test ===");

try
{
    var t0 = typeof(OutboxMessage);
    Console.WriteLine($"[PASS] OutboxMessage 存在");
    var t1 = typeof(MessageDbContext);
    Console.WriteLine($"[PASS] MessageDbContext 存在");
    var t2 = typeof(OutboxBehavior);
    Console.WriteLine($"[PASS] OutboxBehavior 存在");
    var t3 = typeof(OutboxProcessor);
    Console.WriteLine($"[PASS] OutboxProcessor 存在");
    var t4 = typeof(MediatRDependencyInjectionExtensions);
    Console.WriteLine($"[PASS] MediatRDependencyInjectionExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}