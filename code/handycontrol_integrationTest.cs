#load "handycontrol_integration.cs"

Console.WriteLine("=== handycontrol_integration.cs Test ===");

try
{
    // 验证 class: HandyControlOptions
    var type_HandyControlOptions = Type.GetType("HandyControlOptions");
    if (type_HandyControlOptions != null)
    {
        Console.WriteLine("[PASS] 类型 HandyControlOptions (class) 存在");
        var ctors_HandyControlOptions = type_HandyControlOptions.GetConstructors();
        Console.WriteLine($"[PASS] HandyControlOptions 构造函数数量: {ctors_HandyControlOptions.Length}");
        var methods_HandyControlOptions = type_HandyControlOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HandyControlOptions 公开方法数量: {methods_HandyControlOptions.Length}");
        foreach (var m in methods_HandyControlOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HandyControlOptions 未找到，尝试无命名空间...");
        type_HandyControlOptions = Type.GetType("HandyControlOptions");
        if (type_HandyControlOptions != null)
            Console.WriteLine("[PASS] 类型 HandyControlOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HandyControlOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HandyControlService
    var type_HandyControlService = Type.GetType("HandyControlService");
    if (type_HandyControlService != null)
    {
        Console.WriteLine("[PASS] 类型 HandyControlService (class) 存在");
        var ctors_HandyControlService = type_HandyControlService.GetConstructors();
        Console.WriteLine($"[PASS] HandyControlService 构造函数数量: {ctors_HandyControlService.Length}");
        var methods_HandyControlService = type_HandyControlService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HandyControlService 公开方法数量: {methods_HandyControlService.Length}");
        foreach (var m in methods_HandyControlService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HandyControlService 未找到，尝试无命名空间...");
        type_HandyControlService = Type.GetType("HandyControlService");
        if (type_HandyControlService != null)
            Console.WriteLine("[PASS] 类型 HandyControlService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HandyControlService 可能为顶层语句或嵌套类型");
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

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IHandyControlService
    var type_IHandyControlService = Type.GetType("IHandyControlService");
    if (type_IHandyControlService != null)
    {
        Console.WriteLine("[PASS] 类型 IHandyControlService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IHandyControlService 未找到，尝试无命名空间...");
        type_IHandyControlService = Type.GetType("IHandyControlService");
        if (type_IHandyControlService != null)
            Console.WriteLine("[PASS] 类型 IHandyControlService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IHandyControlService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
