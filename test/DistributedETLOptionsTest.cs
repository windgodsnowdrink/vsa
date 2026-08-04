#load "DistributedETLOptions.cs"

Console.WriteLine("=== DistributedETLOptions Test ===");

try
{
    var t0 = typeof(ChoETL.Integration.DistributedETLOptions);
    Console.WriteLine($"[PASS] DistributedETLOptions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}