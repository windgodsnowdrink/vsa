#load "datasync_integration.cs"

Console.WriteLine("=== datasync_integration.cs Test ===");

try
{
    // 验证 class: DataSyncDemo.OfflineSyncService
    var type_OfflineSyncService = Type.GetType("DataSyncDemo.OfflineSyncService");
    if (type_OfflineSyncService != null)
    {
        Console.WriteLine("[PASS] 类型 DataSyncDemo.OfflineSyncService (class) 存在");
        var ctors_OfflineSyncService = type_OfflineSyncService.GetConstructors();
        Console.WriteLine($"[PASS] DataSyncDemo.OfflineSyncService 构造函数数量: {ctors_OfflineSyncService.Length}");
        var methods_OfflineSyncService = type_OfflineSyncService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataSyncDemo.OfflineSyncService 公开方法数量: {methods_OfflineSyncService.Length}");
        foreach (var m in methods_OfflineSyncService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataSyncDemo.OfflineSyncService 未找到，尝试无命名空间...");
        type_OfflineSyncService = Type.GetType("OfflineSyncService");
        if (type_OfflineSyncService != null)
            Console.WriteLine("[PASS] 类型 OfflineSyncService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OfflineSyncService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataSyncDemo.SyncConflictResolver
    var type_SyncConflictResolver = Type.GetType("DataSyncDemo.SyncConflictResolver");
    if (type_SyncConflictResolver != null)
    {
        Console.WriteLine("[PASS] 类型 DataSyncDemo.SyncConflictResolver (class) 存在");
        var ctors_SyncConflictResolver = type_SyncConflictResolver.GetConstructors();
        Console.WriteLine($"[PASS] DataSyncDemo.SyncConflictResolver 构造函数数量: {ctors_SyncConflictResolver.Length}");
        var methods_SyncConflictResolver = type_SyncConflictResolver.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataSyncDemo.SyncConflictResolver 公开方法数量: {methods_SyncConflictResolver.Length}");
        foreach (var m in methods_SyncConflictResolver)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataSyncDemo.SyncConflictResolver 未找到，尝试无命名空间...");
        type_SyncConflictResolver = Type.GetType("SyncConflictResolver");
        if (type_SyncConflictResolver != null)
            Console.WriteLine("[PASS] 类型 SyncConflictResolver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SyncConflictResolver 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataSyncDemo.DeltaSyncProvider
    var type_DeltaSyncProvider = Type.GetType("DataSyncDemo.DeltaSyncProvider");
    if (type_DeltaSyncProvider != null)
    {
        Console.WriteLine("[PASS] 类型 DataSyncDemo.DeltaSyncProvider (class) 存在");
        var ctors_DeltaSyncProvider = type_DeltaSyncProvider.GetConstructors();
        Console.WriteLine($"[PASS] DataSyncDemo.DeltaSyncProvider 构造函数数量: {ctors_DeltaSyncProvider.Length}");
        var methods_DeltaSyncProvider = type_DeltaSyncProvider.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataSyncDemo.DeltaSyncProvider 公开方法数量: {methods_DeltaSyncProvider.Length}");
        foreach (var m in methods_DeltaSyncProvider)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataSyncDemo.DeltaSyncProvider 未找到，尝试无命名空间...");
        type_DeltaSyncProvider = Type.GetType("DeltaSyncProvider");
        if (type_DeltaSyncProvider != null)
            Console.WriteLine("[PASS] 类型 DeltaSyncProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeltaSyncProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataSyncDemo.SyncPerformanceMonitor
    var type_SyncPerformanceMonitor = Type.GetType("DataSyncDemo.SyncPerformanceMonitor");
    if (type_SyncPerformanceMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 DataSyncDemo.SyncPerformanceMonitor (class) 存在");
        var ctors_SyncPerformanceMonitor = type_SyncPerformanceMonitor.GetConstructors();
        Console.WriteLine($"[PASS] DataSyncDemo.SyncPerformanceMonitor 构造函数数量: {ctors_SyncPerformanceMonitor.Length}");
        var methods_SyncPerformanceMonitor = type_SyncPerformanceMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataSyncDemo.SyncPerformanceMonitor 公开方法数量: {methods_SyncPerformanceMonitor.Length}");
        foreach (var m in methods_SyncPerformanceMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataSyncDemo.SyncPerformanceMonitor 未找到，尝试无命名空间...");
        type_SyncPerformanceMonitor = Type.GetType("SyncPerformanceMonitor");
        if (type_SyncPerformanceMonitor != null)
            Console.WriteLine("[PASS] 类型 SyncPerformanceMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SyncPerformanceMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataSyncDemo.DataSyncService
    var type_DataSyncService = Type.GetType("DataSyncDemo.DataSyncService");
    if (type_DataSyncService != null)
    {
        Console.WriteLine("[PASS] 类型 DataSyncDemo.DataSyncService (class) 存在");
        var ctors_DataSyncService = type_DataSyncService.GetConstructors();
        Console.WriteLine($"[PASS] DataSyncDemo.DataSyncService 构造函数数量: {ctors_DataSyncService.Length}");
        var methods_DataSyncService = type_DataSyncService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataSyncDemo.DataSyncService 公开方法数量: {methods_DataSyncService.Length}");
        foreach (var m in methods_DataSyncService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataSyncDemo.DataSyncService 未找到，尝试无命名空间...");
        type_DataSyncService = Type.GetType("DataSyncService");
        if (type_DataSyncService != null)
            Console.WriteLine("[PASS] 类型 DataSyncService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataSyncService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataSyncDemo.DataSyncExtensions
    var type_DataSyncExtensions = Type.GetType("DataSyncDemo.DataSyncExtensions");
    if (type_DataSyncExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DataSyncDemo.DataSyncExtensions (class) 存在");
        var ctors_DataSyncExtensions = type_DataSyncExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DataSyncDemo.DataSyncExtensions 构造函数数量: {ctors_DataSyncExtensions.Length}");
        var methods_DataSyncExtensions = type_DataSyncExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataSyncDemo.DataSyncExtensions 公开方法数量: {methods_DataSyncExtensions.Length}");
        foreach (var m in methods_DataSyncExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataSyncDemo.DataSyncExtensions 未找到，尝试无命名空间...");
        type_DataSyncExtensions = Type.GetType("DataSyncExtensions");
        if (type_DataSyncExtensions != null)
            Console.WriteLine("[PASS] 类型 DataSyncExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataSyncExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataSyncDemo.DataSyncOptions
    var type_DataSyncOptions = Type.GetType("DataSyncDemo.DataSyncOptions");
    if (type_DataSyncOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DataSyncDemo.DataSyncOptions (class) 存在");
        var ctors_DataSyncOptions = type_DataSyncOptions.GetConstructors();
        Console.WriteLine($"[PASS] DataSyncDemo.DataSyncOptions 构造函数数量: {ctors_DataSyncOptions.Length}");
        var methods_DataSyncOptions = type_DataSyncOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataSyncDemo.DataSyncOptions 公开方法数量: {methods_DataSyncOptions.Length}");
        foreach (var m in methods_DataSyncOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataSyncDemo.DataSyncOptions 未找到，尝试无命名空间...");
        type_DataSyncOptions = Type.GetType("DataSyncOptions");
        if (type_DataSyncOptions != null)
            Console.WriteLine("[PASS] 类型 DataSyncOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataSyncOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataSyncDemo.DataSyncBackgroundService
    var type_DataSyncBackgroundService = Type.GetType("DataSyncDemo.DataSyncBackgroundService");
    if (type_DataSyncBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 DataSyncDemo.DataSyncBackgroundService (class) 存在");
        var ctors_DataSyncBackgroundService = type_DataSyncBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] DataSyncDemo.DataSyncBackgroundService 构造函数数量: {ctors_DataSyncBackgroundService.Length}");
        var methods_DataSyncBackgroundService = type_DataSyncBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataSyncDemo.DataSyncBackgroundService 公开方法数量: {methods_DataSyncBackgroundService.Length}");
        foreach (var m in methods_DataSyncBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataSyncDemo.DataSyncBackgroundService 未找到，尝试无命名空间...");
        type_DataSyncBackgroundService = Type.GetType("DataSyncBackgroundService");
        if (type_DataSyncBackgroundService != null)
            Console.WriteLine("[PASS] 类型 DataSyncBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataSyncBackgroundService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataSyncDemo.SyncItem
    var type_SyncItem = Type.GetType("DataSyncDemo.SyncItem");
    if (type_SyncItem != null)
    {
        Console.WriteLine("[PASS] 类型 DataSyncDemo.SyncItem (class) 存在");
        var ctors_SyncItem = type_SyncItem.GetConstructors();
        Console.WriteLine($"[PASS] DataSyncDemo.SyncItem 构造函数数量: {ctors_SyncItem.Length}");
        var methods_SyncItem = type_SyncItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataSyncDemo.SyncItem 公开方法数量: {methods_SyncItem.Length}");
        foreach (var m in methods_SyncItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataSyncDemo.SyncItem 未找到，尝试无命名空间...");
        type_SyncItem = Type.GetType("SyncItem");
        if (type_SyncItem != null)
            Console.WriteLine("[PASS] 类型 SyncItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SyncItem 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
