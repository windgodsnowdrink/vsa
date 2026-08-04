#load "jsonrpc_optimized.cs"

Console.WriteLine("=== jsonrpc_optimized Test ===");

try
{
    var t0 = typeof(RpcMessage);
    Console.WriteLine($"[PASS] RpcMessage 存在");
    var t1 = typeof(RpcConnectionPool);
    Console.WriteLine($"[PASS] RpcConnectionPool 存在");
    var t2 = typeof(RpcPooledObjectPolicy);
    Console.WriteLine($"[PASS] RpcPooledObjectPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}