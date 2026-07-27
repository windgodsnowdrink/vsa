#load "downloader_integration.cs"

Console.WriteLine("=== downloader_integration Test ===");

try
{
    var t0 = typeof(DownloaderIntegration.DownloadService);
    Console.WriteLine($"[PASS] DownloadService 存在");
    var t1 = typeof(DownloaderIntegration.DownloadProgress);
    Console.WriteLine($"[PASS] DownloadProgress 存在");
    var t2 = typeof(DownloaderIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(DownloaderIntegration.IDownloadService);
    Console.WriteLine($"[PASS] IDownloadService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}