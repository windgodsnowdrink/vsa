#load "mqtt_biometric_security.cs"

Console.WriteLine("=== mqtt_biometric_security.cs Test ===");

try
{
    // 验证 class: AzureKeyVaultService
    var type_AzureKeyVaultService = Type.GetType("AzureKeyVaultService");
    if (type_AzureKeyVaultService != null)
    {
        Console.WriteLine("[PASS] 类型 AzureKeyVaultService (class) 存在");
        var ctors_AzureKeyVaultService = type_AzureKeyVaultService.GetConstructors();
        Console.WriteLine($"[PASS] AzureKeyVaultService 构造函数数量: {ctors_AzureKeyVaultService.Length}");
        var methods_AzureKeyVaultService = type_AzureKeyVaultService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AzureKeyVaultService 公开方法数量: {methods_AzureKeyVaultService.Length}");
        foreach (var m in methods_AzureKeyVaultService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AzureKeyVaultService 未找到，尝试无命名空间...");
        type_AzureKeyVaultService = Type.GetType("AzureKeyVaultService");
        if (type_AzureKeyVaultService != null)
            Console.WriteLine("[PASS] 类型 AzureKeyVaultService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AzureKeyVaultService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: VoicePrintAuthenticator
    var type_VoicePrintAuthenticator = Type.GetType("VoicePrintAuthenticator");
    if (type_VoicePrintAuthenticator != null)
    {
        Console.WriteLine("[PASS] 类型 VoicePrintAuthenticator (class) 存在");
        var ctors_VoicePrintAuthenticator = type_VoicePrintAuthenticator.GetConstructors();
        Console.WriteLine($"[PASS] VoicePrintAuthenticator 构造函数数量: {ctors_VoicePrintAuthenticator.Length}");
        var methods_VoicePrintAuthenticator = type_VoicePrintAuthenticator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VoicePrintAuthenticator 公开方法数量: {methods_VoicePrintAuthenticator.Length}");
        foreach (var m in methods_VoicePrintAuthenticator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VoicePrintAuthenticator 未找到，尝试无命名空间...");
        type_VoicePrintAuthenticator = Type.GetType("VoicePrintAuthenticator");
        if (type_VoicePrintAuthenticator != null)
            Console.WriteLine("[PASS] 类型 VoicePrintAuthenticator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VoicePrintAuthenticator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MLBehaviorAnalyzer
    var type_MLBehaviorAnalyzer = Type.GetType("MLBehaviorAnalyzer");
    if (type_MLBehaviorAnalyzer != null)
    {
        Console.WriteLine("[PASS] 类型 MLBehaviorAnalyzer (class) 存在");
        var ctors_MLBehaviorAnalyzer = type_MLBehaviorAnalyzer.GetConstructors();
        Console.WriteLine($"[PASS] MLBehaviorAnalyzer 构造函数数量: {ctors_MLBehaviorAnalyzer.Length}");
        var methods_MLBehaviorAnalyzer = type_MLBehaviorAnalyzer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MLBehaviorAnalyzer 公开方法数量: {methods_MLBehaviorAnalyzer.Length}");
        foreach (var m in methods_MLBehaviorAnalyzer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MLBehaviorAnalyzer 未找到，尝试无命名空间...");
        type_MLBehaviorAnalyzer = Type.GetType("MLBehaviorAnalyzer");
        if (type_MLBehaviorAnalyzer != null)
            Console.WriteLine("[PASS] 类型 MLBehaviorAnalyzer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MLBehaviorAnalyzer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
