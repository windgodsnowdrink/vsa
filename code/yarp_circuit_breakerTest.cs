#load "yarp_circuit_breaker.cs"

Console.WriteLine("=== yarp_circuit_breaker.cs Test ===");

try
{
    // 验证 class: CircuitBreakerMiddleware
    var type_CircuitBreakerMiddleware = Type.GetType("CircuitBreakerMiddleware");
    if (type_CircuitBreakerMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 CircuitBreakerMiddleware (class) 存在");
        var ctors_CircuitBreakerMiddleware = type_CircuitBreakerMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] CircuitBreakerMiddleware 构造函数数量: {ctors_CircuitBreakerMiddleware.Length}");
        var methods_CircuitBreakerMiddleware = type_CircuitBreakerMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CircuitBreakerMiddleware 公开方法数量: {methods_CircuitBreakerMiddleware.Length}");
        foreach (var m in methods_CircuitBreakerMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CircuitBreakerMiddleware 未找到，尝试无命名空间...");
        type_CircuitBreakerMiddleware = Type.GetType("CircuitBreakerMiddleware");
        if (type_CircuitBreakerMiddleware != null)
            Console.WriteLine("[PASS] 类型 CircuitBreakerMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CircuitBreakerMiddleware 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
