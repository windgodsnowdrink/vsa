#load "dtm_demo.cs"

Console.WriteLine("=== dtm_demo Test ===");

try
{
    var t0 = typeof(TransferDbContext);
    Console.WriteLine($"[PASS] TransferDbContext 存在");
    var t1 = typeof(Account);
    Console.WriteLine($"[PASS] Account 存在");
    var t2 = typeof(TransferController);
    Console.WriteLine($"[PASS] TransferController 存在");
    var t3 = typeof(TransferRequest);
    Console.WriteLine($"[PASS] TransferRequest 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}