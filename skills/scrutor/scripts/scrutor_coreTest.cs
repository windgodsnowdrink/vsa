#load "scrutor_core.cs"

Console.WriteLine("=== scrutor_core Test ===");

try
{
    var t0 = typeof(Scrutor.Core.AssemblyScanner);
    Console.WriteLine($"[PASS] AssemblyScanner 存在");
    var t1 = typeof(Scrutor.Core.ServiceDecorator);
    Console.WriteLine($"[PASS] ServiceDecorator 存在");
    var t2 = typeof(Scrutor.Core.ServiceRegistry);
    Console.WriteLine($"[PASS] ServiceRegistry 存在");
    var t3 = typeof(Scrutor.Core.IAssemblyScanner);
    Console.WriteLine($"[PASS] IAssemblyScanner 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(Scrutor.Core.IServiceDecorator);
    Console.WriteLine($"[PASS] IServiceDecorator 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(Scrutor.Core.IServiceRegistry);
    Console.WriteLine($"[PASS] IServiceRegistry 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(Scrutor.Core.in);
    Console.WriteLine($"[PASS] in 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}