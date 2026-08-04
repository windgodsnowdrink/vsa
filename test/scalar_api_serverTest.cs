#load "scalar_api_server.cs"

Console.WriteLine("=== scalar_api_server Test ===");

try
{
    var t0 = typeof(ScalarApiOptions);
    Console.WriteLine($"[PASS] ScalarApiOptions 存在");
    var t1 = typeof(ScalarApiExtensions);
    Console.WriteLine($"[PASS] ScalarApiExtensions 存在");
    var t2 = typeof(ScalarController);
    Console.WriteLine($"[PASS] ScalarController 存在");
    var t3 = typeof(ScalarRequest);
    Console.WriteLine($"[PASS] ScalarRequest 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}