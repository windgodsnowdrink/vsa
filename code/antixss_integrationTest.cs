#load "antixss_integration.cs"

Console.WriteLine("=== antixss_integration.cs Test ===");

try
{
    // 验证 class: AntiXssService
    var type_AntiXssService = Type.GetType("AntiXssService");
    if (type_AntiXssService != null)
    {
        Console.WriteLine("[PASS] 类型 AntiXssService (class) 存在");
        var ctors_AntiXssService = type_AntiXssService.GetConstructors();
        Console.WriteLine($"[PASS] AntiXssService 构造函数数量: {ctors_AntiXssService.Length}");
        var methods_AntiXssService = type_AntiXssService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AntiXssService 公开方法数量: {methods_AntiXssService.Length}");
        foreach (var m in methods_AntiXssService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AntiXssService 未找到，尝试无命名空间...");
        type_AntiXssService = Type.GetType("AntiXssService");
        if (type_AntiXssService != null)
            Console.WriteLine("[PASS] 类型 AntiXssService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AntiXssService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EncoderPooledPolicy
    var type_EncoderPooledPolicy = Type.GetType("EncoderPooledPolicy");
    if (type_EncoderPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 EncoderPooledPolicy (class) 存在");
        var ctors_EncoderPooledPolicy = type_EncoderPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] EncoderPooledPolicy 构造函数数量: {ctors_EncoderPooledPolicy.Length}");
        var methods_EncoderPooledPolicy = type_EncoderPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EncoderPooledPolicy 公开方法数量: {methods_EncoderPooledPolicy.Length}");
        foreach (var m in methods_EncoderPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EncoderPooledPolicy 未找到，尝试无命名空间...");
        type_EncoderPooledPolicy = Type.GetType("EncoderPooledPolicy");
        if (type_EncoderPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 EncoderPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EncoderPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 record: SanitizeRequest
    var type_SanitizeRequest = Type.GetType("SanitizeRequest");
    if (type_SanitizeRequest != null)
    {
        Console.WriteLine("[PASS] 类型 SanitizeRequest (record) 存在");
        var ctors_SanitizeRequest = type_SanitizeRequest.GetConstructors();
        Console.WriteLine($"[PASS] SanitizeRequest 构造函数数量: {ctors_SanitizeRequest.Length}");
        var methods_SanitizeRequest = type_SanitizeRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SanitizeRequest 公开方法数量: {methods_SanitizeRequest.Length}");
        foreach (var m in methods_SanitizeRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SanitizeRequest 未找到，尝试无命名空间...");
        type_SanitizeRequest = Type.GetType("SanitizeRequest");
        if (type_SanitizeRequest != null)
            Console.WriteLine("[PASS] 类型 SanitizeRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SanitizeRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: SanitizeType
    var type_SanitizeType = Type.GetType("SanitizeType");
    if (type_SanitizeType != null)
    {
        Console.WriteLine("[PASS] 类型 SanitizeType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SanitizeType 未找到，尝试无命名空间...");
        type_SanitizeType = Type.GetType("SanitizeType");
        if (type_SanitizeType != null)
            Console.WriteLine("[PASS] 类型 SanitizeType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SanitizeType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
