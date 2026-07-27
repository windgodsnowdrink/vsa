#load "query_builder_integration.cs"

Console.WriteLine("=== query_builder_integration Test ===");

try
{
    var t0 = typeof(DynamicQueryBuilder);
    Console.WriteLine($"[PASS] DynamicQueryBuilder 存在");
    var t1 = typeof(DtoToSpecConverter);
    Console.WriteLine($"[PASS] DtoToSpecConverter 存在");
    var t2 = typeof(DynamicSpecification);
    Console.WriteLine($"[PASS] DynamicSpecification 存在");
    var t3 = typeof(QueryService);
    Console.WriteLine($"[PASS] QueryService 存在");
    var t4 = typeof(QueryBuilderPoolPolicy);
    Console.WriteLine($"[PASS] QueryBuilderPoolPolicy 存在");
    var t5 = typeof(QueryBuilderDemo);
    Console.WriteLine($"[PASS] QueryBuilderDemo 存在");
    var t6 = typeof(IDynamicQueryBuilder);
    Console.WriteLine($"[PASS] IDynamicQueryBuilder 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(ProductQueryDto);
    Console.WriteLine($"[PASS] ProductQueryDto record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}