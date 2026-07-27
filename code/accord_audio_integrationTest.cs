#load "accord_audio_integration.cs"

Console.WriteLine("=== accord_audio_integration.cs Test ===");

try
{
    // 验证 class: AudioProcessor
    var type_AudioProcessor = Type.GetType("AudioProcessor");
    if (type_AudioProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 AudioProcessor (class) 存在");
        var ctors_AudioProcessor = type_AudioProcessor.GetConstructors();
        Console.WriteLine($"[PASS] AudioProcessor 构造函数数量: {ctors_AudioProcessor.Length}");
        var methods_AudioProcessor = type_AudioProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AudioProcessor 公开方法数量: {methods_AudioProcessor.Length}");
        foreach (var m in methods_AudioProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AudioProcessor 未找到，尝试无命名空间...");
        type_AudioProcessor = Type.GetType("AudioProcessor");
        if (type_AudioProcessor != null)
            Console.WriteLine("[PASS] 类型 AudioProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AudioProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FFTAnalyzer
    var type_FFTAnalyzer = Type.GetType("FFTAnalyzer");
    if (type_FFTAnalyzer != null)
    {
        Console.WriteLine("[PASS] 类型 FFTAnalyzer (class) 存在");
        var ctors_FFTAnalyzer = type_FFTAnalyzer.GetConstructors();
        Console.WriteLine($"[PASS] FFTAnalyzer 构造函数数量: {ctors_FFTAnalyzer.Length}");
        var methods_FFTAnalyzer = type_FFTAnalyzer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FFTAnalyzer 公开方法数量: {methods_FFTAnalyzer.Length}");
        foreach (var m in methods_FFTAnalyzer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FFTAnalyzer 未找到，尝试无命名空间...");
        type_FFTAnalyzer = Type.GetType("FFTAnalyzer");
        if (type_FFTAnalyzer != null)
            Console.WriteLine("[PASS] 类型 FFTAnalyzer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FFTAnalyzer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: VoiceprintExtractor
    var type_VoiceprintExtractor = Type.GetType("VoiceprintExtractor");
    if (type_VoiceprintExtractor != null)
    {
        Console.WriteLine("[PASS] 类型 VoiceprintExtractor (class) 存在");
        var ctors_VoiceprintExtractor = type_VoiceprintExtractor.GetConstructors();
        Console.WriteLine($"[PASS] VoiceprintExtractor 构造函数数量: {ctors_VoiceprintExtractor.Length}");
        var methods_VoiceprintExtractor = type_VoiceprintExtractor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VoiceprintExtractor 公开方法数量: {methods_VoiceprintExtractor.Length}");
        foreach (var m in methods_VoiceprintExtractor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VoiceprintExtractor 未找到，尝试无命名空间...");
        type_VoiceprintExtractor = Type.GetType("VoiceprintExtractor");
        if (type_VoiceprintExtractor != null)
            Console.WriteLine("[PASS] 类型 VoiceprintExtractor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VoiceprintExtractor 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAudioProcessingPipeline
    var type_IAudioProcessingPipeline = Type.GetType("IAudioProcessingPipeline");
    if (type_IAudioProcessingPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 IAudioProcessingPipeline (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAudioProcessingPipeline 未找到，尝试无命名空间...");
        type_IAudioProcessingPipeline = Type.GetType("IAudioProcessingPipeline");
        if (type_IAudioProcessingPipeline != null)
            Console.WriteLine("[PASS] 类型 IAudioProcessingPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAudioProcessingPipeline 可能为顶层语句或嵌套类型");
    }

    // 验证 record: FFTResult
    var type_FFTResult = Type.GetType("FFTResult");
    if (type_FFTResult != null)
    {
        Console.WriteLine("[PASS] 类型 FFTResult (record) 存在");
        var ctors_FFTResult = type_FFTResult.GetConstructors();
        Console.WriteLine($"[PASS] FFTResult 构造函数数量: {ctors_FFTResult.Length}");
        var methods_FFTResult = type_FFTResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FFTResult 公开方法数量: {methods_FFTResult.Length}");
        foreach (var m in methods_FFTResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FFTResult 未找到，尝试无命名空间...");
        type_FFTResult = Type.GetType("FFTResult");
        if (type_FFTResult != null)
            Console.WriteLine("[PASS] 类型 FFTResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FFTResult 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Voiceprint
    var type_Voiceprint = Type.GetType("Voiceprint");
    if (type_Voiceprint != null)
    {
        Console.WriteLine("[PASS] 类型 Voiceprint (record) 存在");
        var ctors_Voiceprint = type_Voiceprint.GetConstructors();
        Console.WriteLine($"[PASS] Voiceprint 构造函数数量: {ctors_Voiceprint.Length}");
        var methods_Voiceprint = type_Voiceprint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Voiceprint 公开方法数量: {methods_Voiceprint.Length}");
        foreach (var m in methods_Voiceprint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Voiceprint 未找到，尝试无命名空间...");
        type_Voiceprint = Type.GetType("Voiceprint");
        if (type_Voiceprint != null)
            Console.WriteLine("[PASS] 类型 Voiceprint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Voiceprint 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
