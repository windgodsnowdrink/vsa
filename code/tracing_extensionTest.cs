#load "tracing_extension.cs"

Console.WriteLine("=== tracing_extension.cs Test ===");

try
{
    // 验证 class: TracingExtensions
    var type_TracingExtensions = Type.GetType("TracingExtensions");
    if (type_TracingExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 TracingExtensions (class) 存在");
        var ctors_TracingExtensions = type_TracingExtensions.GetConstructors();
        Console.WriteLine($"[PASS] TracingExtensions 构造函数数量: {ctors_TracingExtensions.Length}");
        var methods_TracingExtensions = type_TracingExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TracingExtensions 公开方法数量: {methods_TracingExtensions.Length}");
        foreach (var m in methods_TracingExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TracingExtensions 未找到，尝试无命名空间...");
        type_TracingExtensions = Type.GetType("TracingExtensions");
        if (type_TracingExtensions != null)
            Console.WriteLine("[PASS] 类型 TracingExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TracingExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
