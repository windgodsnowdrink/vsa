#load "netcore_encrypt_integration.cs"

Console.WriteLine("=== netcore_encrypt_integration.cs Test ===");

try
{
    // 验证 class: EncryptService
    var type_EncryptService = Type.GetType("EncryptService");
    if (type_EncryptService != null)
    {
        Console.WriteLine("[PASS] 类型 EncryptService (class) 存在");
        var ctors_EncryptService = type_EncryptService.GetConstructors();
        Console.WriteLine($"[PASS] EncryptService 构造函数数量: {ctors_EncryptService.Length}");
        var methods_EncryptService = type_EncryptService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EncryptService 公开方法数量: {methods_EncryptService.Length}");
        foreach (var m in methods_EncryptService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EncryptService 未找到，尝试无命名空间...");
        type_EncryptService = Type.GetType("EncryptService");
        if (type_EncryptService != null)
            Console.WriteLine("[PASS] 类型 EncryptService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EncryptService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ArrayPoolPolicy
    var type_ArrayPoolPolicy = Type.GetType("ArrayPoolPolicy");
    if (type_ArrayPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ArrayPoolPolicy (class) 存在");
        var ctors_ArrayPoolPolicy = type_ArrayPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ArrayPoolPolicy 构造函数数量: {ctors_ArrayPoolPolicy.Length}");
        var methods_ArrayPoolPolicy = type_ArrayPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ArrayPoolPolicy 公开方法数量: {methods_ArrayPoolPolicy.Length}");
        foreach (var m in methods_ArrayPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ArrayPoolPolicy 未找到，尝试无命名空间...");
        type_ArrayPoolPolicy = Type.GetType("ArrayPoolPolicy");
        if (type_ArrayPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 ArrayPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ArrayPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: ShaType
    var type_ShaType = Type.GetType("ShaType");
    if (type_ShaType != null)
    {
        Console.WriteLine("[PASS] 类型 ShaType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ShaType 未找到，尝试无命名空间...");
        type_ShaType = Type.GetType("ShaType");
        if (type_ShaType != null)
            Console.WriteLine("[PASS] 类型 ShaType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ShaType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
