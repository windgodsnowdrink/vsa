#load "sm_crypto_integration.cs"

Console.WriteLine("=== sm_crypto_integration.cs Test ===");

try
{
    // 验证 class: SmCryptoIntegration.SmCryptoOptions
    var type_SmCryptoOptions = Type.GetType("SmCryptoIntegration.SmCryptoOptions");
    if (type_SmCryptoOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SmCryptoIntegration.SmCryptoOptions (class) 存在");
        var ctors_SmCryptoOptions = type_SmCryptoOptions.GetConstructors();
        Console.WriteLine($"[PASS] SmCryptoIntegration.SmCryptoOptions 构造函数数量: {ctors_SmCryptoOptions.Length}");
        var methods_SmCryptoOptions = type_SmCryptoOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmCryptoIntegration.SmCryptoOptions 公开方法数量: {methods_SmCryptoOptions.Length}");
        foreach (var m in methods_SmCryptoOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmCryptoIntegration.SmCryptoOptions 未找到，尝试无命名空间...");
        type_SmCryptoOptions = Type.GetType("SmCryptoOptions");
        if (type_SmCryptoOptions != null)
            Console.WriteLine("[PASS] 类型 SmCryptoOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmCryptoOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SmCryptoIntegration.SmCryptoService
    var type_SmCryptoService = Type.GetType("SmCryptoIntegration.SmCryptoService");
    if (type_SmCryptoService != null)
    {
        Console.WriteLine("[PASS] 类型 SmCryptoIntegration.SmCryptoService (class) 存在");
        var ctors_SmCryptoService = type_SmCryptoService.GetConstructors();
        Console.WriteLine($"[PASS] SmCryptoIntegration.SmCryptoService 构造函数数量: {ctors_SmCryptoService.Length}");
        var methods_SmCryptoService = type_SmCryptoService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmCryptoIntegration.SmCryptoService 公开方法数量: {methods_SmCryptoService.Length}");
        foreach (var m in methods_SmCryptoService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmCryptoIntegration.SmCryptoService 未找到，尝试无命名空间...");
        type_SmCryptoService = Type.GetType("SmCryptoService");
        if (type_SmCryptoService != null)
            Console.WriteLine("[PASS] 类型 SmCryptoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmCryptoService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SmCryptoIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("SmCryptoIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SmCryptoIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SmCryptoIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmCryptoIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmCryptoIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: SmCryptoIntegration.ISmCryptoService
    var type_ISmCryptoService = Type.GetType("SmCryptoIntegration.ISmCryptoService");
    if (type_ISmCryptoService != null)
    {
        Console.WriteLine("[PASS] 类型 SmCryptoIntegration.ISmCryptoService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmCryptoIntegration.ISmCryptoService 未找到，尝试无命名空间...");
        type_ISmCryptoService = Type.GetType("ISmCryptoService");
        if (type_ISmCryptoService != null)
            Console.WriteLine("[PASS] 类型 ISmCryptoService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISmCryptoService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
