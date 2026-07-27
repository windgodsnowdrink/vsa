#load "antsk_knowledgebase_integration.cs"

Console.WriteLine("=== antsk_knowledgebase_integration.cs Test ===");

try
{
    // 验证 class: AntSKOptions
    var type_AntSKOptions = Type.GetType("AntSKOptions");
    if (type_AntSKOptions != null)
    {
        Console.WriteLine("[PASS] 类型 AntSKOptions (class) 存在");
        var ctors_AntSKOptions = type_AntSKOptions.GetConstructors();
        Console.WriteLine($"[PASS] AntSKOptions 构造函数数量: {ctors_AntSKOptions.Length}");
        var methods_AntSKOptions = type_AntSKOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AntSKOptions 公开方法数量: {methods_AntSKOptions.Length}");
        foreach (var m in methods_AntSKOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AntSKOptions 未找到，尝试无命名空间...");
        type_AntSKOptions = Type.GetType("AntSKOptions");
        if (type_AntSKOptions != null)
            Console.WriteLine("[PASS] 类型 AntSKOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AntSKOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AntSKService
    var type_AntSKService = Type.GetType("AntSKService");
    if (type_AntSKService != null)
    {
        Console.WriteLine("[PASS] 类型 AntSKService (class) 存在");
        var ctors_AntSKService = type_AntSKService.GetConstructors();
        Console.WriteLine($"[PASS] AntSKService 构造函数数量: {ctors_AntSKService.Length}");
        var methods_AntSKService = type_AntSKService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AntSKService 公开方法数量: {methods_AntSKService.Length}");
        foreach (var m in methods_AntSKService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AntSKService 未找到，尝试无命名空间...");
        type_AntSKService = Type.GetType("AntSKService");
        if (type_AntSKService != null)
            Console.WriteLine("[PASS] 类型 AntSKService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AntSKService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AntSKExtensions
    var type_AntSKExtensions = Type.GetType("AntSKExtensions");
    if (type_AntSKExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 AntSKExtensions (class) 存在");
        var ctors_AntSKExtensions = type_AntSKExtensions.GetConstructors();
        Console.WriteLine($"[PASS] AntSKExtensions 构造函数数量: {ctors_AntSKExtensions.Length}");
        var methods_AntSKExtensions = type_AntSKExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AntSKExtensions 公开方法数量: {methods_AntSKExtensions.Length}");
        foreach (var m in methods_AntSKExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AntSKExtensions 未找到，尝试无命名空间...");
        type_AntSKExtensions = Type.GetType("AntSKExtensions");
        if (type_AntSKExtensions != null)
            Console.WriteLine("[PASS] 类型 AntSKExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AntSKExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AntSKBackgroundService
    var type_AntSKBackgroundService = Type.GetType("AntSKBackgroundService");
    if (type_AntSKBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 AntSKBackgroundService (class) 存在");
        var ctors_AntSKBackgroundService = type_AntSKBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] AntSKBackgroundService 构造函数数量: {ctors_AntSKBackgroundService.Length}");
        var methods_AntSKBackgroundService = type_AntSKBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AntSKBackgroundService 公开方法数量: {methods_AntSKBackgroundService.Length}");
        foreach (var m in methods_AntSKBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AntSKBackgroundService 未找到，尝试无命名空间...");
        type_AntSKBackgroundService = Type.GetType("AntSKBackgroundService");
        if (type_AntSKBackgroundService != null)
            Console.WriteLine("[PASS] 类型 AntSKBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AntSKBackgroundService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IAntSKService
    var type_IAntSKService = Type.GetType("IAntSKService");
    if (type_IAntSKService != null)
    {
        Console.WriteLine("[PASS] 类型 IAntSKService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAntSKService 未找到，尝试无命名空间...");
        type_IAntSKService = Type.GetType("IAntSKService");
        if (type_IAntSKService != null)
            Console.WriteLine("[PASS] 类型 IAntSKService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAntSKService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
