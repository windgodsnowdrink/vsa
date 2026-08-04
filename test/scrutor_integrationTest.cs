#load "scrutor_integration.cs"

Console.WriteLine("=== scrutor_integration Test ===");

try
{
    var t0 = typeof(CoreDataProcessor);
    Console.WriteLine($"[PASS] CoreDataProcessor 存在");
    var t1 = typeof(EncryptionDecorator);
    Console.WriteLine($"[PASS] EncryptionDecorator 存在");
    var t2 = typeof(CompressionDecorator);
    Console.WriteLine($"[PASS] CompressionDecorator 存在");
    var t3 = typeof(DevelopmentService);
    Console.WriteLine($"[PASS] DevelopmentService 存在");
    var t4 = typeof(ProductionService);
    Console.WriteLine($"[PASS] ProductionService 存在");
    var t5 = typeof(FastService);
    Console.WriteLine($"[PASS] FastService 存在");
    var t6 = typeof(ReliableService);
    Console.WriteLine($"[PASS] ReliableService 存在");
    var t7 = typeof(AdvancedDependencyInjection);
    Console.WriteLine($"[PASS] AdvancedDependencyInjection 存在");
    var t8 = typeof(where);
    Console.WriteLine($"[PASS] where 存在");
    var t9 = typeof(AesEncryptionProvider);
    Console.WriteLine($"[PASS] AesEncryptionProvider 存在");
    var t10 = typeof(GzipCompressionProvider);
    Console.WriteLine($"[PASS] GzipCompressionProvider 存在");
    var t11 = typeof(NamedServiceOptions);
    Console.WriteLine($"[PASS] NamedServiceOptions 存在");
    var t12 = typeof(NamedServiceFactory);
    Console.WriteLine($"[PASS] NamedServiceFactory 存在");
    var t13 = typeof(CoreProcessor);
    Console.WriteLine($"[PASS] CoreProcessor 存在");
    var t14 = typeof(LoggingProcessor);
    Console.WriteLine($"[PASS] LoggingProcessor 存在");
    var t15 = typeof(ProcessingOptions);
    Console.WriteLine($"[PASS] ProcessingOptions 存在");
    var t16 = typeof(CreateUserCommand);
    Console.WriteLine($"[PASS] CreateUserCommand 存在");
    var t17 = typeof(CreateUserHandler);
    Console.WriteLine($"[PASS] CreateUserHandler 存在");
    var t18 = typeof(DomainService);
    Console.WriteLine($"[PASS] DomainService 存在");
    var t19 = typeof(AppService);
    Console.WriteLine($"[PASS] AppService 存在");
    var t20 = typeof(Repository);
    Console.WriteLine($"[PASS] Repository 存在");
    var t21 = typeof(ApiController);
    Console.WriteLine($"[PASS] ApiController 存在");
    var t22 = typeof(InjectableAttribute);
    Console.WriteLine($"[PASS] InjectableAttribute 存在");
    var t23 = typeof(DependencyInjection);
    Console.WriteLine($"[PASS] DependencyInjection 存在");
    var t24 = typeof(IDataProcessor);
    Console.WriteLine($"[PASS] IDataProcessor 接口存在 (IsInterface: {t24.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}