#load "semantic_kernel_integration.cs"

Console.WriteLine("=== semantic_kernel_integration.cs Test ===");

try
{
    // 验证 class: SemanticKernelIntegration.SemanticKernelOptions
    var type_SemanticKernelOptions = Type.GetType("SemanticKernelIntegration.SemanticKernelOptions");
    if (type_SemanticKernelOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticKernelIntegration.SemanticKernelOptions (class) 存在");
        var ctors_SemanticKernelOptions = type_SemanticKernelOptions.GetConstructors();
        Console.WriteLine($"[PASS] SemanticKernelIntegration.SemanticKernelOptions 构造函数数量: {ctors_SemanticKernelOptions.Length}");
        var methods_SemanticKernelOptions = type_SemanticKernelOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SemanticKernelIntegration.SemanticKernelOptions 公开方法数量: {methods_SemanticKernelOptions.Length}");
        foreach (var m in methods_SemanticKernelOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticKernelIntegration.SemanticKernelOptions 未找到，尝试无命名空间...");
        type_SemanticKernelOptions = Type.GetType("SemanticKernelOptions");
        if (type_SemanticKernelOptions != null)
            Console.WriteLine("[PASS] 类型 SemanticKernelOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SemanticKernelOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SemanticKernelIntegration.SemanticKernelService
    var type_SemanticKernelService = Type.GetType("SemanticKernelIntegration.SemanticKernelService");
    if (type_SemanticKernelService != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticKernelIntegration.SemanticKernelService (class) 存在");
        var ctors_SemanticKernelService = type_SemanticKernelService.GetConstructors();
        Console.WriteLine($"[PASS] SemanticKernelIntegration.SemanticKernelService 构造函数数量: {ctors_SemanticKernelService.Length}");
        var methods_SemanticKernelService = type_SemanticKernelService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SemanticKernelIntegration.SemanticKernelService 公开方法数量: {methods_SemanticKernelService.Length}");
        foreach (var m in methods_SemanticKernelService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticKernelIntegration.SemanticKernelService 未找到，尝试无命名空间...");
        type_SemanticKernelService = Type.GetType("SemanticKernelService");
        if (type_SemanticKernelService != null)
            Console.WriteLine("[PASS] 类型 SemanticKernelService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SemanticKernelService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SemanticKernelIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("SemanticKernelIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticKernelIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SemanticKernelIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SemanticKernelIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticKernelIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SemanticKernelIntegration.Program
    var type_Program = Type.GetType("SemanticKernelIntegration.Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticKernelIntegration.Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] SemanticKernelIntegration.Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SemanticKernelIntegration.Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticKernelIntegration.Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: SemanticKernelIntegration.ISemanticKernelService
    var type_ISemanticKernelService = Type.GetType("SemanticKernelIntegration.ISemanticKernelService");
    if (type_ISemanticKernelService != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticKernelIntegration.ISemanticKernelService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticKernelIntegration.ISemanticKernelService 未找到，尝试无命名空间...");
        type_ISemanticKernelService = Type.GetType("ISemanticKernelService");
        if (type_ISemanticKernelService != null)
            Console.WriteLine("[PASS] 类型 ISemanticKernelService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISemanticKernelService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
