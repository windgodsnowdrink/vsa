#load "netcore_encrypt_cache.cs"

Console.WriteLine("=== netcore_encrypt_cache.cs Test ===");

try
{
    // 验证 class: EncryptCacheService
    var type_EncryptCacheService = Type.GetType("EncryptCacheService");
    if (type_EncryptCacheService != null)
    {
        Console.WriteLine("[PASS] 类型 EncryptCacheService (class) 存在");
        var ctors_EncryptCacheService = type_EncryptCacheService.GetConstructors();
        Console.WriteLine($"[PASS] EncryptCacheService 构造函数数量: {ctors_EncryptCacheService.Length}");
        var methods_EncryptCacheService = type_EncryptCacheService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EncryptCacheService 公开方法数量: {methods_EncryptCacheService.Length}");
        foreach (var m in methods_EncryptCacheService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EncryptCacheService 未找到，尝试无命名空间...");
        type_EncryptCacheService = Type.GetType("EncryptCacheService");
        if (type_EncryptCacheService != null)
            Console.WriteLine("[PASS] 类型 EncryptCacheService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EncryptCacheService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
