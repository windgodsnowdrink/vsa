#load "file_versioning.cs"

Console.WriteLine("=== file_versioning Test ===");

try
{
    var t0 = typeof(FileVersioningService);
    Console.WriteLine($"[PASS] FileVersioningService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}