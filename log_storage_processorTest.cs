#load "log_storage_processor.cs"

Console.WriteLine("=== log_storage_processor Test ===");

try
{
    var t0 = typeof(LogStorageProcessor);
    Console.WriteLine($"[PASS] LogStorageProcessor 存在");
    var t1 = typeof(LogEntry);
    Console.WriteLine($"[PASS] LogEntry record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}