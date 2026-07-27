#load "yarp_signature_middleware.cs"

Console.WriteLine("=== yarp_signature_middleware Test ===");

try
{
    var t0 = typeof(SignatureMiddleware);
    Console.WriteLine($"[PASS] SignatureMiddleware 存在");
    var t1 = typeof(SignatureOptions);
    Console.WriteLine($"[PASS] SignatureOptions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}