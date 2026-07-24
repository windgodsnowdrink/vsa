#load "dotnetscript_integration.cs"

Console.WriteLine("=== dotnetscript_integration Test ===");

try
{
    var t0 = typeof(ScriptOptions);
    Console.WriteLine($"[PASS] ScriptOptions 存在");
    var t1 = typeof(Script);
    Console.WriteLine($"[PASS] Script 存在");
    var t2 = typeof(ScriptExecutionContext);
    Console.WriteLine($"[PASS] ScriptExecutionContext 存在");
    var t3 = typeof(ScriptSecuritySandbox);
    Console.WriteLine($"[PASS] ScriptSecuritySandbox 存在");
    var t4 = typeof(ScriptPerformanceMonitor);
    Console.WriteLine($"[PASS] ScriptPerformanceMonitor 存在");
    var t5 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t6 = typeof(ScriptPooledPolicy);
    Console.WriteLine($"[PASS] ScriptPooledPolicy 存在");
    var t7 = typeof(ScriptExecutionOptions);
    Console.WriteLine($"[PASS] ScriptExecutionOptions 存在");
    var t8 = typeof(ScriptExecutor);
    Console.WriteLine($"[PASS] ScriptExecutor 存在");
    var t9 = typeof(ScriptCompilerPooledPolicy);
    Console.WriteLine($"[PASS] ScriptCompilerPooledPolicy 存在");
    var t10 = typeof(ScriptResult);
    Console.WriteLine($"[PASS] ScriptResult 存在");
    var t11 = typeof(ScriptExecutionRequest);
    Console.WriteLine($"[PASS] ScriptExecutionRequest record 存在");
    var t12 = typeof(ScriptExecutionResult);
    Console.WriteLine($"[PASS] ScriptExecutionResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}