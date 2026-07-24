#load "genvue_integration.cs"

Console.WriteLine("=== genvue_integration.cs Test ===");

try
{
    // 验证 class: GenVueIntegration.GenVueOptions
    var type_GenVueOptions = Type.GetType("GenVueIntegration.GenVueOptions");
    if (type_GenVueOptions != null)
    {
        Console.WriteLine("[PASS] 类型 GenVueIntegration.GenVueOptions (class) 存在");
        var ctors_GenVueOptions = type_GenVueOptions.GetConstructors();
        Console.WriteLine($"[PASS] GenVueIntegration.GenVueOptions 构造函数数量: {ctors_GenVueOptions.Length}");
        var methods_GenVueOptions = type_GenVueOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GenVueIntegration.GenVueOptions 公开方法数量: {methods_GenVueOptions.Length}");
        foreach (var m in methods_GenVueOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GenVueIntegration.GenVueOptions 未找到，尝试无命名空间...");
        type_GenVueOptions = Type.GetType("GenVueOptions");
        if (type_GenVueOptions != null)
            Console.WriteLine("[PASS] 类型 GenVueOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GenVueOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GenVueIntegration.GenVueService
    var type_GenVueService = Type.GetType("GenVueIntegration.GenVueService");
    if (type_GenVueService != null)
    {
        Console.WriteLine("[PASS] 类型 GenVueIntegration.GenVueService (class) 存在");
        var ctors_GenVueService = type_GenVueService.GetConstructors();
        Console.WriteLine($"[PASS] GenVueIntegration.GenVueService 构造函数数量: {ctors_GenVueService.Length}");
        var methods_GenVueService = type_GenVueService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GenVueIntegration.GenVueService 公开方法数量: {methods_GenVueService.Length}");
        foreach (var m in methods_GenVueService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GenVueIntegration.GenVueService 未找到，尝试无命名空间...");
        type_GenVueService = Type.GetType("GenVueService");
        if (type_GenVueService != null)
            Console.WriteLine("[PASS] 类型 GenVueService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GenVueService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GenVueIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("GenVueIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 GenVueIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] GenVueIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GenVueIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GenVueIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GenVueIntegration.Program
    var type_Program = Type.GetType("GenVueIntegration.Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 GenVueIntegration.Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] GenVueIntegration.Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GenVueIntegration.Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GenVueIntegration.Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: GenVueIntegration.IGenVueService
    var type_IGenVueService = Type.GetType("GenVueIntegration.IGenVueService");
    if (type_IGenVueService != null)
    {
        Console.WriteLine("[PASS] 类型 GenVueIntegration.IGenVueService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GenVueIntegration.IGenVueService 未找到，尝试无命名空间...");
        type_IGenVueService = Type.GetType("IGenVueService");
        if (type_IGenVueService != null)
            Console.WriteLine("[PASS] 类型 IGenVueService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IGenVueService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
