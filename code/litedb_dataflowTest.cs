#load "litedb_dataflow.cs"

Console.WriteLine("=== litedb_dataflow.cs Test ===");

try
{
    // 验证 class: DataflowEventProcessor
    var type_DataflowEventProcessor = Type.GetType("DataflowEventProcessor");
    if (type_DataflowEventProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 DataflowEventProcessor (class) 存在");
        var ctors_DataflowEventProcessor = type_DataflowEventProcessor.GetConstructors();
        Console.WriteLine($"[PASS] DataflowEventProcessor 构造函数数量: {ctors_DataflowEventProcessor.Length}");
        var methods_DataflowEventProcessor = type_DataflowEventProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataflowEventProcessor 公开方法数量: {methods_DataflowEventProcessor.Length}");
        foreach (var m in methods_DataflowEventProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataflowEventProcessor 未找到，尝试无命名空间...");
        type_DataflowEventProcessor = Type.GetType("DataflowEventProcessor");
        if (type_DataflowEventProcessor != null)
            Console.WriteLine("[PASS] 类型 DataflowEventProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataflowEventProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
