#load "naudio_integration.cs"

Console.WriteLine("=== naudio_integration.cs Test ===");

try
{
    // 验证 class: AudioProcessingPipeline
    var type_AudioProcessingPipeline = Type.GetType("AudioProcessingPipeline");
    if (type_AudioProcessingPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 AudioProcessingPipeline (class) 存在");
        var ctors_AudioProcessingPipeline = type_AudioProcessingPipeline.GetConstructors();
        Console.WriteLine($"[PASS] AudioProcessingPipeline 构造函数数量: {ctors_AudioProcessingPipeline.Length}");
        var methods_AudioProcessingPipeline = type_AudioProcessingPipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AudioProcessingPipeline 公开方法数量: {methods_AudioProcessingPipeline.Length}");
        foreach (var m in methods_AudioProcessingPipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AudioProcessingPipeline 未找到，尝试无命名空间...");
        type_AudioProcessingPipeline = Type.GetType("AudioProcessingPipeline");
        if (type_AudioProcessingPipeline != null)
            Console.WriteLine("[PASS] 类型 AudioProcessingPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AudioProcessingPipeline 可能为顶层语句或嵌套类型");
    }

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

    // 验证 class: WaveStreamPooledPolicy
    var type_WaveStreamPooledPolicy = Type.GetType("WaveStreamPooledPolicy");
    if (type_WaveStreamPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 WaveStreamPooledPolicy (class) 存在");
        var ctors_WaveStreamPooledPolicy = type_WaveStreamPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] WaveStreamPooledPolicy 构造函数数量: {ctors_WaveStreamPooledPolicy.Length}");
        var methods_WaveStreamPooledPolicy = type_WaveStreamPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WaveStreamPooledPolicy 公开方法数量: {methods_WaveStreamPooledPolicy.Length}");
        foreach (var m in methods_WaveStreamPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WaveStreamPooledPolicy 未找到，尝试无命名空间...");
        type_WaveStreamPooledPolicy = Type.GetType("WaveStreamPooledPolicy");
        if (type_WaveStreamPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 WaveStreamPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WaveStreamPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SpeechRecognitionService
    var type_SpeechRecognitionService = Type.GetType("SpeechRecognitionService");
    if (type_SpeechRecognitionService != null)
    {
        Console.WriteLine("[PASS] 类型 SpeechRecognitionService (class) 存在");
        var ctors_SpeechRecognitionService = type_SpeechRecognitionService.GetConstructors();
        Console.WriteLine($"[PASS] SpeechRecognitionService 构造函数数量: {ctors_SpeechRecognitionService.Length}");
        var methods_SpeechRecognitionService = type_SpeechRecognitionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SpeechRecognitionService 公开方法数量: {methods_SpeechRecognitionService.Length}");
        foreach (var m in methods_SpeechRecognitionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SpeechRecognitionService 未找到，尝试无命名空间...");
        type_SpeechRecognitionService = Type.GetType("SpeechRecognitionService");
        if (type_SpeechRecognitionService != null)
            Console.WriteLine("[PASS] 类型 SpeechRecognitionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpeechRecognitionService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAudioAnalyzer
    var type_IAudioAnalyzer = Type.GetType("IAudioAnalyzer");
    if (type_IAudioAnalyzer != null)
    {
        Console.WriteLine("[PASS] 类型 IAudioAnalyzer (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAudioAnalyzer 未找到，尝试无命名空间...");
        type_IAudioAnalyzer = Type.GetType("IAudioAnalyzer");
        if (type_IAudioAnalyzer != null)
            Console.WriteLine("[PASS] 类型 IAudioAnalyzer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAudioAnalyzer 可能为顶层语句或嵌套类型");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
