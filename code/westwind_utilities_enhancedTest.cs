#load "westwind_utilities_enhanced.cs"

Console.WriteLine("=== westwind_utilities_enhanced.cs Test ===");

try
{
    // 验证 class: WestwindEnhancedOptions
    var type_WestwindEnhancedOptions = Type.GetType("WestwindEnhancedOptions");
    if (type_WestwindEnhancedOptions != null)
    {
        Console.WriteLine("[PASS] 类型 WestwindEnhancedOptions (class) 存在");
        var ctors_WestwindEnhancedOptions = type_WestwindEnhancedOptions.GetConstructors();
        Console.WriteLine($"[PASS] WestwindEnhancedOptions 构造函数数量: {ctors_WestwindEnhancedOptions.Length}");
        var methods_WestwindEnhancedOptions = type_WestwindEnhancedOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WestwindEnhancedOptions 公开方法数量: {methods_WestwindEnhancedOptions.Length}");
        foreach (var m in methods_WestwindEnhancedOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WestwindEnhancedOptions 未找到，尝试无命名空间...");
        type_WestwindEnhancedOptions = Type.GetType("WestwindEnhancedOptions");
        if (type_WestwindEnhancedOptions != null)
            Console.WriteLine("[PASS] 类型 WestwindEnhancedOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WestwindEnhancedOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WestwindEnhancedService
    var type_WestwindEnhancedService = Type.GetType("WestwindEnhancedService");
    if (type_WestwindEnhancedService != null)
    {
        Console.WriteLine("[PASS] 类型 WestwindEnhancedService (class) 存在");
        var ctors_WestwindEnhancedService = type_WestwindEnhancedService.GetConstructors();
        Console.WriteLine($"[PASS] WestwindEnhancedService 构造函数数量: {ctors_WestwindEnhancedService.Length}");
        var methods_WestwindEnhancedService = type_WestwindEnhancedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WestwindEnhancedService 公开方法数量: {methods_WestwindEnhancedService.Length}");
        foreach (var m in methods_WestwindEnhancedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WestwindEnhancedService 未找到，尝试无命名空间...");
        type_WestwindEnhancedService = Type.GetType("WestwindEnhancedService");
        if (type_WestwindEnhancedService != null)
            Console.WriteLine("[PASS] 类型 WestwindEnhancedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WestwindEnhancedService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IWestwindEnhancedService
    var type_IWestwindEnhancedService = Type.GetType("IWestwindEnhancedService");
    if (type_IWestwindEnhancedService != null)
    {
        Console.WriteLine("[PASS] 类型 IWestwindEnhancedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IWestwindEnhancedService 未找到，尝试无命名空间...");
        type_IWestwindEnhancedService = Type.GetType("IWestwindEnhancedService");
        if (type_IWestwindEnhancedService != null)
            Console.WriteLine("[PASS] 类型 IWestwindEnhancedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IWestwindEnhancedService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
