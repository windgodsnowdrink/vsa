#load "scrutor_demo.cs"

Console.WriteLine("=== scrutor_demo Test ===");

try
{
    var t0 = typeof(Util.ScrutorDemo.User);
    Console.WriteLine($"[PASS] User 存在");
    var t1 = typeof(Util.ScrutorDemo.Product);
    Console.WriteLine($"[PASS] Product 存在");
    var t2 = typeof(Util.ScrutorDemo.UserRepository);
    Console.WriteLine($"[PASS] UserRepository 存在");
    var t3 = typeof(Util.ScrutorDemo.ProductRepository);
    Console.WriteLine($"[PASS] ProductRepository 存在");
    var t4 = typeof(Util.ScrutorDemo.UserService);
    Console.WriteLine($"[PASS] UserService 存在");
    var t5 = typeof(Util.ScrutorDemo.ProductService);
    Console.WriteLine($"[PASS] ProductService 存在");
    var t6 = typeof(Util.ScrutorDemo.LoggerService);
    Console.WriteLine($"[PASS] LoggerService 存在");
    var t7 = typeof(Util.ScrutorDemo.CacheService);
    Console.WriteLine($"[PASS] CacheService 存在");
    var t8 = typeof(Util.ScrutorDemo.LoggingRepositoryDecorator);
    Console.WriteLine($"[PASS] LoggingRepositoryDecorator 存在");
    var t9 = typeof(Util.ScrutorDemo.CachingUserServiceDecorator);
    Console.WriteLine($"[PASS] CachingUserServiceDecorator 存在");
    var t10 = typeof(Util.ScrutorDemo.LoggingServiceDecorator);
    Console.WriteLine($"[PASS] LoggingServiceDecorator 存在");
    var t11 = typeof(Util.ScrutorDemo.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t12 = typeof(Util.ScrutorDemo.IRepository);
    Console.WriteLine($"[PASS] IRepository 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(Util.ScrutorDemo.IUserRepository);
    Console.WriteLine($"[PASS] IUserRepository 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(Util.ScrutorDemo.IProductRepository);
    Console.WriteLine($"[PASS] IProductRepository 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(Util.ScrutorDemo.IService);
    Console.WriteLine($"[PASS] IService 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(Util.ScrutorDemo.IUserService);
    Console.WriteLine($"[PASS] IUserService 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(Util.ScrutorDemo.IProductService);
    Console.WriteLine($"[PASS] IProductService 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(Util.ScrutorDemo.ILoggerService);
    Console.WriteLine($"[PASS] ILoggerService 接口存在 (IsInterface: {t18.IsInterface})");
    var t19 = typeof(Util.ScrutorDemo.ICacheService);
    Console.WriteLine($"[PASS] ICacheService 接口存在 (IsInterface: {t19.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}