#load "csvideo_audio.cs"

Console.WriteLine("=== csvideo_audio.cs Test ===");

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
