#load "audio_enhancement.cs"

Console.WriteLine("=== audio_enhancement.cs Test ===");

try
{
    // 验证 class: AudioPipeline
    var type_AudioPipeline = Type.GetType("AudioPipeline");
    if (type_AudioPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 AudioPipeline (class) 存在");
        var ctors_AudioPipeline = type_AudioPipeline.GetConstructors();
        Console.WriteLine($"[PASS] AudioPipeline 构造函数数量: {ctors_AudioPipeline.Length}");
        var methods_AudioPipeline = type_AudioPipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AudioPipeline 公开方法数量: {methods_AudioPipeline.Length}");
        foreach (var m in methods_AudioPipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AudioPipeline 未找到，尝试无命名空间...");
        type_AudioPipeline = Type.GetType("AudioPipeline");
        if (type_AudioPipeline != null)
            Console.WriteLine("[PASS] 类型 AudioPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AudioPipeline 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WaveSourcePooledPolicy
    var type_WaveSourcePooledPolicy = Type.GetType("WaveSourcePooledPolicy");
    if (type_WaveSourcePooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 WaveSourcePooledPolicy (class) 存在");
        var ctors_WaveSourcePooledPolicy = type_WaveSourcePooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] WaveSourcePooledPolicy 构造函数数量: {ctors_WaveSourcePooledPolicy.Length}");
        var methods_WaveSourcePooledPolicy = type_WaveSourcePooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WaveSourcePooledPolicy 公开方法数量: {methods_WaveSourcePooledPolicy.Length}");
        foreach (var m in methods_WaveSourcePooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WaveSourcePooledPolicy 未找到，尝试无命名空间...");
        type_WaveSourcePooledPolicy = Type.GetType("WaveSourcePooledPolicy");
        if (type_WaveSourcePooledPolicy != null)
            Console.WriteLine("[PASS] 类型 WaveSourcePooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WaveSourcePooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WasapiOutPooledPolicy
    var type_WasapiOutPooledPolicy = Type.GetType("WasapiOutPooledPolicy");
    if (type_WasapiOutPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 WasapiOutPooledPolicy (class) 存在");
        var ctors_WasapiOutPooledPolicy = type_WasapiOutPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] WasapiOutPooledPolicy 构造函数数量: {ctors_WasapiOutPooledPolicy.Length}");
        var methods_WasapiOutPooledPolicy = type_WasapiOutPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WasapiOutPooledPolicy 公开方法数量: {methods_WasapiOutPooledPolicy.Length}");
        foreach (var m in methods_WasapiOutPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WasapiOutPooledPolicy 未找到，尝试无命名空间...");
        type_WasapiOutPooledPolicy = Type.GetType("WasapiOutPooledPolicy");
        if (type_WasapiOutPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 WasapiOutPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WasapiOutPooledPolicy 可能为顶层语句或嵌套类型");
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
