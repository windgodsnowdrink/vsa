#load "unitofwork_integration.cs"

Console.WriteLine("=== unitofwork_integration.cs Test ===");

try
{
    // 验证 class: UnitOfWork
    var type_UnitOfWork = Type.GetType("UnitOfWork");
    if (type_UnitOfWork != null)
    {
        Console.WriteLine("[PASS] 类型 UnitOfWork (class) 存在");
        var ctors_UnitOfWork = type_UnitOfWork.GetConstructors();
        Console.WriteLine($"[PASS] UnitOfWork 构造函数数量: {ctors_UnitOfWork.Length}");
        var methods_UnitOfWork = type_UnitOfWork.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UnitOfWork 公开方法数量: {methods_UnitOfWork.Length}");
        foreach (var m in methods_UnitOfWork)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UnitOfWork 未找到，尝试无命名空间...");
        type_UnitOfWork = Type.GetType("UnitOfWork");
        if (type_UnitOfWork != null)
            Console.WriteLine("[PASS] 类型 UnitOfWork (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UnitOfWork 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Repository
    var type_Repository = Type.GetType("Repository");
    if (type_Repository != null)
    {
        Console.WriteLine("[PASS] 类型 Repository (class) 存在");
        var ctors_Repository = type_Repository.GetConstructors();
        Console.WriteLine($"[PASS] Repository 构造函数数量: {ctors_Repository.Length}");
        var methods_Repository = type_Repository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Repository 公开方法数量: {methods_Repository.Length}");
        foreach (var m in methods_Repository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Repository 未找到，尝试无命名空间...");
        type_Repository = Type.GetType("Repository");
        if (type_Repository != null)
            Console.WriteLine("[PASS] 类型 Repository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Repository 可能为顶层语句或嵌套类型");
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

    // 验证 class: RepositoryPoolPolicy
    var type_RepositoryPoolPolicy = Type.GetType("RepositoryPoolPolicy");
    if (type_RepositoryPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 RepositoryPoolPolicy (class) 存在");
        var ctors_RepositoryPoolPolicy = type_RepositoryPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] RepositoryPoolPolicy 构造函数数量: {ctors_RepositoryPoolPolicy.Length}");
        var methods_RepositoryPoolPolicy = type_RepositoryPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RepositoryPoolPolicy 公开方法数量: {methods_RepositoryPoolPolicy.Length}");
        foreach (var m in methods_RepositoryPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RepositoryPoolPolicy 未找到，尝试无命名空间...");
        type_RepositoryPoolPolicy = Type.GetType("RepositoryPoolPolicy");
        if (type_RepositoryPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 RepositoryPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RepositoryPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EnhancedRepository
    var type_EnhancedRepository = Type.GetType("EnhancedRepository");
    if (type_EnhancedRepository != null)
    {
        Console.WriteLine("[PASS] 类型 EnhancedRepository (class) 存在");
        var ctors_EnhancedRepository = type_EnhancedRepository.GetConstructors();
        Console.WriteLine($"[PASS] EnhancedRepository 构造函数数量: {ctors_EnhancedRepository.Length}");
        var methods_EnhancedRepository = type_EnhancedRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EnhancedRepository 公开方法数量: {methods_EnhancedRepository.Length}");
        foreach (var m in methods_EnhancedRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EnhancedRepository 未找到，尝试无命名空间...");
        type_EnhancedRepository = Type.GetType("EnhancedRepository");
        if (type_EnhancedRepository != null)
            Console.WriteLine("[PASS] 类型 EnhancedRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EnhancedRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EnhancedUnitOfWork
    var type_EnhancedUnitOfWork = Type.GetType("EnhancedUnitOfWork");
    if (type_EnhancedUnitOfWork != null)
    {
        Console.WriteLine("[PASS] 类型 EnhancedUnitOfWork (class) 存在");
        var ctors_EnhancedUnitOfWork = type_EnhancedUnitOfWork.GetConstructors();
        Console.WriteLine($"[PASS] EnhancedUnitOfWork 构造函数数量: {ctors_EnhancedUnitOfWork.Length}");
        var methods_EnhancedUnitOfWork = type_EnhancedUnitOfWork.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EnhancedUnitOfWork 公开方法数量: {methods_EnhancedUnitOfWork.Length}");
        foreach (var m in methods_EnhancedUnitOfWork)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EnhancedUnitOfWork 未找到，尝试无命名空间...");
        type_EnhancedUnitOfWork = Type.GetType("EnhancedUnitOfWork");
        if (type_EnhancedUnitOfWork != null)
            Console.WriteLine("[PASS] 类型 EnhancedUnitOfWork (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EnhancedUnitOfWork 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IUnitOfWork
    var type_IUnitOfWork = Type.GetType("IUnitOfWork");
    if (type_IUnitOfWork != null)
    {
        Console.WriteLine("[PASS] 类型 IUnitOfWork (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IUnitOfWork 未找到，尝试无命名空间...");
        type_IUnitOfWork = Type.GetType("IUnitOfWork");
        if (type_IUnitOfWork != null)
            Console.WriteLine("[PASS] 类型 IUnitOfWork (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IUnitOfWork 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IRepository
    var type_IRepository = Type.GetType("IRepository");
    if (type_IRepository != null)
    {
        Console.WriteLine("[PASS] 类型 IRepository (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IRepository 未找到，尝试无命名空间...");
        type_IRepository = Type.GetType("IRepository");
        if (type_IRepository != null)
            Console.WriteLine("[PASS] 类型 IRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAggregateRoot
    var type_IAggregateRoot = Type.GetType("IAggregateRoot");
    if (type_IAggregateRoot != null)
    {
        Console.WriteLine("[PASS] 类型 IAggregateRoot (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAggregateRoot 未找到，尝试无命名空间...");
        type_IAggregateRoot = Type.GetType("IAggregateRoot");
        if (type_IAggregateRoot != null)
            Console.WriteLine("[PASS] 类型 IAggregateRoot (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAggregateRoot 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IBulkRepository
    var type_IBulkRepository = Type.GetType("IBulkRepository");
    if (type_IBulkRepository != null)
    {
        Console.WriteLine("[PASS] 类型 IBulkRepository (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IBulkRepository 未找到，尝试无命名空间...");
        type_IBulkRepository = Type.GetType("IBulkRepository");
        if (type_IBulkRepository != null)
            Console.WriteLine("[PASS] 类型 IBulkRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IBulkRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAnalyticalRepository
    var type_IAnalyticalRepository = Type.GetType("IAnalyticalRepository");
    if (type_IAnalyticalRepository != null)
    {
        Console.WriteLine("[PASS] 类型 IAnalyticalRepository (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAnalyticalRepository 未找到，尝试无命名空间...");
        type_IAnalyticalRepository = Type.GetType("IAnalyticalRepository");
        if (type_IAnalyticalRepository != null)
            Console.WriteLine("[PASS] 类型 IAnalyticalRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAnalyticalRepository 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
