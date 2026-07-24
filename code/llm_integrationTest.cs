#load "llm_integration.cs"

Console.WriteLine("=== llm_integration.cs Test ===");

try
{
    // 验证 class: LlmProcessor
    var type_LlmProcessor = Type.GetType("LlmProcessor");
    if (type_LlmProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 LlmProcessor (class) 存在");
        var ctors_LlmProcessor = type_LlmProcessor.GetConstructors();
        Console.WriteLine($"[PASS] LlmProcessor 构造函数数量: {ctors_LlmProcessor.Length}");
        var methods_LlmProcessor = type_LlmProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LlmProcessor 公开方法数量: {methods_LlmProcessor.Length}");
        foreach (var m in methods_LlmProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LlmProcessor 未找到，尝试无命名空间...");
        type_LlmProcessor = Type.GetType("LlmProcessor");
        if (type_LlmProcessor != null)
            Console.WriteLine("[PASS] 类型 LlmProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LlmProcessor 可能为顶层语句或嵌套类型");
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

    // 验证 class: MemoryStreamPooledPolicy
    var type_MemoryStreamPooledPolicy = Type.GetType("MemoryStreamPooledPolicy");
    if (type_MemoryStreamPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryStreamPooledPolicy (class) 存在");
        var ctors_MemoryStreamPooledPolicy = type_MemoryStreamPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MemoryStreamPooledPolicy 构造函数数量: {ctors_MemoryStreamPooledPolicy.Length}");
        var methods_MemoryStreamPooledPolicy = type_MemoryStreamPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryStreamPooledPolicy 公开方法数量: {methods_MemoryStreamPooledPolicy.Length}");
        foreach (var m in methods_MemoryStreamPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryStreamPooledPolicy 未找到，尝试无命名空间...");
        type_MemoryStreamPooledPolicy = Type.GetType("MemoryStreamPooledPolicy");
        if (type_MemoryStreamPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 MemoryStreamPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryStreamPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ILlmPipeline
    var type_ILlmPipeline = Type.GetType("ILlmPipeline");
    if (type_ILlmPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 ILlmPipeline (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ILlmPipeline 未找到，尝试无命名空间...");
        type_ILlmPipeline = Type.GetType("ILlmPipeline");
        if (type_ILlmPipeline != null)
            Console.WriteLine("[PASS] 类型 ILlmPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ILlmPipeline 可能为顶层语句或嵌套类型");
    }

    // 验证 record: LlmOptions
    var type_LlmOptions = Type.GetType("LlmOptions");
    if (type_LlmOptions != null)
    {
        Console.WriteLine("[PASS] 类型 LlmOptions (record) 存在");
        var ctors_LlmOptions = type_LlmOptions.GetConstructors();
        Console.WriteLine($"[PASS] LlmOptions 构造函数数量: {ctors_LlmOptions.Length}");
        var methods_LlmOptions = type_LlmOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LlmOptions 公开方法数量: {methods_LlmOptions.Length}");
        foreach (var m in methods_LlmOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LlmOptions 未找到，尝试无命名空间...");
        type_LlmOptions = Type.GetType("LlmOptions");
        if (type_LlmOptions != null)
            Console.WriteLine("[PASS] 类型 LlmOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LlmOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
