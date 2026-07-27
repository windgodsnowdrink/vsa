#load "silky_generator.cs"

Console.WriteLine("=== silky_generator Test ===");

try
{
    var t0 = typeof(SilkySkill.CodeGeneratorService);
    Console.WriteLine($"[PASS] CodeGeneratorService 存在");
    var t1 = typeof(SilkySkill.CodeGeneratorCli);
    Console.WriteLine($"[PASS] CodeGeneratorCli 存在");
    var t2 = typeof(SilkySkill.ICodeGeneratorService);
    Console.WriteLine($"[PASS] ICodeGeneratorService 接口存在 (IsInterface: {t2.IsInterface})");
    var t3 = typeof(SilkySkill.I);
    Console.WriteLine($"[PASS] I 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}