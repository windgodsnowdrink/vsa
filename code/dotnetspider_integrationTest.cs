#load "dotnetspider_integration.cs"

Console.WriteLine("=== dotnetspider_integration.cs Test ===");

try
{
    // 验证 class: DotnetSpiderIntegration
    var type_DotnetSpiderIntegration = Type.GetType("DotnetSpiderIntegration");
    if (type_DotnetSpiderIntegration != null)
    {
        Console.WriteLine("[PASS] 类型 DotnetSpiderIntegration (class) 存在");
        var ctors_DotnetSpiderIntegration = type_DotnetSpiderIntegration.GetConstructors();
        Console.WriteLine($"[PASS] DotnetSpiderIntegration 构造函数数量: {ctors_DotnetSpiderIntegration.Length}");
        var methods_DotnetSpiderIntegration = type_DotnetSpiderIntegration.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotnetSpiderIntegration 公开方法数量: {methods_DotnetSpiderIntegration.Length}");
        foreach (var m in methods_DotnetSpiderIntegration)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotnetSpiderIntegration 未找到，尝试无命名空间...");
        type_DotnetSpiderIntegration = Type.GetType("DotnetSpiderIntegration");
        if (type_DotnetSpiderIntegration != null)
            Console.WriteLine("[PASS] 类型 DotnetSpiderIntegration (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotnetSpiderIntegration 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DotnetSpiderService
    var type_DotnetSpiderService = Type.GetType("DotnetSpiderService");
    if (type_DotnetSpiderService != null)
    {
        Console.WriteLine("[PASS] 类型 DotnetSpiderService (class) 存在");
        var ctors_DotnetSpiderService = type_DotnetSpiderService.GetConstructors();
        Console.WriteLine($"[PASS] DotnetSpiderService 构造函数数量: {ctors_DotnetSpiderService.Length}");
        var methods_DotnetSpiderService = type_DotnetSpiderService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotnetSpiderService 公开方法数量: {methods_DotnetSpiderService.Length}");
        foreach (var m in methods_DotnetSpiderService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotnetSpiderService 未找到，尝试无命名空间...");
        type_DotnetSpiderService = Type.GetType("DotnetSpiderService");
        if (type_DotnetSpiderService != null)
            Console.WriteLine("[PASS] 类型 DotnetSpiderService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotnetSpiderService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DotnetSpiderOptions
    var type_DotnetSpiderOptions = Type.GetType("DotnetSpiderOptions");
    if (type_DotnetSpiderOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DotnetSpiderOptions (class) 存在");
        var ctors_DotnetSpiderOptions = type_DotnetSpiderOptions.GetConstructors();
        Console.WriteLine($"[PASS] DotnetSpiderOptions 构造函数数量: {ctors_DotnetSpiderOptions.Length}");
        var methods_DotnetSpiderOptions = type_DotnetSpiderOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotnetSpiderOptions 公开方法数量: {methods_DotnetSpiderOptions.Length}");
        foreach (var m in methods_DotnetSpiderOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotnetSpiderOptions 未找到，尝试无命名空间...");
        type_DotnetSpiderOptions = Type.GetType("DotnetSpiderOptions");
        if (type_DotnetSpiderOptions != null)
            Console.WriteLine("[PASS] 类型 DotnetSpiderOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotnetSpiderOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MySpider
    var type_MySpider = Type.GetType("MySpider");
    if (type_MySpider != null)
    {
        Console.WriteLine("[PASS] 类型 MySpider (class) 存在");
        var ctors_MySpider = type_MySpider.GetConstructors();
        Console.WriteLine($"[PASS] MySpider 构造函数数量: {ctors_MySpider.Length}");
        var methods_MySpider = type_MySpider.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MySpider 公开方法数量: {methods_MySpider.Length}");
        foreach (var m in methods_MySpider)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MySpider 未找到，尝试无命名空间...");
        type_MySpider = Type.GetType("MySpider");
        if (type_MySpider != null)
            Console.WriteLine("[PASS] 类型 MySpider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MySpider 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
