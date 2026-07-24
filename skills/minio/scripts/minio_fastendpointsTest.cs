#load "minio_fastendpoints.cs"

Console.WriteLine("=== minio_fastendpoints Test ===");

try
{
    var t0 = typeof(MultipartUploadEndpoint);
    Console.WriteLine($"[PASS] MultipartUploadEndpoint 存在");
    var t1 = typeof(StreamUploadEndpoint);
    Console.WriteLine($"[PASS] StreamUploadEndpoint 存在");
    var t2 = typeof(BatchUploadEndpoint);
    Console.WriteLine($"[PASS] BatchUploadEndpoint 存在");
    var t3 = typeof(MultipartWithProgressEndpoint);
    Console.WriteLine($"[PASS] MultipartWithProgressEndpoint 存在");
    var t4 = typeof(DownloadEndpoint);
    Console.WriteLine($"[PASS] DownloadEndpoint 存在");
    var t5 = typeof(FileUploadRequest);
    Console.WriteLine($"[PASS] FileUploadRequest record 存在");
    var t6 = typeof(FileUploadResponse);
    Console.WriteLine($"[PASS] FileUploadResponse record 存在");
    var t7 = typeof(BatchFileUploadRequest);
    Console.WriteLine($"[PASS] BatchFileUploadRequest record 存在");
    var t8 = typeof(BatchUploadResponse);
    Console.WriteLine($"[PASS] BatchUploadResponse record 存在");
    var t9 = typeof(UploadProgressResponse);
    Console.WriteLine($"[PASS] UploadProgressResponse record 存在");
    var t10 = typeof(DownloadRequest);
    Console.WriteLine($"[PASS] DownloadRequest record 存在");
    var t11 = typeof(UploadProgress);
    Console.WriteLine($"[PASS] UploadProgress record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}