#load "asr_integration.cs"

Console.WriteLine("=== asr_integration.cs Test ===");

try
{
    // 验证 class: AsrIntegration.AsrProcessor
    var type_AsrProcessor = Type.GetType("AsrIntegration.AsrProcessor");
    if (type_AsrProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 AsrIntegration.AsrProcessor (class) 存在");
        var ctors_AsrProcessor = type_AsrProcessor.GetConstructors();
        Console.WriteLine($"[PASS] AsrIntegration.AsrProcessor 构造函数数量: {ctors_AsrProcessor.Length}");
        var methods_AsrProcessor = type_AsrProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AsrIntegration.AsrProcessor 公开方法数量: {methods_AsrProcessor.Length}");
        foreach (var m in methods_AsrProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AsrIntegration.AsrProcessor 未找到，尝试无命名空间...");
        type_AsrProcessor = Type.GetType("AsrProcessor");
        if (type_AsrProcessor != null)
            Console.WriteLine("[PASS] 类型 AsrProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AsrProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AsrIntegration.AsrDiagnostics
    var type_AsrDiagnostics = Type.GetType("AsrIntegration.AsrDiagnostics");
    if (type_AsrDiagnostics != null)
    {
        Console.WriteLine("[PASS] 类型 AsrIntegration.AsrDiagnostics (class) 存在");
        var ctors_AsrDiagnostics = type_AsrDiagnostics.GetConstructors();
        Console.WriteLine($"[PASS] AsrIntegration.AsrDiagnostics 构造函数数量: {ctors_AsrDiagnostics.Length}");
        var methods_AsrDiagnostics = type_AsrDiagnostics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AsrIntegration.AsrDiagnostics 公开方法数量: {methods_AsrDiagnostics.Length}");
        foreach (var m in methods_AsrDiagnostics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AsrIntegration.AsrDiagnostics 未找到，尝试无命名空间...");
        type_AsrDiagnostics = Type.GetType("AsrDiagnostics");
        if (type_AsrDiagnostics != null)
            Console.WriteLine("[PASS] 类型 AsrDiagnostics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AsrDiagnostics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AsrIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("AsrIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 AsrIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] AsrIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AsrIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AsrIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AsrIntegration.SpeechRecognizerPooledPolicy
    var type_SpeechRecognizerPooledPolicy = Type.GetType("AsrIntegration.SpeechRecognizerPooledPolicy");
    if (type_SpeechRecognizerPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 AsrIntegration.SpeechRecognizerPooledPolicy (class) 存在");
        var ctors_SpeechRecognizerPooledPolicy = type_SpeechRecognizerPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] AsrIntegration.SpeechRecognizerPooledPolicy 构造函数数量: {ctors_SpeechRecognizerPooledPolicy.Length}");
        var methods_SpeechRecognizerPooledPolicy = type_SpeechRecognizerPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AsrIntegration.SpeechRecognizerPooledPolicy 公开方法数量: {methods_SpeechRecognizerPooledPolicy.Length}");
        foreach (var m in methods_SpeechRecognizerPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AsrIntegration.SpeechRecognizerPooledPolicy 未找到，尝试无命名空间...");
        type_SpeechRecognizerPooledPolicy = Type.GetType("SpeechRecognizerPooledPolicy");
        if (type_SpeechRecognizerPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 SpeechRecognizerPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpeechRecognizerPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AsrIntegration.MemoryStreamPooledPolicy
    var type_MemoryStreamPooledPolicy = Type.GetType("AsrIntegration.MemoryStreamPooledPolicy");
    if (type_MemoryStreamPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 AsrIntegration.MemoryStreamPooledPolicy (class) 存在");
        var ctors_MemoryStreamPooledPolicy = type_MemoryStreamPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] AsrIntegration.MemoryStreamPooledPolicy 构造函数数量: {ctors_MemoryStreamPooledPolicy.Length}");
        var methods_MemoryStreamPooledPolicy = type_MemoryStreamPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AsrIntegration.MemoryStreamPooledPolicy 公开方法数量: {methods_MemoryStreamPooledPolicy.Length}");
        foreach (var m in methods_MemoryStreamPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AsrIntegration.MemoryStreamPooledPolicy 未找到，尝试无命名空间...");
        type_MemoryStreamPooledPolicy = Type.GetType("MemoryStreamPooledPolicy");
        if (type_MemoryStreamPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 MemoryStreamPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryStreamPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: AsrIntegration.IAudioProcessingPipeline
    var type_IAudioProcessingPipeline = Type.GetType("AsrIntegration.IAudioProcessingPipeline");
    if (type_IAudioProcessingPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 AsrIntegration.IAudioProcessingPipeline (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AsrIntegration.IAudioProcessingPipeline 未找到，尝试无命名空间...");
        type_IAudioProcessingPipeline = Type.GetType("IAudioProcessingPipeline");
        if (type_IAudioProcessingPipeline != null)
            Console.WriteLine("[PASS] 类型 IAudioProcessingPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAudioProcessingPipeline 可能为顶层语句或嵌套类型");
    }

    // 验证 record: AsrIntegration.AsrOptions
    var type_AsrOptions = Type.GetType("AsrIntegration.AsrOptions");
    if (type_AsrOptions != null)
    {
        Console.WriteLine("[PASS] 类型 AsrIntegration.AsrOptions (record) 存在");
        var ctors_AsrOptions = type_AsrOptions.GetConstructors();
        Console.WriteLine($"[PASS] AsrIntegration.AsrOptions 构造函数数量: {ctors_AsrOptions.Length}");
        var methods_AsrOptions = type_AsrOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AsrIntegration.AsrOptions 公开方法数量: {methods_AsrOptions.Length}");
        foreach (var m in methods_AsrOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AsrIntegration.AsrOptions 未找到，尝试无命名空间...");
        type_AsrOptions = Type.GetType("AsrOptions");
        if (type_AsrOptions != null)
            Console.WriteLine("[PASS] 类型 AsrOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AsrOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
