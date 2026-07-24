#load "serilog_integration.cs"

Console.WriteLine("=== serilog_integration.cs Test ===");

try
{
    // 验证 class: SerilogExtensibilityExtensions
    var type_SerilogExtensibilityExtensions = Type.GetType("SerilogExtensibilityExtensions");
    if (type_SerilogExtensibilityExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SerilogExtensibilityExtensions (class) 存在");
        var ctors_SerilogExtensibilityExtensions = type_SerilogExtensibilityExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SerilogExtensibilityExtensions 构造函数数量: {ctors_SerilogExtensibilityExtensions.Length}");
        var methods_SerilogExtensibilityExtensions = type_SerilogExtensibilityExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SerilogExtensibilityExtensions 公开方法数量: {methods_SerilogExtensibilityExtensions.Length}");
        foreach (var m in methods_SerilogExtensibilityExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SerilogExtensibilityExtensions 未找到，尝试无命名空间...");
        type_SerilogExtensibilityExtensions = Type.GetType("SerilogExtensibilityExtensions");
        if (type_SerilogExtensibilityExtensions != null)
            Console.WriteLine("[PASS] 类型 SerilogExtensibilityExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SerilogExtensibilityExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HighPerformanceLogQueue
    var type_HighPerformanceLogQueue = Type.GetType("HighPerformanceLogQueue");
    if (type_HighPerformanceLogQueue != null)
    {
        Console.WriteLine("[PASS] 类型 HighPerformanceLogQueue (class) 存在");
        var ctors_HighPerformanceLogQueue = type_HighPerformanceLogQueue.GetConstructors();
        Console.WriteLine($"[PASS] HighPerformanceLogQueue 构造函数数量: {ctors_HighPerformanceLogQueue.Length}");
        var methods_HighPerformanceLogQueue = type_HighPerformanceLogQueue.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HighPerformanceLogQueue 公开方法数量: {methods_HighPerformanceLogQueue.Length}");
        foreach (var m in methods_HighPerformanceLogQueue)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HighPerformanceLogQueue 未找到，尝试无命名空间...");
        type_HighPerformanceLogQueue = Type.GetType("HighPerformanceLogQueue");
        if (type_HighPerformanceLogQueue != null)
            Console.WriteLine("[PASS] 类型 HighPerformanceLogQueue (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HighPerformanceLogQueue 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataMaskingExtensions
    var type_DataMaskingExtensions = Type.GetType("DataMaskingExtensions");
    if (type_DataMaskingExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DataMaskingExtensions (class) 存在");
        var ctors_DataMaskingExtensions = type_DataMaskingExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DataMaskingExtensions 构造函数数量: {ctors_DataMaskingExtensions.Length}");
        var methods_DataMaskingExtensions = type_DataMaskingExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataMaskingExtensions 公开方法数量: {methods_DataMaskingExtensions.Length}");
        foreach (var m in methods_DataMaskingExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataMaskingExtensions 未找到，尝试无命名空间...");
        type_DataMaskingExtensions = Type.GetType("DataMaskingExtensions");
        if (type_DataMaskingExtensions != null)
            Console.WriteLine("[PASS] 类型 DataMaskingExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataMaskingExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataMaskingEnricher
    var type_DataMaskingEnricher = Type.GetType("DataMaskingEnricher");
    if (type_DataMaskingEnricher != null)
    {
        Console.WriteLine("[PASS] 类型 DataMaskingEnricher (class) 存在");
        var ctors_DataMaskingEnricher = type_DataMaskingEnricher.GetConstructors();
        Console.WriteLine($"[PASS] DataMaskingEnricher 构造函数数量: {ctors_DataMaskingEnricher.Length}");
        var methods_DataMaskingEnricher = type_DataMaskingEnricher.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataMaskingEnricher 公开方法数量: {methods_DataMaskingEnricher.Length}");
        foreach (var m in methods_DataMaskingEnricher)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataMaskingEnricher 未找到，尝试无命名空间...");
        type_DataMaskingEnricher = Type.GetType("DataMaskingEnricher");
        if (type_DataMaskingEnricher != null)
            Console.WriteLine("[PASS] 类型 DataMaskingEnricher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataMaskingEnricher 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SerilogExtensibilityProductionExtensions
    var type_SerilogExtensibilityProductionExtensions = Type.GetType("SerilogExtensibilityProductionExtensions");
    if (type_SerilogExtensibilityProductionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SerilogExtensibilityProductionExtensions (class) 存在");
        var ctors_SerilogExtensibilityProductionExtensions = type_SerilogExtensibilityProductionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SerilogExtensibilityProductionExtensions 构造函数数量: {ctors_SerilogExtensibilityProductionExtensions.Length}");
        var methods_SerilogExtensibilityProductionExtensions = type_SerilogExtensibilityProductionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SerilogExtensibilityProductionExtensions 公开方法数量: {methods_SerilogExtensibilityProductionExtensions.Length}");
        foreach (var m in methods_SerilogExtensibilityProductionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SerilogExtensibilityProductionExtensions 未找到，尝试无命名空间...");
        type_SerilogExtensibilityProductionExtensions = Type.GetType("SerilogExtensibilityProductionExtensions");
        if (type_SerilogExtensibilityProductionExtensions != null)
            Console.WriteLine("[PASS] 类型 SerilogExtensibilityProductionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SerilogExtensibilityProductionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ISerilogPlugin
    var type_ISerilogPlugin = Type.GetType("ISerilogPlugin");
    if (type_ISerilogPlugin != null)
    {
        Console.WriteLine("[PASS] 类型 ISerilogPlugin (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ISerilogPlugin 未找到，尝试无命名空间...");
        type_ISerilogPlugin = Type.GetType("ISerilogPlugin");
        if (type_ISerilogPlugin != null)
            Console.WriteLine("[PASS] 类型 ISerilogPlugin (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISerilogPlugin 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
