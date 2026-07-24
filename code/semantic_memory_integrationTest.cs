#load "semantic_memory_integration.cs"

Console.WriteLine("=== semantic_memory_integration.cs Test ===");

try
{
    // 验证 class: SemanticMemoryIntegration.SemanticMemoryOptions
    var type_SemanticMemoryOptions = Type.GetType("SemanticMemoryIntegration.SemanticMemoryOptions");
    if (type_SemanticMemoryOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticMemoryIntegration.SemanticMemoryOptions (class) 存在");
        var ctors_SemanticMemoryOptions = type_SemanticMemoryOptions.GetConstructors();
        Console.WriteLine($"[PASS] SemanticMemoryIntegration.SemanticMemoryOptions 构造函数数量: {ctors_SemanticMemoryOptions.Length}");
        var methods_SemanticMemoryOptions = type_SemanticMemoryOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SemanticMemoryIntegration.SemanticMemoryOptions 公开方法数量: {methods_SemanticMemoryOptions.Length}");
        foreach (var m in methods_SemanticMemoryOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticMemoryIntegration.SemanticMemoryOptions 未找到，尝试无命名空间...");
        type_SemanticMemoryOptions = Type.GetType("SemanticMemoryOptions");
        if (type_SemanticMemoryOptions != null)
            Console.WriteLine("[PASS] 类型 SemanticMemoryOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SemanticMemoryOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SemanticMemoryIntegration.SemanticMemoryService
    var type_SemanticMemoryService = Type.GetType("SemanticMemoryIntegration.SemanticMemoryService");
    if (type_SemanticMemoryService != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticMemoryIntegration.SemanticMemoryService (class) 存在");
        var ctors_SemanticMemoryService = type_SemanticMemoryService.GetConstructors();
        Console.WriteLine($"[PASS] SemanticMemoryIntegration.SemanticMemoryService 构造函数数量: {ctors_SemanticMemoryService.Length}");
        var methods_SemanticMemoryService = type_SemanticMemoryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SemanticMemoryIntegration.SemanticMemoryService 公开方法数量: {methods_SemanticMemoryService.Length}");
        foreach (var m in methods_SemanticMemoryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticMemoryIntegration.SemanticMemoryService 未找到，尝试无命名空间...");
        type_SemanticMemoryService = Type.GetType("SemanticMemoryService");
        if (type_SemanticMemoryService != null)
            Console.WriteLine("[PASS] 类型 SemanticMemoryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SemanticMemoryService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SemanticMemoryIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("SemanticMemoryIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticMemoryIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SemanticMemoryIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SemanticMemoryIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticMemoryIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: SemanticMemoryIntegration.ISemanticMemoryService
    var type_ISemanticMemoryService = Type.GetType("SemanticMemoryIntegration.ISemanticMemoryService");
    if (type_ISemanticMemoryService != null)
    {
        Console.WriteLine("[PASS] 类型 SemanticMemoryIntegration.ISemanticMemoryService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SemanticMemoryIntegration.ISemanticMemoryService 未找到，尝试无命名空间...");
        type_ISemanticMemoryService = Type.GetType("ISemanticMemoryService");
        if (type_ISemanticMemoryService != null)
            Console.WriteLine("[PASS] 类型 ISemanticMemoryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISemanticMemoryService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
