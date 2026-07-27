#load "history_restore_service.cs"

Console.WriteLine("=== history_restore_service Test ===");

try
{
    var t0 = typeof(HistoryRestoreService);
    Console.WriteLine($"[PASS] HistoryRestoreService 存在");
    var t1 = typeof(RestoreRequest);
    Console.WriteLine($"[PASS] RestoreRequest record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}