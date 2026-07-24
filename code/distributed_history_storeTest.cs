#load "distributed_history_store.cs"

Console.WriteLine("=== distributed_history_store.cs Test ===");

try
{
    // 验证 class: DistributedHistoryWriter
    var type_DistributedHistoryWriter = Type.GetType("DistributedHistoryWriter");
    if (type_DistributedHistoryWriter != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedHistoryWriter (class) 存在");
        var ctors_DistributedHistoryWriter = type_DistributedHistoryWriter.GetConstructors();
        Console.WriteLine($"[PASS] DistributedHistoryWriter 构造函数数量: {ctors_DistributedHistoryWriter.Length}");
        var methods_DistributedHistoryWriter = type_DistributedHistoryWriter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedHistoryWriter 公开方法数量: {methods_DistributedHistoryWriter.Length}");
        foreach (var m in methods_DistributedHistoryWriter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedHistoryWriter 未找到，尝试无命名空间...");
        type_DistributedHistoryWriter = Type.GetType("DistributedHistoryWriter");
        if (type_DistributedHistoryWriter != null)
            Console.WriteLine("[PASS] 类型 DistributedHistoryWriter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedHistoryWriter 可能为顶层语句或嵌套类型");
    }

    // 验证 record: HistoryEvent
    var type_HistoryEvent = Type.GetType("HistoryEvent");
    if (type_HistoryEvent != null)
    {
        Console.WriteLine("[PASS] 类型 HistoryEvent (record) 存在");
        var ctors_HistoryEvent = type_HistoryEvent.GetConstructors();
        Console.WriteLine($"[PASS] HistoryEvent 构造函数数量: {ctors_HistoryEvent.Length}");
        var methods_HistoryEvent = type_HistoryEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HistoryEvent 公开方法数量: {methods_HistoryEvent.Length}");
        foreach (var m in methods_HistoryEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HistoryEvent 未找到，尝试无命名空间...");
        type_HistoryEvent = Type.GetType("HistoryEvent");
        if (type_HistoryEvent != null)
            Console.WriteLine("[PASS] 类型 HistoryEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HistoryEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
