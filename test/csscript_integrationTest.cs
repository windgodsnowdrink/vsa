#load "csscript_integration.cs"

Console.WriteLine("=== csscript_integration Test ===");

try
{
    var t0 = typeof(CSharpScriptExecutionContext);
    Console.WriteLine($"[PASS] CSharpScriptExecutionContext 存在");
    var t1 = typeof(ScriptSecuritySandbox);
    Console.WriteLine($"[PASS] ScriptSecuritySandbox 存在");
    var t2 = typeof(ScriptPerformanceMonitor);
    Console.WriteLine($"[PASS] ScriptPerformanceMonitor 存在");
    var t3 = typeof(CSharpScriptOptions);
    Console.WriteLine($"[PASS] CSharpScriptOptions 存在");
    var t4 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t5 = typeof(CSharpScriptPooledPolicy);
    Console.WriteLine($"[PASS] CSharpScriptPooledPolicy 存在");
    var t6 = typeof(CSharpScript);
    Console.WriteLine($"[PASS] CSharpScript 存在");
    var t7 = typeof(ScriptExecutionRequest);
    Console.WriteLine($"[PASS] ScriptExecutionRequest record 存在");
    var t8 = typeof(ScriptExecutionResult);
    Console.WriteLine($"[PASS] ScriptExecutionResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}