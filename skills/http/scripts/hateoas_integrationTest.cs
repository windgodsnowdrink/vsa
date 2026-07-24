#load "hateoas_integration.cs"

Console.WriteLine("=== hateoas_integration Test ===");

try
{
    var t0 = typeof(Resource);
    Console.WriteLine($"[PASS] Resource 存在");
    var t1 = typeof(ProductsController);
    Console.WriteLine($"[PASS] ProductsController 存在");
    var t2 = typeof(Link);
    Console.WriteLine($"[PASS] Link record 存在");
    var t3 = typeof(Product);
    Console.WriteLine($"[PASS] Product record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}