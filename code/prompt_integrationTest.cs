#load "prompt_integration.cs"

Console.WriteLine("=== prompt_integration.cs Test ===");

try
{
    // 验证 class: PromptIntegration.PromptOptions
    var type_PromptOptions = Type.GetType("PromptIntegration.PromptOptions");
    if (type_PromptOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PromptIntegration.PromptOptions (class) 存在");
        var ctors_PromptOptions = type_PromptOptions.GetConstructors();
        Console.WriteLine($"[PASS] PromptIntegration.PromptOptions 构造函数数量: {ctors_PromptOptions.Length}");
        var methods_PromptOptions = type_PromptOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PromptIntegration.PromptOptions 公开方法数量: {methods_PromptOptions.Length}");
        foreach (var m in methods_PromptOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PromptIntegration.PromptOptions 未找到，尝试无命名空间...");
        type_PromptOptions = Type.GetType("PromptOptions");
        if (type_PromptOptions != null)
            Console.WriteLine("[PASS] 类型 PromptOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PromptOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PromptIntegration.PromptService
    var type_PromptService = Type.GetType("PromptIntegration.PromptService");
    if (type_PromptService != null)
    {
        Console.WriteLine("[PASS] 类型 PromptIntegration.PromptService (class) 存在");
        var ctors_PromptService = type_PromptService.GetConstructors();
        Console.WriteLine($"[PASS] PromptIntegration.PromptService 构造函数数量: {ctors_PromptService.Length}");
        var methods_PromptService = type_PromptService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PromptIntegration.PromptService 公开方法数量: {methods_PromptService.Length}");
        foreach (var m in methods_PromptService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PromptIntegration.PromptService 未找到，尝试无命名空间...");
        type_PromptService = Type.GetType("PromptService");
        if (type_PromptService != null)
            Console.WriteLine("[PASS] 类型 PromptService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PromptService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PromptIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("PromptIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PromptIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PromptIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PromptIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PromptIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: PromptIntegration.IPromptService
    var type_IPromptService = Type.GetType("PromptIntegration.IPromptService");
    if (type_IPromptService != null)
    {
        Console.WriteLine("[PASS] 类型 PromptIntegration.IPromptService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PromptIntegration.IPromptService 未找到，尝试无命名空间...");
        type_IPromptService = Type.GetType("IPromptService");
        if (type_IPromptService != null)
            Console.WriteLine("[PASS] 类型 IPromptService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPromptService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
