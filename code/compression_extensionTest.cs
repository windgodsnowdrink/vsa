#load "compression_extension.cs"

Console.WriteLine("=== compression_extension.cs Test ===");

try
{
    // 验证 class: MessageCompressor
    var type_MessageCompressor = Type.GetType("MessageCompressor");
    if (type_MessageCompressor != null)
    {
        Console.WriteLine("[PASS] 类型 MessageCompressor (class) 存在");
        var ctors_MessageCompressor = type_MessageCompressor.GetConstructors();
        Console.WriteLine($"[PASS] MessageCompressor 构造函数数量: {ctors_MessageCompressor.Length}");
        var methods_MessageCompressor = type_MessageCompressor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageCompressor 公开方法数量: {methods_MessageCompressor.Length}");
        foreach (var m in methods_MessageCompressor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageCompressor 未找到，尝试无命名空间...");
        type_MessageCompressor = Type.GetType("MessageCompressor");
        if (type_MessageCompressor != null)
            Console.WriteLine("[PASS] 类型 MessageCompressor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageCompressor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
