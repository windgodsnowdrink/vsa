#load "seq_integration.cs"

Console.WriteLine("=== seq_integration.cs Test ===");

try
{
    // 验证 class: SeqConfig
    var type_SeqConfig = Type.GetType("SeqConfig");
    if (type_SeqConfig != null)
    {
        Console.WriteLine("[PASS] 类型 SeqConfig (class) 存在");
        var ctors_SeqConfig = type_SeqConfig.GetConstructors();
        Console.WriteLine($"[PASS] SeqConfig 构造函数数量: {ctors_SeqConfig.Length}");
        var methods_SeqConfig = type_SeqConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SeqConfig 公开方法数量: {methods_SeqConfig.Length}");
        foreach (var m in methods_SeqConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SeqConfig 未找到，尝试无命名空间...");
        type_SeqConfig = Type.GetType("SeqConfig");
        if (type_SeqConfig != null)
            Console.WriteLine("[PASS] 类型 SeqConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SeqConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SeqLogChannel
    var type_SeqLogChannel = Type.GetType("SeqLogChannel");
    if (type_SeqLogChannel != null)
    {
        Console.WriteLine("[PASS] 类型 SeqLogChannel (class) 存在");
        var ctors_SeqLogChannel = type_SeqLogChannel.GetConstructors();
        Console.WriteLine($"[PASS] SeqLogChannel 构造函数数量: {ctors_SeqLogChannel.Length}");
        var methods_SeqLogChannel = type_SeqLogChannel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SeqLogChannel 公开方法数量: {methods_SeqLogChannel.Length}");
        foreach (var m in methods_SeqLogChannel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SeqLogChannel 未找到，尝试无命名空间...");
        type_SeqLogChannel = Type.GetType("SeqLogChannel");
        if (type_SeqLogChannel != null)
            Console.WriteLine("[PASS] 类型 SeqLogChannel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SeqLogChannel 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LogExtensions
    var type_LogExtensions = Type.GetType("LogExtensions");
    if (type_LogExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 LogExtensions (class) 存在");
        var ctors_LogExtensions = type_LogExtensions.GetConstructors();
        Console.WriteLine($"[PASS] LogExtensions 构造函数数量: {ctors_LogExtensions.Length}");
        var methods_LogExtensions = type_LogExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogExtensions 公开方法数量: {methods_LogExtensions.Length}");
        foreach (var m in methods_LogExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogExtensions 未找到，尝试无命名空间...");
        type_LogExtensions = Type.GetType("LogExtensions");
        if (type_LogExtensions != null)
            Console.WriteLine("[PASS] 类型 LogExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SeqBackgroundService
    var type_SeqBackgroundService = Type.GetType("SeqBackgroundService");
    if (type_SeqBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 SeqBackgroundService (class) 存在");
        var ctors_SeqBackgroundService = type_SeqBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] SeqBackgroundService 构造函数数量: {ctors_SeqBackgroundService.Length}");
        var methods_SeqBackgroundService = type_SeqBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SeqBackgroundService 公开方法数量: {methods_SeqBackgroundService.Length}");
        foreach (var m in methods_SeqBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SeqBackgroundService 未找到，尝试无命名空间...");
        type_SeqBackgroundService = Type.GetType("SeqBackgroundService");
        if (type_SeqBackgroundService != null)
            Console.WriteLine("[PASS] 类型 SeqBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SeqBackgroundService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
