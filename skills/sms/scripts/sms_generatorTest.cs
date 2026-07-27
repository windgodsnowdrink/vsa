#load "sms_generator.cs"

Console.WriteLine("=== sms_generator Test ===");

try
{
    var t0 = typeof(SmsSkill.Generator.CodeGeneratorService);
    Console.WriteLine($"[PASS] CodeGeneratorService 存在");
    var t1 = typeof(SmsSkill.Generator.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(SmsSkill.Generator.ScrutorUsageExamples);
    Console.WriteLine($"[PASS] ScrutorUsageExamples 存在");
    var t3 = typeof(SmsSkill.Generator.SmsServiceLoggingDecorator);
    Console.WriteLine($"[PASS] SmsServiceLoggingDecorator 存在");
    var t4 = typeof(SmsSkill.Generator.SmsServiceRetryDecorator);
    Console.WriteLine($"[PASS] SmsServiceRetryDecorator 存在");
    var t5 = typeof(SmsSkill.Generator.SmsServiceValidationDecorator);
    Console.WriteLine($"[PASS] SmsServiceValidationDecorator 存在");
    var t6 = typeof(SmsSkill.Generator.ICodeGeneratorService);
    Console.WriteLine($"[PASS] ICodeGeneratorService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(SmsSkill.Generator.I);
    Console.WriteLine($"[PASS] I 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}