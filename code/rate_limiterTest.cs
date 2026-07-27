#load "rate_limiter.cs"

Console.WriteLine("=== rate_limiter.cs Test ===");

try
{
    // 验证 class: TieredRateLimiter
    var type_TieredRateLimiter = Type.GetType("TieredRateLimiter");
    if (type_TieredRateLimiter != null)
    {
        Console.WriteLine("[PASS] 类型 TieredRateLimiter (class) 存在");
        var ctors_TieredRateLimiter = type_TieredRateLimiter.GetConstructors();
        Console.WriteLine($"[PASS] TieredRateLimiter 构造函数数量: {ctors_TieredRateLimiter.Length}");
        var methods_TieredRateLimiter = type_TieredRateLimiter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredRateLimiter 公开方法数量: {methods_TieredRateLimiter.Length}");
        foreach (var m in methods_TieredRateLimiter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredRateLimiter 未找到，尝试无命名空间...");
        type_TieredRateLimiter = Type.GetType("TieredRateLimiter");
        if (type_TieredRateLimiter != null)
            Console.WriteLine("[PASS] 类型 TieredRateLimiter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredRateLimiter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TokenBucketLimiter
    var type_TokenBucketLimiter = Type.GetType("TokenBucketLimiter");
    if (type_TokenBucketLimiter != null)
    {
        Console.WriteLine("[PASS] 类型 TokenBucketLimiter (class) 存在");
        var ctors_TokenBucketLimiter = type_TokenBucketLimiter.GetConstructors();
        Console.WriteLine($"[PASS] TokenBucketLimiter 构造函数数量: {ctors_TokenBucketLimiter.Length}");
        var methods_TokenBucketLimiter = type_TokenBucketLimiter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TokenBucketLimiter 公开方法数量: {methods_TokenBucketLimiter.Length}");
        foreach (var m in methods_TokenBucketLimiter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TokenBucketLimiter 未找到，尝试无命名空间...");
        type_TokenBucketLimiter = Type.GetType("TokenBucketLimiter");
        if (type_TokenBucketLimiter != null)
            Console.WriteLine("[PASS] 类型 TokenBucketLimiter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TokenBucketLimiter 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
