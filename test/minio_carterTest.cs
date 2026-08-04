#load "minio_carter.cs"

Console.WriteLine("=== minio_carter Test ===");

try
{
    var t0 = typeof(MinioModule);
    Console.WriteLine($"[PASS] MinioModule 存在");
    var t1 = typeof(UploadProgress);
    Console.WriteLine($"[PASS] UploadProgress record 存在");
    var t2 = typeof(BatchDeleteRequest);
    Console.WriteLine($"[PASS] BatchDeleteRequest record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}