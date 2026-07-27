#load "halcyon_hal_integration.cs"

Console.WriteLine("=== halcyon_hal_integration Test ===");

try
{
    var t0 = typeof(ProductResource);
    Console.WriteLine($"[PASS] ProductResource 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}