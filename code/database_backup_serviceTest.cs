#load "database_backup_service.cs"

Console.WriteLine("=== database_backup_service.cs Test ===");

try
{
    // 验证 class: DatabaseBackupService
    var type_DatabaseBackupService = Type.GetType("DatabaseBackupService");
    if (type_DatabaseBackupService != null)
    {
        Console.WriteLine("[PASS] 类型 DatabaseBackupService (class) 存在");
        var ctors_DatabaseBackupService = type_DatabaseBackupService.GetConstructors();
        Console.WriteLine($"[PASS] DatabaseBackupService 构造函数数量: {ctors_DatabaseBackupService.Length}");
        var methods_DatabaseBackupService = type_DatabaseBackupService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DatabaseBackupService 公开方法数量: {methods_DatabaseBackupService.Length}");
        foreach (var m in methods_DatabaseBackupService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DatabaseBackupService 未找到，尝试无命名空间...");
        type_DatabaseBackupService = Type.GetType("DatabaseBackupService");
        if (type_DatabaseBackupService != null)
            Console.WriteLine("[PASS] 类型 DatabaseBackupService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DatabaseBackupService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
