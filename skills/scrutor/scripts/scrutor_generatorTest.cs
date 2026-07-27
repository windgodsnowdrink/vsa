#load "scrutor_generator.cs"

Console.WriteLine("=== scrutor_generator Test ===");

try
{
    var t0 = typeof(Scrutor.Generator.TemplateInfo);
    Console.WriteLine($"[PASS] TemplateInfo 存在");
    var t1 = typeof(Scrutor.Generator.CodeGenerator);
    Console.WriteLine($"[PASS] CodeGenerator 存在");
    var t2 = typeof(Scrutor.Generator.TemplateManager);
    Console.WriteLine($"[PASS] TemplateManager 存在");
    var t3 = typeof(Scrutor.Generator.DependencyInjectionCodeGenerator);
    Console.WriteLine($"[PASS] DependencyInjectionCodeGenerator 存在");
    var t4 = typeof(Scrutor.Generator.ServiceCollectionCodeGenerator);
    Console.WriteLine($"[PASS] ServiceCollectionCodeGenerator 存在");
    var t5 = typeof(Scrutor.Generator.WebApplicationCodeGenerator);
    Console.WriteLine($"[PASS] WebApplicationCodeGenerator 存在");
    var t6 = typeof(Scrutor.Generator.ICodeGenerator);
    Console.WriteLine($"[PASS] ICodeGenerator 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(Scrutor.Generator.ITemplateManager);
    Console.WriteLine($"[PASS] ITemplateManager 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(Scrutor.Generator.ICodeGeneratorStrategy);
    Console.WriteLine($"[PASS] ICodeGeneratorStrategy 接口存在 (IsInterface: {t8.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}