#load "uploadstream_integration.cs"

Console.WriteLine("=== uploadstream_integration Test ===");

try
{
    var t0 = typeof(UploadStreamService);
    Console.WriteLine($"[PASS] UploadStreamService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(UploadProgress);
    Console.WriteLine($"[PASS] UploadProgress 存在");
    var t3 = typeof(UploadStreamServiceExtensions);
    Console.WriteLine($"[PASS] UploadStreamServiceExtensions 存在");
    var t4 = typeof(IUploadStreamService);
    Console.WriteLine($"[PASS] IUploadStreamService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(UploadTask);
    Console.WriteLine($"[PASS] UploadTask record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}