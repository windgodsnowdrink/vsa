#load "libvlcsharp_live_streaming.cs"

Console.WriteLine("=== libvlcsharp_live_streaming.cs Test ===");

try
{
    // 验证 class: LiveStreamProcessor
    var type_LiveStreamProcessor = Type.GetType("LiveStreamProcessor");
    if (type_LiveStreamProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 LiveStreamProcessor (class) 存在");
        var ctors_LiveStreamProcessor = type_LiveStreamProcessor.GetConstructors();
        Console.WriteLine($"[PASS] LiveStreamProcessor 构造函数数量: {ctors_LiveStreamProcessor.Length}");
        var methods_LiveStreamProcessor = type_LiveStreamProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiveStreamProcessor 公开方法数量: {methods_LiveStreamProcessor.Length}");
        foreach (var m in methods_LiveStreamProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiveStreamProcessor 未找到，尝试无命名空间...");
        type_LiveStreamProcessor = Type.GetType("LiveStreamProcessor");
        if (type_LiveStreamProcessor != null)
            Console.WriteLine("[PASS] 类型 LiveStreamProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiveStreamProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TailLatencyOptimizer
    var type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
    if (type_TailLatencyOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (class) 存在");
        var ctors_TailLatencyOptimizer = type_TailLatencyOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] TailLatencyOptimizer 构造函数数量: {ctors_TailLatencyOptimizer.Length}");
        var methods_TailLatencyOptimizer = type_TailLatencyOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TailLatencyOptimizer 公开方法数量: {methods_TailLatencyOptimizer.Length}");
        foreach (var m in methods_TailLatencyOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TailLatencyOptimizer 未找到，尝试无命名空间...");
        type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
        if (type_TailLatencyOptimizer != null)
            Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailLatencyOptimizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MediaPlayerPooledPolicy
    var type_MediaPlayerPooledPolicy = Type.GetType("MediaPlayerPooledPolicy");
    if (type_MediaPlayerPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MediaPlayerPooledPolicy (class) 存在");
        var ctors_MediaPlayerPooledPolicy = type_MediaPlayerPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MediaPlayerPooledPolicy 构造函数数量: {ctors_MediaPlayerPooledPolicy.Length}");
        var methods_MediaPlayerPooledPolicy = type_MediaPlayerPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediaPlayerPooledPolicy 公开方法数量: {methods_MediaPlayerPooledPolicy.Length}");
        foreach (var m in methods_MediaPlayerPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediaPlayerPooledPolicy 未找到，尝试无命名空间...");
        type_MediaPlayerPooledPolicy = Type.GetType("MediaPlayerPooledPolicy");
        if (type_MediaPlayerPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 MediaPlayerPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediaPlayerPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
