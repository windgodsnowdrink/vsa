#load "jsonrpc_impl.cs"

Console.WriteLine("=== jsonrpc_impl Test ===");

try
{
    var t0 = typeof(RpcServer);
    Console.WriteLine($"[PASS] RpcServer 存在");
    var t1 = typeof(RpcClient);
    Console.WriteLine($"[PASS] RpcClient 存在");
    var t2 = typeof(RpcDemo);
    Console.WriteLine($"[PASS] RpcDemo 存在");
    var t3 = typeof(DuplexStream);
    Console.WriteLine($"[PASS] DuplexStream 存在");
    var t4 = typeof(IRpcService);
    Console.WriteLine($"[PASS] IRpcService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}