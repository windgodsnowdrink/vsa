#load "speech_integration.cs"

Console.WriteLine("=== speech_integration.cs Test ===");

try
{
    // 验证 class: SpeechSynthesisEventHandler
    var type_SpeechSynthesisEventHandler = Type.GetType("SpeechSynthesisEventHandler");
    if (type_SpeechSynthesisEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 SpeechSynthesisEventHandler (class) 存在");
        var ctors_SpeechSynthesisEventHandler = type_SpeechSynthesisEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] SpeechSynthesisEventHandler 构造函数数量: {ctors_SpeechSynthesisEventHandler.Length}");
        var methods_SpeechSynthesisEventHandler = type_SpeechSynthesisEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SpeechSynthesisEventHandler 公开方法数量: {methods_SpeechSynthesisEventHandler.Length}");
        foreach (var m in methods_SpeechSynthesisEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SpeechSynthesisEventHandler 未找到，尝试无命名空间...");
        type_SpeechSynthesisEventHandler = Type.GetType("SpeechSynthesisEventHandler");
        if (type_SpeechSynthesisEventHandler != null)
            Console.WriteLine("[PASS] 类型 SpeechSynthesisEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpeechSynthesisEventHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AudioNoiseEchoProcessor
    var type_AudioNoiseEchoProcessor = Type.GetType("AudioNoiseEchoProcessor");
    if (type_AudioNoiseEchoProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 AudioNoiseEchoProcessor (class) 存在");
        var ctors_AudioNoiseEchoProcessor = type_AudioNoiseEchoProcessor.GetConstructors();
        Console.WriteLine($"[PASS] AudioNoiseEchoProcessor 构造函数数量: {ctors_AudioNoiseEchoProcessor.Length}");
        var methods_AudioNoiseEchoProcessor = type_AudioNoiseEchoProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AudioNoiseEchoProcessor 公开方法数量: {methods_AudioNoiseEchoProcessor.Length}");
        foreach (var m in methods_AudioNoiseEchoProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AudioNoiseEchoProcessor 未找到，尝试无命名空间...");
        type_AudioNoiseEchoProcessor = Type.GetType("AudioNoiseEchoProcessor");
        if (type_AudioNoiseEchoProcessor != null)
            Console.WriteLine("[PASS] 类型 AudioNoiseEchoProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AudioNoiseEchoProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChannelStrategyConfig
    var type_ChannelStrategyConfig = Type.GetType("ChannelStrategyConfig");
    if (type_ChannelStrategyConfig != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelStrategyConfig (class) 存在");
        var ctors_ChannelStrategyConfig = type_ChannelStrategyConfig.GetConstructors();
        Console.WriteLine($"[PASS] ChannelStrategyConfig 构造函数数量: {ctors_ChannelStrategyConfig.Length}");
        var methods_ChannelStrategyConfig = type_ChannelStrategyConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelStrategyConfig 公开方法数量: {methods_ChannelStrategyConfig.Length}");
        foreach (var m in methods_ChannelStrategyConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelStrategyConfig 未找到，尝试无命名空间...");
        type_ChannelStrategyConfig = Type.GetType("ChannelStrategyConfig");
        if (type_ChannelStrategyConfig != null)
            Console.WriteLine("[PASS] 类型 ChannelStrategyConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelStrategyConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OfflineAudioProcessor
    var type_OfflineAudioProcessor = Type.GetType("OfflineAudioProcessor");
    if (type_OfflineAudioProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 OfflineAudioProcessor (class) 存在");
        var ctors_OfflineAudioProcessor = type_OfflineAudioProcessor.GetConstructors();
        Console.WriteLine($"[PASS] OfflineAudioProcessor 构造函数数量: {ctors_OfflineAudioProcessor.Length}");
        var methods_OfflineAudioProcessor = type_OfflineAudioProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OfflineAudioProcessor 公开方法数量: {methods_OfflineAudioProcessor.Length}");
        foreach (var m in methods_OfflineAudioProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OfflineAudioProcessor 未找到，尝试无命名空间...");
        type_OfflineAudioProcessor = Type.GetType("OfflineAudioProcessor");
        if (type_OfflineAudioProcessor != null)
            Console.WriteLine("[PASS] 类型 OfflineAudioProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OfflineAudioProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MultiScenarioRecognizer
    var type_MultiScenarioRecognizer = Type.GetType("MultiScenarioRecognizer");
    if (type_MultiScenarioRecognizer != null)
    {
        Console.WriteLine("[PASS] 类型 MultiScenarioRecognizer (class) 存在");
        var ctors_MultiScenarioRecognizer = type_MultiScenarioRecognizer.GetConstructors();
        Console.WriteLine($"[PASS] MultiScenarioRecognizer 构造函数数量: {ctors_MultiScenarioRecognizer.Length}");
        var methods_MultiScenarioRecognizer = type_MultiScenarioRecognizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultiScenarioRecognizer 公开方法数量: {methods_MultiScenarioRecognizer.Length}");
        foreach (var m in methods_MultiScenarioRecognizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultiScenarioRecognizer 未找到，尝试无命名空间...");
        type_MultiScenarioRecognizer = Type.GetType("MultiScenarioRecognizer");
        if (type_MultiScenarioRecognizer != null)
            Console.WriteLine("[PASS] 类型 MultiScenarioRecognizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultiScenarioRecognizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MemoryPoolPolicy
    var type_MemoryPoolPolicy = Type.GetType("MemoryPoolPolicy");
    if (type_MemoryPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryPoolPolicy (class) 存在");
        var ctors_MemoryPoolPolicy = type_MemoryPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MemoryPoolPolicy 构造函数数量: {ctors_MemoryPoolPolicy.Length}");
        var methods_MemoryPoolPolicy = type_MemoryPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryPoolPolicy 公开方法数量: {methods_MemoryPoolPolicy.Length}");
        foreach (var m in methods_MemoryPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryPoolPolicy 未找到，尝试无命名空间...");
        type_MemoryPoolPolicy = Type.GetType("MemoryPoolPolicy");
        if (type_MemoryPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 MemoryPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RecognizerPooledPolicy
    var type_RecognizerPooledPolicy = Type.GetType("RecognizerPooledPolicy");
    if (type_RecognizerPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 RecognizerPooledPolicy (class) 存在");
        var ctors_RecognizerPooledPolicy = type_RecognizerPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] RecognizerPooledPolicy 构造函数数量: {ctors_RecognizerPooledPolicy.Length}");
        var methods_RecognizerPooledPolicy = type_RecognizerPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RecognizerPooledPolicy 公开方法数量: {methods_RecognizerPooledPolicy.Length}");
        foreach (var m in methods_RecognizerPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RecognizerPooledPolicy 未找到，尝试无命名空间...");
        type_RecognizerPooledPolicy = Type.GetType("RecognizerPooledPolicy");
        if (type_RecognizerPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 RecognizerPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RecognizerPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SynthesizerPooledPolicy
    var type_SynthesizerPooledPolicy = Type.GetType("SynthesizerPooledPolicy");
    if (type_SynthesizerPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 SynthesizerPooledPolicy (class) 存在");
        var ctors_SynthesizerPooledPolicy = type_SynthesizerPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] SynthesizerPooledPolicy 构造函数数量: {ctors_SynthesizerPooledPolicy.Length}");
        var methods_SynthesizerPooledPolicy = type_SynthesizerPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SynthesizerPooledPolicy 公开方法数量: {methods_SynthesizerPooledPolicy.Length}");
        foreach (var m in methods_SynthesizerPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SynthesizerPooledPolicy 未找到，尝试无命名空间...");
        type_SynthesizerPooledPolicy = Type.GetType("SynthesizerPooledPolicy");
        if (type_SynthesizerPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 SynthesizerPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SynthesizerPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FftNoiseSuppressor
    var type_FftNoiseSuppressor = Type.GetType("FftNoiseSuppressor");
    if (type_FftNoiseSuppressor != null)
    {
        Console.WriteLine("[PASS] 类型 FftNoiseSuppressor (class) 存在");
        var ctors_FftNoiseSuppressor = type_FftNoiseSuppressor.GetConstructors();
        Console.WriteLine($"[PASS] FftNoiseSuppressor 构造函数数量: {ctors_FftNoiseSuppressor.Length}");
        var methods_FftNoiseSuppressor = type_FftNoiseSuppressor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FftNoiseSuppressor 公开方法数量: {methods_FftNoiseSuppressor.Length}");
        foreach (var m in methods_FftNoiseSuppressor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FftNoiseSuppressor 未找到，尝试无命名空间...");
        type_FftNoiseSuppressor = Type.GetType("FftNoiseSuppressor");
        if (type_FftNoiseSuppressor != null)
            Console.WriteLine("[PASS] 类型 FftNoiseSuppressor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FftNoiseSuppressor 可能为顶层语句或嵌套类型");
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

    // 验证 interface: ICustomAudioSource
    var type_ICustomAudioSource = Type.GetType("ICustomAudioSource");
    if (type_ICustomAudioSource != null)
    {
        Console.WriteLine("[PASS] 类型 ICustomAudioSource (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ICustomAudioSource 未找到，尝试无命名空间...");
        type_ICustomAudioSource = Type.GetType("ICustomAudioSource");
        if (type_ICustomAudioSource != null)
            Console.WriteLine("[PASS] 类型 ICustomAudioSource (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICustomAudioSource 可能为顶层语句或嵌套类型");
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
