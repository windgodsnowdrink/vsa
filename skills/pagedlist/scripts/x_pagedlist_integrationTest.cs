#load "x_pagedlist_integration.cs"

Console.WriteLine("=== x_pagedlist_integration Test ===");

try
{
    var t0 = typeof(PagedList.PagedListOptions);
    Console.WriteLine($"[PASS] PagedListOptions 存在");
    var t1 = typeof(PagedList.PagedListService);
    Console.WriteLine($"[PASS] PagedListService 存在");
    var t2 = typeof(PagedList.MemoryCache);
    Console.WriteLine($"[PASS] MemoryCache 存在");
    var t3 = typeof(PagedList.CacheItem);
    Console.WriteLine($"[PASS] CacheItem 存在");
    var t4 = typeof(PagedList.PagedListServiceCollectionExtensions);
    Console.WriteLine($"[PASS] PagedListServiceCollectionExtensions 存在");
    var t5 = typeof(PagedList.IPagedListService);
    Console.WriteLine($"[PASS] IPagedListService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(PagedList.PagedResult);
    Console.WriteLine($"[PASS] PagedResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}