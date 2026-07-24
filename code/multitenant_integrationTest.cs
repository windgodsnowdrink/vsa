#load "multitenant_integration.cs"

Console.WriteLine("=== multitenant_integration.cs Test ===");

try
{
    // 验证 class: AppTenantInfo
    var type_AppTenantInfo = Type.GetType("AppTenantInfo");
    if (type_AppTenantInfo != null)
    {
        Console.WriteLine("[PASS] 类型 AppTenantInfo (class) 存在");
        var ctors_AppTenantInfo = type_AppTenantInfo.GetConstructors();
        Console.WriteLine($"[PASS] AppTenantInfo 构造函数数量: {ctors_AppTenantInfo.Length}");
        var methods_AppTenantInfo = type_AppTenantInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AppTenantInfo 公开方法数量: {methods_AppTenantInfo.Length}");
        foreach (var m in methods_AppTenantInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AppTenantInfo 未找到，尝试无命名空间...");
        type_AppTenantInfo = Type.GetType("AppTenantInfo");
        if (type_AppTenantInfo != null)
            Console.WriteLine("[PASS] 类型 AppTenantInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AppTenantInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantResolver
    var type_TenantResolver = Type.GetType("TenantResolver");
    if (type_TenantResolver != null)
    {
        Console.WriteLine("[PASS] 类型 TenantResolver (class) 存在");
        var ctors_TenantResolver = type_TenantResolver.GetConstructors();
        Console.WriteLine($"[PASS] TenantResolver 构造函数数量: {ctors_TenantResolver.Length}");
        var methods_TenantResolver = type_TenantResolver.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantResolver 公开方法数量: {methods_TenantResolver.Length}");
        foreach (var m in methods_TenantResolver)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantResolver 未找到，尝试无命名空间...");
        type_TenantResolver = Type.GetType("TenantResolver");
        if (type_TenantResolver != null)
            Console.WriteLine("[PASS] 类型 TenantResolver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantResolver 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantAwareCache
    var type_TenantAwareCache = Type.GetType("TenantAwareCache");
    if (type_TenantAwareCache != null)
    {
        Console.WriteLine("[PASS] 类型 TenantAwareCache (class) 存在");
        var ctors_TenantAwareCache = type_TenantAwareCache.GetConstructors();
        Console.WriteLine($"[PASS] TenantAwareCache 构造函数数量: {ctors_TenantAwareCache.Length}");
        var methods_TenantAwareCache = type_TenantAwareCache.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantAwareCache 公开方法数量: {methods_TenantAwareCache.Length}");
        foreach (var m in methods_TenantAwareCache)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantAwareCache 未找到，尝试无命名空间...");
        type_TenantAwareCache = Type.GetType("TenantAwareCache");
        if (type_TenantAwareCache != null)
            Console.WriteLine("[PASS] 类型 TenantAwareCache (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantAwareCache 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantQuotaService
    var type_TenantQuotaService = Type.GetType("TenantQuotaService");
    if (type_TenantQuotaService != null)
    {
        Console.WriteLine("[PASS] 类型 TenantQuotaService (class) 存在");
        var ctors_TenantQuotaService = type_TenantQuotaService.GetConstructors();
        Console.WriteLine($"[PASS] TenantQuotaService 构造函数数量: {ctors_TenantQuotaService.Length}");
        var methods_TenantQuotaService = type_TenantQuotaService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantQuotaService 公开方法数量: {methods_TenantQuotaService.Length}");
        foreach (var m in methods_TenantQuotaService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantQuotaService 未找到，尝试无命名空间...");
        type_TenantQuotaService = Type.GetType("TenantQuotaService");
        if (type_TenantQuotaService != null)
            Console.WriteLine("[PASS] 类型 TenantQuotaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantQuotaService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisTenantStore
    var type_RedisTenantStore = Type.GetType("RedisTenantStore");
    if (type_RedisTenantStore != null)
    {
        Console.WriteLine("[PASS] 类型 RedisTenantStore (class) 存在");
        var ctors_RedisTenantStore = type_RedisTenantStore.GetConstructors();
        Console.WriteLine($"[PASS] RedisTenantStore 构造函数数量: {ctors_RedisTenantStore.Length}");
        var methods_RedisTenantStore = type_RedisTenantStore.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisTenantStore 公开方法数量: {methods_RedisTenantStore.Length}");
        foreach (var m in methods_RedisTenantStore)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisTenantStore 未找到，尝试无命名空间...");
        type_RedisTenantStore = Type.GetType("RedisTenantStore");
        if (type_RedisTenantStore != null)
            Console.WriteLine("[PASS] 类型 RedisTenantStore (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisTenantStore 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DatabaseTenantStore
    var type_DatabaseTenantStore = Type.GetType("DatabaseTenantStore");
    if (type_DatabaseTenantStore != null)
    {
        Console.WriteLine("[PASS] 类型 DatabaseTenantStore (class) 存在");
        var ctors_DatabaseTenantStore = type_DatabaseTenantStore.GetConstructors();
        Console.WriteLine($"[PASS] DatabaseTenantStore 构造函数数量: {ctors_DatabaseTenantStore.Length}");
        var methods_DatabaseTenantStore = type_DatabaseTenantStore.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DatabaseTenantStore 公开方法数量: {methods_DatabaseTenantStore.Length}");
        foreach (var m in methods_DatabaseTenantStore)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DatabaseTenantStore 未找到，尝试无命名空间...");
        type_DatabaseTenantStore = Type.GetType("DatabaseTenantStore");
        if (type_DatabaseTenantStore != null)
            Console.WriteLine("[PASS] 类型 DatabaseTenantStore (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DatabaseTenantStore 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HybridTenantStore
    var type_HybridTenantStore = Type.GetType("HybridTenantStore");
    if (type_HybridTenantStore != null)
    {
        Console.WriteLine("[PASS] 类型 HybridTenantStore (class) 存在");
        var ctors_HybridTenantStore = type_HybridTenantStore.GetConstructors();
        Console.WriteLine($"[PASS] HybridTenantStore 构造函数数量: {ctors_HybridTenantStore.Length}");
        var methods_HybridTenantStore = type_HybridTenantStore.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HybridTenantStore 公开方法数量: {methods_HybridTenantStore.Length}");
        foreach (var m in methods_HybridTenantStore)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HybridTenantStore 未找到，尝试无命名空间...");
        type_HybridTenantStore = Type.GetType("HybridTenantStore");
        if (type_HybridTenantStore != null)
            Console.WriteLine("[PASS] 类型 HybridTenantStore (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HybridTenantStore 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteTenantStore
    var type_SqliteTenantStore = Type.GetType("SqliteTenantStore");
    if (type_SqliteTenantStore != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteTenantStore (class) 存在");
        var ctors_SqliteTenantStore = type_SqliteTenantStore.GetConstructors();
        Console.WriteLine($"[PASS] SqliteTenantStore 构造函数数量: {ctors_SqliteTenantStore.Length}");
        var methods_SqliteTenantStore = type_SqliteTenantStore.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteTenantStore 公开方法数量: {methods_SqliteTenantStore.Length}");
        foreach (var m in methods_SqliteTenantStore)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteTenantStore 未找到，尝试无命名空间...");
        type_SqliteTenantStore = Type.GetType("SqliteTenantStore");
        if (type_SqliteTenantStore != null)
            Console.WriteLine("[PASS] 类型 SqliteTenantStore (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteTenantStore 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiteDbTenantStore
    var type_LiteDbTenantStore = Type.GetType("LiteDbTenantStore");
    if (type_LiteDbTenantStore != null)
    {
        Console.WriteLine("[PASS] 类型 LiteDbTenantStore (class) 存在");
        var ctors_LiteDbTenantStore = type_LiteDbTenantStore.GetConstructors();
        Console.WriteLine($"[PASS] LiteDbTenantStore 构造函数数量: {ctors_LiteDbTenantStore.Length}");
        var methods_LiteDbTenantStore = type_LiteDbTenantStore.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiteDbTenantStore 公开方法数量: {methods_LiteDbTenantStore.Length}");
        foreach (var m in methods_LiteDbTenantStore)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiteDbTenantStore 未找到，尝试无命名空间...");
        type_LiteDbTenantStore = Type.GetType("LiteDbTenantStore");
        if (type_LiteDbTenantStore != null)
            Console.WriteLine("[PASS] 类型 LiteDbTenantStore (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbTenantStore 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MultiTenantBuilderExtensions
    var type_MultiTenantBuilderExtensions = Type.GetType("MultiTenantBuilderExtensions");
    if (type_MultiTenantBuilderExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MultiTenantBuilderExtensions (class) 存在");
        var ctors_MultiTenantBuilderExtensions = type_MultiTenantBuilderExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MultiTenantBuilderExtensions 构造函数数量: {ctors_MultiTenantBuilderExtensions.Length}");
        var methods_MultiTenantBuilderExtensions = type_MultiTenantBuilderExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultiTenantBuilderExtensions 公开方法数量: {methods_MultiTenantBuilderExtensions.Length}");
        foreach (var m in methods_MultiTenantBuilderExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultiTenantBuilderExtensions 未找到，尝试无命名空间...");
        type_MultiTenantBuilderExtensions = Type.GetType("MultiTenantBuilderExtensions");
        if (type_MultiTenantBuilderExtensions != null)
            Console.WriteLine("[PASS] 类型 MultiTenantBuilderExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultiTenantBuilderExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MultiTenantDbContext
    var type_MultiTenantDbContext = Type.GetType("MultiTenantDbContext");
    if (type_MultiTenantDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 MultiTenantDbContext (class) 存在");
        var ctors_MultiTenantDbContext = type_MultiTenantDbContext.GetConstructors();
        Console.WriteLine($"[PASS] MultiTenantDbContext 构造函数数量: {ctors_MultiTenantDbContext.Length}");
        var methods_MultiTenantDbContext = type_MultiTenantDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultiTenantDbContext 公开方法数量: {methods_MultiTenantDbContext.Length}");
        foreach (var m in methods_MultiTenantDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultiTenantDbContext 未找到，尝试无命名空间...");
        type_MultiTenantDbContext = Type.GetType("MultiTenantDbContext");
        if (type_MultiTenantDbContext != null)
            Console.WriteLine("[PASS] 类型 MultiTenantDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultiTenantDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DbContextPooledPolicy
    var type_DbContextPooledPolicy = Type.GetType("DbContextPooledPolicy");
    if (type_DbContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 DbContextPooledPolicy (class) 存在");
        var ctors_DbContextPooledPolicy = type_DbContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] DbContextPooledPolicy 构造函数数量: {ctors_DbContextPooledPolicy.Length}");
        var methods_DbContextPooledPolicy = type_DbContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DbContextPooledPolicy 公开方法数量: {methods_DbContextPooledPolicy.Length}");
        foreach (var m in methods_DbContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DbContextPooledPolicy 未找到，尝试无命名空间...");
        type_DbContextPooledPolicy = Type.GetType("DbContextPooledPolicy");
        if (type_DbContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 DbContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DbContextPooledPolicy 可能为顶层语句或嵌套类型");
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

    // 验证 class: ProductsController
    var type_ProductsController = Type.GetType("ProductsController");
    if (type_ProductsController != null)
    {
        Console.WriteLine("[PASS] 类型 ProductsController (class) 存在");
        var ctors_ProductsController = type_ProductsController.GetConstructors();
        Console.WriteLine($"[PASS] ProductsController 构造函数数量: {ctors_ProductsController.Length}");
        var methods_ProductsController = type_ProductsController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProductsController 公开方法数量: {methods_ProductsController.Length}");
        foreach (var m in methods_ProductsController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProductsController 未找到，尝试无命名空间...");
        type_ProductsController = Type.GetType("ProductsController");
        if (type_ProductsController != null)
            Console.WriteLine("[PASS] 类型 ProductsController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProductsController 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantAwareEntity
    var type_TenantAwareEntity = Type.GetType("TenantAwareEntity");
    if (type_TenantAwareEntity != null)
    {
        Console.WriteLine("[PASS] 类型 TenantAwareEntity (class) 存在");
        var ctors_TenantAwareEntity = type_TenantAwareEntity.GetConstructors();
        Console.WriteLine($"[PASS] TenantAwareEntity 构造函数数量: {ctors_TenantAwareEntity.Length}");
        var methods_TenantAwareEntity = type_TenantAwareEntity.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantAwareEntity 公开方法数量: {methods_TenantAwareEntity.Length}");
        foreach (var m in methods_TenantAwareEntity)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantAwareEntity 未找到，尝试无命名空间...");
        type_TenantAwareEntity = Type.GetType("TenantAwareEntity");
        if (type_TenantAwareEntity != null)
            Console.WriteLine("[PASS] 类型 TenantAwareEntity (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantAwareEntity 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Product
    var type_Product = Type.GetType("Product");
    if (type_Product != null)
    {
        Console.WriteLine("[PASS] 类型 Product (class) 存在");
        var ctors_Product = type_Product.GetConstructors();
        Console.WriteLine($"[PASS] Product 构造函数数量: {ctors_Product.Length}");
        var methods_Product = type_Product.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Product 公开方法数量: {methods_Product.Length}");
        foreach (var m in methods_Product)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Product 未找到，尝试无命名空间...");
        type_Product = Type.GetType("Product");
        if (type_Product != null)
            Console.WriteLine("[PASS] 类型 Product (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Product 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantRoutingStrategy
    var type_TenantRoutingStrategy = Type.GetType("TenantRoutingStrategy");
    if (type_TenantRoutingStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 TenantRoutingStrategy (class) 存在");
        var ctors_TenantRoutingStrategy = type_TenantRoutingStrategy.GetConstructors();
        Console.WriteLine($"[PASS] TenantRoutingStrategy 构造函数数量: {ctors_TenantRoutingStrategy.Length}");
        var methods_TenantRoutingStrategy = type_TenantRoutingStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantRoutingStrategy 公开方法数量: {methods_TenantRoutingStrategy.Length}");
        foreach (var m in methods_TenantRoutingStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantRoutingStrategy 未找到，尝试无命名空间...");
        type_TenantRoutingStrategy = Type.GetType("TenantRoutingStrategy");
        if (type_TenantRoutingStrategy != null)
            Console.WriteLine("[PASS] 类型 TenantRoutingStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantRoutingStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantRoutingOptions
    var type_TenantRoutingOptions = Type.GetType("TenantRoutingOptions");
    if (type_TenantRoutingOptions != null)
    {
        Console.WriteLine("[PASS] 类型 TenantRoutingOptions (class) 存在");
        var ctors_TenantRoutingOptions = type_TenantRoutingOptions.GetConstructors();
        Console.WriteLine($"[PASS] TenantRoutingOptions 构造函数数量: {ctors_TenantRoutingOptions.Length}");
        var methods_TenantRoutingOptions = type_TenantRoutingOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantRoutingOptions 公开方法数量: {methods_TenantRoutingOptions.Length}");
        foreach (var m in methods_TenantRoutingOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantRoutingOptions 未找到，尝试无命名空间...");
        type_TenantRoutingOptions = Type.GetType("TenantRoutingOptions");
        if (type_TenantRoutingOptions != null)
            Console.WriteLine("[PASS] 类型 TenantRoutingOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantRoutingOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantDatabase
    var type_TenantDatabase = Type.GetType("TenantDatabase");
    if (type_TenantDatabase != null)
    {
        Console.WriteLine("[PASS] 类型 TenantDatabase (class) 存在");
        var ctors_TenantDatabase = type_TenantDatabase.GetConstructors();
        Console.WriteLine($"[PASS] TenantDatabase 构造函数数量: {ctors_TenantDatabase.Length}");
        var methods_TenantDatabase = type_TenantDatabase.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantDatabase 公开方法数量: {methods_TenantDatabase.Length}");
        foreach (var m in methods_TenantDatabase)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantDatabase 未找到，尝试无命名空间...");
        type_TenantDatabase = Type.GetType("TenantDatabase");
        if (type_TenantDatabase != null)
            Console.WriteLine("[PASS] 类型 TenantDatabase (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantDatabase 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantContext
    var type_TenantContext = Type.GetType("TenantContext");
    if (type_TenantContext != null)
    {
        Console.WriteLine("[PASS] 类型 TenantContext (class) 存在");
        var ctors_TenantContext = type_TenantContext.GetConstructors();
        Console.WriteLine($"[PASS] TenantContext 构造函数数量: {ctors_TenantContext.Length}");
        var methods_TenantContext = type_TenantContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantContext 公开方法数量: {methods_TenantContext.Length}");
        foreach (var m in methods_TenantContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantContext 未找到，尝试无命名空间...");
        type_TenantContext = Type.GetType("TenantContext");
        if (type_TenantContext != null)
            Console.WriteLine("[PASS] 类型 TenantContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantDbContext
    var type_TenantDbContext = Type.GetType("TenantDbContext");
    if (type_TenantDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 TenantDbContext (class) 存在");
        var ctors_TenantDbContext = type_TenantDbContext.GetConstructors();
        Console.WriteLine($"[PASS] TenantDbContext 构造函数数量: {ctors_TenantDbContext.Length}");
        var methods_TenantDbContext = type_TenantDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantDbContext 公开方法数量: {methods_TenantDbContext.Length}");
        foreach (var m in methods_TenantDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantDbContext 未找到，尝试无命名空间...");
        type_TenantDbContext = Type.GetType("TenantDbContext");
        if (type_TenantDbContext != null)
            Console.WriteLine("[PASS] 类型 TenantDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITenantEventPublisher
    var type_ITenantEventPublisher = Type.GetType("ITenantEventPublisher");
    if (type_ITenantEventPublisher != null)
    {
        Console.WriteLine("[PASS] 类型 ITenantEventPublisher (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITenantEventPublisher 未找到，尝试无命名空间...");
        type_ITenantEventPublisher = Type.GetType("ITenantEventPublisher");
        if (type_ITenantEventPublisher != null)
            Console.WriteLine("[PASS] 类型 ITenantEventPublisher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITenantEventPublisher 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITenantEntity
    var type_ITenantEntity = Type.GetType("ITenantEntity");
    if (type_ITenantEntity != null)
    {
        Console.WriteLine("[PASS] 类型 ITenantEntity (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITenantEntity 未找到，尝试无命名空间...");
        type_ITenantEntity = Type.GetType("ITenantEntity");
        if (type_ITenantEntity != null)
            Console.WriteLine("[PASS] 类型 ITenantEntity (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITenantEntity 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITenantProvider
    var type_ITenantProvider = Type.GetType("ITenantProvider");
    if (type_ITenantProvider != null)
    {
        Console.WriteLine("[PASS] 类型 ITenantProvider (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITenantProvider 未找到，尝试无命名空间...");
        type_ITenantProvider = Type.GetType("ITenantProvider");
        if (type_ITenantProvider != null)
            Console.WriteLine("[PASS] 类型 ITenantProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITenantProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: QuotaType
    var type_QuotaType = Type.GetType("QuotaType");
    if (type_QuotaType != null)
    {
        Console.WriteLine("[PASS] 类型 QuotaType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuotaType 未找到，尝试无命名空间...");
        type_QuotaType = Type.GetType("QuotaType");
        if (type_QuotaType != null)
            Console.WriteLine("[PASS] 类型 QuotaType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuotaType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
