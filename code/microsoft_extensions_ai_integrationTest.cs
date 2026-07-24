#load "microsoft_extensions_ai_integration.cs"

Console.WriteLine("=== microsoft_extensions_ai_integration.cs Test ===");

try
{
    // 验证 class: Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIOptions
    var type_MicrosoftExtensionsAIOptions = Type.GetType("Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIOptions");
    if (type_MicrosoftExtensionsAIOptions != null)
    {
        Console.WriteLine("[PASS] 类型 Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIOptions (class) 存在");
        var ctors_MicrosoftExtensionsAIOptions = type_MicrosoftExtensionsAIOptions.GetConstructors();
        Console.WriteLine($"[PASS] Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIOptions 构造函数数量: {ctors_MicrosoftExtensionsAIOptions.Length}");
        var methods_MicrosoftExtensionsAIOptions = type_MicrosoftExtensionsAIOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIOptions 公开方法数量: {methods_MicrosoftExtensionsAIOptions.Length}");
        foreach (var m in methods_MicrosoftExtensionsAIOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIOptions 未找到，尝试无命名空间...");
        type_MicrosoftExtensionsAIOptions = Type.GetType("MicrosoftExtensionsAIOptions");
        if (type_MicrosoftExtensionsAIOptions != null)
            Console.WriteLine("[PASS] 类型 MicrosoftExtensionsAIOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MicrosoftExtensionsAIOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIService
    var type_MicrosoftExtensionsAIService = Type.GetType("Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIService");
    if (type_MicrosoftExtensionsAIService != null)
    {
        Console.WriteLine("[PASS] 类型 Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIService (class) 存在");
        var ctors_MicrosoftExtensionsAIService = type_MicrosoftExtensionsAIService.GetConstructors();
        Console.WriteLine($"[PASS] Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIService 构造函数数量: {ctors_MicrosoftExtensionsAIService.Length}");
        var methods_MicrosoftExtensionsAIService = type_MicrosoftExtensionsAIService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIService 公开方法数量: {methods_MicrosoftExtensionsAIService.Length}");
        foreach (var m in methods_MicrosoftExtensionsAIService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIService 未找到，尝试无命名空间...");
        type_MicrosoftExtensionsAIService = Type.GetType("MicrosoftExtensionsAIService");
        if (type_MicrosoftExtensionsAIService != null)
            Console.WriteLine("[PASS] 类型 MicrosoftExtensionsAIService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MicrosoftExtensionsAIService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Microsoft.Extensions.AI.Integration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("Microsoft.Extensions.AI.Integration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 Microsoft.Extensions.AI.Integration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] Microsoft.Extensions.AI.Integration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Microsoft.Extensions.AI.Integration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Microsoft.Extensions.AI.Integration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Microsoft.Extensions.AI.Integration.Program
    var type_Program = Type.GetType("Microsoft.Extensions.AI.Integration.Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Microsoft.Extensions.AI.Integration.Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Microsoft.Extensions.AI.Integration.Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Microsoft.Extensions.AI.Integration.Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Microsoft.Extensions.AI.Integration.Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: Microsoft.Extensions.AI.Integration.IMicrosoftExtensionsAIService
    var type_IMicrosoftExtensionsAIService = Type.GetType("Microsoft.Extensions.AI.Integration.IMicrosoftExtensionsAIService");
    if (type_IMicrosoftExtensionsAIService != null)
    {
        Console.WriteLine("[PASS] 类型 Microsoft.Extensions.AI.Integration.IMicrosoftExtensionsAIService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Microsoft.Extensions.AI.Integration.IMicrosoftExtensionsAIService 未找到，尝试无命名空间...");
        type_IMicrosoftExtensionsAIService = Type.GetType("IMicrosoftExtensionsAIService");
        if (type_IMicrosoftExtensionsAIService != null)
            Console.WriteLine("[PASS] 类型 IMicrosoftExtensionsAIService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMicrosoftExtensionsAIService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
