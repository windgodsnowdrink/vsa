#load "agileconfig_integration.cs"

Console.WriteLine("=== agileconfig_integration.cs Test ===");

try
{
    // 验证 class: AgileConfigService
    var type_AgileConfigService = Type.GetType("AgileConfigService");
    if (type_AgileConfigService != null)
    {
        Console.WriteLine("[PASS] 类型 AgileConfigService (class) 存在");
        var ctors_AgileConfigService = type_AgileConfigService.GetConstructors();
        Console.WriteLine($"[PASS] AgileConfigService 构造函数数量: {ctors_AgileConfigService.Length}");
        var methods_AgileConfigService = type_AgileConfigService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AgileConfigService 公开方法数量: {methods_AgileConfigService.Length}");
        foreach (var m in methods_AgileConfigService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AgileConfigService 未找到，尝试无命名空间...");
        type_AgileConfigService = Type.GetType("AgileConfigService");
        if (type_AgileConfigService != null)
            Console.WriteLine("[PASS] 类型 AgileConfigService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AgileConfigService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AgileConfigExtensions
    var type_AgileConfigExtensions = Type.GetType("AgileConfigExtensions");
    if (type_AgileConfigExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 AgileConfigExtensions (class) 存在");
        var ctors_AgileConfigExtensions = type_AgileConfigExtensions.GetConstructors();
        Console.WriteLine($"[PASS] AgileConfigExtensions 构造函数数量: {ctors_AgileConfigExtensions.Length}");
        var methods_AgileConfigExtensions = type_AgileConfigExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AgileConfigExtensions 公开方法数量: {methods_AgileConfigExtensions.Length}");
        foreach (var m in methods_AgileConfigExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AgileConfigExtensions 未找到，尝试无命名空间...");
        type_AgileConfigExtensions = Type.GetType("AgileConfigExtensions");
        if (type_AgileConfigExtensions != null)
            Console.WriteLine("[PASS] 类型 AgileConfigExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AgileConfigExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAgileConfigService
    var type_IAgileConfigService = Type.GetType("IAgileConfigService");
    if (type_IAgileConfigService != null)
    {
        Console.WriteLine("[PASS] 类型 IAgileConfigService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAgileConfigService 未找到，尝试无命名空间...");
        type_IAgileConfigService = Type.GetType("IAgileConfigService");
        if (type_IAgileConfigService != null)
            Console.WriteLine("[PASS] 类型 IAgileConfigService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAgileConfigService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
