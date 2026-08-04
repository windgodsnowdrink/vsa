#load "sharpziplib_integration.cs"

Console.WriteLine("=== sharpziplib_integration Test ===");

try
{
    var t0 = typeof(ZipOptions);
    Console.WriteLine($"[PASS] ZipOptions 存在");
    var t1 = typeof(ZipService);
    Console.WriteLine($"[PASS] ZipService 存在");
    var t2 = typeof(ZipServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ZipServiceCollectionExtensions 存在");
    var t3 = typeof(ZipDemo);
    Console.WriteLine($"[PASS] ZipDemo 存在");
    var t4 = typeof(IZipService);
    Console.WriteLine($"[PASS] IZipService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}