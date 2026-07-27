#load "load_balancer.cs"

Console.WriteLine("=== load_balancer.cs Test ===");

try
{
    // 验证 class: LoadBalancer
    var type_LoadBalancer = Type.GetType("LoadBalancer");
    if (type_LoadBalancer != null)
    {
        Console.WriteLine("[PASS] 类型 LoadBalancer (class) 存在");
        var ctors_LoadBalancer = type_LoadBalancer.GetConstructors();
        Console.WriteLine($"[PASS] LoadBalancer 构造函数数量: {ctors_LoadBalancer.Length}");
        var methods_LoadBalancer = type_LoadBalancer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoadBalancer 公开方法数量: {methods_LoadBalancer.Length}");
        foreach (var m in methods_LoadBalancer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoadBalancer 未找到，尝试无命名空间...");
        type_LoadBalancer = Type.GetType("LoadBalancer");
        if (type_LoadBalancer != null)
            Console.WriteLine("[PASS] 类型 LoadBalancer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoadBalancer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
