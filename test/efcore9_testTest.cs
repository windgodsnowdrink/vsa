#load "efcore9_test.cs"

Console.WriteLine("=== efcore9_test Test ===");

try
{
    var t0 = typeof(AppJsonSerializerContext);
    Console.WriteLine($"[PASS] AppJsonSerializerContext 存在");
    var t1 = typeof(MySettings);
    Console.WriteLine($"[PASS] MySettings 存在");
    var t2 = typeof(ProductService);
    Console.WriteLine($"[PASS] ProductService 存在");
    var t3 = typeof(OrderService);
    Console.WriteLine($"[PASS] OrderService 存在");
    var t4 = typeof(ProductRepository);
    Console.WriteLine($"[PASS] ProductRepository 存在");
    var t5 = typeof(AppDbContext);
    Console.WriteLine($"[PASS] AppDbContext 存在");
    var t6 = typeof(QueryFilterContext);
    Console.WriteLine($"[PASS] QueryFilterContext 存在");
    var t7 = typeof(Product);
    Console.WriteLine($"[PASS] Product 存在");
    var t8 = typeof(ProductMetadata);
    Console.WriteLine($"[PASS] ProductMetadata 存在");
    var t9 = typeof(Order);
    Console.WriteLine($"[PASS] Order 存在");
    var t10 = typeof(OrderItem);
    Console.WriteLine($"[PASS] OrderItem 存在");
    var t11 = typeof(Result);
    Console.WriteLine($"[PASS] Result 存在");
    var t12 = typeof(MyExtensions);
    Console.WriteLine($"[PASS] MyExtensions 存在");
    var t13 = typeof(ProductDto);
    Console.WriteLine($"[PASS] ProductDto record 存在");
    var t14 = typeof(PaginatedResult);
    Console.WriteLine($"[PASS] PaginatedResult record 存在");
    var t15 = typeof(PagedResult);
    Console.WriteLine($"[PASS] PagedResult record 存在");
    var t16 = typeof(OrderRequest);
    Console.WriteLine($"[PASS] OrderRequest record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}