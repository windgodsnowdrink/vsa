#load "grpc_integration.cs"

Console.WriteLine("=== grpc_integration Test ===");

try
{
    var t0 = typeof(GrpcChannelProcessor);
    Console.WriteLine($"[PASS] GrpcChannelProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}