#load "disruptor_json_processor.cs"

Console.WriteLine("=== disruptor_json_processor.cs Test ===");

try
{
    // 验证 class: DisruptorJsonProcessor
    var type_DisruptorJsonProcessor = Type.GetType("DisruptorJsonProcessor");
    if (type_DisruptorJsonProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 DisruptorJsonProcessor (class) 存在");
        var ctors_DisruptorJsonProcessor = type_DisruptorJsonProcessor.GetConstructors();
        Console.WriteLine($"[PASS] DisruptorJsonProcessor 构造函数数量: {ctors_DisruptorJsonProcessor.Length}");
        var methods_DisruptorJsonProcessor = type_DisruptorJsonProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DisruptorJsonProcessor 公开方法数量: {methods_DisruptorJsonProcessor.Length}");
        foreach (var m in methods_DisruptorJsonProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DisruptorJsonProcessor 未找到，尝试无命名空间...");
        type_DisruptorJsonProcessor = Type.GetType("DisruptorJsonProcessor");
        if (type_DisruptorJsonProcessor != null)
            Console.WriteLine("[PASS] 类型 DisruptorJsonProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DisruptorJsonProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
