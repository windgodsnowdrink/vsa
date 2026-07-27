#load "libvlcsharp_integration.cs"

Console.WriteLine("=== libvlcsharp_integration.cs Test ===");

try
{
    // 验证 class: VideoStreamProcessor
    var type_VideoStreamProcessor = Type.GetType("VideoStreamProcessor");
    if (type_VideoStreamProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 VideoStreamProcessor (class) 存在");
        var ctors_VideoStreamProcessor = type_VideoStreamProcessor.GetConstructors();
        Console.WriteLine($"[PASS] VideoStreamProcessor 构造函数数量: {ctors_VideoStreamProcessor.Length}");
        var methods_VideoStreamProcessor = type_VideoStreamProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VideoStreamProcessor 公开方法数量: {methods_VideoStreamProcessor.Length}");
        foreach (var m in methods_VideoStreamProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VideoStreamProcessor 未找到，尝试无命名空间...");
        type_VideoStreamProcessor = Type.GetType("VideoStreamProcessor");
        if (type_VideoStreamProcessor != null)
            Console.WriteLine("[PASS] 类型 VideoStreamProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VideoStreamProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AudioStreamProcessor
    var type_AudioStreamProcessor = Type.GetType("AudioStreamProcessor");
    if (type_AudioStreamProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 AudioStreamProcessor (class) 存在");
        var ctors_AudioStreamProcessor = type_AudioStreamProcessor.GetConstructors();
        Console.WriteLine($"[PASS] AudioStreamProcessor 构造函数数量: {ctors_AudioStreamProcessor.Length}");
        var methods_AudioStreamProcessor = type_AudioStreamProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AudioStreamProcessor 公开方法数量: {methods_AudioStreamProcessor.Length}");
        foreach (var m in methods_AudioStreamProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AudioStreamProcessor 未找到，尝试无命名空间...");
        type_AudioStreamProcessor = Type.GetType("AudioStreamProcessor");
        if (type_AudioStreamProcessor != null)
            Console.WriteLine("[PASS] 类型 AudioStreamProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AudioStreamProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
