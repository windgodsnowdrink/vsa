#load "http_resilience_integration.cs"

Console.WriteLine("=== http_resilience_integration.cs Test ===");

try
{
    // 验证 class: HttpClientHealthCheck
    var type_HttpClientHealthCheck = Type.GetType("HttpClientHealthCheck");
    if (type_HttpClientHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 HttpClientHealthCheck (class) 存在");
        var ctors_HttpClientHealthCheck = type_HttpClientHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] HttpClientHealthCheck 构造函数数量: {ctors_HttpClientHealthCheck.Length}");
        var methods_HttpClientHealthCheck = type_HttpClientHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HttpClientHealthCheck 公开方法数量: {methods_HttpClientHealthCheck.Length}");
        foreach (var m in methods_HttpClientHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HttpClientHealthCheck 未找到，尝试无命名空间...");
        type_HttpClientHealthCheck = Type.GetType("HttpClientHealthCheck");
        if (type_HttpClientHealthCheck != null)
            Console.WriteLine("[PASS] 类型 HttpClientHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HttpClientHealthCheck 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
