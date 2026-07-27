#load "acme_easy_rsa_acme.cs"

Console.WriteLine("=== acme_easy_rsa_acme.cs Test ===");

try
{
    // 验证 class: AcmeCertificateService
    var type_AcmeCertificateService = Type.GetType("AcmeCertificateService");
    if (type_AcmeCertificateService != null)
    {
        Console.WriteLine("[PASS] 类型 AcmeCertificateService (class) 存在");
        var ctors_AcmeCertificateService = type_AcmeCertificateService.GetConstructors();
        Console.WriteLine($"[PASS] AcmeCertificateService 构造函数数量: {ctors_AcmeCertificateService.Length}");
        var methods_AcmeCertificateService = type_AcmeCertificateService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AcmeCertificateService 公开方法数量: {methods_AcmeCertificateService.Length}");
        foreach (var m in methods_AcmeCertificateService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AcmeCertificateService 未找到，尝试无命名空间...");
        type_AcmeCertificateService = Type.GetType("AcmeCertificateService");
        if (type_AcmeCertificateService != null)
            Console.WriteLine("[PASS] 类型 AcmeCertificateService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AcmeCertificateService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RsaPooledPolicy
    var type_RsaPooledPolicy = Type.GetType("RsaPooledPolicy");
    if (type_RsaPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 RsaPooledPolicy (class) 存在");
        var ctors_RsaPooledPolicy = type_RsaPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] RsaPooledPolicy 构造函数数量: {ctors_RsaPooledPolicy.Length}");
        var methods_RsaPooledPolicy = type_RsaPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RsaPooledPolicy 公开方法数量: {methods_RsaPooledPolicy.Length}");
        foreach (var m in methods_RsaPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RsaPooledPolicy 未找到，尝试无命名空间...");
        type_RsaPooledPolicy = Type.GetType("RsaPooledPolicy");
        if (type_RsaPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 RsaPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RsaPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
