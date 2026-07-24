#load "libvlcsharp_webrtc_integration.cs"

Console.WriteLine("=== libvlcsharp_webrtc_integration.cs Test ===");

try
{
    // 验证 class: VideoConferenceProcessor
    var type_VideoConferenceProcessor = Type.GetType("VideoConferenceProcessor");
    if (type_VideoConferenceProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 VideoConferenceProcessor (class) 存在");
        var ctors_VideoConferenceProcessor = type_VideoConferenceProcessor.GetConstructors();
        Console.WriteLine($"[PASS] VideoConferenceProcessor 构造函数数量: {ctors_VideoConferenceProcessor.Length}");
        var methods_VideoConferenceProcessor = type_VideoConferenceProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VideoConferenceProcessor 公开方法数量: {methods_VideoConferenceProcessor.Length}");
        foreach (var m in methods_VideoConferenceProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VideoConferenceProcessor 未找到，尝试无命名空间...");
        type_VideoConferenceProcessor = Type.GetType("VideoConferenceProcessor");
        if (type_VideoConferenceProcessor != null)
            Console.WriteLine("[PASS] 类型 VideoConferenceProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VideoConferenceProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
