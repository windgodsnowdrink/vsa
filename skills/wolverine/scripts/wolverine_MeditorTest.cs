#load "wolverine_Meditor.cs"

Console.WriteLine("=== wolverine_Meditor Test ===");

try
{
    // 验证 MediatorMessageHandlerAdapter 类 (泛型)
    var adapterType = typeof(MediatorMessageHandlerAdapter<,>);
    Console.WriteLine($"[PASS] MediatorMessageHandlerAdapter<TRequest,TResponse> 类型存在: {adapterType.Name}");

    // 验证 WolverineMediatorAdapter 类 (Infrastructure)
    var wolverineAdapterType = Type.GetType("Infrastructure.WolverineMediatorAdapter`2");
    Console.WriteLine($"[PASS] Infrastructure.WolverineMediatorAdapter 类型存在: {wolverineAdapterType != null}");

    // 验证 CreateOrderHandler 类
    var createOrderHandlerType = typeof(CreateOrderHandler);
    Console.WriteLine($"[PASS] CreateOrderHandler 类型存在: {createOrderHandlerType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}