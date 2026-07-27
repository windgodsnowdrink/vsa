#load "fastendpoints_impl.cs"

Console.WriteLine("=== fastendpoints_impl.cs Test ===");

try
{
    // 验证 class: FastEndpointsExtensions
    var type_FastEndpointsExtensions = Type.GetType("FastEndpointsExtensions");
    if (type_FastEndpointsExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 FastEndpointsExtensions (class) 存在");
        var ctors_FastEndpointsExtensions = type_FastEndpointsExtensions.GetConstructors();
        Console.WriteLine($"[PASS] FastEndpointsExtensions 构造函数数量: {ctors_FastEndpointsExtensions.Length}");
        var methods_FastEndpointsExtensions = type_FastEndpointsExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FastEndpointsExtensions 公开方法数量: {methods_FastEndpointsExtensions.Length}");
        foreach (var m in methods_FastEndpointsExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FastEndpointsExtensions 未找到，尝试无命名空间...");
        type_FastEndpointsExtensions = Type.GetType("FastEndpointsExtensions");
        if (type_FastEndpointsExtensions != null)
            Console.WriteLine("[PASS] 类型 FastEndpointsExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FastEndpointsExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
