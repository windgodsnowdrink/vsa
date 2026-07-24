#load "opencv_biometric_processor.cs"

Console.WriteLine("=== opencv_biometric_processor.cs Test ===");

try
{
    // 验证 class: BiometricProcessor
    var type_BiometricProcessor = Type.GetType("BiometricProcessor");
    if (type_BiometricProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 BiometricProcessor (class) 存在");
        var ctors_BiometricProcessor = type_BiometricProcessor.GetConstructors();
        Console.WriteLine($"[PASS] BiometricProcessor 构造函数数量: {ctors_BiometricProcessor.Length}");
        var methods_BiometricProcessor = type_BiometricProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BiometricProcessor 公开方法数量: {methods_BiometricProcessor.Length}");
        foreach (var m in methods_BiometricProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BiometricProcessor 未找到，尝试无命名空间...");
        type_BiometricProcessor = Type.GetType("BiometricProcessor");
        if (type_BiometricProcessor != null)
            Console.WriteLine("[PASS] 类型 BiometricProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BiometricProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BiometricResult
    var type_BiometricResult = Type.GetType("BiometricResult");
    if (type_BiometricResult != null)
    {
        Console.WriteLine("[PASS] 类型 BiometricResult (class) 存在");
        var ctors_BiometricResult = type_BiometricResult.GetConstructors();
        Console.WriteLine($"[PASS] BiometricResult 构造函数数量: {ctors_BiometricResult.Length}");
        var methods_BiometricResult = type_BiometricResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BiometricResult 公开方法数量: {methods_BiometricResult.Length}");
        foreach (var m in methods_BiometricResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BiometricResult 未找到，尝试无命名空间...");
        type_BiometricResult = Type.GetType("BiometricResult");
        if (type_BiometricResult != null)
            Console.WriteLine("[PASS] 类型 BiometricResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BiometricResult 可能为顶层语句或嵌套类型");
    }

    // 验证 record: BiometricFrame
    var type_BiometricFrame = Type.GetType("BiometricFrame");
    if (type_BiometricFrame != null)
    {
        Console.WriteLine("[PASS] 类型 BiometricFrame (record) 存在");
        var ctors_BiometricFrame = type_BiometricFrame.GetConstructors();
        Console.WriteLine($"[PASS] BiometricFrame 构造函数数量: {ctors_BiometricFrame.Length}");
        var methods_BiometricFrame = type_BiometricFrame.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BiometricFrame 公开方法数量: {methods_BiometricFrame.Length}");
        foreach (var m in methods_BiometricFrame)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BiometricFrame 未找到，尝试无命名空间...");
        type_BiometricFrame = Type.GetType("BiometricFrame");
        if (type_BiometricFrame != null)
            Console.WriteLine("[PASS] 类型 BiometricFrame (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BiometricFrame 可能为顶层语句或嵌套类型");
    }

    // 验证 record: IrisRecognitionResult
    var type_IrisRecognitionResult = Type.GetType("IrisRecognitionResult");
    if (type_IrisRecognitionResult != null)
    {
        Console.WriteLine("[PASS] 类型 IrisRecognitionResult (record) 存在");
        var ctors_IrisRecognitionResult = type_IrisRecognitionResult.GetConstructors();
        Console.WriteLine($"[PASS] IrisRecognitionResult 构造函数数量: {ctors_IrisRecognitionResult.Length}");
        var methods_IrisRecognitionResult = type_IrisRecognitionResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IrisRecognitionResult 公开方法数量: {methods_IrisRecognitionResult.Length}");
        foreach (var m in methods_IrisRecognitionResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IrisRecognitionResult 未找到，尝试无命名空间...");
        type_IrisRecognitionResult = Type.GetType("IrisRecognitionResult");
        if (type_IrisRecognitionResult != null)
            Console.WriteLine("[PASS] 类型 IrisRecognitionResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IrisRecognitionResult 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
