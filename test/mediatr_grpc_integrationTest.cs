#load "mediatr_grpc_integration.cs"

Console.WriteLine("=== mediatr_grpc_integration Test ===");

try
{
    var t0 = typeof(GrpcRequest);
    Console.WriteLine($"[PASS] GrpcRequest 存在");
    var t1 = typeof(GrpcResponse);
    Console.WriteLine($"[PASS] GrpcResponse 存在");
    var t2 = typeof(MediatRGatewayService);
    Console.WriteLine($"[PASS] MediatRGatewayService 存在");
    var t3 = typeof(DistributedTransactionInterceptor);
    Console.WriteLine($"[PASS] DistributedTransactionInterceptor 存在");
    var t4 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t5 = typeof(ExceptionInterceptor);
    Console.WriteLine($"[PASS] ExceptionInterceptor 存在");
    var t6 = typeof(IMediatRGateway);
    Console.WriteLine($"[PASS] IMediatRGateway 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}