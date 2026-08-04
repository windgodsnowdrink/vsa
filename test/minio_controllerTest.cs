#load "minio_controller.cs"

Console.WriteLine("=== minio_controller Test ===");

try
{
    var t0 = typeof(MinioUploadController);
    Console.WriteLine($"[PASS] MinioUploadController 存在");
    var t1 = typeof(UploadProgress);
    Console.WriteLine($"[PASS] UploadProgress record 存在");
    var t2 = typeof(EncryptedFileRequest);
    Console.WriteLine($"[PASS] EncryptedFileRequest record 存在");
    var t3 = typeof(ResumableUploadRequest);
    Console.WriteLine($"[PASS] ResumableUploadRequest record 存在");
    var t4 = typeof(BatchDeleteRequest);
    Console.WriteLine($"[PASS] BatchDeleteRequest record 存在");
    var t5 = typeof(CacheClearRequest);
    Console.WriteLine($"[PASS] CacheClearRequest record 存在");
    var t6 = typeof(CachePreloadRequest);
    Console.WriteLine($"[PASS] CachePreloadRequest record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}