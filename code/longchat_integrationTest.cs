#load "longchat_integration.cs"

Console.WriteLine("=== longchat_integration.cs Test ===");

try
{
    // 验证 class: LongChatIntegration.LongChatOptions
    var type_LongChatOptions = Type.GetType("LongChatIntegration.LongChatOptions");
    if (type_LongChatOptions != null)
    {
        Console.WriteLine("[PASS] 类型 LongChatIntegration.LongChatOptions (class) 存在");
        var ctors_LongChatOptions = type_LongChatOptions.GetConstructors();
        Console.WriteLine($"[PASS] LongChatIntegration.LongChatOptions 构造函数数量: {ctors_LongChatOptions.Length}");
        var methods_LongChatOptions = type_LongChatOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LongChatIntegration.LongChatOptions 公开方法数量: {methods_LongChatOptions.Length}");
        foreach (var m in methods_LongChatOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LongChatIntegration.LongChatOptions 未找到，尝试无命名空间...");
        type_LongChatOptions = Type.GetType("LongChatOptions");
        if (type_LongChatOptions != null)
            Console.WriteLine("[PASS] 类型 LongChatOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LongChatOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LongChatIntegration.LongChatService
    var type_LongChatService = Type.GetType("LongChatIntegration.LongChatService");
    if (type_LongChatService != null)
    {
        Console.WriteLine("[PASS] 类型 LongChatIntegration.LongChatService (class) 存在");
        var ctors_LongChatService = type_LongChatService.GetConstructors();
        Console.WriteLine($"[PASS] LongChatIntegration.LongChatService 构造函数数量: {ctors_LongChatService.Length}");
        var methods_LongChatService = type_LongChatService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LongChatIntegration.LongChatService 公开方法数量: {methods_LongChatService.Length}");
        foreach (var m in methods_LongChatService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LongChatIntegration.LongChatService 未找到，尝试无命名空间...");
        type_LongChatService = Type.GetType("LongChatService");
        if (type_LongChatService != null)
            Console.WriteLine("[PASS] 类型 LongChatService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LongChatService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LongChatIntegration.LongChatExtensions
    var type_LongChatExtensions = Type.GetType("LongChatIntegration.LongChatExtensions");
    if (type_LongChatExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 LongChatIntegration.LongChatExtensions (class) 存在");
        var ctors_LongChatExtensions = type_LongChatExtensions.GetConstructors();
        Console.WriteLine($"[PASS] LongChatIntegration.LongChatExtensions 构造函数数量: {ctors_LongChatExtensions.Length}");
        var methods_LongChatExtensions = type_LongChatExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LongChatIntegration.LongChatExtensions 公开方法数量: {methods_LongChatExtensions.Length}");
        foreach (var m in methods_LongChatExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LongChatIntegration.LongChatExtensions 未找到，尝试无命名空间...");
        type_LongChatExtensions = Type.GetType("LongChatExtensions");
        if (type_LongChatExtensions != null)
            Console.WriteLine("[PASS] 类型 LongChatExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LongChatExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LongChatIntegration.Program
    var type_Program = Type.GetType("LongChatIntegration.Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 LongChatIntegration.Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] LongChatIntegration.Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LongChatIntegration.Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LongChatIntegration.Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: LongChatIntegration.ILongChatService
    var type_ILongChatService = Type.GetType("LongChatIntegration.ILongChatService");
    if (type_ILongChatService != null)
    {
        Console.WriteLine("[PASS] 类型 LongChatIntegration.ILongChatService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LongChatIntegration.ILongChatService 未找到，尝试无命名空间...");
        type_ILongChatService = Type.GetType("ILongChatService");
        if (type_ILongChatService != null)
            Console.WriteLine("[PASS] 类型 ILongChatService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ILongChatService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
