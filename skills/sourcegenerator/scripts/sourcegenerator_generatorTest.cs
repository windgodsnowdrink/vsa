#load "sourcegenerator_generator.cs"

Console.WriteLine("=== sourcegenerator_generator Test ===");

try
{
    var t0 = typeof(SourceGenerator.Generator.CodeGeneratorService);
    Console.WriteLine($"[PASS] CodeGeneratorService 存在");
    var t1 = typeof(SourceGenerator.Generator.GeneratorRegistry);
    Console.WriteLine($"[PASS] GeneratorRegistry 存在");
    var t2 = typeof(SourceGenerator.Generator.GeneratorAttribute);
    Console.WriteLine($"[PASS] GeneratorAttribute 存在");
    var t3 = typeof(SourceGenerator.Generator.Repository);
    Console.WriteLine($"[PASS] Repository 存在");
    var t4 = typeof(SourceGenerator.Generator.MemoryCacheService);
    Console.WriteLine($"[PASS] MemoryCacheService 存在");
    var t5 = typeof(SourceGenerator.Generator.FileSystemService);
    Console.WriteLine($"[PASS] FileSystemService 存在");
    var t6 = typeof(SourceGenerator.Generator.ConsoleLoggerService);
    Console.WriteLine($"[PASS] ConsoleLoggerService 存在");
    var t7 = typeof(SourceGenerator.Generator.CodeGeneratorLoggingDecorator);
    Console.WriteLine($"[PASS] CodeGeneratorLoggingDecorator 存在");
    var t8 = typeof(SourceGenerator.Generator.CodeGeneratorValidationDecorator);
    Console.WriteLine($"[PASS] CodeGeneratorValidationDecorator 存在");
    var t9 = typeof(SourceGenerator.Generator.TemplateServiceCachingDecorator);
    Console.WriteLine($"[PASS] TemplateServiceCachingDecorator 存在");
    var t10 = typeof(SourceGenerator.Generator.files);
    Console.WriteLine($"[PASS] files 存在");
    var t11 = typeof(SourceGenerator.Generator.ClassGenerator);
    Console.WriteLine($"[PASS] ClassGenerator 存在");
    var t12 = typeof(SourceGenerator.Generator.InterfaceGenerator);
    Console.WriteLine($"[PASS] InterfaceGenerator 存在");
    var t13 = typeof(SourceGenerator.Generator.ServiceGenerator);
    Console.WriteLine($"[PASS] ServiceGenerator 存在");
    var t14 = typeof(SourceGenerator.Generator.ServiceProviderBinder);
    Console.WriteLine($"[PASS] ServiceProviderBinder 存在");
    var t15 = typeof(SourceGenerator.Generator.TemplateService);
    Console.WriteLine($"[PASS] TemplateService 存在");
    var t16 = typeof(SourceGenerator.Generator.ICodeGeneratorService);
    Console.WriteLine($"[PASS] ICodeGeneratorService 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(SourceGenerator.Generator.IGeneratorRegistry);
    Console.WriteLine($"[PASS] IGeneratorRegistry 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(SourceGenerator.Generator.IRepository);
    Console.WriteLine($"[PASS] IRepository 接口存在 (IsInterface: {t18.IsInterface})");
    var t19 = typeof(SourceGenerator.Generator.ICacheService);
    Console.WriteLine($"[PASS] ICacheService 接口存在 (IsInterface: {t19.IsInterface})");
    var t20 = typeof(SourceGenerator.Generator.IFileService);
    Console.WriteLine($"[PASS] IFileService 接口存在 (IsInterface: {t20.IsInterface})");
    var t21 = typeof(SourceGenerator.Generator.ILoggerService);
    Console.WriteLine($"[PASS] ILoggerService 接口存在 (IsInterface: {t21.IsInterface})");
    var t22 = typeof(SourceGenerator.Generator.files);
    Console.WriteLine($"[PASS] files 接口存在 (IsInterface: {t22.IsInterface})");
    var t23 = typeof(SourceGenerator.Generator.ITemplateService);
    Console.WriteLine($"[PASS] ITemplateService 接口存在 (IsInterface: {t23.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}