#load "wolverine_Meditor.cs"

Console.WriteLine("=== wolverine_Meditor Test ===");

try
{
    var t0 = typeof(Contracts.Commands.MediatorMessageHandlerAdapter);
    Console.WriteLine($"[PASS] MediatorMessageHandlerAdapter 存在");
    var t1 = typeof(Contracts.Commands.CreateOrderHandler);
    Console.WriteLine($"[PASS] CreateOrderHandler 存在");
    var t2 = typeof(Contracts.Commands.GetOrderHandler);
    Console.WriteLine($"[PASS] GetOrderHandler 存在");
    var t3 = typeof(Contracts.Commands.WolverineMediatorAdapter);
    Console.WriteLine($"[PASS] WolverineMediatorAdapter 存在");
    var t4 = typeof(Contracts.Commands.CreateOrder);
    Console.WriteLine($"[PASS] CreateOrder record 存在");
    var t5 = typeof(Contracts.Commands.最简洁);
    Console.WriteLine($"[PASS] 最简洁 record 存在");
    var t6 = typeof(Contracts.Commands.GetOrder);
    Console.WriteLine($"[PASS] GetOrder record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}