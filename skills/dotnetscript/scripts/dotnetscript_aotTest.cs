#load "dotnetscript_aot.cs"

Console.WriteLine("=== dotnetscript_aot Test ===");

try
{
    var t0 = typeof(DotNetScript.AOT.DotNetScriptOptions);
    Console.WriteLine($"[PASS] DotNetScriptOptions 存在");
    var t1 = typeof(DotNetScript.AOT.DotNetScriptCommandResult);
    Console.WriteLine($"[PASS] DotNetScriptCommandResult 存在");
    var t2 = typeof(DotNetScript.AOT.DotNetScriptService);
    Console.WriteLine($"[PASS] DotNetScriptService 存在");
    var t3 = typeof(DotNetScript.AOT.DotNetScriptAotEngine);
    Console.WriteLine($"[PASS] DotNetScriptAotEngine 存在");
    var t4 = typeof(DotNetScript.AOT.DotNetScriptServiceExtensions);
    Console.WriteLine($"[PASS] DotNetScriptServiceExtensions 存在");
    var t5 = typeof(DotNetScript.AOT.IDotNetScriptService);
    Console.WriteLine($"[PASS] IDotNetScriptService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(DotNetScript.AOT.DotNetScriptCommandType);
    Console.WriteLine($"[PASS] DotNetScriptCommandType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}