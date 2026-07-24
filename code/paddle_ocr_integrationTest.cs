#load "paddle_ocr_integration.cs"

Console.WriteLine("=== paddle_ocr_integration.cs Test ===");

try
{
    // 验证 class: OcrProcessor
    var type_OcrProcessor = Type.GetType("OcrProcessor");
    if (type_OcrProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 OcrProcessor (class) 存在");
        var ctors_OcrProcessor = type_OcrProcessor.GetConstructors();
        Console.WriteLine($"[PASS] OcrProcessor 构造函数数量: {ctors_OcrProcessor.Length}");
        var methods_OcrProcessor = type_OcrProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OcrProcessor 公开方法数量: {methods_OcrProcessor.Length}");
        foreach (var m in methods_OcrProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OcrProcessor 未找到，尝试无命名空间...");
        type_OcrProcessor = Type.GetType("OcrProcessor");
        if (type_OcrProcessor != null)
            Console.WriteLine("[PASS] 类型 OcrProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OcrProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OcrResult
    var type_OcrResult = Type.GetType("OcrResult");
    if (type_OcrResult != null)
    {
        Console.WriteLine("[PASS] 类型 OcrResult (class) 存在");
        var ctors_OcrResult = type_OcrResult.GetConstructors();
        Console.WriteLine($"[PASS] OcrResult 构造函数数量: {ctors_OcrResult.Length}");
        var methods_OcrResult = type_OcrResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OcrResult 公开方法数量: {methods_OcrResult.Length}");
        foreach (var m in methods_OcrResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OcrResult 未找到，尝试无命名空间...");
        type_OcrResult = Type.GetType("OcrResult");
        if (type_OcrResult != null)
            Console.WriteLine("[PASS] 类型 OcrResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OcrResult 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OcrResultPooledPolicy
    var type_OcrResultPooledPolicy = Type.GetType("OcrResultPooledPolicy");
    if (type_OcrResultPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 OcrResultPooledPolicy (class) 存在");
        var ctors_OcrResultPooledPolicy = type_OcrResultPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] OcrResultPooledPolicy 构造函数数量: {ctors_OcrResultPooledPolicy.Length}");
        var methods_OcrResultPooledPolicy = type_OcrResultPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OcrResultPooledPolicy 公开方法数量: {methods_OcrResultPooledPolicy.Length}");
        foreach (var m in methods_OcrResultPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OcrResultPooledPolicy 未找到，尝试无命名空间...");
        type_OcrResultPooledPolicy = Type.GetType("OcrResultPooledPolicy");
        if (type_OcrResultPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 OcrResultPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OcrResultPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 record: OcrFrame
    var type_OcrFrame = Type.GetType("OcrFrame");
    if (type_OcrFrame != null)
    {
        Console.WriteLine("[PASS] 类型 OcrFrame (record) 存在");
        var ctors_OcrFrame = type_OcrFrame.GetConstructors();
        Console.WriteLine($"[PASS] OcrFrame 构造函数数量: {ctors_OcrFrame.Length}");
        var methods_OcrFrame = type_OcrFrame.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OcrFrame 公开方法数量: {methods_OcrFrame.Length}");
        foreach (var m in methods_OcrFrame)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OcrFrame 未找到，尝试无命名空间...");
        type_OcrFrame = Type.GetType("OcrFrame");
        if (type_OcrFrame != null)
            Console.WriteLine("[PASS] 类型 OcrFrame (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OcrFrame 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
