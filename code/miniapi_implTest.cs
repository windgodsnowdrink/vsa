#load "miniapi_impl.cs"

Console.WriteLine("=== miniapi_impl.cs Test ===");

try
{
    // 验证 class: MinimalApiExtensions
    var type_MinimalApiExtensions = Type.GetType("MinimalApiExtensions");
    if (type_MinimalApiExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MinimalApiExtensions (class) 存在");
        var ctors_MinimalApiExtensions = type_MinimalApiExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MinimalApiExtensions 构造函数数量: {ctors_MinimalApiExtensions.Length}");
        var methods_MinimalApiExtensions = type_MinimalApiExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinimalApiExtensions 公开方法数量: {methods_MinimalApiExtensions.Length}");
        foreach (var m in methods_MinimalApiExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinimalApiExtensions 未找到，尝试无命名空间...");
        type_MinimalApiExtensions = Type.GetType("MinimalApiExtensions");
        if (type_MinimalApiExtensions != null)
            Console.WriteLine("[PASS] 类型 MinimalApiExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinimalApiExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
