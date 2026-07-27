#load "ThingModelService.cs"

Console.WriteLine("=== ThingModelService Test ===");

try
{
    var t0 = typeof(ThingModelService);
    Console.WriteLine($"[PASS] ThingModelService 存在");
    var t1 = typeof(struct);
    Console.WriteLine($"[PASS] struct record 存在");
    var t2 = typeof(CacheEntry);
    Console.WriteLine($"[PASS] CacheEntry struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}