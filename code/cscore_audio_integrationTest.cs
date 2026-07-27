#load "cscore_audio_integration.cs"

Console.WriteLine("=== cscore_audio_integration.cs Test ===");

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

    // 验证 record: AudioPipelineConfig
    var type_AudioPipelineConfig = Type.GetType("AudioPipelineConfig");
    if (type_AudioPipelineConfig != null)
    {
        Console.WriteLine("[PASS] 类型 AudioPipelineConfig (record) 存在");
        var ctors_AudioPipelineConfig = type_AudioPipelineConfig.GetConstructors();
        Console.WriteLine($"[PASS] AudioPipelineConfig 构造函数数量: {ctors_AudioPipelineConfig.Length}");
        var methods_AudioPipelineConfig = type_AudioPipelineConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AudioPipelineConfig 公开方法数量: {methods_AudioPipelineConfig.Length}");
        foreach (var m in methods_AudioPipelineConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AudioPipelineConfig 未找到，尝试无命名空间...");
        type_AudioPipelineConfig = Type.GetType("AudioPipelineConfig");
        if (type_AudioPipelineConfig != null)
            Console.WriteLine("[PASS] 类型 AudioPipelineConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AudioPipelineConfig 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
