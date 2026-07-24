#load "high_frequency_100k.cs"

Console.WriteLine("=== high_frequency_100k.cs Test ===");

try
{
    // 验证 class: ZeroCopySqlProcessor
    var type_ZeroCopySqlProcessor = Type.GetType("ZeroCopySqlProcessor");
    if (type_ZeroCopySqlProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ZeroCopySqlProcessor (class) 存在");
        var ctors_ZeroCopySqlProcessor = type_ZeroCopySqlProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ZeroCopySqlProcessor 构造函数数量: {ctors_ZeroCopySqlProcessor.Length}");
        var methods_ZeroCopySqlProcessor = type_ZeroCopySqlProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZeroCopySqlProcessor 公开方法数量: {methods_ZeroCopySqlProcessor.Length}");
        foreach (var m in methods_ZeroCopySqlProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZeroCopySqlProcessor 未找到，尝试无命名空间...");
        type_ZeroCopySqlProcessor = Type.GetType("ZeroCopySqlProcessor");
        if (type_ZeroCopySqlProcessor != null)
            Console.WriteLine("[PASS] 类型 ZeroCopySqlProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZeroCopySqlProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
