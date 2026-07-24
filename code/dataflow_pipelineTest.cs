#load "dataflow_pipeline.cs"

Console.WriteLine("=== dataflow_pipeline.cs Test ===");

try
{
    // 验证 class: DataflowMessagePipeline
    var type_DataflowMessagePipeline = Type.GetType("DataflowMessagePipeline");
    if (type_DataflowMessagePipeline != null)
    {
        Console.WriteLine("[PASS] 类型 DataflowMessagePipeline (class) 存在");
        var ctors_DataflowMessagePipeline = type_DataflowMessagePipeline.GetConstructors();
        Console.WriteLine($"[PASS] DataflowMessagePipeline 构造函数数量: {ctors_DataflowMessagePipeline.Length}");
        var methods_DataflowMessagePipeline = type_DataflowMessagePipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataflowMessagePipeline 公开方法数量: {methods_DataflowMessagePipeline.Length}");
        foreach (var m in methods_DataflowMessagePipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataflowMessagePipeline 未找到，尝试无命名空间...");
        type_DataflowMessagePipeline = Type.GetType("DataflowMessagePipeline");
        if (type_DataflowMessagePipeline != null)
            Console.WriteLine("[PASS] 类型 DataflowMessagePipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataflowMessagePipeline 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
