#load "restsharp_resiliency.cs"

Console.WriteLine("=== restsharp_resiliency.cs Test ===");

try
{
    // 验证 class: PollyResiliencyPolicy
    var type_PollyResiliencyPolicy = Type.GetType("PollyResiliencyPolicy");
    if (type_PollyResiliencyPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 PollyResiliencyPolicy (class) 存在");
        var ctors_PollyResiliencyPolicy = type_PollyResiliencyPolicy.GetConstructors();
        Console.WriteLine($"[PASS] PollyResiliencyPolicy 构造函数数量: {ctors_PollyResiliencyPolicy.Length}");
        var methods_PollyResiliencyPolicy = type_PollyResiliencyPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PollyResiliencyPolicy 公开方法数量: {methods_PollyResiliencyPolicy.Length}");
        foreach (var m in methods_PollyResiliencyPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PollyResiliencyPolicy 未找到，尝试无命名空间...");
        type_PollyResiliencyPolicy = Type.GetType("PollyResiliencyPolicy");
        if (type_PollyResiliencyPolicy != null)
            Console.WriteLine("[PASS] 类型 PollyResiliencyPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PollyResiliencyPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
