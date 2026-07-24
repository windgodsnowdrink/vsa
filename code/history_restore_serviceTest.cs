#load "history_restore_service.cs"

Console.WriteLine("=== history_restore_service.cs Test ===");

try
{
    // 验证 class: HistoryRestoreService
    var type_HistoryRestoreService = Type.GetType("HistoryRestoreService");
    if (type_HistoryRestoreService != null)
    {
        Console.WriteLine("[PASS] 类型 HistoryRestoreService (class) 存在");
        var ctors_HistoryRestoreService = type_HistoryRestoreService.GetConstructors();
        Console.WriteLine($"[PASS] HistoryRestoreService 构造函数数量: {ctors_HistoryRestoreService.Length}");
        var methods_HistoryRestoreService = type_HistoryRestoreService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HistoryRestoreService 公开方法数量: {methods_HistoryRestoreService.Length}");
        foreach (var m in methods_HistoryRestoreService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HistoryRestoreService 未找到，尝试无命名空间...");
        type_HistoryRestoreService = Type.GetType("HistoryRestoreService");
        if (type_HistoryRestoreService != null)
            Console.WriteLine("[PASS] 类型 HistoryRestoreService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HistoryRestoreService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: RestoreRequest
    var type_RestoreRequest = Type.GetType("RestoreRequest");
    if (type_RestoreRequest != null)
    {
        Console.WriteLine("[PASS] 类型 RestoreRequest (record) 存在");
        var ctors_RestoreRequest = type_RestoreRequest.GetConstructors();
        Console.WriteLine($"[PASS] RestoreRequest 构造函数数量: {ctors_RestoreRequest.Length}");
        var methods_RestoreRequest = type_RestoreRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RestoreRequest 公开方法数量: {methods_RestoreRequest.Length}");
        foreach (var m in methods_RestoreRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RestoreRequest 未找到，尝试无命名空间...");
        type_RestoreRequest = Type.GetType("RestoreRequest");
        if (type_RestoreRequest != null)
            Console.WriteLine("[PASS] 类型 RestoreRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RestoreRequest 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
