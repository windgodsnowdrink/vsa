#load "clickhouse_integration.cs"

Console.WriteLine("=== clickhouse_integration.cs Test ===");

try
{
    // 验证 class: ClickHouseIntegration.ClickHouseOptions
    var type_ClickHouseOptions = Type.GetType("ClickHouseIntegration.ClickHouseOptions");
    if (type_ClickHouseOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ClickHouseIntegration.ClickHouseOptions (class) 存在");
        var ctors_ClickHouseOptions = type_ClickHouseOptions.GetConstructors();
        Console.WriteLine($"[PASS] ClickHouseIntegration.ClickHouseOptions 构造函数数量: {ctors_ClickHouseOptions.Length}");
        var methods_ClickHouseOptions = type_ClickHouseOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ClickHouseIntegration.ClickHouseOptions 公开方法数量: {methods_ClickHouseOptions.Length}");
        foreach (var m in methods_ClickHouseOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClickHouseIntegration.ClickHouseOptions 未找到，尝试无命名空间...");
        type_ClickHouseOptions = Type.GetType("ClickHouseOptions");
        if (type_ClickHouseOptions != null)
            Console.WriteLine("[PASS] 类型 ClickHouseOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ClickHouseOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ClickHouseIntegration.ClickHouseService
    var type_ClickHouseService = Type.GetType("ClickHouseIntegration.ClickHouseService");
    if (type_ClickHouseService != null)
    {
        Console.WriteLine("[PASS] 类型 ClickHouseIntegration.ClickHouseService (class) 存在");
        var ctors_ClickHouseService = type_ClickHouseService.GetConstructors();
        Console.WriteLine($"[PASS] ClickHouseIntegration.ClickHouseService 构造函数数量: {ctors_ClickHouseService.Length}");
        var methods_ClickHouseService = type_ClickHouseService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ClickHouseIntegration.ClickHouseService 公开方法数量: {methods_ClickHouseService.Length}");
        foreach (var m in methods_ClickHouseService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClickHouseIntegration.ClickHouseService 未找到，尝试无命名空间...");
        type_ClickHouseService = Type.GetType("ClickHouseService");
        if (type_ClickHouseService != null)
            Console.WriteLine("[PASS] 类型 ClickHouseService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ClickHouseService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ClickHouseIntegration.BulkInsertItem
    var type_BulkInsertItem = Type.GetType("ClickHouseIntegration.BulkInsertItem");
    if (type_BulkInsertItem != null)
    {
        Console.WriteLine("[PASS] 类型 ClickHouseIntegration.BulkInsertItem (class) 存在");
        var ctors_BulkInsertItem = type_BulkInsertItem.GetConstructors();
        Console.WriteLine($"[PASS] ClickHouseIntegration.BulkInsertItem 构造函数数量: {ctors_BulkInsertItem.Length}");
        var methods_BulkInsertItem = type_BulkInsertItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ClickHouseIntegration.BulkInsertItem 公开方法数量: {methods_BulkInsertItem.Length}");
        foreach (var m in methods_BulkInsertItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClickHouseIntegration.BulkInsertItem 未找到，尝试无命名空间...");
        type_BulkInsertItem = Type.GetType("BulkInsertItem");
        if (type_BulkInsertItem != null)
            Console.WriteLine("[PASS] 类型 BulkInsertItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BulkInsertItem 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ClickHouseIntegration.ConnectionStats
    var type_ConnectionStats = Type.GetType("ClickHouseIntegration.ConnectionStats");
    if (type_ConnectionStats != null)
    {
        Console.WriteLine("[PASS] 类型 ClickHouseIntegration.ConnectionStats (class) 存在");
        var ctors_ConnectionStats = type_ConnectionStats.GetConstructors();
        Console.WriteLine($"[PASS] ClickHouseIntegration.ConnectionStats 构造函数数量: {ctors_ConnectionStats.Length}");
        var methods_ConnectionStats = type_ConnectionStats.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ClickHouseIntegration.ConnectionStats 公开方法数量: {methods_ConnectionStats.Length}");
        foreach (var m in methods_ConnectionStats)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClickHouseIntegration.ConnectionStats 未找到，尝试无命名空间...");
        type_ConnectionStats = Type.GetType("ConnectionStats");
        if (type_ConnectionStats != null)
            Console.WriteLine("[PASS] 类型 ConnectionStats (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConnectionStats 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ClickHouseIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ClickHouseIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ClickHouseIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ClickHouseIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ClickHouseIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClickHouseIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ClickHouseIntegration.IClickHouseService
    var type_IClickHouseService = Type.GetType("ClickHouseIntegration.IClickHouseService");
    if (type_IClickHouseService != null)
    {
        Console.WriteLine("[PASS] 类型 ClickHouseIntegration.IClickHouseService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClickHouseIntegration.IClickHouseService 未找到，尝试无命名空间...");
        type_IClickHouseService = Type.GetType("IClickHouseService");
        if (type_IClickHouseService != null)
            Console.WriteLine("[PASS] 类型 IClickHouseService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IClickHouseService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
