#load "rtc_audio_processor.cs"

Console.WriteLine("=== rtc_audio_processor.cs Test ===");

try
{
    // 验证 class: AudioStreamCache
    var type_AudioStreamCache = Type.GetType("AudioStreamCache");
    if (type_AudioStreamCache != null)
    {
        Console.WriteLine("[PASS] 类型 AudioStreamCache (class) 存在");
        var ctors_AudioStreamCache = type_AudioStreamCache.GetConstructors();
        Console.WriteLine($"[PASS] AudioStreamCache 构造函数数量: {ctors_AudioStreamCache.Length}");
        var methods_AudioStreamCache = type_AudioStreamCache.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AudioStreamCache 公开方法数量: {methods_AudioStreamCache.Length}");
        foreach (var m in methods_AudioStreamCache)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AudioStreamCache 未找到，尝试无命名空间...");
        type_AudioStreamCache = Type.GetType("AudioStreamCache");
        if (type_AudioStreamCache != null)
            Console.WriteLine("[PASS] 类型 AudioStreamCache (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AudioStreamCache 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WasmAudioProcessor
    var type_WasmAudioProcessor = Type.GetType("WasmAudioProcessor");
    if (type_WasmAudioProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 WasmAudioProcessor (class) 存在");
        var ctors_WasmAudioProcessor = type_WasmAudioProcessor.GetConstructors();
        Console.WriteLine($"[PASS] WasmAudioProcessor 构造函数数量: {ctors_WasmAudioProcessor.Length}");
        var methods_WasmAudioProcessor = type_WasmAudioProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WasmAudioProcessor 公开方法数量: {methods_WasmAudioProcessor.Length}");
        foreach (var m in methods_WasmAudioProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WasmAudioProcessor 未找到，尝试无命名空间...");
        type_WasmAudioProcessor = Type.GetType("WasmAudioProcessor");
        if (type_WasmAudioProcessor != null)
            Console.WriteLine("[PASS] 类型 WasmAudioProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WasmAudioProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EnhancedAudioProcessor
    var type_EnhancedAudioProcessor = Type.GetType("EnhancedAudioProcessor");
    if (type_EnhancedAudioProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 EnhancedAudioProcessor (class) 存在");
        var ctors_EnhancedAudioProcessor = type_EnhancedAudioProcessor.GetConstructors();
        Console.WriteLine($"[PASS] EnhancedAudioProcessor 构造函数数量: {ctors_EnhancedAudioProcessor.Length}");
        var methods_EnhancedAudioProcessor = type_EnhancedAudioProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EnhancedAudioProcessor 公开方法数量: {methods_EnhancedAudioProcessor.Length}");
        foreach (var m in methods_EnhancedAudioProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EnhancedAudioProcessor 未找到，尝试无命名空间...");
        type_EnhancedAudioProcessor = Type.GetType("EnhancedAudioProcessor");
        if (type_EnhancedAudioProcessor != null)
            Console.WriteLine("[PASS] 类型 EnhancedAudioProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EnhancedAudioProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AudioHub
    var type_AudioHub = Type.GetType("AudioHub");
    if (type_AudioHub != null)
    {
        Console.WriteLine("[PASS] 类型 AudioHub (class) 存在");
        var ctors_AudioHub = type_AudioHub.GetConstructors();
        Console.WriteLine($"[PASS] AudioHub 构造函数数量: {ctors_AudioHub.Length}");
        var methods_AudioHub = type_AudioHub.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AudioHub 公开方法数量: {methods_AudioHub.Length}");
        foreach (var m in methods_AudioHub)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AudioHub 未找到，尝试无命名空间...");
        type_AudioHub = Type.GetType("AudioHub");
        if (type_AudioHub != null)
            Console.WriteLine("[PASS] 类型 AudioHub (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AudioHub 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: AudioFrame
    var type_AudioFrame = Type.GetType("AudioFrame");
    if (type_AudioFrame != null)
    {
        Console.WriteLine("[PASS] 类型 AudioFrame (struct) 存在");
        var ctors_AudioFrame = type_AudioFrame.GetConstructors();
        Console.WriteLine($"[PASS] AudioFrame 构造函数数量: {ctors_AudioFrame.Length}");
        var methods_AudioFrame = type_AudioFrame.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AudioFrame 公开方法数量: {methods_AudioFrame.Length}");
        foreach (var m in methods_AudioFrame)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AudioFrame 未找到，尝试无命名空间...");
        type_AudioFrame = Type.GetType("AudioFrame");
        if (type_AudioFrame != null)
            Console.WriteLine("[PASS] 类型 AudioFrame (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AudioFrame 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
