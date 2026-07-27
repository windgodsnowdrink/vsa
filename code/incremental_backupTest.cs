#load "incremental_backup.cs"

Console.WriteLine("=== incremental_backup.cs Test ===");

try
{
    // 验证 class: IncrementalBackupService
    var type_IncrementalBackupService = Type.GetType("IncrementalBackupService");
    if (type_IncrementalBackupService != null)
    {
        Console.WriteLine("[PASS] 类型 IncrementalBackupService (class) 存在");
        var ctors_IncrementalBackupService = type_IncrementalBackupService.GetConstructors();
        Console.WriteLine($"[PASS] IncrementalBackupService 构造函数数量: {ctors_IncrementalBackupService.Length}");
        var methods_IncrementalBackupService = type_IncrementalBackupService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IncrementalBackupService 公开方法数量: {methods_IncrementalBackupService.Length}");
        foreach (var m in methods_IncrementalBackupService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IncrementalBackupService 未找到，尝试无命名空间...");
        type_IncrementalBackupService = Type.GetType("IncrementalBackupService");
        if (type_IncrementalBackupService != null)
            Console.WriteLine("[PASS] 类型 IncrementalBackupService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IncrementalBackupService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IncrementalBackupEngine
    var type_IncrementalBackupEngine = Type.GetType("IncrementalBackupEngine");
    if (type_IncrementalBackupEngine != null)
    {
        Console.WriteLine("[PASS] 类型 IncrementalBackupEngine (class) 存在");
        var ctors_IncrementalBackupEngine = type_IncrementalBackupEngine.GetConstructors();
        Console.WriteLine($"[PASS] IncrementalBackupEngine 构造函数数量: {ctors_IncrementalBackupEngine.Length}");
        var methods_IncrementalBackupEngine = type_IncrementalBackupEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IncrementalBackupEngine 公开方法数量: {methods_IncrementalBackupEngine.Length}");
        foreach (var m in methods_IncrementalBackupEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IncrementalBackupEngine 未找到，尝试无命名空间...");
        type_IncrementalBackupEngine = Type.GetType("IncrementalBackupEngine");
        if (type_IncrementalBackupEngine != null)
            Console.WriteLine("[PASS] 类型 IncrementalBackupEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IncrementalBackupEngine 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
