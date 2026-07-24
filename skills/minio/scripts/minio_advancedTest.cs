#load "minio_advanced.cs"

Console.WriteLine("=== minio_advanced Test ===");

try
{
    var t0 = typeof(AdvancedMinioService);
    Console.WriteLine($"[PASS] AdvancedMinioService 存在");
    var t1 = typeof(MinioServiceExtensions);
    Console.WriteLine($"[PASS] MinioServiceExtensions 存在");
    var t2 = typeof(AlignedMemoryPool);
    Console.WriteLine($"[PASS] AlignedMemoryPool 存在");
    var t3 = typeof(AlignedMemoryOwner);
    Console.WriteLine($"[PASS] AlignedMemoryOwner 存在");
    var t4 = typeof(MinioObject);
    Console.WriteLine($"[PASS] MinioObject record 存在");
    var t5 = typeof(UploadPart);
    Console.WriteLine($"[PASS] UploadPart record 存在");
    var t6 = typeof(MinioUploadEvent);
    Console.WriteLine($"[PASS] MinioUploadEvent record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}