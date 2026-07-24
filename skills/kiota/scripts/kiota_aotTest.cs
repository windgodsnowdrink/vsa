#load "kiota_aot.cs"

Console.WriteLine("=== kiota_aot Test ===");

try
{
    Console.WriteLine("[PASS] kiota_aot.cs 文件加载成功 (namespace KiotaAot)");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}