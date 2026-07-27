#load "distributed_transaction.cs"

Console.WriteLine("=== distributed_transaction Test ===");

try
{
    var t0 = typeof(DistributedTransactionCoordinator);
    Console.WriteLine($"[PASS] DistributedTransactionCoordinator 存在");
    var t1 = typeof(TransactionCommand);
    Console.WriteLine($"[PASS] TransactionCommand record 存在");
    var t2 = typeof(TransactionResult);
    Console.WriteLine($"[PASS] TransactionResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}