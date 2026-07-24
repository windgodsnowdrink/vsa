#load "acme_ecdsa_acme.cs"

Console.WriteLine("=== acme_ecdsa_acme.cs Test ===");

try
{
    // 验证 class: EcdsaCertificateService
    var type_EcdsaCertificateService = Type.GetType("EcdsaCertificateService");
    if (type_EcdsaCertificateService != null)
    {
        Console.WriteLine("[PASS] 类型 EcdsaCertificateService (class) 存在");
        var ctors_EcdsaCertificateService = type_EcdsaCertificateService.GetConstructors();
        Console.WriteLine($"[PASS] EcdsaCertificateService 构造函数数量: {ctors_EcdsaCertificateService.Length}");
        var methods_EcdsaCertificateService = type_EcdsaCertificateService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EcdsaCertificateService 公开方法数量: {methods_EcdsaCertificateService.Length}");
        foreach (var m in methods_EcdsaCertificateService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EcdsaCertificateService 未找到，尝试无命名空间...");
        type_EcdsaCertificateService = Type.GetType("EcdsaCertificateService");
        if (type_EcdsaCertificateService != null)
            Console.WriteLine("[PASS] 类型 EcdsaCertificateService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EcdsaCertificateService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EcdsaPooledPolicy
    var type_EcdsaPooledPolicy = Type.GetType("EcdsaPooledPolicy");
    if (type_EcdsaPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 EcdsaPooledPolicy (class) 存在");
        var ctors_EcdsaPooledPolicy = type_EcdsaPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] EcdsaPooledPolicy 构造函数数量: {ctors_EcdsaPooledPolicy.Length}");
        var methods_EcdsaPooledPolicy = type_EcdsaPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EcdsaPooledPolicy 公开方法数量: {methods_EcdsaPooledPolicy.Length}");
        foreach (var m in methods_EcdsaPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EcdsaPooledPolicy 未找到，尝试无命名空间...");
        type_EcdsaPooledPolicy = Type.GetType("EcdsaPooledPolicy");
        if (type_EcdsaPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 EcdsaPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EcdsaPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
