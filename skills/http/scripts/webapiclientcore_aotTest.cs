#load "webapiclientcore_aot.cs"

Console.WriteLine("=== webapiclientcore_aot Test ===");

try
{
    var t0 = typeof(AotClientProxyGenerator);
    Console.WriteLine($"[PASS] AotClientProxyGenerator 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}