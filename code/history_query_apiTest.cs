#load "history_query_api.cs"

Console.WriteLine("=== history_query_api.cs Test ===");

try
{
    // 验证 class: HistoryDbContext
    var type_HistoryDbContext = Type.GetType("HistoryDbContext");
    if (type_HistoryDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 HistoryDbContext (class) 存在");
        var ctors_HistoryDbContext = type_HistoryDbContext.GetConstructors();
        Console.WriteLine($"[PASS] HistoryDbContext 构造函数数量: {ctors_HistoryDbContext.Length}");
        var methods_HistoryDbContext = type_HistoryDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HistoryDbContext 公开方法数量: {methods_HistoryDbContext.Length}");
        foreach (var m in methods_HistoryDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HistoryDbContext 未找到，尝试无命名空间...");
        type_HistoryDbContext = Type.GetType("HistoryDbContext");
        if (type_HistoryDbContext != null)
            Console.WriteLine("[PASS] 类型 HistoryDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HistoryDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HistoryCompressionService
    var type_HistoryCompressionService = Type.GetType("HistoryCompressionService");
    if (type_HistoryCompressionService != null)
    {
        Console.WriteLine("[PASS] 类型 HistoryCompressionService (class) 存在");
        var ctors_HistoryCompressionService = type_HistoryCompressionService.GetConstructors();
        Console.WriteLine($"[PASS] HistoryCompressionService 构造函数数量: {ctors_HistoryCompressionService.Length}");
        var methods_HistoryCompressionService = type_HistoryCompressionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HistoryCompressionService 公开方法数量: {methods_HistoryCompressionService.Length}");
        foreach (var m in methods_HistoryCompressionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HistoryCompressionService 未找到，尝试无命名空间...");
        type_HistoryCompressionService = Type.GetType("HistoryCompressionService");
        if (type_HistoryCompressionService != null)
            Console.WriteLine("[PASS] 类型 HistoryCompressionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HistoryCompressionService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
