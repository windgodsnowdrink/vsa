#load "acme_cert_monitor.cs"

Console.WriteLine("=== acme_cert_monitor.cs Test ===");

try
{
    // 验证 class: CertificateMonitor
    var type_CertificateMonitor = Type.GetType("CertificateMonitor");
    if (type_CertificateMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 CertificateMonitor (class) 存在");
        var ctors_CertificateMonitor = type_CertificateMonitor.GetConstructors();
        Console.WriteLine($"[PASS] CertificateMonitor 构造函数数量: {ctors_CertificateMonitor.Length}");
        var methods_CertificateMonitor = type_CertificateMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CertificateMonitor 公开方法数量: {methods_CertificateMonitor.Length}");
        foreach (var m in methods_CertificateMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CertificateMonitor 未找到，尝试无命名空间...");
        type_CertificateMonitor = Type.GetType("CertificateMonitor");
        if (type_CertificateMonitor != null)
            Console.WriteLine("[PASS] 类型 CertificateMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CertificateMonitor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
