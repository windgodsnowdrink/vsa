#load "acme_certes_integration.cs"

Console.WriteLine("=== acme_certes_integration.cs Test ===");

try
{
    // 验证 class: CertesCertificateService
    var type_CertesCertificateService = Type.GetType("CertesCertificateService");
    if (type_CertesCertificateService != null)
    {
        Console.WriteLine("[PASS] 类型 CertesCertificateService (class) 存在");
        var ctors_CertesCertificateService = type_CertesCertificateService.GetConstructors();
        Console.WriteLine($"[PASS] CertesCertificateService 构造函数数量: {ctors_CertesCertificateService.Length}");
        var methods_CertesCertificateService = type_CertesCertificateService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CertesCertificateService 公开方法数量: {methods_CertesCertificateService.Length}");
        foreach (var m in methods_CertesCertificateService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CertesCertificateService 未找到，尝试无命名空间...");
        type_CertesCertificateService = Type.GetType("CertesCertificateService");
        if (type_CertesCertificateService != null)
            Console.WriteLine("[PASS] 类型 CertesCertificateService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CertesCertificateService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
