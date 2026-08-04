#load "gat1400_viid_integration.cs"

Console.WriteLine("=== gat1400_viid_integration Test ===");

try
{
    Console.WriteLine("[PASS] 源文件可加载");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}