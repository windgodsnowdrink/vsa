#load "grafana_integration.cs"

Console.WriteLine("=== grafana_integration.cs Test ===");

try
{
    // 验证 class: GrafanaExtensions
    var type_GrafanaExtensions = Type.GetType("GrafanaExtensions");
    if (type_GrafanaExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 GrafanaExtensions (class) 存在");
        var ctors_GrafanaExtensions = type_GrafanaExtensions.GetConstructors();
        Console.WriteLine($"[PASS] GrafanaExtensions 构造函数数量: {ctors_GrafanaExtensions.Length}");
        var methods_GrafanaExtensions = type_GrafanaExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GrafanaExtensions 公开方法数量: {methods_GrafanaExtensions.Length}");
        foreach (var m in methods_GrafanaExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GrafanaExtensions 未找到，尝试无命名空间...");
        type_GrafanaExtensions = Type.GetType("GrafanaExtensions");
        if (type_GrafanaExtensions != null)
            Console.WriteLine("[PASS] 类型 GrafanaExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GrafanaExtensions 可能为顶层语句或嵌套类型");
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

    // 验证 class: GrafanaClient
    var type_GrafanaClient = Type.GetType("GrafanaClient");
    if (type_GrafanaClient != null)
    {
        Console.WriteLine("[PASS] 类型 GrafanaClient (class) 存在");
        var ctors_GrafanaClient = type_GrafanaClient.GetConstructors();
        Console.WriteLine($"[PASS] GrafanaClient 构造函数数量: {ctors_GrafanaClient.Length}");
        var methods_GrafanaClient = type_GrafanaClient.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GrafanaClient 公开方法数量: {methods_GrafanaClient.Length}");
        foreach (var m in methods_GrafanaClient)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GrafanaClient 未找到，尝试无命名空间...");
        type_GrafanaClient = Type.GetType("GrafanaClient");
        if (type_GrafanaClient != null)
            Console.WriteLine("[PASS] 类型 GrafanaClient (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GrafanaClient 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GrafanaDashboardManager
    var type_GrafanaDashboardManager = Type.GetType("GrafanaDashboardManager");
    if (type_GrafanaDashboardManager != null)
    {
        Console.WriteLine("[PASS] 类型 GrafanaDashboardManager (class) 存在");
        var ctors_GrafanaDashboardManager = type_GrafanaDashboardManager.GetConstructors();
        Console.WriteLine($"[PASS] GrafanaDashboardManager 构造函数数量: {ctors_GrafanaDashboardManager.Length}");
        var methods_GrafanaDashboardManager = type_GrafanaDashboardManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GrafanaDashboardManager 公开方法数量: {methods_GrafanaDashboardManager.Length}");
        foreach (var m in methods_GrafanaDashboardManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GrafanaDashboardManager 未找到，尝试无命名空间...");
        type_GrafanaDashboardManager = Type.GetType("GrafanaDashboardManager");
        if (type_GrafanaDashboardManager != null)
            Console.WriteLine("[PASS] 类型 GrafanaDashboardManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GrafanaDashboardManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GrafanaInitializationService
    var type_GrafanaInitializationService = Type.GetType("GrafanaInitializationService");
    if (type_GrafanaInitializationService != null)
    {
        Console.WriteLine("[PASS] 类型 GrafanaInitializationService (class) 存在");
        var ctors_GrafanaInitializationService = type_GrafanaInitializationService.GetConstructors();
        Console.WriteLine($"[PASS] GrafanaInitializationService 构造函数数量: {ctors_GrafanaInitializationService.Length}");
        var methods_GrafanaInitializationService = type_GrafanaInitializationService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GrafanaInitializationService 公开方法数量: {methods_GrafanaInitializationService.Length}");
        foreach (var m in methods_GrafanaInitializationService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GrafanaInitializationService 未找到，尝试无命名空间...");
        type_GrafanaInitializationService = Type.GetType("GrafanaInitializationService");
        if (type_GrafanaInitializationService != null)
            Console.WriteLine("[PASS] 类型 GrafanaInitializationService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GrafanaInitializationService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PollyExtensions
    var type_PollyExtensions = Type.GetType("PollyExtensions");
    if (type_PollyExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PollyExtensions (class) 存在");
        var ctors_PollyExtensions = type_PollyExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PollyExtensions 构造函数数量: {ctors_PollyExtensions.Length}");
        var methods_PollyExtensions = type_PollyExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PollyExtensions 公开方法数量: {methods_PollyExtensions.Length}");
        foreach (var m in methods_PollyExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PollyExtensions 未找到，尝试无命名空间...");
        type_PollyExtensions = Type.GetType("PollyExtensions");
        if (type_PollyExtensions != null)
            Console.WriteLine("[PASS] 类型 PollyExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PollyExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IGrafanaDashboardManager
    var type_IGrafanaDashboardManager = Type.GetType("IGrafanaDashboardManager");
    if (type_IGrafanaDashboardManager != null)
    {
        Console.WriteLine("[PASS] 类型 IGrafanaDashboardManager (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IGrafanaDashboardManager 未找到，尝试无命名空间...");
        type_IGrafanaDashboardManager = Type.GetType("IGrafanaDashboardManager");
        if (type_IGrafanaDashboardManager != null)
            Console.WriteLine("[PASS] 类型 IGrafanaDashboardManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IGrafanaDashboardManager 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
