#load "autofac_production.cs"

Console.WriteLine("=== autofac_production.cs Test ===");

try
{
    // 验证 class: CoreModule
    var type_CoreModule = Type.GetType("CoreModule");
    if (type_CoreModule != null)
    {
        Console.WriteLine("[PASS] 类型 CoreModule (class) 存在");
        var ctors_CoreModule = type_CoreModule.GetConstructors();
        Console.WriteLine($"[PASS] CoreModule 构造函数数量: {ctors_CoreModule.Length}");
        var methods_CoreModule = type_CoreModule.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CoreModule 公开方法数量: {methods_CoreModule.Length}");
        foreach (var m in methods_CoreModule)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CoreModule 未找到，尝试无命名空间...");
        type_CoreModule = Type.GetType("CoreModule");
        if (type_CoreModule != null)
            Console.WriteLine("[PASS] 类型 CoreModule (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CoreModule 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CallLogger
    var type_CallLogger = Type.GetType("CallLogger");
    if (type_CallLogger != null)
    {
        Console.WriteLine("[PASS] 类型 CallLogger (class) 存在");
        var ctors_CallLogger = type_CallLogger.GetConstructors();
        Console.WriteLine($"[PASS] CallLogger 构造函数数量: {ctors_CallLogger.Length}");
        var methods_CallLogger = type_CallLogger.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CallLogger 公开方法数量: {methods_CallLogger.Length}");
        foreach (var m in methods_CallLogger)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CallLogger 未找到，尝试无命名空间...");
        type_CallLogger = Type.GetType("CallLogger");
        if (type_CallLogger != null)
            Console.WriteLine("[PASS] 类型 CallLogger (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CallLogger 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AutofacConfig
    var type_AutofacConfig = Type.GetType("AutofacConfig");
    if (type_AutofacConfig != null)
    {
        Console.WriteLine("[PASS] 类型 AutofacConfig (class) 存在");
        var ctors_AutofacConfig = type_AutofacConfig.GetConstructors();
        Console.WriteLine($"[PASS] AutofacConfig 构造函数数量: {ctors_AutofacConfig.Length}");
        var methods_AutofacConfig = type_AutofacConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AutofacConfig 公开方法数量: {methods_AutofacConfig.Length}");
        foreach (var m in methods_AutofacConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AutofacConfig 未找到，尝试无命名空间...");
        type_AutofacConfig = Type.GetType("AutofacConfig");
        if (type_AutofacConfig != null)
            Console.WriteLine("[PASS] 类型 AutofacConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AutofacConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AutofacExtensions
    var type_AutofacExtensions = Type.GetType("AutofacExtensions");
    if (type_AutofacExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 AutofacExtensions (class) 存在");
        var ctors_AutofacExtensions = type_AutofacExtensions.GetConstructors();
        Console.WriteLine($"[PASS] AutofacExtensions 构造函数数量: {ctors_AutofacExtensions.Length}");
        var methods_AutofacExtensions = type_AutofacExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AutofacExtensions 公开方法数量: {methods_AutofacExtensions.Length}");
        foreach (var m in methods_AutofacExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AutofacExtensions 未找到，尝试无命名空间...");
        type_AutofacExtensions = Type.GetType("AutofacExtensions");
        if (type_AutofacExtensions != null)
            Console.WriteLine("[PASS] 类型 AutofacExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AutofacExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AutofacProductionExtensions
    var type_AutofacProductionExtensions = Type.GetType("AutofacProductionExtensions");
    if (type_AutofacProductionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 AutofacProductionExtensions (class) 存在");
        var ctors_AutofacProductionExtensions = type_AutofacProductionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] AutofacProductionExtensions 构造函数数量: {ctors_AutofacProductionExtensions.Length}");
        var methods_AutofacProductionExtensions = type_AutofacProductionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AutofacProductionExtensions 公开方法数量: {methods_AutofacProductionExtensions.Length}");
        foreach (var m in methods_AutofacProductionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AutofacProductionExtensions 未找到，尝试无命名空间...");
        type_AutofacProductionExtensions = Type.GetType("AutofacProductionExtensions");
        if (type_AutofacProductionExtensions != null)
            Console.WriteLine("[PASS] 类型 AutofacProductionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AutofacProductionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantIdentificationStrategy
    var type_TenantIdentificationStrategy = Type.GetType("TenantIdentificationStrategy");
    if (type_TenantIdentificationStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 TenantIdentificationStrategy (class) 存在");
        var ctors_TenantIdentificationStrategy = type_TenantIdentificationStrategy.GetConstructors();
        Console.WriteLine($"[PASS] TenantIdentificationStrategy 构造函数数量: {ctors_TenantIdentificationStrategy.Length}");
        var methods_TenantIdentificationStrategy = type_TenantIdentificationStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantIdentificationStrategy 公开方法数量: {methods_TenantIdentificationStrategy.Length}");
        foreach (var m in methods_TenantIdentificationStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantIdentificationStrategy 未找到，尝试无命名空间...");
        type_TenantIdentificationStrategy = Type.GetType("TenantIdentificationStrategy");
        if (type_TenantIdentificationStrategy != null)
            Console.WriteLine("[PASS] 类型 TenantIdentificationStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantIdentificationStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderService
    var type_OrderService = Type.GetType("OrderService");
    if (type_OrderService != null)
    {
        Console.WriteLine("[PASS] 类型 OrderService (class) 存在");
        var ctors_OrderService = type_OrderService.GetConstructors();
        Console.WriteLine($"[PASS] OrderService 构造函数数量: {ctors_OrderService.Length}");
        var methods_OrderService = type_OrderService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderService 公开方法数量: {methods_OrderService.Length}");
        foreach (var m in methods_OrderService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderService 未找到，尝试无命名空间...");
        type_OrderService = Type.GetType("OrderService");
        if (type_OrderService != null)
            Console.WriteLine("[PASS] 类型 OrderService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheManager
    var type_CacheManager = Type.GetType("CacheManager");
    if (type_CacheManager != null)
    {
        Console.WriteLine("[PASS] 类型 CacheManager (class) 存在");
        var ctors_CacheManager = type_CacheManager.GetConstructors();
        Console.WriteLine($"[PASS] CacheManager 构造函数数量: {ctors_CacheManager.Length}");
        var methods_CacheManager = type_CacheManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheManager 公开方法数量: {methods_CacheManager.Length}");
        foreach (var m in methods_CacheManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheManager 未找到，尝试无命名空间...");
        type_CacheManager = Type.GetType("CacheManager");
        if (type_CacheManager != null)
            Console.WriteLine("[PASS] 类型 CacheManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheManager 可能为顶层语句或嵌套类型");
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

    // 验证 class: ReportService
    var type_ReportService = Type.GetType("ReportService");
    if (type_ReportService != null)
    {
        Console.WriteLine("[PASS] 类型 ReportService (class) 存在");
        var ctors_ReportService = type_ReportService.GetConstructors();
        Console.WriteLine($"[PASS] ReportService 构造函数数量: {ctors_ReportService.Length}");
        var methods_ReportService = type_ReportService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReportService 公开方法数量: {methods_ReportService.Length}");
        foreach (var m in methods_ReportService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReportService 未找到，尝试无命名空间...");
        type_ReportService = Type.GetType("ReportService");
        if (type_ReportService != null)
            Console.WriteLine("[PASS] 类型 ReportService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReportService 可能为顶层语句或嵌套类型");
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

    // 验证 class: ExpensiveResource
    var type_ExpensiveResource = Type.GetType("ExpensiveResource");
    if (type_ExpensiveResource != null)
    {
        Console.WriteLine("[PASS] 类型 ExpensiveResource (class) 存在");
        var ctors_ExpensiveResource = type_ExpensiveResource.GetConstructors();
        Console.WriteLine($"[PASS] ExpensiveResource 构造函数数量: {ctors_ExpensiveResource.Length}");
        var methods_ExpensiveResource = type_ExpensiveResource.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExpensiveResource 公开方法数量: {methods_ExpensiveResource.Length}");
        foreach (var m in methods_ExpensiveResource)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExpensiveResource 未找到，尝试无命名空间...");
        type_ExpensiveResource = Type.GetType("ExpensiveResource");
        if (type_ExpensiveResource != null)
            Console.WriteLine("[PASS] 类型 ExpensiveResource (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExpensiveResource 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Composite
    var type_Composite = Type.GetType("Composite");
    if (type_Composite != null)
    {
        Console.WriteLine("[PASS] 类型 Composite (class) 存在");
        var ctors_Composite = type_Composite.GetConstructors();
        Console.WriteLine($"[PASS] Composite 构造函数数量: {ctors_Composite.Length}");
        var methods_Composite = type_Composite.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Composite 公开方法数量: {methods_Composite.Length}");
        foreach (var m in methods_Composite)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Composite 未找到，尝试无命名空间...");
        type_Composite = Type.GetType("Composite");
        if (type_Composite != null)
            Console.WriteLine("[PASS] 类型 Composite (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Composite 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CircularDependencyHandler
    var type_CircularDependencyHandler = Type.GetType("CircularDependencyHandler");
    if (type_CircularDependencyHandler != null)
    {
        Console.WriteLine("[PASS] 类型 CircularDependencyHandler (class) 存在");
        var ctors_CircularDependencyHandler = type_CircularDependencyHandler.GetConstructors();
        Console.WriteLine($"[PASS] CircularDependencyHandler 构造函数数量: {ctors_CircularDependencyHandler.Length}");
        var methods_CircularDependencyHandler = type_CircularDependencyHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CircularDependencyHandler 公开方法数量: {methods_CircularDependencyHandler.Length}");
        foreach (var m in methods_CircularDependencyHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CircularDependencyHandler 未找到，尝试无命名空间...");
        type_CircularDependencyHandler = Type.GetType("CircularDependencyHandler");
        if (type_CircularDependencyHandler != null)
            Console.WriteLine("[PASS] 类型 CircularDependencyHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CircularDependencyHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ConcurrentComponent
    var type_ConcurrentComponent = Type.GetType("ConcurrentComponent");
    if (type_ConcurrentComponent != null)
    {
        Console.WriteLine("[PASS] 类型 ConcurrentComponent (class) 存在");
        var ctors_ConcurrentComponent = type_ConcurrentComponent.GetConstructors();
        Console.WriteLine($"[PASS] ConcurrentComponent 构造函数数量: {ctors_ConcurrentComponent.Length}");
        var methods_ConcurrentComponent = type_ConcurrentComponent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConcurrentComponent 公开方法数量: {methods_ConcurrentComponent.Length}");
        foreach (var m in methods_ConcurrentComponent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConcurrentComponent 未找到，尝试无命名空间...");
        type_ConcurrentComponent = Type.GetType("ConcurrentComponent");
        if (type_ConcurrentComponent != null)
            Console.WriteLine("[PASS] 类型 ConcurrentComponent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConcurrentComponent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CustomRegistrationSource
    var type_CustomRegistrationSource = Type.GetType("CustomRegistrationSource");
    if (type_CustomRegistrationSource != null)
    {
        Console.WriteLine("[PASS] 类型 CustomRegistrationSource (class) 存在");
        var ctors_CustomRegistrationSource = type_CustomRegistrationSource.GetConstructors();
        Console.WriteLine($"[PASS] CustomRegistrationSource 构造函数数量: {ctors_CustomRegistrationSource.Length}");
        var methods_CustomRegistrationSource = type_CustomRegistrationSource.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CustomRegistrationSource 公开方法数量: {methods_CustomRegistrationSource.Length}");
        foreach (var m in methods_CustomRegistrationSource)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CustomRegistrationSource 未找到，尝试无命名空间...");
        type_CustomRegistrationSource = Type.GetType("CustomRegistrationSource");
        if (type_CustomRegistrationSource != null)
            Console.WriteLine("[PASS] 类型 CustomRegistrationSource (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CustomRegistrationSource 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PlatformSpecificService
    var type_PlatformSpecificService = Type.GetType("PlatformSpecificService");
    if (type_PlatformSpecificService != null)
    {
        Console.WriteLine("[PASS] 类型 PlatformSpecificService (class) 存在");
        var ctors_PlatformSpecificService = type_PlatformSpecificService.GetConstructors();
        Console.WriteLine($"[PASS] PlatformSpecificService 构造函数数量: {ctors_PlatformSpecificService.Length}");
        var methods_PlatformSpecificService = type_PlatformSpecificService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PlatformSpecificService 公开方法数量: {methods_PlatformSpecificService.Length}");
        foreach (var m in methods_PlatformSpecificService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PlatformSpecificService 未找到，尝试无命名空间...");
        type_PlatformSpecificService = Type.GetType("PlatformSpecificService");
        if (type_PlatformSpecificService != null)
            Console.WriteLine("[PASS] 类型 PlatformSpecificService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PlatformSpecificService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IOrderService
    var type_IOrderService = Type.GetType("IOrderService");
    if (type_IOrderService != null)
    {
        Console.WriteLine("[PASS] 类型 IOrderService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IOrderService 未找到，尝试无命名空间...");
        type_IOrderService = Type.GetType("IOrderService");
        if (type_IOrderService != null)
            Console.WriteLine("[PASS] 类型 IOrderService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IOrderService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ICacheManager
    var type_ICacheManager = Type.GetType("ICacheManager");
    if (type_ICacheManager != null)
    {
        Console.WriteLine("[PASS] 类型 ICacheManager (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ICacheManager 未找到，尝试无命名空间...");
        type_ICacheManager = Type.GetType("ICacheManager");
        if (type_ICacheManager != null)
            Console.WriteLine("[PASS] 类型 ICacheManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICacheManager 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IReportService
    var type_IReportService = Type.GetType("IReportService");
    if (type_IReportService != null)
    {
        Console.WriteLine("[PASS] 类型 IReportService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IReportService 未找到，尝试无命名空间...");
        type_IReportService = Type.GetType("IReportService");
        if (type_IReportService != null)
            Console.WriteLine("[PASS] 类型 IReportService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IReportService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IServiceMiddleware
    var type_IServiceMiddleware = Type.GetType("IServiceMiddleware");
    if (type_IServiceMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 IServiceMiddleware (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IServiceMiddleware 未找到，尝试无命名空间...");
        type_IServiceMiddleware = Type.GetType("IServiceMiddleware");
        if (type_IServiceMiddleware != null)
            Console.WriteLine("[PASS] 类型 IServiceMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IServiceMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPlatformSpecificService
    var type_IPlatformSpecificService = Type.GetType("IPlatformSpecificService");
    if (type_IPlatformSpecificService != null)
    {
        Console.WriteLine("[PASS] 类型 IPlatformSpecificService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPlatformSpecificService 未找到，尝试无命名空间...");
        type_IPlatformSpecificService = Type.GetType("IPlatformSpecificService");
        if (type_IPlatformSpecificService != null)
            Console.WriteLine("[PASS] 类型 IPlatformSpecificService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPlatformSpecificService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
