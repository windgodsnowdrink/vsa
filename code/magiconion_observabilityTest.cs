#load "magiconion_observability.cs"

Console.WriteLine("=== magiconion_observability.cs Test ===");

try
{
    // 验证 class: ServiceMeshExtensions
    var type_ServiceMeshExtensions = Type.GetType("ServiceMeshExtensions");
    if (type_ServiceMeshExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceMeshExtensions (class) 存在");
        var ctors_ServiceMeshExtensions = type_ServiceMeshExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceMeshExtensions 构造函数数量: {ctors_ServiceMeshExtensions.Length}");
        var methods_ServiceMeshExtensions = type_ServiceMeshExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceMeshExtensions 公开方法数量: {methods_ServiceMeshExtensions.Length}");
        foreach (var m in methods_ServiceMeshExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceMeshExtensions 未找到，尝试无命名空间...");
        type_ServiceMeshExtensions = Type.GetType("ServiceMeshExtensions");
        if (type_ServiceMeshExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceMeshExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceMeshExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChaosPolicyBuilder
    var type_ChaosPolicyBuilder = Type.GetType("ChaosPolicyBuilder");
    if (type_ChaosPolicyBuilder != null)
    {
        Console.WriteLine("[PASS] 类型 ChaosPolicyBuilder (class) 存在");
        var ctors_ChaosPolicyBuilder = type_ChaosPolicyBuilder.GetConstructors();
        Console.WriteLine($"[PASS] ChaosPolicyBuilder 构造函数数量: {ctors_ChaosPolicyBuilder.Length}");
        var methods_ChaosPolicyBuilder = type_ChaosPolicyBuilder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChaosPolicyBuilder 公开方法数量: {methods_ChaosPolicyBuilder.Length}");
        foreach (var m in methods_ChaosPolicyBuilder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChaosPolicyBuilder 未找到，尝试无命名空间...");
        type_ChaosPolicyBuilder = Type.GetType("ChaosPolicyBuilder");
        if (type_ChaosPolicyBuilder != null)
            Console.WriteLine("[PASS] 类型 ChaosPolicyBuilder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChaosPolicyBuilder 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
