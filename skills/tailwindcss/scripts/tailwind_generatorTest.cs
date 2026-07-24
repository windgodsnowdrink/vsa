#load "tailwind_generator.cs"

Console.WriteLine("=== tailwind_generator Test ===");

try
{
    var t0 = typeof(TailwindCSS.Generator.CodeGeneratorService);
    Console.WriteLine($"[PASS] CodeGeneratorService 存在");
    var t1 = typeof(TailwindCSS.Generator.TemplateService);
    Console.WriteLine($"[PASS] TemplateService 存在");
    var t2 = typeof(TailwindCSS.Generator.ScrutorDemoService);
    Console.WriteLine($"[PASS] ScrutorDemoService 存在");
    var t3 = typeof(TailwindCSS.Generator.CodeGeneratorLoggingDecorator);
    Console.WriteLine($"[PASS] CodeGeneratorLoggingDecorator 存在");
    var t4 = typeof(TailwindCSS.Generator.CodeGeneratorCachingDecorator);
    Console.WriteLine($"[PASS] CodeGeneratorCachingDecorator 存在");
    var t5 = typeof(TailwindCSS.Generator.ICodeGeneratorService);
    Console.WriteLine($"[PASS] ICodeGeneratorService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(TailwindCSS.Generator.ITemplateService);
    Console.WriteLine($"[PASS] ITemplateService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(TailwindCSS.Generator.IScrutorDemoService);
    Console.WriteLine($"[PASS] IScrutorDemoService 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}