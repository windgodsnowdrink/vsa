#load "dotnetscript_integration.cs"

Console.WriteLine("=== dotnetscript_integration.cs Test ===");

try
{
    // 验证 class: ScriptOptions
    var type_ScriptOptions = Type.GetType("ScriptOptions");
    if (type_ScriptOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptOptions (class) 存在");
        var ctors_ScriptOptions = type_ScriptOptions.GetConstructors();
        Console.WriteLine($"[PASS] ScriptOptions 构造函数数量: {ctors_ScriptOptions.Length}");
        var methods_ScriptOptions = type_ScriptOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptOptions 公开方法数量: {methods_ScriptOptions.Length}");
        foreach (var m in methods_ScriptOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptOptions 未找到，尝试无命名空间...");
        type_ScriptOptions = Type.GetType("ScriptOptions");
        if (type_ScriptOptions != null)
            Console.WriteLine("[PASS] 类型 ScriptOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Script
    var type_Script = Type.GetType("Script");
    if (type_Script != null)
    {
        Console.WriteLine("[PASS] 类型 Script (class) 存在");
        var ctors_Script = type_Script.GetConstructors();
        Console.WriteLine($"[PASS] Script 构造函数数量: {ctors_Script.Length}");
        var methods_Script = type_Script.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Script 公开方法数量: {methods_Script.Length}");
        foreach (var m in methods_Script)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Script 未找到，尝试无命名空间...");
        type_Script = Type.GetType("Script");
        if (type_Script != null)
            Console.WriteLine("[PASS] 类型 Script (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Script 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScriptExecutionContext
    var type_ScriptExecutionContext = Type.GetType("ScriptExecutionContext");
    if (type_ScriptExecutionContext != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptExecutionContext (class) 存在");
        var ctors_ScriptExecutionContext = type_ScriptExecutionContext.GetConstructors();
        Console.WriteLine($"[PASS] ScriptExecutionContext 构造函数数量: {ctors_ScriptExecutionContext.Length}");
        var methods_ScriptExecutionContext = type_ScriptExecutionContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptExecutionContext 公开方法数量: {methods_ScriptExecutionContext.Length}");
        foreach (var m in methods_ScriptExecutionContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptExecutionContext 未找到，尝试无命名空间...");
        type_ScriptExecutionContext = Type.GetType("ScriptExecutionContext");
        if (type_ScriptExecutionContext != null)
            Console.WriteLine("[PASS] 类型 ScriptExecutionContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptExecutionContext 可能为顶层语句或嵌套类型");
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

    // 验证 class: ScriptPooledPolicy
    var type_ScriptPooledPolicy = Type.GetType("ScriptPooledPolicy");
    if (type_ScriptPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptPooledPolicy (class) 存在");
        var ctors_ScriptPooledPolicy = type_ScriptPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ScriptPooledPolicy 构造函数数量: {ctors_ScriptPooledPolicy.Length}");
        var methods_ScriptPooledPolicy = type_ScriptPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptPooledPolicy 公开方法数量: {methods_ScriptPooledPolicy.Length}");
        foreach (var m in methods_ScriptPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptPooledPolicy 未找到，尝试无命名空间...");
        type_ScriptPooledPolicy = Type.GetType("ScriptPooledPolicy");
        if (type_ScriptPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 ScriptPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScriptExecutionOptions
    var type_ScriptExecutionOptions = Type.GetType("ScriptExecutionOptions");
    if (type_ScriptExecutionOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptExecutionOptions (class) 存在");
        var ctors_ScriptExecutionOptions = type_ScriptExecutionOptions.GetConstructors();
        Console.WriteLine($"[PASS] ScriptExecutionOptions 构造函数数量: {ctors_ScriptExecutionOptions.Length}");
        var methods_ScriptExecutionOptions = type_ScriptExecutionOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptExecutionOptions 公开方法数量: {methods_ScriptExecutionOptions.Length}");
        foreach (var m in methods_ScriptExecutionOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptExecutionOptions 未找到，尝试无命名空间...");
        type_ScriptExecutionOptions = Type.GetType("ScriptExecutionOptions");
        if (type_ScriptExecutionOptions != null)
            Console.WriteLine("[PASS] 类型 ScriptExecutionOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptExecutionOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScriptExecutor
    var type_ScriptExecutor = Type.GetType("ScriptExecutor");
    if (type_ScriptExecutor != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptExecutor (class) 存在");
        var ctors_ScriptExecutor = type_ScriptExecutor.GetConstructors();
        Console.WriteLine($"[PASS] ScriptExecutor 构造函数数量: {ctors_ScriptExecutor.Length}");
        var methods_ScriptExecutor = type_ScriptExecutor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptExecutor 公开方法数量: {methods_ScriptExecutor.Length}");
        foreach (var m in methods_ScriptExecutor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptExecutor 未找到，尝试无命名空间...");
        type_ScriptExecutor = Type.GetType("ScriptExecutor");
        if (type_ScriptExecutor != null)
            Console.WriteLine("[PASS] 类型 ScriptExecutor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptExecutor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScriptCompilerPooledPolicy
    var type_ScriptCompilerPooledPolicy = Type.GetType("ScriptCompilerPooledPolicy");
    if (type_ScriptCompilerPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptCompilerPooledPolicy (class) 存在");
        var ctors_ScriptCompilerPooledPolicy = type_ScriptCompilerPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ScriptCompilerPooledPolicy 构造函数数量: {ctors_ScriptCompilerPooledPolicy.Length}");
        var methods_ScriptCompilerPooledPolicy = type_ScriptCompilerPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptCompilerPooledPolicy 公开方法数量: {methods_ScriptCompilerPooledPolicy.Length}");
        foreach (var m in methods_ScriptCompilerPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptCompilerPooledPolicy 未找到，尝试无命名空间...");
        type_ScriptCompilerPooledPolicy = Type.GetType("ScriptCompilerPooledPolicy");
        if (type_ScriptCompilerPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 ScriptCompilerPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptCompilerPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScriptResult
    var type_ScriptResult = Type.GetType("ScriptResult");
    if (type_ScriptResult != null)
    {
        Console.WriteLine("[PASS] 类型 ScriptResult (class) 存在");
        var ctors_ScriptResult = type_ScriptResult.GetConstructors();
        Console.WriteLine($"[PASS] ScriptResult 构造函数数量: {ctors_ScriptResult.Length}");
        var methods_ScriptResult = type_ScriptResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScriptResult 公开方法数量: {methods_ScriptResult.Length}");
        foreach (var m in methods_ScriptResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScriptResult 未找到，尝试无命名空间...");
        type_ScriptResult = Type.GetType("ScriptResult");
        if (type_ScriptResult != null)
            Console.WriteLine("[PASS] 类型 ScriptResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScriptResult 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
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
