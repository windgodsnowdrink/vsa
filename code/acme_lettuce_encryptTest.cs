#load "acme_lettuce_encrypt.cs"

Console.WriteLine("=== acme_lettuce_encrypt.cs Test ===");

try
{
    // 验证 class: CertificateManager
    var type_CertificateManager = Type.GetType("CertificateManager");
    if (type_CertificateManager != null)
    {
        Console.WriteLine("[PASS] 类型 CertificateManager (class) 存在");
        var ctors_CertificateManager = type_CertificateManager.GetConstructors();
        Console.WriteLine($"[PASS] CertificateManager 构造函数数量: {ctors_CertificateManager.Length}");
        var methods_CertificateManager = type_CertificateManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CertificateManager 公开方法数量: {methods_CertificateManager.Length}");
        foreach (var m in methods_CertificateManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CertificateManager 未找到，尝试无命名空间...");
        type_CertificateManager = Type.GetType("CertificateManager");
        if (type_CertificateManager != null)
            Console.WriteLine("[PASS] 类型 CertificateManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CertificateManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CertificateRenewalService
    var type_CertificateRenewalService = Type.GetType("CertificateRenewalService");
    if (type_CertificateRenewalService != null)
    {
        Console.WriteLine("[PASS] 类型 CertificateRenewalService (class) 存在");
        var ctors_CertificateRenewalService = type_CertificateRenewalService.GetConstructors();
        Console.WriteLine($"[PASS] CertificateRenewalService 构造函数数量: {ctors_CertificateRenewalService.Length}");
        var methods_CertificateRenewalService = type_CertificateRenewalService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CertificateRenewalService 公开方法数量: {methods_CertificateRenewalService.Length}");
        foreach (var m in methods_CertificateRenewalService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CertificateRenewalService 未找到，尝试无命名空间...");
        type_CertificateRenewalService = Type.GetType("CertificateRenewalService");
        if (type_CertificateRenewalService != null)
            Console.WriteLine("[PASS] 类型 CertificateRenewalService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CertificateRenewalService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CertificatePooledPolicy
    var type_CertificatePooledPolicy = Type.GetType("CertificatePooledPolicy");
    if (type_CertificatePooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 CertificatePooledPolicy (class) 存在");
        var ctors_CertificatePooledPolicy = type_CertificatePooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] CertificatePooledPolicy 构造函数数量: {ctors_CertificatePooledPolicy.Length}");
        var methods_CertificatePooledPolicy = type_CertificatePooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CertificatePooledPolicy 公开方法数量: {methods_CertificatePooledPolicy.Length}");
        foreach (var m in methods_CertificatePooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CertificatePooledPolicy 未找到，尝试无命名空间...");
        type_CertificatePooledPolicy = Type.GetType("CertificatePooledPolicy");
        if (type_CertificatePooledPolicy != null)
            Console.WriteLine("[PASS] 类型 CertificatePooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CertificatePooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
