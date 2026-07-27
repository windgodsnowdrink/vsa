#load "realtime_analytics.cs"

Console.WriteLine("=== realtime_analytics.cs Test ===");

try
{
    // 验证 class: RealtimeAnalyticsEngine
    var type_RealtimeAnalyticsEngine = Type.GetType("RealtimeAnalyticsEngine");
    if (type_RealtimeAnalyticsEngine != null)
    {
        Console.WriteLine("[PASS] 类型 RealtimeAnalyticsEngine (class) 存在");
        var ctors_RealtimeAnalyticsEngine = type_RealtimeAnalyticsEngine.GetConstructors();
        Console.WriteLine($"[PASS] RealtimeAnalyticsEngine 构造函数数量: {ctors_RealtimeAnalyticsEngine.Length}");
        var methods_RealtimeAnalyticsEngine = type_RealtimeAnalyticsEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RealtimeAnalyticsEngine 公开方法数量: {methods_RealtimeAnalyticsEngine.Length}");
        foreach (var m in methods_RealtimeAnalyticsEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RealtimeAnalyticsEngine 未找到，尝试无命名空间...");
        type_RealtimeAnalyticsEngine = Type.GetType("RealtimeAnalyticsEngine");
        if (type_RealtimeAnalyticsEngine != null)
            Console.WriteLine("[PASS] 类型 RealtimeAnalyticsEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RealtimeAnalyticsEngine 可能为顶层语句或嵌套类型");
    }

    // 验证 record: AnalyticsData
    var type_AnalyticsData = Type.GetType("AnalyticsData");
    if (type_AnalyticsData != null)
    {
        Console.WriteLine("[PASS] 类型 AnalyticsData (record) 存在");
        var ctors_AnalyticsData = type_AnalyticsData.GetConstructors();
        Console.WriteLine($"[PASS] AnalyticsData 构造函数数量: {ctors_AnalyticsData.Length}");
        var methods_AnalyticsData = type_AnalyticsData.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AnalyticsData 公开方法数量: {methods_AnalyticsData.Length}");
        foreach (var m in methods_AnalyticsData)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AnalyticsData 未找到，尝试无命名空间...");
        type_AnalyticsData = Type.GetType("AnalyticsData");
        if (type_AnalyticsData != null)
            Console.WriteLine("[PASS] 类型 AnalyticsData (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AnalyticsData 可能为顶层语句或嵌套类型");
    }

    // 验证 record: AnalyticsResult
    var type_AnalyticsResult = Type.GetType("AnalyticsResult");
    if (type_AnalyticsResult != null)
    {
        Console.WriteLine("[PASS] 类型 AnalyticsResult (record) 存在");
        var ctors_AnalyticsResult = type_AnalyticsResult.GetConstructors();
        Console.WriteLine($"[PASS] AnalyticsResult 构造函数数量: {ctors_AnalyticsResult.Length}");
        var methods_AnalyticsResult = type_AnalyticsResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AnalyticsResult 公开方法数量: {methods_AnalyticsResult.Length}");
        foreach (var m in methods_AnalyticsResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AnalyticsResult 未找到，尝试无命名空间...");
        type_AnalyticsResult = Type.GetType("AnalyticsResult");
        if (type_AnalyticsResult != null)
            Console.WriteLine("[PASS] 类型 AnalyticsResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AnalyticsResult 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
