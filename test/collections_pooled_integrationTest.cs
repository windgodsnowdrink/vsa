#load "collections_pooled_integration.cs"

Console.WriteLine("=== collections_pooled_integration Test ===");

try
{
    var t0 = typeof(PooledCollectionsOptions);
    Console.WriteLine($"[PASS] PooledCollectionsOptions 存在");
    var t1 = typeof(PooledCollectionsService);
    Console.WriteLine($"[PASS] PooledCollectionsService 存在");
    var t2 = typeof(PooledCollectionsExtensions);
    Console.WriteLine($"[PASS] PooledCollectionsExtensions 存在");
    var t3 = typeof(PooledCollectionsExample);
    Console.WriteLine($"[PASS] PooledCollectionsExample 存在");
    var t4 = typeof(IPooledCollectionsService);
    Console.WriteLine($"[PASS] IPooledCollectionsService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}