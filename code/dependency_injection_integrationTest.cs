#load "dependency_injection_integration.cs"

Console.WriteLine("=== dependency_injection_integration.cs Test ===");

try
{
    // 验证 class: TransientService
    var type_TransientService = Type.GetType("TransientService");
    if (type_TransientService != null)
    {
        Console.WriteLine("[PASS] 类型 TransientService (class) 存在");
        var ctors_TransientService = type_TransientService.GetConstructors();
        Console.WriteLine($"[PASS] TransientService 构造函数数量: {ctors_TransientService.Length}");
        var methods_TransientService = type_TransientService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TransientService 公开方法数量: {methods_TransientService.Length}");
        foreach (var m in methods_TransientService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TransientService 未找到，尝试无命名空间...");
        type_TransientService = Type.GetType("TransientService");
        if (type_TransientService != null)
            Console.WriteLine("[PASS] 类型 TransientService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TransientService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScopedService
    var type_ScopedService = Type.GetType("ScopedService");
    if (type_ScopedService != null)
    {
        Console.WriteLine("[PASS] 类型 ScopedService (class) 存在");
        var ctors_ScopedService = type_ScopedService.GetConstructors();
        Console.WriteLine($"[PASS] ScopedService 构造函数数量: {ctors_ScopedService.Length}");
        var methods_ScopedService = type_ScopedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScopedService 公开方法数量: {methods_ScopedService.Length}");
        foreach (var m in methods_ScopedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScopedService 未找到，尝试无命名空间...");
        type_ScopedService = Type.GetType("ScopedService");
        if (type_ScopedService != null)
            Console.WriteLine("[PASS] 类型 ScopedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScopedService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SingletonService
    var type_SingletonService = Type.GetType("SingletonService");
    if (type_SingletonService != null)
    {
        Console.WriteLine("[PASS] 类型 SingletonService (class) 存在");
        var ctors_SingletonService = type_SingletonService.GetConstructors();
        Console.WriteLine($"[PASS] SingletonService 构造函数数量: {ctors_SingletonService.Length}");
        var methods_SingletonService = type_SingletonService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SingletonService 公开方法数量: {methods_SingletonService.Length}");
        foreach (var m in methods_SingletonService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SingletonService 未找到，尝试无命名空间...");
        type_SingletonService = Type.GetType("SingletonService");
        if (type_SingletonService != null)
            Console.WriteLine("[PASS] 类型 SingletonService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SingletonService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DecoratedService
    var type_DecoratedService = Type.GetType("DecoratedService");
    if (type_DecoratedService != null)
    {
        Console.WriteLine("[PASS] 类型 DecoratedService (class) 存在");
        var ctors_DecoratedService = type_DecoratedService.GetConstructors();
        Console.WriteLine($"[PASS] DecoratedService 构造函数数量: {ctors_DecoratedService.Length}");
        var methods_DecoratedService = type_DecoratedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DecoratedService 公开方法数量: {methods_DecoratedService.Length}");
        foreach (var m in methods_DecoratedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DecoratedService 未找到，尝试无命名空间...");
        type_DecoratedService = Type.GetType("DecoratedService");
        if (type_DecoratedService != null)
            Console.WriteLine("[PASS] 类型 DecoratedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DecoratedService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DecoratorService
    var type_DecoratorService = Type.GetType("DecoratorService");
    if (type_DecoratorService != null)
    {
        Console.WriteLine("[PASS] 类型 DecoratorService (class) 存在");
        var ctors_DecoratorService = type_DecoratorService.GetConstructors();
        Console.WriteLine($"[PASS] DecoratorService 构造函数数量: {ctors_DecoratorService.Length}");
        var methods_DecoratorService = type_DecoratorService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DecoratorService 公开方法数量: {methods_DecoratorService.Length}");
        foreach (var m in methods_DecoratorService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DecoratorService 未找到，尝试无命名空间...");
        type_DecoratorService = Type.GetType("DecoratorService");
        if (type_DecoratorService != null)
            Console.WriteLine("[PASS] 类型 DecoratorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DecoratorService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceA
    var type_ServiceA = Type.GetType("ServiceA");
    if (type_ServiceA != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceA (class) 存在");
        var ctors_ServiceA = type_ServiceA.GetConstructors();
        Console.WriteLine($"[PASS] ServiceA 构造函数数量: {ctors_ServiceA.Length}");
        var methods_ServiceA = type_ServiceA.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceA 公开方法数量: {methods_ServiceA.Length}");
        foreach (var m in methods_ServiceA)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceA 未找到，尝试无命名空间...");
        type_ServiceA = Type.GetType("ServiceA");
        if (type_ServiceA != null)
            Console.WriteLine("[PASS] 类型 ServiceA (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceA 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceB
    var type_ServiceB = Type.GetType("ServiceB");
    if (type_ServiceB != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceB (class) 存在");
        var ctors_ServiceB = type_ServiceB.GetConstructors();
        Console.WriteLine($"[PASS] ServiceB 构造函数数量: {ctors_ServiceB.Length}");
        var methods_ServiceB = type_ServiceB.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceB 公开方法数量: {methods_ServiceB.Length}");
        foreach (var m in methods_ServiceB)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceB 未找到，尝试无命名空间...");
        type_ServiceB = Type.GetType("ServiceB");
        if (type_ServiceB != null)
            Console.WriteLine("[PASS] 类型 ServiceB (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceB 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceFactory
    var type_ServiceFactory = Type.GetType("ServiceFactory");
    if (type_ServiceFactory != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceFactory (class) 存在");
        var ctors_ServiceFactory = type_ServiceFactory.GetConstructors();
        Console.WriteLine($"[PASS] ServiceFactory 构造函数数量: {ctors_ServiceFactory.Length}");
        var methods_ServiceFactory = type_ServiceFactory.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceFactory 公开方法数量: {methods_ServiceFactory.Length}");
        foreach (var m in methods_ServiceFactory)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceFactory 未找到，尝试无命名空间...");
        type_ServiceFactory = Type.GetType("ServiceFactory");
        if (type_ServiceFactory != null)
            Console.WriteLine("[PASS] 类型 ServiceFactory (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceFactory 可能为顶层语句或嵌套类型");
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

    // 验证 class: NamedServiceA
    var type_NamedServiceA = Type.GetType("NamedServiceA");
    if (type_NamedServiceA != null)
    {
        Console.WriteLine("[PASS] 类型 NamedServiceA (class) 存在");
        var ctors_NamedServiceA = type_NamedServiceA.GetConstructors();
        Console.WriteLine($"[PASS] NamedServiceA 构造函数数量: {ctors_NamedServiceA.Length}");
        var methods_NamedServiceA = type_NamedServiceA.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NamedServiceA 公开方法数量: {methods_NamedServiceA.Length}");
        foreach (var m in methods_NamedServiceA)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NamedServiceA 未找到，尝试无命名空间...");
        type_NamedServiceA = Type.GetType("NamedServiceA");
        if (type_NamedServiceA != null)
            Console.WriteLine("[PASS] 类型 NamedServiceA (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NamedServiceA 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NamedServiceB
    var type_NamedServiceB = Type.GetType("NamedServiceB");
    if (type_NamedServiceB != null)
    {
        Console.WriteLine("[PASS] 类型 NamedServiceB (class) 存在");
        var ctors_NamedServiceB = type_NamedServiceB.GetConstructors();
        Console.WriteLine($"[PASS] NamedServiceB 构造函数数量: {ctors_NamedServiceB.Length}");
        var methods_NamedServiceB = type_NamedServiceB.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NamedServiceB 公开方法数量: {methods_NamedServiceB.Length}");
        foreach (var m in methods_NamedServiceB)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NamedServiceB 未找到，尝试无命名空间...");
        type_NamedServiceB = Type.GetType("NamedServiceB");
        if (type_NamedServiceB != null)
            Console.WriteLine("[PASS] 类型 NamedServiceB (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NamedServiceB 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AppSettings
    var type_AppSettings = Type.GetType("AppSettings");
    if (type_AppSettings != null)
    {
        Console.WriteLine("[PASS] 类型 AppSettings (class) 存在");
        var ctors_AppSettings = type_AppSettings.GetConstructors();
        Console.WriteLine($"[PASS] AppSettings 构造函数数量: {ctors_AppSettings.Length}");
        var methods_AppSettings = type_AppSettings.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AppSettings 公开方法数量: {methods_AppSettings.Length}");
        foreach (var m in methods_AppSettings)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AppSettings 未找到，尝试无命名空间...");
        type_AppSettings = Type.GetType("AppSettings");
        if (type_AppSettings != null)
            Console.WriteLine("[PASS] 类型 AppSettings (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AppSettings 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AppLifetimeEvents
    var type_AppLifetimeEvents = Type.GetType("AppLifetimeEvents");
    if (type_AppLifetimeEvents != null)
    {
        Console.WriteLine("[PASS] 类型 AppLifetimeEvents (class) 存在");
        var ctors_AppLifetimeEvents = type_AppLifetimeEvents.GetConstructors();
        Console.WriteLine($"[PASS] AppLifetimeEvents 构造函数数量: {ctors_AppLifetimeEvents.Length}");
        var methods_AppLifetimeEvents = type_AppLifetimeEvents.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AppLifetimeEvents 公开方法数量: {methods_AppLifetimeEvents.Length}");
        foreach (var m in methods_AppLifetimeEvents)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AppLifetimeEvents 未找到，尝试无命名空间...");
        type_AppLifetimeEvents = Type.GetType("AppLifetimeEvents");
        if (type_AppLifetimeEvents != null)
            Console.WriteLine("[PASS] 类型 AppLifetimeEvents (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AppLifetimeEvents 可能为顶层语句或嵌套类型");
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

    // 验证 class: where
    var type_where = Type.GetType("where");
    if (type_where != null)
    {
        Console.WriteLine("[PASS] 类型 where (class) 存在");
        var ctors_where = type_where.GetConstructors();
        Console.WriteLine($"[PASS] where 构造函数数量: {ctors_where.Length}");
        var methods_where = type_where.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] where 公开方法数量: {methods_where.Length}");
        foreach (var m in methods_where)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 where 未找到，尝试无命名空间...");
        type_where = Type.GetType("where");
        if (type_where != null)
            Console.WriteLine("[PASS] 类型 where (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 where 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITransientService
    var type_ITransientService = Type.GetType("ITransientService");
    if (type_ITransientService != null)
    {
        Console.WriteLine("[PASS] 类型 ITransientService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITransientService 未找到，尝试无命名空间...");
        type_ITransientService = Type.GetType("ITransientService");
        if (type_ITransientService != null)
            Console.WriteLine("[PASS] 类型 ITransientService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITransientService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IScopedService
    var type_IScopedService = Type.GetType("IScopedService");
    if (type_IScopedService != null)
    {
        Console.WriteLine("[PASS] 类型 IScopedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IScopedService 未找到，尝试无命名空间...");
        type_IScopedService = Type.GetType("IScopedService");
        if (type_IScopedService != null)
            Console.WriteLine("[PASS] 类型 IScopedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IScopedService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ISingletonService
    var type_ISingletonService = Type.GetType("ISingletonService");
    if (type_ISingletonService != null)
    {
        Console.WriteLine("[PASS] 类型 ISingletonService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ISingletonService 未找到，尝试无命名空间...");
        type_ISingletonService = Type.GetType("ISingletonService");
        if (type_ISingletonService != null)
            Console.WriteLine("[PASS] 类型 ISingletonService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISingletonService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDecoratedService
    var type_IDecoratedService = Type.GetType("IDecoratedService");
    if (type_IDecoratedService != null)
    {
        Console.WriteLine("[PASS] 类型 IDecoratedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDecoratedService 未找到，尝试无命名空间...");
        type_IDecoratedService = Type.GetType("IDecoratedService");
        if (type_IDecoratedService != null)
            Console.WriteLine("[PASS] 类型 IDecoratedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDecoratedService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IService
    var type_IService = Type.GetType("IService");
    if (type_IService != null)
    {
        Console.WriteLine("[PASS] 类型 IService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IService 未找到，尝试无命名空间...");
        type_IService = Type.GetType("IService");
        if (type_IService != null)
            Console.WriteLine("[PASS] 类型 IService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IServiceFactory
    var type_IServiceFactory = Type.GetType("IServiceFactory");
    if (type_IServiceFactory != null)
    {
        Console.WriteLine("[PASS] 类型 IServiceFactory (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IServiceFactory 未找到，尝试无命名空间...");
        type_IServiceFactory = Type.GetType("IServiceFactory");
        if (type_IServiceFactory != null)
            Console.WriteLine("[PASS] 类型 IServiceFactory (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IServiceFactory 可能为顶层语句或嵌套类型");
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

    // 验证 interface: INamedService
    var type_INamedService = Type.GetType("INamedService");
    if (type_INamedService != null)
    {
        Console.WriteLine("[PASS] 类型 INamedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 INamedService 未找到，尝试无命名空间...");
        type_INamedService = Type.GetType("INamedService");
        if (type_INamedService != null)
            Console.WriteLine("[PASS] 类型 INamedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 INamedService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
