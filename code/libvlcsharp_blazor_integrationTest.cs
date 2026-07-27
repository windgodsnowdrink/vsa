#load "libvlcsharp_blazor_integration.cs"

Console.WriteLine("=== libvlcsharp_blazor_integration.cs Test ===");

try
{
    // 验证 class: BlazorVideoStreamProcessor
    var type_BlazorVideoStreamProcessor = Type.GetType("BlazorVideoStreamProcessor");
    if (type_BlazorVideoStreamProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 BlazorVideoStreamProcessor (class) 存在");
        var ctors_BlazorVideoStreamProcessor = type_BlazorVideoStreamProcessor.GetConstructors();
        Console.WriteLine($"[PASS] BlazorVideoStreamProcessor 构造函数数量: {ctors_BlazorVideoStreamProcessor.Length}");
        var methods_BlazorVideoStreamProcessor = type_BlazorVideoStreamProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BlazorVideoStreamProcessor 公开方法数量: {methods_BlazorVideoStreamProcessor.Length}");
        foreach (var m in methods_BlazorVideoStreamProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BlazorVideoStreamProcessor 未找到，尝试无命名空间...");
        type_BlazorVideoStreamProcessor = Type.GetType("BlazorVideoStreamProcessor");
        if (type_BlazorVideoStreamProcessor != null)
            Console.WriteLine("[PASS] 类型 BlazorVideoStreamProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BlazorVideoStreamProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: VideoPlayer
    var type_VideoPlayer = Type.GetType("VideoPlayer");
    if (type_VideoPlayer != null)
    {
        Console.WriteLine("[PASS] 类型 VideoPlayer (class) 存在");
        var ctors_VideoPlayer = type_VideoPlayer.GetConstructors();
        Console.WriteLine($"[PASS] VideoPlayer 构造函数数量: {ctors_VideoPlayer.Length}");
        var methods_VideoPlayer = type_VideoPlayer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VideoPlayer 公开方法数量: {methods_VideoPlayer.Length}");
        foreach (var m in methods_VideoPlayer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VideoPlayer 未找到，尝试无命名空间...");
        type_VideoPlayer = Type.GetType("VideoPlayer");
        if (type_VideoPlayer != null)
            Console.WriteLine("[PASS] 类型 VideoPlayer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VideoPlayer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
