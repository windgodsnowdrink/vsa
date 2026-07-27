#load "craftsman_integration.cs"

Console.WriteLine("=== craftsman_integration.cs Test ===");

try
{
    // 验证 class: CraftsmanIntegration
    var type_CraftsmanIntegration = Type.GetType("CraftsmanIntegration");
    if (type_CraftsmanIntegration != null)
    {
        Console.WriteLine("[PASS] 类型 CraftsmanIntegration (class) 存在");
        var ctors_CraftsmanIntegration = type_CraftsmanIntegration.GetConstructors();
        Console.WriteLine($"[PASS] CraftsmanIntegration 构造函数数量: {ctors_CraftsmanIntegration.Length}");
        var methods_CraftsmanIntegration = type_CraftsmanIntegration.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CraftsmanIntegration 公开方法数量: {methods_CraftsmanIntegration.Length}");
        foreach (var m in methods_CraftsmanIntegration)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CraftsmanIntegration 未找到，尝试无命名空间...");
        type_CraftsmanIntegration = Type.GetType("CraftsmanIntegration");
        if (type_CraftsmanIntegration != null)
            Console.WriteLine("[PASS] 类型 CraftsmanIntegration (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CraftsmanIntegration 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CraftsmanService
    var type_CraftsmanService = Type.GetType("CraftsmanService");
    if (type_CraftsmanService != null)
    {
        Console.WriteLine("[PASS] 类型 CraftsmanService (class) 存在");
        var ctors_CraftsmanService = type_CraftsmanService.GetConstructors();
        Console.WriteLine($"[PASS] CraftsmanService 构造函数数量: {ctors_CraftsmanService.Length}");
        var methods_CraftsmanService = type_CraftsmanService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CraftsmanService 公开方法数量: {methods_CraftsmanService.Length}");
        foreach (var m in methods_CraftsmanService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CraftsmanService 未找到，尝试无命名空间...");
        type_CraftsmanService = Type.GetType("CraftsmanService");
        if (type_CraftsmanService != null)
            Console.WriteLine("[PASS] 类型 CraftsmanService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CraftsmanService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ICraftsmanService
    var type_ICraftsmanService = Type.GetType("ICraftsmanService");
    if (type_ICraftsmanService != null)
    {
        Console.WriteLine("[PASS] 类型 ICraftsmanService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ICraftsmanService 未找到，尝试无命名空间...");
        type_ICraftsmanService = Type.GetType("ICraftsmanService");
        if (type_ICraftsmanService != null)
            Console.WriteLine("[PASS] 类型 ICraftsmanService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICraftsmanService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
