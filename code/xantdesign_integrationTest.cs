#load "xantdesign_integration.cs"

Console.WriteLine("=== xantdesign_integration.cs Test ===");

try
{
    // 验证 class: XAntDesignIntegration.XAntDesignOptions
    var type_XAntDesignOptions = Type.GetType("XAntDesignIntegration.XAntDesignOptions");
    if (type_XAntDesignOptions != null)
    {
        Console.WriteLine("[PASS] 类型 XAntDesignIntegration.XAntDesignOptions (class) 存在");
        var ctors_XAntDesignOptions = type_XAntDesignOptions.GetConstructors();
        Console.WriteLine($"[PASS] XAntDesignIntegration.XAntDesignOptions 构造函数数量: {ctors_XAntDesignOptions.Length}");
        var methods_XAntDesignOptions = type_XAntDesignOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] XAntDesignIntegration.XAntDesignOptions 公开方法数量: {methods_XAntDesignOptions.Length}");
        foreach (var m in methods_XAntDesignOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 XAntDesignIntegration.XAntDesignOptions 未找到，尝试无命名空间...");
        type_XAntDesignOptions = Type.GetType("XAntDesignOptions");
        if (type_XAntDesignOptions != null)
            Console.WriteLine("[PASS] 类型 XAntDesignOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 XAntDesignOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: XAntDesignIntegration.XAntDesignService
    var type_XAntDesignService = Type.GetType("XAntDesignIntegration.XAntDesignService");
    if (type_XAntDesignService != null)
    {
        Console.WriteLine("[PASS] 类型 XAntDesignIntegration.XAntDesignService (class) 存在");
        var ctors_XAntDesignService = type_XAntDesignService.GetConstructors();
        Console.WriteLine($"[PASS] XAntDesignIntegration.XAntDesignService 构造函数数量: {ctors_XAntDesignService.Length}");
        var methods_XAntDesignService = type_XAntDesignService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] XAntDesignIntegration.XAntDesignService 公开方法数量: {methods_XAntDesignService.Length}");
        foreach (var m in methods_XAntDesignService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 XAntDesignIntegration.XAntDesignService 未找到，尝试无命名空间...");
        type_XAntDesignService = Type.GetType("XAntDesignService");
        if (type_XAntDesignService != null)
            Console.WriteLine("[PASS] 类型 XAntDesignService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 XAntDesignService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: XAntDesignIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("XAntDesignIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 XAntDesignIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] XAntDesignIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] XAntDesignIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 XAntDesignIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: XAntDesignIntegration.WebAssemblyHostBuilderExtensions
    var type_WebAssemblyHostBuilderExtensions = Type.GetType("XAntDesignIntegration.WebAssemblyHostBuilderExtensions");
    if (type_WebAssemblyHostBuilderExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 XAntDesignIntegration.WebAssemblyHostBuilderExtensions (class) 存在");
        var ctors_WebAssemblyHostBuilderExtensions = type_WebAssemblyHostBuilderExtensions.GetConstructors();
        Console.WriteLine($"[PASS] XAntDesignIntegration.WebAssemblyHostBuilderExtensions 构造函数数量: {ctors_WebAssemblyHostBuilderExtensions.Length}");
        var methods_WebAssemblyHostBuilderExtensions = type_WebAssemblyHostBuilderExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] XAntDesignIntegration.WebAssemblyHostBuilderExtensions 公开方法数量: {methods_WebAssemblyHostBuilderExtensions.Length}");
        foreach (var m in methods_WebAssemblyHostBuilderExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 XAntDesignIntegration.WebAssemblyHostBuilderExtensions 未找到，尝试无命名空间...");
        type_WebAssemblyHostBuilderExtensions = Type.GetType("WebAssemblyHostBuilderExtensions");
        if (type_WebAssemblyHostBuilderExtensions != null)
            Console.WriteLine("[PASS] 类型 WebAssemblyHostBuilderExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WebAssemblyHostBuilderExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: XAntDesignIntegration.IXAntDesignService
    var type_IXAntDesignService = Type.GetType("XAntDesignIntegration.IXAntDesignService");
    if (type_IXAntDesignService != null)
    {
        Console.WriteLine("[PASS] 类型 XAntDesignIntegration.IXAntDesignService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 XAntDesignIntegration.IXAntDesignService 未找到，尝试无命名空间...");
        type_IXAntDesignService = Type.GetType("IXAntDesignService");
        if (type_IXAntDesignService != null)
            Console.WriteLine("[PASS] 类型 IXAntDesignService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IXAntDesignService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
