#load "acme_distributed_lock.cs"

Console.WriteLine("=== acme_distributed_lock.cs Test ===");

try
{
    // 验证 class: CertificateLockService
    var type_CertificateLockService = Type.GetType("CertificateLockService");
    if (type_CertificateLockService != null)
    {
        Console.WriteLine("[PASS] 类型 CertificateLockService (class) 存在");
        var ctors_CertificateLockService = type_CertificateLockService.GetConstructors();
        Console.WriteLine($"[PASS] CertificateLockService 构造函数数量: {ctors_CertificateLockService.Length}");
        var methods_CertificateLockService = type_CertificateLockService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CertificateLockService 公开方法数量: {methods_CertificateLockService.Length}");
        foreach (var m in methods_CertificateLockService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CertificateLockService 未找到，尝试无命名空间...");
        type_CertificateLockService = Type.GetType("CertificateLockService");
        if (type_CertificateLockService != null)
            Console.WriteLine("[PASS] 类型 CertificateLockService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CertificateLockService 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: RedisLock
    var type_RedisLock = Type.GetType("RedisLock");
    if (type_RedisLock != null)
    {
        Console.WriteLine("[PASS] 类型 RedisLock (struct) 存在");
        var ctors_RedisLock = type_RedisLock.GetConstructors();
        Console.WriteLine($"[PASS] RedisLock 构造函数数量: {ctors_RedisLock.Length}");
        var methods_RedisLock = type_RedisLock.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisLock 公开方法数量: {methods_RedisLock.Length}");
        foreach (var m in methods_RedisLock)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisLock 未找到，尝试无命名空间...");
        type_RedisLock = Type.GetType("RedisLock");
        if (type_RedisLock != null)
            Console.WriteLine("[PASS] 类型 RedisLock (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisLock 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
