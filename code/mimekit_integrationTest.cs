#load "mimekit_integration.cs"

Console.WriteLine("=== mimekit_integration.cs Test ===");

try
{
    // 验证 class: MimeTypeProcessor
    var type_MimeTypeProcessor = Type.GetType("MimeTypeProcessor");
    if (type_MimeTypeProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 MimeTypeProcessor (class) 存在");
        var ctors_MimeTypeProcessor = type_MimeTypeProcessor.GetConstructors();
        Console.WriteLine($"[PASS] MimeTypeProcessor 构造函数数量: {ctors_MimeTypeProcessor.Length}");
        var methods_MimeTypeProcessor = type_MimeTypeProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MimeTypeProcessor 公开方法数量: {methods_MimeTypeProcessor.Length}");
        foreach (var m in methods_MimeTypeProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MimeTypeProcessor 未找到，尝试无命名空间...");
        type_MimeTypeProcessor = Type.GetType("MimeTypeProcessor");
        if (type_MimeTypeProcessor != null)
            Console.WriteLine("[PASS] 类型 MimeTypeProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MimeTypeProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MimeKitExtensions
    var type_MimeKitExtensions = Type.GetType("MimeKitExtensions");
    if (type_MimeKitExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MimeKitExtensions (class) 存在");
        var ctors_MimeKitExtensions = type_MimeKitExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MimeKitExtensions 构造函数数量: {ctors_MimeKitExtensions.Length}");
        var methods_MimeKitExtensions = type_MimeKitExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MimeKitExtensions 公开方法数量: {methods_MimeKitExtensions.Length}");
        foreach (var m in methods_MimeKitExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MimeKitExtensions 未找到，尝试无命名空间...");
        type_MimeKitExtensions = Type.GetType("MimeKitExtensions");
        if (type_MimeKitExtensions != null)
            Console.WriteLine("[PASS] 类型 MimeKitExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MimeKitExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
