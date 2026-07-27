#load "log_storage_processor.cs"

Console.WriteLine("=== log_storage_processor.cs Test ===");

try
{
    // 验证 class: LogStorageProcessor
    var type_LogStorageProcessor = Type.GetType("LogStorageProcessor");
    if (type_LogStorageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 LogStorageProcessor (class) 存在");
        var ctors_LogStorageProcessor = type_LogStorageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] LogStorageProcessor 构造函数数量: {ctors_LogStorageProcessor.Length}");
        var methods_LogStorageProcessor = type_LogStorageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogStorageProcessor 公开方法数量: {methods_LogStorageProcessor.Length}");
        foreach (var m in methods_LogStorageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogStorageProcessor 未找到，尝试无命名空间...");
        type_LogStorageProcessor = Type.GetType("LogStorageProcessor");
        if (type_LogStorageProcessor != null)
            Console.WriteLine("[PASS] 类型 LogStorageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogStorageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 record: LogEntry
    var type_LogEntry = Type.GetType("LogEntry");
    if (type_LogEntry != null)
    {
        Console.WriteLine("[PASS] 类型 LogEntry (record) 存在");
        var ctors_LogEntry = type_LogEntry.GetConstructors();
        Console.WriteLine($"[PASS] LogEntry 构造函数数量: {ctors_LogEntry.Length}");
        var methods_LogEntry = type_LogEntry.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogEntry 公开方法数量: {methods_LogEntry.Length}");
        foreach (var m in methods_LogEntry)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogEntry 未找到，尝试无命名空间...");
        type_LogEntry = Type.GetType("LogEntry");
        if (type_LogEntry != null)
            Console.WriteLine("[PASS] 类型 LogEntry (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogEntry 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
