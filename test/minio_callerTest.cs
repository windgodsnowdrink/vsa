#load "minio_caller.cs"

Console.WriteLine("=== minio_caller Test ===");

try
{
    var t0 = typeof(DataProcessedHandler);
    Console.WriteLine($"[PASS] DataProcessedHandler 存在");
    var t1 = typeof(ISharedService);
    Console.WriteLine($"[PASS] ISharedService 接口存在 (IsInterface: {t1.IsInterface})");
    var t2 = typeof(ProcessCommand);
    Console.WriteLine($"[PASS] ProcessCommand record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}