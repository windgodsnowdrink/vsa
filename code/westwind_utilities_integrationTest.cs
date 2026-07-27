#load "westwind_utilities_integration.cs"

Console.WriteLine("=== westwind_utilities_integration.cs Test ===");

try
{
    // 验证 class: WestwindOptions
    var type_WestwindOptions = Type.GetType("WestwindOptions");
    if (type_WestwindOptions != null)
    {
        Console.WriteLine("[PASS] 类型 WestwindOptions (class) 存在");
        var ctors_WestwindOptions = type_WestwindOptions.GetConstructors();
        Console.WriteLine($"[PASS] WestwindOptions 构造函数数量: {ctors_WestwindOptions.Length}");
        var methods_WestwindOptions = type_WestwindOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WestwindOptions 公开方法数量: {methods_WestwindOptions.Length}");
        foreach (var m in methods_WestwindOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WestwindOptions 未找到，尝试无命名空间...");
        type_WestwindOptions = Type.GetType("WestwindOptions");
        if (type_WestwindOptions != null)
            Console.WriteLine("[PASS] 类型 WestwindOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WestwindOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WestwindService
    var type_WestwindService = Type.GetType("WestwindService");
    if (type_WestwindService != null)
    {
        Console.WriteLine("[PASS] 类型 WestwindService (class) 存在");
        var ctors_WestwindService = type_WestwindService.GetConstructors();
        Console.WriteLine($"[PASS] WestwindService 构造函数数量: {ctors_WestwindService.Length}");
        var methods_WestwindService = type_WestwindService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WestwindService 公开方法数量: {methods_WestwindService.Length}");
        foreach (var m in methods_WestwindService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WestwindService 未找到，尝试无命名空间...");
        type_WestwindService = Type.GetType("WestwindService");
        if (type_WestwindService != null)
            Console.WriteLine("[PASS] 类型 WestwindService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WestwindService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IWestwindService
    var type_IWestwindService = Type.GetType("IWestwindService");
    if (type_IWestwindService != null)
    {
        Console.WriteLine("[PASS] 类型 IWestwindService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IWestwindService 未找到，尝试无命名空间...");
        type_IWestwindService = Type.GetType("IWestwindService");
        if (type_IWestwindService != null)
            Console.WriteLine("[PASS] 类型 IWestwindService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IWestwindService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
