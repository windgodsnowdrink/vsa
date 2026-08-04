#load "program.cs"

Console.WriteLine("=== program Test ===");

try
{
    Console.WriteLine("[PASS] 源文件可加载");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}