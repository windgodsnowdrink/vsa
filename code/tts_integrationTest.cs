#load "tts_integration.cs"

Console.WriteLine("=== tts_integration.cs Test ===");

try
{
    // 验证 class: TtsIntegration.TtsOptions
    var type_TtsOptions = Type.GetType("TtsIntegration.TtsOptions");
    if (type_TtsOptions != null)
    {
        Console.WriteLine("[PASS] 类型 TtsIntegration.TtsOptions (class) 存在");
        var ctors_TtsOptions = type_TtsOptions.GetConstructors();
        Console.WriteLine($"[PASS] TtsIntegration.TtsOptions 构造函数数量: {ctors_TtsOptions.Length}");
        var methods_TtsOptions = type_TtsOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TtsIntegration.TtsOptions 公开方法数量: {methods_TtsOptions.Length}");
        foreach (var m in methods_TtsOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TtsIntegration.TtsOptions 未找到，尝试无命名空间...");
        type_TtsOptions = Type.GetType("TtsOptions");
        if (type_TtsOptions != null)
            Console.WriteLine("[PASS] 类型 TtsOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TtsOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TtsIntegration.TtsProcessor
    var type_TtsProcessor = Type.GetType("TtsIntegration.TtsProcessor");
    if (type_TtsProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 TtsIntegration.TtsProcessor (class) 存在");
        var ctors_TtsProcessor = type_TtsProcessor.GetConstructors();
        Console.WriteLine($"[PASS] TtsIntegration.TtsProcessor 构造函数数量: {ctors_TtsProcessor.Length}");
        var methods_TtsProcessor = type_TtsProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TtsIntegration.TtsProcessor 公开方法数量: {methods_TtsProcessor.Length}");
        foreach (var m in methods_TtsProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TtsIntegration.TtsProcessor 未找到，尝试无命名空间...");
        type_TtsProcessor = Type.GetType("TtsProcessor");
        if (type_TtsProcessor != null)
            Console.WriteLine("[PASS] 类型 TtsProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TtsProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TtsIntegration.TtsContext
    var type_TtsContext = Type.GetType("TtsIntegration.TtsContext");
    if (type_TtsContext != null)
    {
        Console.WriteLine("[PASS] 类型 TtsIntegration.TtsContext (class) 存在");
        var ctors_TtsContext = type_TtsContext.GetConstructors();
        Console.WriteLine($"[PASS] TtsIntegration.TtsContext 构造函数数量: {ctors_TtsContext.Length}");
        var methods_TtsContext = type_TtsContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TtsIntegration.TtsContext 公开方法数量: {methods_TtsContext.Length}");
        foreach (var m in methods_TtsContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TtsIntegration.TtsContext 未找到，尝试无命名空间...");
        type_TtsContext = Type.GetType("TtsContext");
        if (type_TtsContext != null)
            Console.WriteLine("[PASS] 类型 TtsContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TtsContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TtsIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("TtsIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 TtsIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] TtsIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TtsIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TtsIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
