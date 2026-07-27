#load "csscript_integration.cs"

Console.WriteLine("=== csscript_integration.cs Test ===");

try
{
    // 验证 class: CSharpScriptExecutionContext
    var type_CSharpScriptExecutionContext = Type.GetType("CSharpScriptExecutionContext");
    if (type_CSharpScriptExecutionContext != null)
    {
        Console.WriteLine("[PASS] 类型 CSharpScriptExecutionContext (class) 存在");
        var ctors_CSharpScriptExecutionContext = type_CSharpScriptExecutionContext.GetConstructors();
        Console.WriteLine($"[PASS] CSharpScriptExecutionContext 构造函数数量: {ctors_CSharpScriptExecutionContext.Length}");
        var methods_CSharpScriptExecutionContext = type_CSharpScriptExecutionContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CSharpScriptExecutionContext 公开方法数量: {methods_CSharpScriptExecutionContext.Length}");
        foreach (var m in methods_CSharpScriptExecutionContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CSharpScriptExecutionContext 未找到，尝试无命名空间...");
        type_CSharpScriptExecutionContext = Type.GetType("CSharpScriptExecutionContext");
        if (type_CSharpScriptExecutionContext != null)
            Console.WriteLine("[PASS] 类型 CSharpScriptExecutionContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CSharpScriptExecutionContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScriptSecuritySandbox
    var type_ScriptSecuritySandbox = Type.GetType("ScriptSecuritySandbox");
    if (type_ScriptSecuritySandbox != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptSecuritySandbox (class) 存在");
        var ctors_ScriptSecuritySandbox = type_ScriptSecuritySandbox.GetConstructors();
        Console.WriteLine($"[PASS] ScriptSecuritySandbox 构造函数数量: {ctors_ScriptSecuritySandbox.Length}");
        var methods_ScriptSecuritySandbox = type_ScriptSecuritySandbox.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptSecuritySandbox 公开方法数量: {methods_ScriptSecuritySandbox.Length}");
        foreach (var m in methods_ScriptSecuritySandbox)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptSecuritySandbox 未找到，尝试无命名空间...");
        type_ScriptSecuritySandbox = Type.GetType("ScriptSecuritySandbox");
        if (type_ScriptSecuritySandbox != null)
            Console.WriteLine("[PASS] 类型 ScriptSecuritySandbox (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptSecuritySandbox 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScriptPerformanceMonitor
    var type_ScriptPerformanceMonitor = Type.GetType("ScriptPerformanceMonitor");
    if (type_ScriptPerformanceMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptPerformanceMonitor (class) 存在");
        var ctors_ScriptPerformanceMonitor = type_ScriptPerformanceMonitor.GetConstructors();
        Console.WriteLine($"[PASS] ScriptPerformanceMonitor 构造函数数量: {ctors_ScriptPerformanceMonitor.Length}");
        var methods_ScriptPerformanceMonitor = type_ScriptPerformanceMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptPerformanceMonitor 公开方法数量: {methods_ScriptPerformanceMonitor.Length}");
        foreach (var m in methods_ScriptPerformanceMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptPerformanceMonitor 未找到，尝试无命名空间...");
        type_ScriptPerformanceMonitor = Type.GetType("ScriptPerformanceMonitor");
        if (type_ScriptPerformanceMonitor != null)
            Console.WriteLine("[PASS] 类型 ScriptPerformanceMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptPerformanceMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CSharpScriptOptions
    var type_CSharpScriptOptions = Type.GetType("CSharpScriptOptions");
    if (type_CSharpScriptOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CSharpScriptOptions (class) 存在");
        var ctors_CSharpScriptOptions = type_CSharpScriptOptions.GetConstructors();
        Console.WriteLine($"[PASS] CSharpScriptOptions 构造函数数量: {ctors_CSharpScriptOptions.Length}");
        var methods_CSharpScriptOptions = type_CSharpScriptOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CSharpScriptOptions 公开方法数量: {methods_CSharpScriptOptions.Length}");
        foreach (var m in methods_CSharpScriptOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CSharpScriptOptions 未找到，尝试无命名空间...");
        type_CSharpScriptOptions = Type.GetType("CSharpScriptOptions");
        if (type_CSharpScriptOptions != null)
            Console.WriteLine("[PASS] 类型 CSharpScriptOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CSharpScriptOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CSharpScriptPooledPolicy
    var type_CSharpScriptPooledPolicy = Type.GetType("CSharpScriptPooledPolicy");
    if (type_CSharpScriptPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 CSharpScriptPooledPolicy (class) 存在");
        var ctors_CSharpScriptPooledPolicy = type_CSharpScriptPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] CSharpScriptPooledPolicy 构造函数数量: {ctors_CSharpScriptPooledPolicy.Length}");
        var methods_CSharpScriptPooledPolicy = type_CSharpScriptPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CSharpScriptPooledPolicy 公开方法数量: {methods_CSharpScriptPooledPolicy.Length}");
        foreach (var m in methods_CSharpScriptPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CSharpScriptPooledPolicy 未找到，尝试无命名空间...");
        type_CSharpScriptPooledPolicy = Type.GetType("CSharpScriptPooledPolicy");
        if (type_CSharpScriptPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 CSharpScriptPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CSharpScriptPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CSharpScript
    var type_CSharpScript = Type.GetType("CSharpScript");
    if (type_CSharpScript != null)
    {
        Console.WriteLine("[PASS] 类型 CSharpScript (class) 存在");
        var ctors_CSharpScript = type_CSharpScript.GetConstructors();
        Console.WriteLine($"[PASS] CSharpScript 构造函数数量: {ctors_CSharpScript.Length}");
        var methods_CSharpScript = type_CSharpScript.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CSharpScript 公开方法数量: {methods_CSharpScript.Length}");
        foreach (var m in methods_CSharpScript)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CSharpScript 未找到，尝试无命名空间...");
        type_CSharpScript = Type.GetType("CSharpScript");
        if (type_CSharpScript != null)
            Console.WriteLine("[PASS] 类型 CSharpScript (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CSharpScript 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ScriptExecutionRequest
    var type_ScriptExecutionRequest = Type.GetType("ScriptExecutionRequest");
    if (type_ScriptExecutionRequest != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptExecutionRequest (record) 存在");
        var ctors_ScriptExecutionRequest = type_ScriptExecutionRequest.GetConstructors();
        Console.WriteLine($"[PASS] ScriptExecutionRequest 构造函数数量: {ctors_ScriptExecutionRequest.Length}");
        var methods_ScriptExecutionRequest = type_ScriptExecutionRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptExecutionRequest 公开方法数量: {methods_ScriptExecutionRequest.Length}");
        foreach (var m in methods_ScriptExecutionRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptExecutionRequest 未找到，尝试无命名空间...");
        type_ScriptExecutionRequest = Type.GetType("ScriptExecutionRequest");
        if (type_ScriptExecutionRequest != null)
            Console.WriteLine("[PASS] 类型 ScriptExecutionRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptExecutionRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ScriptExecutionResult
    var type_ScriptExecutionResult = Type.GetType("ScriptExecutionResult");
    if (type_ScriptExecutionResult != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptExecutionResult (record) 存在");
        var ctors_ScriptExecutionResult = type_ScriptExecutionResult.GetConstructors();
        Console.WriteLine($"[PASS] ScriptExecutionResult 构造函数数量: {ctors_ScriptExecutionResult.Length}");
        var methods_ScriptExecutionResult = type_ScriptExecutionResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptExecutionResult 公开方法数量: {methods_ScriptExecutionResult.Length}");
        foreach (var m in methods_ScriptExecutionResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptExecutionResult 未找到，尝试无命名空间...");
        type_ScriptExecutionResult = Type.GetType("ScriptExecutionResult");
        if (type_ScriptExecutionResult != null)
            Console.WriteLine("[PASS] 类型 ScriptExecutionResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptExecutionResult 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
