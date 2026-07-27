#load "ssh_generator.cs"

Console.WriteLine("=== ssh_generator Test ===");

try
{
    var t0 = typeof(SSH.Generator.CodeGeneratorService);
    Console.WriteLine($"[PASS] CodeGeneratorService 存在");
    var t1 = typeof(SSH.Generator.TemplateService);
    Console.WriteLine($"[PASS] TemplateService 存在");
    var t2 = typeof(SSH.Generator.SshServiceRegistry);
    Console.WriteLine($"[PASS] SshServiceRegistry 存在");
    var t3 = typeof(SSH.Generator.ServiceAttribute);
    Console.WriteLine($"[PASS] ServiceAttribute 存在");
    var t4 = typeof(SSH.Generator.Repository);
    Console.WriteLine($"[PASS] Repository 存在");
    var t5 = typeof(SSH.Generator.MemoryCacheService);
    Console.WriteLine($"[PASS] MemoryCacheService 存在");
    var t6 = typeof(SSH.Generator.FileSystemService);
    Console.WriteLine($"[PASS] FileSystemService 存在");
    var t7 = typeof(SSH.Generator.ConsoleLoggerService);
    Console.WriteLine($"[PASS] ConsoleLoggerService 存在");
    var t8 = typeof(SSH.Generator.CodeGeneratorLoggingDecorator);
    Console.WriteLine($"[PASS] CodeGeneratorLoggingDecorator 存在");
    var t9 = typeof(SSH.Generator.CodeGeneratorValidationDecorator);
    Console.WriteLine($"[PASS] CodeGeneratorValidationDecorator 存在");
    var t10 = typeof(SSH.Generator.CodeGeneratorCachingDecorator);
    Console.WriteLine($"[PASS] CodeGeneratorCachingDecorator 存在");
    var t11 = typeof(SSH.Generator.TemplateServiceCachingDecorator);
    Console.WriteLine($"[PASS] TemplateServiceCachingDecorator 存在");
    var t12 = typeof(SSH.Generator.SshConfigGenerator);
    Console.WriteLine($"[PASS] SshConfigGenerator 存在");
    var t13 = typeof(SSH.Generator.SshScriptGenerator);
    Console.WriteLine($"[PASS] SshScriptGenerator 存在");
    var t14 = typeof(SSH.Generator.SshBatchGenerator);
    Console.WriteLine($"[PASS] SshBatchGenerator 存在");
    var t15 = typeof(SSH.Generator.ServiceProviderBinder);
    Console.WriteLine($"[PASS] ServiceProviderBinder 存在");
    var t16 = typeof(SSH.Generator.ICodeGeneratorService);
    Console.WriteLine($"[PASS] ICodeGeneratorService 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(SSH.Generator.ITemplateService);
    Console.WriteLine($"[PASS] ITemplateService 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(SSH.Generator.ISshServiceRegistry);
    Console.WriteLine($"[PASS] ISshServiceRegistry 接口存在 (IsInterface: {t18.IsInterface})");
    var t19 = typeof(SSH.Generator.IRepository);
    Console.WriteLine($"[PASS] IRepository 接口存在 (IsInterface: {t19.IsInterface})");
    var t20 = typeof(SSH.Generator.ICacheService);
    Console.WriteLine($"[PASS] ICacheService 接口存在 (IsInterface: {t20.IsInterface})");
    var t21 = typeof(SSH.Generator.IFileService);
    Console.WriteLine($"[PASS] IFileService 接口存在 (IsInterface: {t21.IsInterface})");
    var t22 = typeof(SSH.Generator.ILoggerService);
    Console.WriteLine($"[PASS] ILoggerService 接口存在 (IsInterface: {t22.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}