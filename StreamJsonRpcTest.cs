#load "StreamJsonRpc.cs"

Console.WriteLine("=== StreamJsonRpc Test ===");

try
{
    var t0 = typeof(GreeterRpcService);
    Console.WriteLine($"[PASS] GreeterRpcService 存在");
    var t1 = typeof(IGreeterRpcService);
    Console.WriteLine($"[PASS] IGreeterRpcService 接口存在 (IsInterface: {t1.IsInterface})");
    var t2 = typeof(IClientCallback);
    Console.WriteLine($"[PASS] IClientCallback 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}