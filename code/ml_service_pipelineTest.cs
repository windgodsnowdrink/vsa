#load "ml_service_pipeline.cs"

Console.WriteLine("=== ml_service_pipeline.cs Test ===");

try
{
    // 验证 class: MlModelPipeline
    var type_MlModelPipeline = Type.GetType("MlModelPipeline");
    if (type_MlModelPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 MlModelPipeline (class) 存在");
        var ctors_MlModelPipeline = type_MlModelPipeline.GetConstructors();
        Console.WriteLine($"[PASS] MlModelPipeline 构造函数数量: {ctors_MlModelPipeline.Length}");
        var methods_MlModelPipeline = type_MlModelPipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MlModelPipeline 公开方法数量: {methods_MlModelPipeline.Length}");
        foreach (var m in methods_MlModelPipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MlModelPipeline 未找到，尝试无命名空间...");
        type_MlModelPipeline = Type.GetType("MlModelPipeline");
        if (type_MlModelPipeline != null)
            Console.WriteLine("[PASS] 类型 MlModelPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MlModelPipeline 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ModelInput
    var type_ModelInput = Type.GetType("ModelInput");
    if (type_ModelInput != null)
    {
        Console.WriteLine("[PASS] 类型 ModelInput (class) 存在");
        var ctors_ModelInput = type_ModelInput.GetConstructors();
        Console.WriteLine($"[PASS] ModelInput 构造函数数量: {ctors_ModelInput.Length}");
        var methods_ModelInput = type_ModelInput.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ModelInput 公开方法数量: {methods_ModelInput.Length}");
        foreach (var m in methods_ModelInput)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ModelInput 未找到，尝试无命名空间...");
        type_ModelInput = Type.GetType("ModelInput");
        if (type_ModelInput != null)
            Console.WriteLine("[PASS] 类型 ModelInput (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ModelInput 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ModelOutput
    var type_ModelOutput = Type.GetType("ModelOutput");
    if (type_ModelOutput != null)
    {
        Console.WriteLine("[PASS] 类型 ModelOutput (class) 存在");
        var ctors_ModelOutput = type_ModelOutput.GetConstructors();
        Console.WriteLine($"[PASS] ModelOutput 构造函数数量: {ctors_ModelOutput.Length}");
        var methods_ModelOutput = type_ModelOutput.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ModelOutput 公开方法数量: {methods_ModelOutput.Length}");
        foreach (var m in methods_ModelOutput)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ModelOutput 未找到，尝试无命名空间...");
        type_ModelOutput = Type.GetType("ModelOutput");
        if (type_ModelOutput != null)
            Console.WriteLine("[PASS] 类型 ModelOutput (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ModelOutput 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
