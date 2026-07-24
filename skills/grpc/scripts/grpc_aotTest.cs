#load "grpc_aot.cs"

Console.WriteLine("=== grpc_aot Test ===");

try
{
    var t0 = typeof(GrpcAOT.GrpcOptions);
    Console.WriteLine($"[PASS] GrpcOptions 存在");
    var t1 = typeof(GrpcAOT.HelloRequest);
    Console.WriteLine($"[PASS] HelloRequest 存在");
    var t2 = typeof(GrpcAOT.HelloResponse);
    Console.WriteLine($"[PASS] HelloResponse 存在");
    var t3 = typeof(GrpcAOT.HealthCheckRequest);
    Console.WriteLine($"[PASS] HealthCheckRequest 存在");
    var t4 = typeof(GrpcAOT.HealthCheckResponse);
    Console.WriteLine($"[PASS] HealthCheckResponse 存在");
    var t5 = typeof(GrpcAOT.PingRequest);
    Console.WriteLine($"[PASS] PingRequest 存在");
    var t6 = typeof(GrpcAOT.PingResponse);
    Console.WriteLine($"[PASS] PingResponse 存在");
    var t7 = typeof(GrpcAOT.GreeterService);
    Console.WriteLine($"[PASS] GreeterService 存在");
    var t8 = typeof(GrpcAOT.HealthService);
    Console.WriteLine($"[PASS] HealthService 存在");
    var t9 = typeof(GrpcAOT.PingService);
    Console.WriteLine($"[PASS] PingService 存在");
    var t10 = typeof(GrpcAOT.GrpcServer);
    Console.WriteLine($"[PASS] GrpcServer 存在");
    var t11 = typeof(GrpcAOT.GrpcClient);
    Console.WriteLine($"[PASS] GrpcClient 存在");
    var t12 = typeof(GrpcAOT.GrpcAotEngine);
    Console.WriteLine($"[PASS] GrpcAotEngine 存在");
    var t13 = typeof(GrpcAOT.GrpcCommandHandler);
    Console.WriteLine($"[PASS] GrpcCommandHandler 存在");
    var t14 = typeof(GrpcAOT.GrpcProtobuf);
    Console.WriteLine($"[PASS] GrpcProtobuf 存在");
    var t15 = typeof(GrpcAOT.Greeter);
    Console.WriteLine($"[PASS] Greeter 存在");
    var t16 = typeof(GrpcAOT.GreeterBase);
    Console.WriteLine($"[PASS] GreeterBase 存在");
    var t17 = typeof(GrpcAOT.GreeterClient);
    Console.WriteLine($"[PASS] GreeterClient 存在");
    var t18 = typeof(GrpcAOT.Health);
    Console.WriteLine($"[PASS] Health 存在");
    var t19 = typeof(GrpcAOT.HealthBase);
    Console.WriteLine($"[PASS] HealthBase 存在");
    var t20 = typeof(GrpcAOT.HealthClient);
    Console.WriteLine($"[PASS] HealthClient 存在");
    var t21 = typeof(GrpcAOT.Ping);
    Console.WriteLine($"[PASS] Ping 存在");
    var t22 = typeof(GrpcAOT.PingBase);
    Console.WriteLine($"[PASS] PingBase 存在");
    var t23 = typeof(GrpcAOT.PingClient);
    Console.WriteLine($"[PASS] PingClient 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}