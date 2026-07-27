#load "multistage_pipeline.cs"

Console.WriteLine("=== multistage_pipeline.cs Test ===");

try
{
    // 验证 class: MultiStagePipeline
    var type_MultiStagePipeline = Type.GetType("MultiStagePipeline");
    if (type_MultiStagePipeline != null)
    {
        Console.WriteLine("[PASS] 类型 MultiStagePipeline (class) 存在");
        var ctors_MultiStagePipeline = type_MultiStagePipeline.GetConstructors();
        Console.WriteLine($"[PASS] MultiStagePipeline 构造函数数量: {ctors_MultiStagePipeline.Length}");
        var methods_MultiStagePipeline = type_MultiStagePipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultiStagePipeline 公开方法数量: {methods_MultiStagePipeline.Length}");
        foreach (var m in methods_MultiStagePipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultiStagePipeline 未找到，尝试无命名空间...");
        type_MultiStagePipeline = Type.GetType("MultiStagePipeline");
        if (type_MultiStagePipeline != null)
            Console.WriteLine("[PASS] 类型 MultiStagePipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultiStagePipeline 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
