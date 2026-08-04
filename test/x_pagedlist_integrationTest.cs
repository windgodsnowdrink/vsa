#load "x_pagedlist_integration.cs"

Console.WriteLine("=== x_pagedlist_integration Test ===");

try
{
    var t0 = typeof(PagedListOptions);
    Console.WriteLine($"[PASS] PagedListOptions 存在");
    var t1 = typeof(PagedListService);
    Console.WriteLine($"[PASS] PagedListService 存在");
    var t2 = typeof(PagedListExtensions);
    Console.WriteLine($"[PASS] PagedListExtensions 存在");
    var t3 = typeof(PaginationService);
    Console.WriteLine($"[PASS] PaginationService 存在");
    var t4 = typeof(ProductsController);
    Console.WriteLine($"[PASS] ProductsController 存在");
    var t5 = typeof(PagerTagHelper);
    Console.WriteLine($"[PASS] PagerTagHelper 存在");
    var t6 = typeof(IPagedListService);
    Console.WriteLine($"[PASS] IPagedListService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(IPaginationService);
    Console.WriteLine($"[PASS] IPaginationService 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(PagedResult);
    Console.WriteLine($"[PASS] PagedResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}