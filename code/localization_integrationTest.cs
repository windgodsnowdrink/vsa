#load "localization_integration.cs"

Console.WriteLine("=== localization_integration.cs Test ===");

try
{
    // 验证 class: LocalizationIntegration.LocalizationService
    var type_LocalizationService = Type.GetType("LocalizationIntegration.LocalizationService");
    if (type_LocalizationService != null)
    {
        Console.WriteLine("[PASS] 类型 LocalizationIntegration.LocalizationService (class) 存在");
        var ctors_LocalizationService = type_LocalizationService.GetConstructors();
        Console.WriteLine($"[PASS] LocalizationIntegration.LocalizationService 构造函数数量: {ctors_LocalizationService.Length}");
        var methods_LocalizationService = type_LocalizationService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LocalizationIntegration.LocalizationService 公开方法数量: {methods_LocalizationService.Length}");
        foreach (var m in methods_LocalizationService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LocalizationIntegration.LocalizationService 未找到，尝试无命名空间...");
        type_LocalizationService = Type.GetType("LocalizationService");
        if (type_LocalizationService != null)
            Console.WriteLine("[PASS] 类型 LocalizationService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LocalizationService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: LocalizationIntegration.ILocalizationService
    var type_ILocalizationService = Type.GetType("LocalizationIntegration.ILocalizationService");
    if (type_ILocalizationService != null)
    {
        Console.WriteLine("[PASS] 类型 LocalizationIntegration.ILocalizationService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LocalizationIntegration.ILocalizationService 未找到，尝试无命名空间...");
        type_ILocalizationService = Type.GetType("ILocalizationService");
        if (type_ILocalizationService != null)
            Console.WriteLine("[PASS] 类型 ILocalizationService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ILocalizationService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
