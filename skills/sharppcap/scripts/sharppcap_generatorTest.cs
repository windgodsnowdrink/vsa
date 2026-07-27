#load "sharppcap_generator.cs"

Console.WriteLine("=== sharppcap_generator Test ===");

try
{
    var t0 = typeof(SharpPcapSkill.CodeGeneratorService);
    Console.WriteLine($"[PASS] CodeGeneratorService 存在");
    var t1 = typeof(SharpPcapSkill.CodeGeneratorCli);
    Console.WriteLine($"[PASS] CodeGeneratorCli 存在");
    var t2 = typeof(SharpPcapSkill.ICodeGeneratorService);
    Console.WriteLine($"[PASS] ICodeGeneratorService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}