#load "silky_core.cs"

Console.WriteLine("=== silky_core Test ===");

try
{
    var t0 = typeof(SilkySkill.ServiceInfo);
    Console.WriteLine($"[PASS] ServiceInfo 存在");
    var t1 = typeof(SilkySkill.RegistryService);
    Console.WriteLine($"[PASS] RegistryService 存在");
    var t2 = typeof(SilkySkill.ProjectCreatorService);
    Console.WriteLine($"[PASS] ProjectCreatorService 存在");
    var t3 = typeof(SilkySkill.ProjectBuilderService);
    Console.WriteLine($"[PASS] ProjectBuilderService 存在");
    var t4 = typeof(SilkySkill.ProjectRunnerService);
    Console.WriteLine($"[PASS] ProjectRunnerService 存在");
    var t5 = typeof(SilkySkill.SilkyCli);
    Console.WriteLine($"[PASS] SilkyCli 存在");
    var t6 = typeof(SilkySkill.IRegistryService);
    Console.WriteLine($"[PASS] IRegistryService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(SilkySkill.IProjectCreatorService);
    Console.WriteLine($"[PASS] IProjectCreatorService 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(SilkySkill.I);
    Console.WriteLine($"[PASS] I 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(SilkySkill.IProjectBuilderService);
    Console.WriteLine($"[PASS] IProjectBuilderService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(SilkySkill.IProjectRunnerService);
    Console.WriteLine($"[PASS] IProjectRunnerService 接口存在 (IsInterface: {t10.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}