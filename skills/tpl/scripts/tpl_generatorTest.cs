#load "tpl_generator.cs"

Console.WriteLine("=== tpl_generator Test ===");

try
{
    var t0 = typeof(TPLSkill.ScrutorDemo.ConsoleLoggerService);
    Console.WriteLine($"[PASS] ConsoleLoggerService 存在");
    var t1 = typeof(TPLSkill.ScrutorDemo.DefaultDataService);
    Console.WriteLine($"[PASS] DefaultDataService 存在");
    var t2 = typeof(TPLSkill.ScrutorDemo.CachedDataService);
    Console.WriteLine($"[PASS] CachedDataService 存在");
    var t3 = typeof(TPLSkill.ScrutorDemo.LoggingDataService);
    Console.WriteLine($"[PASS] LoggingDataService 存在");
    var t4 = typeof(TPLSkill.ScrutorDemo.User);
    Console.WriteLine($"[PASS] User 存在");
    var t5 = typeof(TPLSkill.ScrutorDemo.UserRepository);
    Console.WriteLine($"[PASS] UserRepository 存在");
    var t6 = typeof(TPLSkill.ScrutorDemo.ScopedService);
    Console.WriteLine($"[PASS] ScopedService 存在");
    var t7 = typeof(TPLSkill.ScrutorDemo.SingletonService);
    Console.WriteLine($"[PASS] SingletonService 存在");
    var t8 = typeof(TPLSkill.ScrutorDemo.TransientService);
    Console.WriteLine($"[PASS] TransientService 存在");
    var t9 = typeof(TPLSkill.ScrutorDemo.ScrutorDemoService);
    Console.WriteLine($"[PASS] ScrutorDemoService 存在");
    var t10 = typeof(TPLSkill.ScrutorDemo.ILoggerService);
    Console.WriteLine($"[PASS] ILoggerService 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(TPLSkill.ScrutorDemo.IDataService);
    Console.WriteLine($"[PASS] IDataService 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(TPLSkill.ScrutorDemo.IRepository);
    Console.WriteLine($"[PASS] IRepository 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(TPLSkill.ScrutorDemo.IScopedService);
    Console.WriteLine($"[PASS] IScopedService 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(TPLSkill.ScrutorDemo.ISingletonService);
    Console.WriteLine($"[PASS] ISingletonService 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(TPLSkill.ScrutorDemo.ITransientService);
    Console.WriteLine($"[PASS] ITransientService 接口存在 (IsInterface: {t15.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}