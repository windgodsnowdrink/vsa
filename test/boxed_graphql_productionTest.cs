#load "boxed_graphql_production.cs"

Console.WriteLine("=== boxed_graphql_production Test ===");

try
{
    var t0 = typeof(Query);
    Console.WriteLine($"[PASS] Query 存在");
    var t1 = typeof(Mutation);
    Console.WriteLine($"[PASS] Mutation 存在");
    var t2 = typeof(Subscription);
    Console.WriteLine($"[PASS] Subscription 存在");
    var t3 = typeof(GraphQLChannelProcessor);
    Console.WriteLine($"[PASS] GraphQLChannelProcessor 存在");
    var t4 = typeof(GraphQLMessage);
    Console.WriteLine($"[PASS] GraphQLMessage 存在");
    var t5 = typeof(Todo);
    Console.WriteLine($"[PASS] Todo 存在");
    var t6 = typeof(TodoDbContext);
    Console.WriteLine($"[PASS] TodoDbContext 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}