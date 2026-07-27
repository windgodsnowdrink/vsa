#load "realtime_etl.cs"

Console.WriteLine("=== realtime_etl.cs Test ===");

try
{
    // 验证 class: RealtimeEtlProcessor
    var type_RealtimeEtlProcessor = Type.GetType("RealtimeEtlProcessor");
    if (type_RealtimeEtlProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 RealtimeEtlProcessor (class) 存在");
        var ctors_RealtimeEtlProcessor = type_RealtimeEtlProcessor.GetConstructors();
        Console.WriteLine($"[PASS] RealtimeEtlProcessor 构造函数数量: {ctors_RealtimeEtlProcessor.Length}");
        var methods_RealtimeEtlProcessor = type_RealtimeEtlProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RealtimeEtlProcessor 公开方法数量: {methods_RealtimeEtlProcessor.Length}");
        foreach (var m in methods_RealtimeEtlProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RealtimeEtlProcessor 未找到，尝试无命名空间...");
        type_RealtimeEtlProcessor = Type.GetType("RealtimeEtlProcessor");
        if (type_RealtimeEtlProcessor != null)
            Console.WriteLine("[PASS] 类型 RealtimeEtlProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RealtimeEtlProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ParsedData
    var type_ParsedData = Type.GetType("ParsedData");
    if (type_ParsedData != null)
    {
        Console.WriteLine("[PASS] 类型 ParsedData (record) 存在");
        var ctors_ParsedData = type_ParsedData.GetConstructors();
        Console.WriteLine($"[PASS] ParsedData 构造函数数量: {ctors_ParsedData.Length}");
        var methods_ParsedData = type_ParsedData.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ParsedData 公开方法数量: {methods_ParsedData.Length}");
        foreach (var m in methods_ParsedData)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ParsedData 未找到，尝试无命名空间...");
        type_ParsedData = Type.GetType("ParsedData");
        if (type_ParsedData != null)
            Console.WriteLine("[PASS] 类型 ParsedData (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ParsedData 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TransformedData
    var type_TransformedData = Type.GetType("TransformedData");
    if (type_TransformedData != null)
    {
        Console.WriteLine("[PASS] 类型 TransformedData (record) 存在");
        var ctors_TransformedData = type_TransformedData.GetConstructors();
        Console.WriteLine($"[PASS] TransformedData 构造函数数量: {ctors_TransformedData.Length}");
        var methods_TransformedData = type_TransformedData.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TransformedData 公开方法数量: {methods_TransformedData.Length}");
        foreach (var m in methods_TransformedData)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TransformedData 未找到，尝试无命名空间...");
        type_TransformedData = Type.GetType("TransformedData");
        if (type_TransformedData != null)
            Console.WriteLine("[PASS] 类型 TransformedData (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TransformedData 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
