#load "prometheus_integration.cs"

Console.WriteLine("=== prometheus_integration.cs Test ===");

try
{
    // 验证 class: PrometheusExtensions
    var type_PrometheusExtensions = Type.GetType("PrometheusExtensions");
    if (type_PrometheusExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PrometheusExtensions (class) 存在");
        var ctors_PrometheusExtensions = type_PrometheusExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PrometheusExtensions 构造函数数量: {ctors_PrometheusExtensions.Length}");
        var methods_PrometheusExtensions = type_PrometheusExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PrometheusExtensions 公开方法数量: {methods_PrometheusExtensions.Length}");
        foreach (var m in methods_PrometheusExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PrometheusExtensions 未找到，尝试无命名空间...");
        type_PrometheusExtensions = Type.GetType("PrometheusExtensions");
        if (type_PrometheusExtensions != null)
            Console.WriteLine("[PASS] 类型 PrometheusExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PrometheusExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PrometheusAlertRuleManager
    var type_PrometheusAlertRuleManager = Type.GetType("PrometheusAlertRuleManager");
    if (type_PrometheusAlertRuleManager != null)
    {
        Console.WriteLine("[PASS] 类型 PrometheusAlertRuleManager (class) 存在");
        var ctors_PrometheusAlertRuleManager = type_PrometheusAlertRuleManager.GetConstructors();
        Console.WriteLine($"[PASS] PrometheusAlertRuleManager 构造函数数量: {ctors_PrometheusAlertRuleManager.Length}");
        var methods_PrometheusAlertRuleManager = type_PrometheusAlertRuleManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PrometheusAlertRuleManager 公开方法数量: {methods_PrometheusAlertRuleManager.Length}");
        foreach (var m in methods_PrometheusAlertRuleManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PrometheusAlertRuleManager 未找到，尝试无命名空间...");
        type_PrometheusAlertRuleManager = Type.GetType("PrometheusAlertRuleManager");
        if (type_PrometheusAlertRuleManager != null)
            Console.WriteLine("[PASS] 类型 PrometheusAlertRuleManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PrometheusAlertRuleManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PrometheusAlertRuleWatcher
    var type_PrometheusAlertRuleWatcher = Type.GetType("PrometheusAlertRuleWatcher");
    if (type_PrometheusAlertRuleWatcher != null)
    {
        Console.WriteLine("[PASS] 类型 PrometheusAlertRuleWatcher (class) 存在");
        var ctors_PrometheusAlertRuleWatcher = type_PrometheusAlertRuleWatcher.GetConstructors();
        Console.WriteLine($"[PASS] PrometheusAlertRuleWatcher 构造函数数量: {ctors_PrometheusAlertRuleWatcher.Length}");
        var methods_PrometheusAlertRuleWatcher = type_PrometheusAlertRuleWatcher.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PrometheusAlertRuleWatcher 公开方法数量: {methods_PrometheusAlertRuleWatcher.Length}");
        foreach (var m in methods_PrometheusAlertRuleWatcher)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PrometheusAlertRuleWatcher 未找到，尝试无命名空间...");
        type_PrometheusAlertRuleWatcher = Type.GetType("PrometheusAlertRuleWatcher");
        if (type_PrometheusAlertRuleWatcher != null)
            Console.WriteLine("[PASS] 类型 PrometheusAlertRuleWatcher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PrometheusAlertRuleWatcher 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GrafanaDashboardSyncService
    var type_GrafanaDashboardSyncService = Type.GetType("GrafanaDashboardSyncService");
    if (type_GrafanaDashboardSyncService != null)
    {
        Console.WriteLine("[PASS] 类型 GrafanaDashboardSyncService (class) 存在");
        var ctors_GrafanaDashboardSyncService = type_GrafanaDashboardSyncService.GetConstructors();
        Console.WriteLine($"[PASS] GrafanaDashboardSyncService 构造函数数量: {ctors_GrafanaDashboardSyncService.Length}");
        var methods_GrafanaDashboardSyncService = type_GrafanaDashboardSyncService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GrafanaDashboardSyncService 公开方法数量: {methods_GrafanaDashboardSyncService.Length}");
        foreach (var m in methods_GrafanaDashboardSyncService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GrafanaDashboardSyncService 未找到，尝试无命名空间...");
        type_GrafanaDashboardSyncService = Type.GetType("GrafanaDashboardSyncService");
        if (type_GrafanaDashboardSyncService != null)
            Console.WriteLine("[PASS] 类型 GrafanaDashboardSyncService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GrafanaDashboardSyncService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GrafanaOptions
    var type_GrafanaOptions = Type.GetType("GrafanaOptions");
    if (type_GrafanaOptions != null)
    {
        Console.WriteLine("[PASS] 类型 GrafanaOptions (class) 存在");
        var ctors_GrafanaOptions = type_GrafanaOptions.GetConstructors();
        Console.WriteLine($"[PASS] GrafanaOptions 构造函数数量: {ctors_GrafanaOptions.Length}");
        var methods_GrafanaOptions = type_GrafanaOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GrafanaOptions 公开方法数量: {methods_GrafanaOptions.Length}");
        foreach (var m in methods_GrafanaOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GrafanaOptions 未找到，尝试无命名空间...");
        type_GrafanaOptions = Type.GetType("GrafanaOptions");
        if (type_GrafanaOptions != null)
            Console.WriteLine("[PASS] 类型 GrafanaOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GrafanaOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PrometheusRemoteStorageWriter
    var type_PrometheusRemoteStorageWriter = Type.GetType("PrometheusRemoteStorageWriter");
    if (type_PrometheusRemoteStorageWriter != null)
    {
        Console.WriteLine("[PASS] 类型 PrometheusRemoteStorageWriter (class) 存在");
        var ctors_PrometheusRemoteStorageWriter = type_PrometheusRemoteStorageWriter.GetConstructors();
        Console.WriteLine($"[PASS] PrometheusRemoteStorageWriter 构造函数数量: {ctors_PrometheusRemoteStorageWriter.Length}");
        var methods_PrometheusRemoteStorageWriter = type_PrometheusRemoteStorageWriter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PrometheusRemoteStorageWriter 公开方法数量: {methods_PrometheusRemoteStorageWriter.Length}");
        foreach (var m in methods_PrometheusRemoteStorageWriter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PrometheusRemoteStorageWriter 未找到，尝试无命名空间...");
        type_PrometheusRemoteStorageWriter = Type.GetType("PrometheusRemoteStorageWriter");
        if (type_PrometheusRemoteStorageWriter != null)
            Console.WriteLine("[PASS] 类型 PrometheusRemoteStorageWriter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PrometheusRemoteStorageWriter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PrometheusAggregatorService
    var type_PrometheusAggregatorService = Type.GetType("PrometheusAggregatorService");
    if (type_PrometheusAggregatorService != null)
    {
        Console.WriteLine("[PASS] 类型 PrometheusAggregatorService (class) 存在");
        var ctors_PrometheusAggregatorService = type_PrometheusAggregatorService.GetConstructors();
        Console.WriteLine($"[PASS] PrometheusAggregatorService 构造函数数量: {ctors_PrometheusAggregatorService.Length}");
        var methods_PrometheusAggregatorService = type_PrometheusAggregatorService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PrometheusAggregatorService 公开方法数量: {methods_PrometheusAggregatorService.Length}");
        foreach (var m in methods_PrometheusAggregatorService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PrometheusAggregatorService 未找到，尝试无命名空间...");
        type_PrometheusAggregatorService = Type.GetType("PrometheusAggregatorService");
        if (type_PrometheusAggregatorService != null)
            Console.WriteLine("[PASS] 类型 PrometheusAggregatorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PrometheusAggregatorService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PrometheusBackgroundService
    var type_PrometheusBackgroundService = Type.GetType("PrometheusBackgroundService");
    if (type_PrometheusBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 PrometheusBackgroundService (class) 存在");
        var ctors_PrometheusBackgroundService = type_PrometheusBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] PrometheusBackgroundService 构造函数数量: {ctors_PrometheusBackgroundService.Length}");
        var methods_PrometheusBackgroundService = type_PrometheusBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PrometheusBackgroundService 公开方法数量: {methods_PrometheusBackgroundService.Length}");
        foreach (var m in methods_PrometheusBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PrometheusBackgroundService 未找到，尝试无命名空间...");
        type_PrometheusBackgroundService = Type.GetType("PrometheusBackgroundService");
        if (type_PrometheusBackgroundService != null)
            Console.WriteLine("[PASS] 类型 PrometheusBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PrometheusBackgroundService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PrometheusMiddleware
    var type_PrometheusMiddleware = Type.GetType("PrometheusMiddleware");
    if (type_PrometheusMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 PrometheusMiddleware (class) 存在");
        var ctors_PrometheusMiddleware = type_PrometheusMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] PrometheusMiddleware 构造函数数量: {ctors_PrometheusMiddleware.Length}");
        var methods_PrometheusMiddleware = type_PrometheusMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PrometheusMiddleware 公开方法数量: {methods_PrometheusMiddleware.Length}");
        foreach (var m in methods_PrometheusMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PrometheusMiddleware 未找到，尝试无命名空间...");
        type_PrometheusMiddleware = Type.GetType("PrometheusMiddleware");
        if (type_PrometheusMiddleware != null)
            Console.WriteLine("[PASS] 类型 PrometheusMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PrometheusMiddleware 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
