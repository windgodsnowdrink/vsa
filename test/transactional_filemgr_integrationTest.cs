#load "transactional_filemgr_integration.cs"

Console.WriteLine("=== transactional_filemgr_integration Test ===");

try
{
    var t0 = typeof(FileTransactionService);
    Console.WriteLine($"[PASS] FileTransactionService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}