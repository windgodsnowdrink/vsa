#load "sourcegenerator_core.cs"

Console.WriteLine("=== sourcegenerator_core Test ===");

try
{
    var t0 = typeof(SourceGeneratorSkill.CodeTemplate);
    Console.WriteLine($"[PASS] CodeTemplate 存在");
    var t1 = typeof(SourceGeneratorSkill.CustomAttribute);
    Console.WriteLine($"[PASS] CustomAttribute 存在");
    var t2 = typeof(SourceGeneratorSkill.CodeAnalysisResult);
    Console.WriteLine($"[PASS] CodeAnalysisResult 存在");
    var t3 = typeof(SourceGeneratorSkill.CodeIssue);
    Console.WriteLine($"[PASS] CodeIssue 存在");
    var t4 = typeof(SourceGeneratorSkill.SourceGeneratorOptions);
    Console.WriteLine($"[PASS] SourceGeneratorOptions 存在");
    var t5 = typeof(SourceGeneratorSkill.TemplateService);
    Console.WriteLine($"[PASS] TemplateService 存在");
    var t6 = typeof(SourceGeneratorSkill.AttributeService);
    Console.WriteLine($"[PASS] AttributeService 存在");
    var t7 = typeof(SourceGeneratorSkill.CodeAnalysisService);
    Console.WriteLine($"[PASS] CodeAnalysisService 存在");
    var t8 = typeof(SourceGeneratorSkill.SourceGeneratorService);
    Console.WriteLine($"[PASS] SourceGeneratorService 存在");
    var t9 = typeof(SourceGeneratorSkill.ISourceGeneratorService);
    Console.WriteLine($"[PASS] ISourceGeneratorService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(SourceGeneratorSkill.ITemplateService);
    Console.WriteLine($"[PASS] ITemplateService 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(SourceGeneratorSkill.IAttributeService);
    Console.WriteLine($"[PASS] IAttributeService 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(SourceGeneratorSkill.ICodeAnalysisService);
    Console.WriteLine($"[PASS] ICodeAnalysisService 接口存在 (IsInterface: {t12.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}