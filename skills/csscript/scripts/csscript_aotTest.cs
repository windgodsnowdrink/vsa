#load "csscript_aot.cs"

Console.WriteLine("=== csscript_aot Test ===");

try
{
    var t0 = typeof(Csscript.AOT.CsscriptOptions);
    Console.WriteLine($"[PASS] CsscriptOptions 存在");
    var t1 = typeof(Csscript.AOT.ScriptExecutionRequest);
    Console.WriteLine($"[PASS] ScriptExecutionRequest 存在");
    var t2 = typeof(Csscript.AOT.CsscriptResult);
    Console.WriteLine($"[PASS] CsscriptResult 存在");
    var t3 = typeof(Csscript.AOT.ScriptCompilationResult);
    Console.WriteLine($"[PASS] ScriptCompilationResult 存在");
    var t4 = typeof(Csscript.AOT.CsscriptStatus);
    Console.WriteLine($"[PASS] CsscriptStatus 存在");
    var t5 = typeof(Csscript.AOT.CsscriptService);
    Console.WriteLine($"[PASS] CsscriptService 存在");
    var t6 = typeof(Csscript.AOT.CsscriptAotEngine);
    Console.WriteLine($"[PASS] CsscriptAotEngine 存在");
    var t7 = typeof(Csscript.AOT.ICsscriptService);
    Console.WriteLine($"[PASS] ICsscriptService 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}