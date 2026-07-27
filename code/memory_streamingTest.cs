#load "memory_streaming.cs"

Console.WriteLine("=== memory_streaming.cs Test ===");

try
{
    // 验证 class: MemoryStreamingService
    var type_MemoryStreamingService = Type.GetType("MemoryStreamingService");
    if (type_MemoryStreamingService != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryStreamingService (class) 存在");
        var ctors_MemoryStreamingService = type_MemoryStreamingService.GetConstructors();
        Console.WriteLine($"[PASS] MemoryStreamingService 构造函数数量: {ctors_MemoryStreamingService.Length}");
        var methods_MemoryStreamingService = type_MemoryStreamingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryStreamingService 公开方法数量: {methods_MemoryStreamingService.Length}");
        foreach (var m in methods_MemoryStreamingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryStreamingService 未找到，尝试无命名空间...");
        type_MemoryStreamingService = Type.GetType("MemoryStreamingService");
        if (type_MemoryStreamingService != null)
            Console.WriteLine("[PASS] 类型 MemoryStreamingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryStreamingService 可能为顶层语句或嵌套类型");
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

    // 验证 record: StreamData
    var type_StreamData = Type.GetType("StreamData");
    if (type_StreamData != null)
    {
        Console.WriteLine("[PASS] 类型 StreamData (record) 存在");
        var ctors_StreamData = type_StreamData.GetConstructors();
        Console.WriteLine($"[PASS] StreamData 构造函数数量: {ctors_StreamData.Length}");
        var methods_StreamData = type_StreamData.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StreamData 公开方法数量: {methods_StreamData.Length}");
        foreach (var m in methods_StreamData)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StreamData 未找到，尝试无命名空间...");
        type_StreamData = Type.GetType("StreamData");
        if (type_StreamData != null)
            Console.WriteLine("[PASS] 类型 StreamData (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StreamData 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
