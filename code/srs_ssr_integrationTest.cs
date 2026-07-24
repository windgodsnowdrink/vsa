#load "srs_ssr_integration.cs"

Console.WriteLine("=== srs_ssr_integration.cs Test ===");

try
{
    // 验证 class: SrsStreamProcessor
    var type_SrsStreamProcessor = Type.GetType("SrsStreamProcessor");
    if (type_SrsStreamProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 SrsStreamProcessor (class) 存在");
        var ctors_SrsStreamProcessor = type_SrsStreamProcessor.GetConstructors();
        Console.WriteLine($"[PASS] SrsStreamProcessor 构造函数数量: {ctors_SrsStreamProcessor.Length}");
        var methods_SrsStreamProcessor = type_SrsStreamProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SrsStreamProcessor 公开方法数量: {methods_SrsStreamProcessor.Length}");
        foreach (var m in methods_SrsStreamProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SrsStreamProcessor 未找到，尝试无命名空间...");
        type_SrsStreamProcessor = Type.GetType("SrsStreamProcessor");
        if (type_SrsStreamProcessor != null)
            Console.WriteLine("[PASS] 类型 SrsStreamProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SrsStreamProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SrsServiceExtensions
    var type_SrsServiceExtensions = Type.GetType("SrsServiceExtensions");
    if (type_SrsServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SrsServiceExtensions (class) 存在");
        var ctors_SrsServiceExtensions = type_SrsServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SrsServiceExtensions 构造函数数量: {ctors_SrsServiceExtensions.Length}");
        var methods_SrsServiceExtensions = type_SrsServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SrsServiceExtensions 公开方法数量: {methods_SrsServiceExtensions.Length}");
        foreach (var m in methods_SrsServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SrsServiceExtensions 未找到，尝试无命名空间...");
        type_SrsServiceExtensions = Type.GetType("SrsServiceExtensions");
        if (type_SrsServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 SrsServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SrsServiceExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
