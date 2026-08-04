#load "minio_integration.cs"

Console.WriteLine("=== minio_integration Test ===");

try
{
    var t0 = typeof(MinioFileService);
    Console.WriteLine($"[PASS] MinioFileService 存在");
    var t1 = typeof(MinioExtensions);
    Console.WriteLine($"[PASS] MinioExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}