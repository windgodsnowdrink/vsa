#load "version_diff_analyzer.cs"

Console.WriteLine("=== version_diff_analyzer Test ===");

try
{
    var t0 = typeof(VersionDiffAnalyzer);
    Console.WriteLine($"[PASS] VersionDiffAnalyzer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}