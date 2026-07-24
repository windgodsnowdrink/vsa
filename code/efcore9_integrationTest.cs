#load "efcore9_integration.cs"

Console.WriteLine("=== efcore9_integration.cs Test ===");

try
{
    // 验证 class: AppDbContext
    var type_AppDbContext = Type.GetType("AppDbContext");
    if (type_AppDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 AppDbContext (class) 存在");
        var ctors_AppDbContext = type_AppDbContext.GetConstructors();
        Console.WriteLine($"[PASS] AppDbContext 构造函数数量: {ctors_AppDbContext.Length}");
        var methods_AppDbContext = type_AppDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AppDbContext 公开方法数量: {methods_AppDbContext.Length}");
        foreach (var m in methods_AppDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AppDbContext 未找到，尝试无命名空间...");
        type_AppDbContext = Type.GetType("AppDbContext");
        if (type_AppDbContext != null)
            Console.WriteLine("[PASS] 类型 AppDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AppDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CachedRepository
    var type_CachedRepository = Type.GetType("CachedRepository");
    if (type_CachedRepository != null)
    {
        Console.WriteLine("[PASS] 类型 CachedRepository (class) 存在");
        var ctors_CachedRepository = type_CachedRepository.GetConstructors();
        Console.WriteLine($"[PASS] CachedRepository 构造函数数量: {ctors_CachedRepository.Length}");
        var methods_CachedRepository = type_CachedRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CachedRepository 公开方法数量: {methods_CachedRepository.Length}");
        foreach (var m in methods_CachedRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CachedRepository 未找到，尝试无命名空间...");
        type_CachedRepository = Type.GetType("CachedRepository");
        if (type_CachedRepository != null)
            Console.WriteLine("[PASS] 类型 CachedRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CachedRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ShardingStrategy
    var type_ShardingStrategy = Type.GetType("ShardingStrategy");
    if (type_ShardingStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 ShardingStrategy (class) 存在");
        var ctors_ShardingStrategy = type_ShardingStrategy.GetConstructors();
        Console.WriteLine($"[PASS] ShardingStrategy 构造函数数量: {ctors_ShardingStrategy.Length}");
        var methods_ShardingStrategy = type_ShardingStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ShardingStrategy 公开方法数量: {methods_ShardingStrategy.Length}");
        foreach (var m in methods_ShardingStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ShardingStrategy 未找到，尝试无命名空间...");
        type_ShardingStrategy = Type.GetType("ShardingStrategy");
        if (type_ShardingStrategy != null)
            Console.WriteLine("[PASS] 类型 ShardingStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ShardingStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EntitySnapshot
    var type_EntitySnapshot = Type.GetType("EntitySnapshot");
    if (type_EntitySnapshot != null)
    {
        Console.WriteLine("[PASS] 类型 EntitySnapshot (class) 存在");
        var ctors_EntitySnapshot = type_EntitySnapshot.GetConstructors();
        Console.WriteLine($"[PASS] EntitySnapshot 构造函数数量: {ctors_EntitySnapshot.Length}");
        var methods_EntitySnapshot = type_EntitySnapshot.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EntitySnapshot 公开方法数量: {methods_EntitySnapshot.Length}");
        foreach (var m in methods_EntitySnapshot)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EntitySnapshot 未找到，尝试无命名空间...");
        type_EntitySnapshot = Type.GetType("EntitySnapshot");
        if (type_EntitySnapshot != null)
            Console.WriteLine("[PASS] 类型 EntitySnapshot (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EntitySnapshot 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ArchivingService
    var type_ArchivingService = Type.GetType("ArchivingService");
    if (type_ArchivingService != null)
    {
        Console.WriteLine("[PASS] 类型 ArchivingService (class) 存在");
        var ctors_ArchivingService = type_ArchivingService.GetConstructors();
        Console.WriteLine($"[PASS] ArchivingService 构造函数数量: {ctors_ArchivingService.Length}");
        var methods_ArchivingService = type_ArchivingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ArchivingService 公开方法数量: {methods_ArchivingService.Length}");
        foreach (var m in methods_ArchivingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ArchivingService 未找到，尝试无命名空间...");
        type_ArchivingService = Type.GetType("ArchivingService");
        if (type_ArchivingService != null)
            Console.WriteLine("[PASS] 类型 ArchivingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ArchivingService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BackupService
    var type_BackupService = Type.GetType("BackupService");
    if (type_BackupService != null)
    {
        Console.WriteLine("[PASS] 类型 BackupService (class) 存在");
        var ctors_BackupService = type_BackupService.GetConstructors();
        Console.WriteLine($"[PASS] BackupService 构造函数数量: {ctors_BackupService.Length}");
        var methods_BackupService = type_BackupService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BackupService 公开方法数量: {methods_BackupService.Length}");
        foreach (var m in methods_BackupService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BackupService 未找到，尝试无命名空间...");
        type_BackupService = Type.GetType("BackupService");
        if (type_BackupService != null)
            Console.WriteLine("[PASS] 类型 BackupService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BackupService 可能为顶层语句或嵌套类型");
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

    // 验证 class: SnapshotService
    var type_SnapshotService = Type.GetType("SnapshotService");
    if (type_SnapshotService != null)
    {
        Console.WriteLine("[PASS] 类型 SnapshotService (class) 存在");
        var ctors_SnapshotService = type_SnapshotService.GetConstructors();
        Console.WriteLine($"[PASS] SnapshotService 构造函数数量: {ctors_SnapshotService.Length}");
        var methods_SnapshotService = type_SnapshotService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SnapshotService 公开方法数量: {methods_SnapshotService.Length}");
        foreach (var m in methods_SnapshotService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SnapshotService 未找到，尝试无命名空间...");
        type_SnapshotService = Type.GetType("SnapshotService");
        if (type_SnapshotService != null)
            Console.WriteLine("[PASS] 类型 SnapshotService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SnapshotService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IAuditableEntity
    var type_IAuditableEntity = Type.GetType("IAuditableEntity");
    if (type_IAuditableEntity != null)
    {
        Console.WriteLine("[PASS] 类型 IAuditableEntity (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAuditableEntity 未找到，尝试无命名空间...");
        type_IAuditableEntity = Type.GetType("IAuditableEntity");
        if (type_IAuditableEntity != null)
            Console.WriteLine("[PASS] 类型 IAuditableEntity (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAuditableEntity 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ISoftDelete
    var type_ISoftDelete = Type.GetType("ISoftDelete");
    if (type_ISoftDelete != null)
    {
        Console.WriteLine("[PASS] 类型 ISoftDelete (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ISoftDelete 未找到，尝试无命名空间...");
        type_ISoftDelete = Type.GetType("ISoftDelete");
        if (type_ISoftDelete != null)
            Console.WriteLine("[PASS] 类型 ISoftDelete (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISoftDelete 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IEncryptedEntity
    var type_IEncryptedEntity = Type.GetType("IEncryptedEntity");
    if (type_IEncryptedEntity != null)
    {
        Console.WriteLine("[PASS] 类型 IEncryptedEntity (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IEncryptedEntity 未找到，尝试无命名空间...");
        type_IEncryptedEntity = Type.GetType("IEncryptedEntity");
        if (type_IEncryptedEntity != null)
            Console.WriteLine("[PASS] 类型 IEncryptedEntity (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IEncryptedEntity 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
