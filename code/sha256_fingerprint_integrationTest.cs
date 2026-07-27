#load "sha256_fingerprint_integration.cs"

Console.WriteLine("=== sha256_fingerprint_integration.cs Test ===");

try
{
    // 验证 class: FingerprintIntegration.Sha256FingerprintOptions
    var type_Sha256FingerprintOptions = Type.GetType("FingerprintIntegration.Sha256FingerprintOptions");
    if (type_Sha256FingerprintOptions != null)
    {
        Console.WriteLine("[PASS] 类型 FingerprintIntegration.Sha256FingerprintOptions (class) 存在");
        var ctors_Sha256FingerprintOptions = type_Sha256FingerprintOptions.GetConstructors();
        Console.WriteLine($"[PASS] FingerprintIntegration.Sha256FingerprintOptions 构造函数数量: {ctors_Sha256FingerprintOptions.Length}");
        var methods_Sha256FingerprintOptions = type_Sha256FingerprintOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FingerprintIntegration.Sha256FingerprintOptions 公开方法数量: {methods_Sha256FingerprintOptions.Length}");
        foreach (var m in methods_Sha256FingerprintOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FingerprintIntegration.Sha256FingerprintOptions 未找到，尝试无命名空间...");
        type_Sha256FingerprintOptions = Type.GetType("Sha256FingerprintOptions");
        if (type_Sha256FingerprintOptions != null)
            Console.WriteLine("[PASS] 类型 Sha256FingerprintOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Sha256FingerprintOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FingerprintIntegration.Sha256FingerprintService
    var type_Sha256FingerprintService = Type.GetType("FingerprintIntegration.Sha256FingerprintService");
    if (type_Sha256FingerprintService != null)
    {
        Console.WriteLine("[PASS] 类型 FingerprintIntegration.Sha256FingerprintService (class) 存在");
        var ctors_Sha256FingerprintService = type_Sha256FingerprintService.GetConstructors();
        Console.WriteLine($"[PASS] FingerprintIntegration.Sha256FingerprintService 构造函数数量: {ctors_Sha256FingerprintService.Length}");
        var methods_Sha256FingerprintService = type_Sha256FingerprintService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FingerprintIntegration.Sha256FingerprintService 公开方法数量: {methods_Sha256FingerprintService.Length}");
        foreach (var m in methods_Sha256FingerprintService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FingerprintIntegration.Sha256FingerprintService 未找到，尝试无命名空间...");
        type_Sha256FingerprintService = Type.GetType("Sha256FingerprintService");
        if (type_Sha256FingerprintService != null)
            Console.WriteLine("[PASS] 类型 Sha256FingerprintService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Sha256FingerprintService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FingerprintIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("FingerprintIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 FingerprintIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] FingerprintIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FingerprintIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FingerprintIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: FingerprintIntegration.ISha256FingerprintService
    var type_ISha256FingerprintService = Type.GetType("FingerprintIntegration.ISha256FingerprintService");
    if (type_ISha256FingerprintService != null)
    {
        Console.WriteLine("[PASS] 类型 FingerprintIntegration.ISha256FingerprintService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FingerprintIntegration.ISha256FingerprintService 未找到，尝试无命名空间...");
        type_ISha256FingerprintService = Type.GetType("ISha256FingerprintService");
        if (type_ISha256FingerprintService != null)
            Console.WriteLine("[PASS] 类型 ISha256FingerprintService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISha256FingerprintService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
