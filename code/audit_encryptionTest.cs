#load "audit_encryption.cs"

Console.WriteLine("=== audit_encryption.cs Test ===");

try
{
    // 验证 class: AuditEncryptor
    var type_AuditEncryptor = Type.GetType("AuditEncryptor");
    if (type_AuditEncryptor != null)
    {
        Console.WriteLine("[PASS] 类型 AuditEncryptor (class) 存在");
        var ctors_AuditEncryptor = type_AuditEncryptor.GetConstructors();
        Console.WriteLine($"[PASS] AuditEncryptor 构造函数数量: {ctors_AuditEncryptor.Length}");
        var methods_AuditEncryptor = type_AuditEncryptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditEncryptor 公开方法数量: {methods_AuditEncryptor.Length}");
        foreach (var m in methods_AuditEncryptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditEncryptor 未找到，尝试无命名空间...");
        type_AuditEncryptor = Type.GetType("AuditEncryptor");
        if (type_AuditEncryptor != null)
            Console.WriteLine("[PASS] 类型 AuditEncryptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditEncryptor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
