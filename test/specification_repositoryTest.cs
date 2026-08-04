#load "specification_repository.cs"

Console.WriteLine("=== specification_repository Test ===");

try
{
    var t0 = typeof(HighPerformanceRepository);
    Console.WriteLine($"[PASS] HighPerformanceRepository 存在");
    var t1 = typeof(AotOptimizedSpecificationEvaluator);
    Console.WriteLine($"[PASS] AotOptimizedSpecificationEvaluator 存在");
    var t2 = typeof(RepositoryFactory);
    Console.WriteLine($"[PASS] RepositoryFactory 存在");
    var t3 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(SpecificationRepositoryOptions);
    Console.WriteLine($"[PASS] SpecificationRepositoryOptions 存在");
    var t5 = typeof(DistributedRepository);
    Console.WriteLine($"[PASS] DistributedRepository 存在");
    var t6 = typeof(CachedReadRepository);
    Console.WriteLine($"[PASS] CachedReadRepository 存在");
    var t7 = typeof(ShardingRepository);
    Console.WriteLine($"[PASS] ShardingRepository 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}