#load "mcpserver_arch_integration.cs"

Console.WriteLine("=== mcpserver_arch_integration Test ===");

try
{
    var programType = typeof(App.Program);
    Console.WriteLine("[PASS] Program type found: " + programType.FullName);

    var randomNumberToolsType = typeof(App.RandomNumberTools);
    Console.WriteLine("[PASS] RandomNumberTools class found: " + randomNumberToolsType.FullName);
    var getRandomNumberMethod = randomNumberToolsType.GetMethod("GetRandomNumber");
    Console.WriteLine(getRandomNumberMethod != null ? "[PASS] RandomNumberTools.GetRandomNumber exists" : "[FAIL] GetRandomNumber missing");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}