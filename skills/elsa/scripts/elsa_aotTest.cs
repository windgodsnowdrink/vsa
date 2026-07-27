#load "elsa_aot.cs"

Console.WriteLine("=== elsa_aot Test ===");

try
{
    var t0 = typeof(Elsa.AOT.ElsaOptions);
    Console.WriteLine($"[PASS] ElsaOptions 存在");
    var t1 = typeof(Elsa.AOT.WorkflowInstance);
    Console.WriteLine($"[PASS] WorkflowInstance 存在");
    var t2 = typeof(Elsa.AOT.ElsaCommandResult);
    Console.WriteLine($"[PASS] ElsaCommandResult 存在");
    var t3 = typeof(Elsa.AOT.ElsaService);
    Console.WriteLine($"[PASS] ElsaService 存在");
    var t4 = typeof(Elsa.AOT.ElsaAotEngine);
    Console.WriteLine($"[PASS] ElsaAotEngine 存在");
    var t5 = typeof(Elsa.AOT.IElsaService);
    Console.WriteLine($"[PASS] IElsaService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(Elsa.AOT.ElsaCommandType);
    Console.WriteLine($"[PASS] ElsaCommandType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}