#load "memory_sse_streaming.cs"

Console.WriteLine("=== memory_sse_streaming.cs Test ===");

try
{
    // 验证 class: SseStreamingService
    var type_SseStreamingService = Type.GetType("SseStreamingService");
    if (type_SseStreamingService != null)
    {
        Console.WriteLine("[PASS] 类型 SseStreamingService (class) 存在");
        var ctors_SseStreamingService = type_SseStreamingService.GetConstructors();
        Console.WriteLine($"[PASS] SseStreamingService 构造函数数量: {ctors_SseStreamingService.Length}");
        var methods_SseStreamingService = type_SseStreamingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SseStreamingService 公开方法数量: {methods_SseStreamingService.Length}");
        foreach (var m in methods_SseStreamingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SseStreamingService 未找到，尝试无命名空间...");
        type_SseStreamingService = Type.GetType("SseStreamingService");
        if (type_SseStreamingService != null)
            Console.WriteLine("[PASS] 类型 SseStreamingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SseStreamingService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RecyclableMemoryStreamPooledPolicy
    var type_RecyclableMemoryStreamPooledPolicy = Type.GetType("RecyclableMemoryStreamPooledPolicy");
    if (type_RecyclableMemoryStreamPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 RecyclableMemoryStreamPooledPolicy (class) 存在");
        var ctors_RecyclableMemoryStreamPooledPolicy = type_RecyclableMemoryStreamPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] RecyclableMemoryStreamPooledPolicy 构造函数数量: {ctors_RecyclableMemoryStreamPooledPolicy.Length}");
        var methods_RecyclableMemoryStreamPooledPolicy = type_RecyclableMemoryStreamPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RecyclableMemoryStreamPooledPolicy 公开方法数量: {methods_RecyclableMemoryStreamPooledPolicy.Length}");
        foreach (var m in methods_RecyclableMemoryStreamPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RecyclableMemoryStreamPooledPolicy 未找到，尝试无命名空间...");
        type_RecyclableMemoryStreamPooledPolicy = Type.GetType("RecyclableMemoryStreamPooledPolicy");
        if (type_RecyclableMemoryStreamPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 RecyclableMemoryStreamPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RecyclableMemoryStreamPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
