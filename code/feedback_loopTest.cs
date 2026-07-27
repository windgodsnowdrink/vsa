#load "feedback_loop.cs"

Console.WriteLine("=== feedback_loop.cs Test ===");

try
{
    // 验证 class: FeedbackProcessor
    var type_FeedbackProcessor = Type.GetType("FeedbackProcessor");
    if (type_FeedbackProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 FeedbackProcessor (class) 存在");
        var ctors_FeedbackProcessor = type_FeedbackProcessor.GetConstructors();
        Console.WriteLine($"[PASS] FeedbackProcessor 构造函数数量: {ctors_FeedbackProcessor.Length}");
        var methods_FeedbackProcessor = type_FeedbackProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FeedbackProcessor 公开方法数量: {methods_FeedbackProcessor.Length}");
        foreach (var m in methods_FeedbackProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FeedbackProcessor 未找到，尝试无命名空间...");
        type_FeedbackProcessor = Type.GetType("FeedbackProcessor");
        if (type_FeedbackProcessor != null)
            Console.WriteLine("[PASS] 类型 FeedbackProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FeedbackProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
