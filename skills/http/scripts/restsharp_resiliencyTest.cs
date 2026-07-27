#load "restsharp_resiliency.cs"

Console.WriteLine("=== restsharp_resiliency Test ===");

try
{
    var t0 = typeof(PollyResiliencyPolicy);
    Console.WriteLine($"[PASS] PollyResiliencyPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}