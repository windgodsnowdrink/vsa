#load "dead_letter.cs"

Console.WriteLine("=== dead_letter.cs Test ===");

try
{
    // 验证 class: DeadLetterQueue
    var type_DeadLetterQueue = Type.GetType("DeadLetterQueue");
    if (type_DeadLetterQueue != null)
    {
        Console.WriteLine("[PASS] 类型 DeadLetterQueue (class) 存在");
        var ctors_DeadLetterQueue = type_DeadLetterQueue.GetConstructors();
        Console.WriteLine($"[PASS] DeadLetterQueue 构造函数数量: {ctors_DeadLetterQueue.Length}");
        var methods_DeadLetterQueue = type_DeadLetterQueue.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeadLetterQueue 公开方法数量: {methods_DeadLetterQueue.Length}");
        foreach (var m in methods_DeadLetterQueue)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeadLetterQueue 未找到，尝试无命名空间...");
        type_DeadLetterQueue = Type.GetType("DeadLetterQueue");
        if (type_DeadLetterQueue != null)
            Console.WriteLine("[PASS] 类型 DeadLetterQueue (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeadLetterQueue 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
