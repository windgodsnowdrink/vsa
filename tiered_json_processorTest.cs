#load "tiered_json_processor.cs"

Console.WriteLine("=== tiered_json_processor Test ===");

try
{
    var t0 = typeof(TieredJsonProcessor);
    Console.WriteLine($"[PASS] TieredJsonProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}