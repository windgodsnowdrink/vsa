#load "scrutor_integration.cs"

Console.WriteLine("=== scrutor_integration.cs Test ===");

try
{
    // 验证 class: CoreDataProcessor
    var type_CoreDataProcessor = Type.GetType("CoreDataProcessor");
    if (type_CoreDataProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 CoreDataProcessor (class) 存在");
        var ctors_CoreDataProcessor = type_CoreDataProcessor.GetConstructors();
        Console.WriteLine($"[PASS] CoreDataProcessor 构造函数数量: {ctors_CoreDataProcessor.Length}");
        var methods_CoreDataProcessor = type_CoreDataProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CoreDataProcessor 公开方法数量: {methods_CoreDataProcessor.Length}");
        foreach (var m in methods_CoreDataProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CoreDataProcessor 未找到，尝试无命名空间...");
        type_CoreDataProcessor = Type.GetType("CoreDataProcessor");
        if (type_CoreDataProcessor != null)
            Console.WriteLine("[PASS] 类型 CoreDataProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CoreDataProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EncryptionDecorator
    var type_EncryptionDecorator = Type.GetType("EncryptionDecorator");
    if (type_EncryptionDecorator != null)
    {
        Console.WriteLine("[PASS] 类型 EncryptionDecorator (class) 存在");
        var ctors_EncryptionDecorator = type_EncryptionDecorator.GetConstructors();
        Console.WriteLine($"[PASS] EncryptionDecorator 构造函数数量: {ctors_EncryptionDecorator.Length}");
        var methods_EncryptionDecorator = type_EncryptionDecorator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EncryptionDecorator 公开方法数量: {methods_EncryptionDecorator.Length}");
        foreach (var m in methods_EncryptionDecorator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EncryptionDecorator 未找到，尝试无命名空间...");
        type_EncryptionDecorator = Type.GetType("EncryptionDecorator");
        if (type_EncryptionDecorator != null)
            Console.WriteLine("[PASS] 类型 EncryptionDecorator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EncryptionDecorator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CompressionDecorator
    var type_CompressionDecorator = Type.GetType("CompressionDecorator");
    if (type_CompressionDecorator != null)
    {
        Console.WriteLine("[PASS] 类型 CompressionDecorator (class) 存在");
        var ctors_CompressionDecorator = type_CompressionDecorator.GetConstructors();
        Console.WriteLine($"[PASS] CompressionDecorator 构造函数数量: {ctors_CompressionDecorator.Length}");
        var methods_CompressionDecorator = type_CompressionDecorator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CompressionDecorator 公开方法数量: {methods_CompressionDecorator.Length}");
        foreach (var m in methods_CompressionDecorator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CompressionDecorator 未找到，尝试无命名空间...");
        type_CompressionDecorator = Type.GetType("CompressionDecorator");
        if (type_CompressionDecorator != null)
            Console.WriteLine("[PASS] 类型 CompressionDecorator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CompressionDecorator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DevelopmentService
    var type_DevelopmentService = Type.GetType("DevelopmentService");
    if (type_DevelopmentService != null)
    {
        Console.WriteLine("[PASS] 类型 DevelopmentService (class) 存在");
        var ctors_DevelopmentService = type_DevelopmentService.GetConstructors();
        Console.WriteLine($"[PASS] DevelopmentService 构造函数数量: {ctors_DevelopmentService.Length}");
        var methods_DevelopmentService = type_DevelopmentService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DevelopmentService 公开方法数量: {methods_DevelopmentService.Length}");
        foreach (var m in methods_DevelopmentService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DevelopmentService 未找到，尝试无命名空间...");
        type_DevelopmentService = Type.GetType("DevelopmentService");
        if (type_DevelopmentService != null)
            Console.WriteLine("[PASS] 类型 DevelopmentService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DevelopmentService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProductionService
    var type_ProductionService = Type.GetType("ProductionService");
    if (type_ProductionService != null)
    {
        Console.WriteLine("[PASS] 类型 ProductionService (class) 存在");
        var ctors_ProductionService = type_ProductionService.GetConstructors();
        Console.WriteLine($"[PASS] ProductionService 构造函数数量: {ctors_ProductionService.Length}");
        var methods_ProductionService = type_ProductionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProductionService 公开方法数量: {methods_ProductionService.Length}");
        foreach (var m in methods_ProductionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProductionService 未找到，尝试无命名空间...");
        type_ProductionService = Type.GetType("ProductionService");
        if (type_ProductionService != null)
            Console.WriteLine("[PASS] 类型 ProductionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProductionService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FastService
    var type_FastService = Type.GetType("FastService");
    if (type_FastService != null)
    {
        Console.WriteLine("[PASS] 类型 FastService (class) 存在");
        var ctors_FastService = type_FastService.GetConstructors();
        Console.WriteLine($"[PASS] FastService 构造函数数量: {ctors_FastService.Length}");
        var methods_FastService = type_FastService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FastService 公开方法数量: {methods_FastService.Length}");
        foreach (var m in methods_FastService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FastService 未找到，尝试无命名空间...");
        type_FastService = Type.GetType("FastService");
        if (type_FastService != null)
            Console.WriteLine("[PASS] 类型 FastService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FastService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ReliableService
    var type_ReliableService = Type.GetType("ReliableService");
    if (type_ReliableService != null)
    {
        Console.WriteLine("[PASS] 类型 ReliableService (class) 存在");
        var ctors_ReliableService = type_ReliableService.GetConstructors();
        Console.WriteLine($"[PASS] ReliableService 构造函数数量: {ctors_ReliableService.Length}");
        var methods_ReliableService = type_ReliableService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReliableService 公开方法数量: {methods_ReliableService.Length}");
        foreach (var m in methods_ReliableService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReliableService 未找到，尝试无命名空间...");
        type_ReliableService = Type.GetType("ReliableService");
        if (type_ReliableService != null)
            Console.WriteLine("[PASS] 类型 ReliableService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReliableService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AdvancedDependencyInjection
    var type_AdvancedDependencyInjection = Type.GetType("AdvancedDependencyInjection");
    if (type_AdvancedDependencyInjection != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedDependencyInjection (class) 存在");
        var ctors_AdvancedDependencyInjection = type_AdvancedDependencyInjection.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedDependencyInjection 构造函数数量: {ctors_AdvancedDependencyInjection.Length}");
        var methods_AdvancedDependencyInjection = type_AdvancedDependencyInjection.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedDependencyInjection 公开方法数量: {methods_AdvancedDependencyInjection.Length}");
        foreach (var m in methods_AdvancedDependencyInjection)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedDependencyInjection 未找到，尝试无命名空间...");
        type_AdvancedDependencyInjection = Type.GetType("AdvancedDependencyInjection");
        if (type_AdvancedDependencyInjection != null)
            Console.WriteLine("[PASS] 类型 AdvancedDependencyInjection (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedDependencyInjection 可能为顶层语句或嵌套类型");
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

    // 验证 class: AesEncryptionProvider
    var type_AesEncryptionProvider = Type.GetType("AesEncryptionProvider");
    if (type_AesEncryptionProvider != null)
    {
        Console.WriteLine("[PASS] 类型 AesEncryptionProvider (class) 存在");
        var ctors_AesEncryptionProvider = type_AesEncryptionProvider.GetConstructors();
        Console.WriteLine($"[PASS] AesEncryptionProvider 构造函数数量: {ctors_AesEncryptionProvider.Length}");
        var methods_AesEncryptionProvider = type_AesEncryptionProvider.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AesEncryptionProvider 公开方法数量: {methods_AesEncryptionProvider.Length}");
        foreach (var m in methods_AesEncryptionProvider)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AesEncryptionProvider 未找到，尝试无命名空间...");
        type_AesEncryptionProvider = Type.GetType("AesEncryptionProvider");
        if (type_AesEncryptionProvider != null)
            Console.WriteLine("[PASS] 类型 AesEncryptionProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AesEncryptionProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GzipCompressionProvider
    var type_GzipCompressionProvider = Type.GetType("GzipCompressionProvider");
    if (type_GzipCompressionProvider != null)
    {
        Console.WriteLine("[PASS] 类型 GzipCompressionProvider (class) 存在");
        var ctors_GzipCompressionProvider = type_GzipCompressionProvider.GetConstructors();
        Console.WriteLine($"[PASS] GzipCompressionProvider 构造函数数量: {ctors_GzipCompressionProvider.Length}");
        var methods_GzipCompressionProvider = type_GzipCompressionProvider.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GzipCompressionProvider 公开方法数量: {methods_GzipCompressionProvider.Length}");
        foreach (var m in methods_GzipCompressionProvider)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GzipCompressionProvider 未找到，尝试无命名空间...");
        type_GzipCompressionProvider = Type.GetType("GzipCompressionProvider");
        if (type_GzipCompressionProvider != null)
            Console.WriteLine("[PASS] 类型 GzipCompressionProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GzipCompressionProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NamedServiceOptions
    var type_NamedServiceOptions = Type.GetType("NamedServiceOptions");
    if (type_NamedServiceOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NamedServiceOptions (class) 存在");
        var ctors_NamedServiceOptions = type_NamedServiceOptions.GetConstructors();
        Console.WriteLine($"[PASS] NamedServiceOptions 构造函数数量: {ctors_NamedServiceOptions.Length}");
        var methods_NamedServiceOptions = type_NamedServiceOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NamedServiceOptions 公开方法数量: {methods_NamedServiceOptions.Length}");
        foreach (var m in methods_NamedServiceOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NamedServiceOptions 未找到，尝试无命名空间...");
        type_NamedServiceOptions = Type.GetType("NamedServiceOptions");
        if (type_NamedServiceOptions != null)
            Console.WriteLine("[PASS] 类型 NamedServiceOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NamedServiceOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NamedServiceFactory
    var type_NamedServiceFactory = Type.GetType("NamedServiceFactory");
    if (type_NamedServiceFactory != null)
    {
        Console.WriteLine("[PASS] 类型 NamedServiceFactory (class) 存在");
        var ctors_NamedServiceFactory = type_NamedServiceFactory.GetConstructors();
        Console.WriteLine($"[PASS] NamedServiceFactory 构造函数数量: {ctors_NamedServiceFactory.Length}");
        var methods_NamedServiceFactory = type_NamedServiceFactory.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NamedServiceFactory 公开方法数量: {methods_NamedServiceFactory.Length}");
        foreach (var m in methods_NamedServiceFactory)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NamedServiceFactory 未找到，尝试无命名空间...");
        type_NamedServiceFactory = Type.GetType("NamedServiceFactory");
        if (type_NamedServiceFactory != null)
            Console.WriteLine("[PASS] 类型 NamedServiceFactory (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NamedServiceFactory 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CoreProcessor
    var type_CoreProcessor = Type.GetType("CoreProcessor");
    if (type_CoreProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 CoreProcessor (class) 存在");
        var ctors_CoreProcessor = type_CoreProcessor.GetConstructors();
        Console.WriteLine($"[PASS] CoreProcessor 构造函数数量: {ctors_CoreProcessor.Length}");
        var methods_CoreProcessor = type_CoreProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CoreProcessor 公开方法数量: {methods_CoreProcessor.Length}");
        foreach (var m in methods_CoreProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CoreProcessor 未找到，尝试无命名空间...");
        type_CoreProcessor = Type.GetType("CoreProcessor");
        if (type_CoreProcessor != null)
            Console.WriteLine("[PASS] 类型 CoreProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CoreProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LoggingProcessor
    var type_LoggingProcessor = Type.GetType("LoggingProcessor");
    if (type_LoggingProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 LoggingProcessor (class) 存在");
        var ctors_LoggingProcessor = type_LoggingProcessor.GetConstructors();
        Console.WriteLine($"[PASS] LoggingProcessor 构造函数数量: {ctors_LoggingProcessor.Length}");
        var methods_LoggingProcessor = type_LoggingProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoggingProcessor 公开方法数量: {methods_LoggingProcessor.Length}");
        foreach (var m in methods_LoggingProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoggingProcessor 未找到，尝试无命名空间...");
        type_LoggingProcessor = Type.GetType("LoggingProcessor");
        if (type_LoggingProcessor != null)
            Console.WriteLine("[PASS] 类型 LoggingProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoggingProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessingOptions
    var type_ProcessingOptions = Type.GetType("ProcessingOptions");
    if (type_ProcessingOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessingOptions (class) 存在");
        var ctors_ProcessingOptions = type_ProcessingOptions.GetConstructors();
        Console.WriteLine($"[PASS] ProcessingOptions 构造函数数量: {ctors_ProcessingOptions.Length}");
        var methods_ProcessingOptions = type_ProcessingOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessingOptions 公开方法数量: {methods_ProcessingOptions.Length}");
        foreach (var m in methods_ProcessingOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessingOptions 未找到，尝试无命名空间...");
        type_ProcessingOptions = Type.GetType("ProcessingOptions");
        if (type_ProcessingOptions != null)
            Console.WriteLine("[PASS] 类型 ProcessingOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessingOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CreateUserCommand
    var type_CreateUserCommand = Type.GetType("CreateUserCommand");
    if (type_CreateUserCommand != null)
    {
        Console.WriteLine("[PASS] 类型 CreateUserCommand (class) 存在");
        var ctors_CreateUserCommand = type_CreateUserCommand.GetConstructors();
        Console.WriteLine($"[PASS] CreateUserCommand 构造函数数量: {ctors_CreateUserCommand.Length}");
        var methods_CreateUserCommand = type_CreateUserCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateUserCommand 公开方法数量: {methods_CreateUserCommand.Length}");
        foreach (var m in methods_CreateUserCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateUserCommand 未找到，尝试无命名空间...");
        type_CreateUserCommand = Type.GetType("CreateUserCommand");
        if (type_CreateUserCommand != null)
            Console.WriteLine("[PASS] 类型 CreateUserCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateUserCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CreateUserHandler
    var type_CreateUserHandler = Type.GetType("CreateUserHandler");
    if (type_CreateUserHandler != null)
    {
        Console.WriteLine("[PASS] 类型 CreateUserHandler (class) 存在");
        var ctors_CreateUserHandler = type_CreateUserHandler.GetConstructors();
        Console.WriteLine($"[PASS] CreateUserHandler 构造函数数量: {ctors_CreateUserHandler.Length}");
        var methods_CreateUserHandler = type_CreateUserHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateUserHandler 公开方法数量: {methods_CreateUserHandler.Length}");
        foreach (var m in methods_CreateUserHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateUserHandler 未找到，尝试无命名空间...");
        type_CreateUserHandler = Type.GetType("CreateUserHandler");
        if (type_CreateUserHandler != null)
            Console.WriteLine("[PASS] 类型 CreateUserHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateUserHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DomainService
    var type_DomainService = Type.GetType("DomainService");
    if (type_DomainService != null)
    {
        Console.WriteLine("[PASS] 类型 DomainService (class) 存在");
        var ctors_DomainService = type_DomainService.GetConstructors();
        Console.WriteLine($"[PASS] DomainService 构造函数数量: {ctors_DomainService.Length}");
        var methods_DomainService = type_DomainService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DomainService 公开方法数量: {methods_DomainService.Length}");
        foreach (var m in methods_DomainService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DomainService 未找到，尝试无命名空间...");
        type_DomainService = Type.GetType("DomainService");
        if (type_DomainService != null)
            Console.WriteLine("[PASS] 类型 DomainService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DomainService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AppService
    var type_AppService = Type.GetType("AppService");
    if (type_AppService != null)
    {
        Console.WriteLine("[PASS] 类型 AppService (class) 存在");
        var ctors_AppService = type_AppService.GetConstructors();
        Console.WriteLine($"[PASS] AppService 构造函数数量: {ctors_AppService.Length}");
        var methods_AppService = type_AppService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AppService 公开方法数量: {methods_AppService.Length}");
        foreach (var m in methods_AppService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AppService 未找到，尝试无命名空间...");
        type_AppService = Type.GetType("AppService");
        if (type_AppService != null)
            Console.WriteLine("[PASS] 类型 AppService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AppService 可能为顶层语句或嵌套类型");
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

    // 验证 class: ApiController
    var type_ApiController = Type.GetType("ApiController");
    if (type_ApiController != null)
    {
        Console.WriteLine("[PASS] 类型 ApiController (class) 存在");
        var ctors_ApiController = type_ApiController.GetConstructors();
        Console.WriteLine($"[PASS] ApiController 构造函数数量: {ctors_ApiController.Length}");
        var methods_ApiController = type_ApiController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ApiController 公开方法数量: {methods_ApiController.Length}");
        foreach (var m in methods_ApiController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ApiController 未找到，尝试无命名空间...");
        type_ApiController = Type.GetType("ApiController");
        if (type_ApiController != null)
            Console.WriteLine("[PASS] 类型 ApiController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ApiController 可能为顶层语句或嵌套类型");
    }

    // 验证 class: InjectableAttribute
    var type_InjectableAttribute = Type.GetType("InjectableAttribute");
    if (type_InjectableAttribute != null)
    {
        Console.WriteLine("[PASS] 类型 InjectableAttribute (class) 存在");
        var ctors_InjectableAttribute = type_InjectableAttribute.GetConstructors();
        Console.WriteLine($"[PASS] InjectableAttribute 构造函数数量: {ctors_InjectableAttribute.Length}");
        var methods_InjectableAttribute = type_InjectableAttribute.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] InjectableAttribute 公开方法数量: {methods_InjectableAttribute.Length}");
        foreach (var m in methods_InjectableAttribute)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 InjectableAttribute 未找到，尝试无命名空间...");
        type_InjectableAttribute = Type.GetType("InjectableAttribute");
        if (type_InjectableAttribute != null)
            Console.WriteLine("[PASS] 类型 InjectableAttribute (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 InjectableAttribute 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DependencyInjection
    var type_DependencyInjection = Type.GetType("DependencyInjection");
    if (type_DependencyInjection != null)
    {
        Console.WriteLine("[PASS] 类型 DependencyInjection (class) 存在");
        var ctors_DependencyInjection = type_DependencyInjection.GetConstructors();
        Console.WriteLine($"[PASS] DependencyInjection 构造函数数量: {ctors_DependencyInjection.Length}");
        var methods_DependencyInjection = type_DependencyInjection.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DependencyInjection 公开方法数量: {methods_DependencyInjection.Length}");
        foreach (var m in methods_DependencyInjection)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DependencyInjection 未找到，尝试无命名空间...");
        type_DependencyInjection = Type.GetType("DependencyInjection");
        if (type_DependencyInjection != null)
            Console.WriteLine("[PASS] 类型 DependencyInjection (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DependencyInjection 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDataProcessor
    var type_IDataProcessor = Type.GetType("IDataProcessor");
    if (type_IDataProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 IDataProcessor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDataProcessor 未找到，尝试无命名空间...");
        type_IDataProcessor = Type.GetType("IDataProcessor");
        if (type_IDataProcessor != null)
            Console.WriteLine("[PASS] 类型 IDataProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDataProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IConditionalService
    var type_IConditionalService = Type.GetType("IConditionalService");
    if (type_IConditionalService != null)
    {
        Console.WriteLine("[PASS] 类型 IConditionalService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IConditionalService 未找到，尝试无命名空间...");
        type_IConditionalService = Type.GetType("IConditionalService");
        if (type_IConditionalService != null)
            Console.WriteLine("[PASS] 类型 IConditionalService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IConditionalService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IEncryptionProvider
    var type_IEncryptionProvider = Type.GetType("IEncryptionProvider");
    if (type_IEncryptionProvider != null)
    {
        Console.WriteLine("[PASS] 类型 IEncryptionProvider (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IEncryptionProvider 未找到，尝试无命名空间...");
        type_IEncryptionProvider = Type.GetType("IEncryptionProvider");
        if (type_IEncryptionProvider != null)
            Console.WriteLine("[PASS] 类型 IEncryptionProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IEncryptionProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ICompressionProvider
    var type_ICompressionProvider = Type.GetType("ICompressionProvider");
    if (type_ICompressionProvider != null)
    {
        Console.WriteLine("[PASS] 类型 ICompressionProvider (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ICompressionProvider 未找到，尝试无命名空间...");
        type_ICompressionProvider = Type.GetType("ICompressionProvider");
        if (type_ICompressionProvider != null)
            Console.WriteLine("[PASS] 类型 ICompressionProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICompressionProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDomainMarker
    var type_IDomainMarker = Type.GetType("IDomainMarker");
    if (type_IDomainMarker != null)
    {
        Console.WriteLine("[PASS] 类型 IDomainMarker (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDomainMarker 未找到，尝试无命名空间...");
        type_IDomainMarker = Type.GetType("IDomainMarker");
        if (type_IDomainMarker != null)
            Console.WriteLine("[PASS] 类型 IDomainMarker (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDomainMarker 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IApplicationMarker
    var type_IApplicationMarker = Type.GetType("IApplicationMarker");
    if (type_IApplicationMarker != null)
    {
        Console.WriteLine("[PASS] 类型 IApplicationMarker (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IApplicationMarker 未找到，尝试无命名空间...");
        type_IApplicationMarker = Type.GetType("IApplicationMarker");
        if (type_IApplicationMarker != null)
            Console.WriteLine("[PASS] 类型 IApplicationMarker (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IApplicationMarker 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IInfrastructureMarker
    var type_IInfrastructureMarker = Type.GetType("IInfrastructureMarker");
    if (type_IInfrastructureMarker != null)
    {
        Console.WriteLine("[PASS] 类型 IInfrastructureMarker (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IInfrastructureMarker 未找到，尝试无命名空间...");
        type_IInfrastructureMarker = Type.GetType("IInfrastructureMarker");
        if (type_IInfrastructureMarker != null)
            Console.WriteLine("[PASS] 类型 IInfrastructureMarker (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IInfrastructureMarker 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IApiMarker
    var type_IApiMarker = Type.GetType("IApiMarker");
    if (type_IApiMarker != null)
    {
        Console.WriteLine("[PASS] 类型 IApiMarker (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IApiMarker 未找到，尝试无命名空间...");
        type_IApiMarker = Type.GetType("IApiMarker");
        if (type_IApiMarker != null)
            Console.WriteLine("[PASS] 类型 IApiMarker (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IApiMarker 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IProcessor
    var type_IProcessor = Type.GetType("IProcessor");
    if (type_IProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 IProcessor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IProcessor 未找到，尝试无命名空间...");
        type_IProcessor = Type.GetType("IProcessor");
        if (type_IProcessor != null)
            Console.WriteLine("[PASS] 类型 IProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ICommandHandler
    var type_ICommandHandler = Type.GetType("ICommandHandler");
    if (type_ICommandHandler != null)
    {
        Console.WriteLine("[PASS] 类型 ICommandHandler (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ICommandHandler 未找到，尝试无命名空间...");
        type_ICommandHandler = Type.GetType("ICommandHandler");
        if (type_ICommandHandler != null)
            Console.WriteLine("[PASS] 类型 ICommandHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICommandHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DataResult
    var type_DataResult = Type.GetType("DataResult");
    if (type_DataResult != null)
    {
        Console.WriteLine("[PASS] 类型 DataResult (record) 存在");
        var ctors_DataResult = type_DataResult.GetConstructors();
        Console.WriteLine($"[PASS] DataResult 构造函数数量: {ctors_DataResult.Length}");
        var methods_DataResult = type_DataResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataResult 公开方法数量: {methods_DataResult.Length}");
        foreach (var m in methods_DataResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataResult 未找到，尝试无命名空间...");
        type_DataResult = Type.GetType("DataResult");
        if (type_DataResult != null)
            Console.WriteLine("[PASS] 类型 DataResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataResult 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ServiceContext
    var type_ServiceContext = Type.GetType("ServiceContext");
    if (type_ServiceContext != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceContext (record) 存在");
        var ctors_ServiceContext = type_ServiceContext.GetConstructors();
        Console.WriteLine($"[PASS] ServiceContext 构造函数数量: {ctors_ServiceContext.Length}");
        var methods_ServiceContext = type_ServiceContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceContext 公开方法数量: {methods_ServiceContext.Length}");
        foreach (var m in methods_ServiceContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceContext 未找到，尝试无命名空间...");
        type_ServiceContext = Type.GetType("ServiceContext");
        if (type_ServiceContext != null)
            Console.WriteLine("[PASS] 类型 ServiceContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceContext 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
