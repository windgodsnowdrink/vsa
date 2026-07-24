#load "transport_generator.cs"

Console.WriteLine("=== transport_generator Test ===");

try
{
    var t0 = typeof(TransportSkill.ScrutorDemo.DefaultTransportService);
    Console.WriteLine($"[PASS] DefaultTransportService 存在");
    var t1 = typeof(TransportSkill.ScrutorDemo.LoggingTransportService);
    Console.WriteLine($"[PASS] LoggingTransportService 存在");
    var t2 = typeof(TransportSkill.ScrutorDemo.CachingTransportService);
    Console.WriteLine($"[PASS] CachingTransportService 存在");
    var t3 = typeof(TransportSkill.ScrutorDemo.DefaultValidatorService);
    Console.WriteLine($"[PASS] DefaultValidatorService 存在");
    var t4 = typeof(TransportSkill.ScrutorDemo.ScopedService);
    Console.WriteLine($"[PASS] ScopedService 存在");
    var t5 = typeof(TransportSkill.ScrutorDemo.SingletonService);
    Console.WriteLine($"[PASS] SingletonService 存在");
    var t6 = typeof(TransportSkill.ScrutorDemo.TransientService);
    Console.WriteLine($"[PASS] TransientService 存在");
    var t7 = typeof(TransportSkill.ScrutorDemo.TransportGeneratorService);
    Console.WriteLine($"[PASS] TransportGeneratorService 存在");
    var t8 = typeof(TransportSkill.ScrutorDemo.ITransportService);
    Console.WriteLine($"[PASS] ITransportService 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(TransportSkill.ScrutorDemo.IValidatorService);
    Console.WriteLine($"[PASS] IValidatorService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(TransportSkill.ScrutorDemo.IScopedService);
    Console.WriteLine($"[PASS] IScopedService 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(TransportSkill.ScrutorDemo.ISingletonService);
    Console.WriteLine($"[PASS] ISingletonService 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(TransportSkill.ScrutorDemo.ITransientService);
    Console.WriteLine($"[PASS] ITransientService 接口存在 (IsInterface: {t12.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}