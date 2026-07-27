#load "minio_progress_hub.cs"

Console.WriteLine("=== minio_progress_hub Test ===");

try
{
    var t0 = typeof(UploadProgressHub);
    Console.WriteLine($"[PASS] UploadProgressHub 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}