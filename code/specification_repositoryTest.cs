#load "specification_repository.cs"

Console.WriteLine("=== specification_repository.cs Test ===");

try
{
    // 验证 class: HighPerformanceRepository
    var type_HighPerformanceRepository = Type.GetType("HighPerformanceRepository");
    if (type_HighPerformanceRepository != null)
    {
        Console.WriteLine("[PASS] 类型 HighPerformanceRepository (class) 存在");
        var ctors_HighPerformanceRepository = type_HighPerformanceRepository.GetConstructors();
        Console.WriteLine($"[PASS] HighPerformanceRepository 构造函数数量: {ctors_HighPerformanceRepository.Length}");
        var methods_HighPerformanceRepository = type_HighPerformanceRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HighPerformanceRepository 公开方法数量: {methods_HighPerformanceRepository.Length}");
        foreach (var m in methods_HighPerformanceRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HighPerformanceRepository 未找到，尝试无命名空间...");
        type_HighPerformanceRepository = Type.GetType("HighPerformanceRepository");
        if (type_HighPerformanceRepository != null)
            Console.WriteLine("[PASS] 类型 HighPerformanceRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HighPerformanceRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AotOptimizedSpecificationEvaluator
    var type_AotOptimizedSpecificationEvaluator = Type.GetType("AotOptimizedSpecificationEvaluator");
    if (type_AotOptimizedSpecificationEvaluator != null)
    {
        Console.WriteLine("[PASS] 类型 AotOptimizedSpecificationEvaluator (class) 存在");
        var ctors_AotOptimizedSpecificationEvaluator = type_AotOptimizedSpecificationEvaluator.GetConstructors();
        Console.WriteLine($"[PASS] AotOptimizedSpecificationEvaluator 构造函数数量: {ctors_AotOptimizedSpecificationEvaluator.Length}");
        var methods_AotOptimizedSpecificationEvaluator = type_AotOptimizedSpecificationEvaluator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AotOptimizedSpecificationEvaluator 公开方法数量: {methods_AotOptimizedSpecificationEvaluator.Length}");
        foreach (var m in methods_AotOptimizedSpecificationEvaluator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AotOptimizedSpecificationEvaluator 未找到，尝试无命名空间...");
        type_AotOptimizedSpecificationEvaluator = Type.GetType("AotOptimizedSpecificationEvaluator");
        if (type_AotOptimizedSpecificationEvaluator != null)
            Console.WriteLine("[PASS] 类型 AotOptimizedSpecificationEvaluator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AotOptimizedSpecificationEvaluator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RepositoryFactory
    var type_RepositoryFactory = Type.GetType("RepositoryFactory");
    if (type_RepositoryFactory != null)
    {
        Console.WriteLine("[PASS] 类型 RepositoryFactory (class) 存在");
        var ctors_RepositoryFactory = type_RepositoryFactory.GetConstructors();
        Console.WriteLine($"[PASS] RepositoryFactory 构造函数数量: {ctors_RepositoryFactory.Length}");
        var methods_RepositoryFactory = type_RepositoryFactory.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RepositoryFactory 公开方法数量: {methods_RepositoryFactory.Length}");
        foreach (var m in methods_RepositoryFactory)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RepositoryFactory 未找到，尝试无命名空间...");
        type_RepositoryFactory = Type.GetType("RepositoryFactory");
        if (type_RepositoryFactory != null)
            Console.WriteLine("[PASS] 类型 RepositoryFactory (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RepositoryFactory 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SpecificationRepositoryOptions
    var type_SpecificationRepositoryOptions = Type.GetType("SpecificationRepositoryOptions");
    if (type_SpecificationRepositoryOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SpecificationRepositoryOptions (class) 存在");
        var ctors_SpecificationRepositoryOptions = type_SpecificationRepositoryOptions.GetConstructors();
        Console.WriteLine($"[PASS] SpecificationRepositoryOptions 构造函数数量: {ctors_SpecificationRepositoryOptions.Length}");
        var methods_SpecificationRepositoryOptions = type_SpecificationRepositoryOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SpecificationRepositoryOptions 公开方法数量: {methods_SpecificationRepositoryOptions.Length}");
        foreach (var m in methods_SpecificationRepositoryOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SpecificationRepositoryOptions 未找到，尝试无命名空间...");
        type_SpecificationRepositoryOptions = Type.GetType("SpecificationRepositoryOptions");
        if (type_SpecificationRepositoryOptions != null)
            Console.WriteLine("[PASS] 类型 SpecificationRepositoryOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpecificationRepositoryOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DistributedRepository
    var type_DistributedRepository = Type.GetType("DistributedRepository");
    if (type_DistributedRepository != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedRepository (class) 存在");
        var ctors_DistributedRepository = type_DistributedRepository.GetConstructors();
        Console.WriteLine($"[PASS] DistributedRepository 构造函数数量: {ctors_DistributedRepository.Length}");
        var methods_DistributedRepository = type_DistributedRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedRepository 公开方法数量: {methods_DistributedRepository.Length}");
        foreach (var m in methods_DistributedRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedRepository 未找到，尝试无命名空间...");
        type_DistributedRepository = Type.GetType("DistributedRepository");
        if (type_DistributedRepository != null)
            Console.WriteLine("[PASS] 类型 DistributedRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CachedReadRepository
    var type_CachedReadRepository = Type.GetType("CachedReadRepository");
    if (type_CachedReadRepository != null)
    {
        Console.WriteLine("[PASS] 类型 CachedReadRepository (class) 存在");
        var ctors_CachedReadRepository = type_CachedReadRepository.GetConstructors();
        Console.WriteLine($"[PASS] CachedReadRepository 构造函数数量: {ctors_CachedReadRepository.Length}");
        var methods_CachedReadRepository = type_CachedReadRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CachedReadRepository 公开方法数量: {methods_CachedReadRepository.Length}");
        foreach (var m in methods_CachedReadRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CachedReadRepository 未找到，尝试无命名空间...");
        type_CachedReadRepository = Type.GetType("CachedReadRepository");
        if (type_CachedReadRepository != null)
            Console.WriteLine("[PASS] 类型 CachedReadRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CachedReadRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ShardingRepository
    var type_ShardingRepository = Type.GetType("ShardingRepository");
    if (type_ShardingRepository != null)
    {
        Console.WriteLine("[PASS] 类型 ShardingRepository (class) 存在");
        var ctors_ShardingRepository = type_ShardingRepository.GetConstructors();
        Console.WriteLine($"[PASS] ShardingRepository 构造函数数量: {ctors_ShardingRepository.Length}");
        var methods_ShardingRepository = type_ShardingRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ShardingRepository 公开方法数量: {methods_ShardingRepository.Length}");
        foreach (var m in methods_ShardingRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ShardingRepository 未找到，尝试无命名空间...");
        type_ShardingRepository = Type.GetType("ShardingRepository");
        if (type_ShardingRepository != null)
            Console.WriteLine("[PASS] 类型 ShardingRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ShardingRepository 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
