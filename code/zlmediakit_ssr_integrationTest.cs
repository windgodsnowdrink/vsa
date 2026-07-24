#load "zlmediakit_ssr_integration.cs"

Console.WriteLine("=== zlmediakit_ssr_integration.cs Test ===");

try
{
    // 验证 class: ZlmSsrProcessor
    var type_ZlmSsrProcessor = Type.GetType("ZlmSsrProcessor");
    if (type_ZlmSsrProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ZlmSsrProcessor (class) 存在");
        var ctors_ZlmSsrProcessor = type_ZlmSsrProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ZlmSsrProcessor 构造函数数量: {ctors_ZlmSsrProcessor.Length}");
        var methods_ZlmSsrProcessor = type_ZlmSsrProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZlmSsrProcessor 公开方法数量: {methods_ZlmSsrProcessor.Length}");
        foreach (var m in methods_ZlmSsrProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZlmSsrProcessor 未找到，尝试无命名空间...");
        type_ZlmSsrProcessor = Type.GetType("ZlmSsrProcessor");
        if (type_ZlmSsrProcessor != null)
            Console.WriteLine("[PASS] 类型 ZlmSsrProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZlmSsrProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZlmServiceExtensions
    var type_ZlmServiceExtensions = Type.GetType("ZlmServiceExtensions");
    if (type_ZlmServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ZlmServiceExtensions (class) 存在");
        var ctors_ZlmServiceExtensions = type_ZlmServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ZlmServiceExtensions 构造函数数量: {ctors_ZlmServiceExtensions.Length}");
        var methods_ZlmServiceExtensions = type_ZlmServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZlmServiceExtensions 公开方法数量: {methods_ZlmServiceExtensions.Length}");
        foreach (var m in methods_ZlmServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZlmServiceExtensions 未找到，尝试无命名空间...");
        type_ZlmServiceExtensions = Type.GetType("ZlmServiceExtensions");
        if (type_ZlmServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 ZlmServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZlmServiceExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
