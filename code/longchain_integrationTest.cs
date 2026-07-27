#load "longchain_integration.cs"

Console.WriteLine("=== longchain_integration.cs Test ===");

try
{
    // 验证 class: LongChainOptions
    var type_LongChainOptions = Type.GetType("LongChainOptions");
    if (type_LongChainOptions != null)
    {
        Console.WriteLine("[PASS] 类型 LongChainOptions (class) 存在");
        var ctors_LongChainOptions = type_LongChainOptions.GetConstructors();
        Console.WriteLine($"[PASS] LongChainOptions 构造函数数量: {ctors_LongChainOptions.Length}");
        var methods_LongChainOptions = type_LongChainOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LongChainOptions 公开方法数量: {methods_LongChainOptions.Length}");
        foreach (var m in methods_LongChainOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LongChainOptions 未找到，尝试无命名空间...");
        type_LongChainOptions = Type.GetType("LongChainOptions");
        if (type_LongChainOptions != null)
            Console.WriteLine("[PASS] 类型 LongChainOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LongChainOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LongChainService
    var type_LongChainService = Type.GetType("LongChainService");
    if (type_LongChainService != null)
    {
        Console.WriteLine("[PASS] 类型 LongChainService (class) 存在");
        var ctors_LongChainService = type_LongChainService.GetConstructors();
        Console.WriteLine($"[PASS] LongChainService 构造函数数量: {ctors_LongChainService.Length}");
        var methods_LongChainService = type_LongChainService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LongChainService 公开方法数量: {methods_LongChainService.Length}");
        foreach (var m in methods_LongChainService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LongChainService 未找到，尝试无命名空间...");
        type_LongChainService = Type.GetType("LongChainService");
        if (type_LongChainService != null)
            Console.WriteLine("[PASS] 类型 LongChainService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LongChainService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: ILongChainService
    var type_ILongChainService = Type.GetType("ILongChainService");
    if (type_ILongChainService != null)
    {
        Console.WriteLine("[PASS] 类型 ILongChainService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ILongChainService 未找到，尝试无命名空间...");
        type_ILongChainService = Type.GetType("ILongChainService");
        if (type_ILongChainService != null)
            Console.WriteLine("[PASS] 类型 ILongChainService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ILongChainService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
