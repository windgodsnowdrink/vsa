#load "aspire_integration.cs"

Console.WriteLine("=== aspire_integration.cs Test ===");

try
{
    // 验证 class: AspireIntegration.AspireOptions
    var type_AspireOptions = Type.GetType("AspireIntegration.AspireOptions");
    if (type_AspireOptions != null)
    {
        Console.WriteLine("[PASS] 类型 AspireIntegration.AspireOptions (class) 存在");
        var ctors_AspireOptions = type_AspireOptions.GetConstructors();
        Console.WriteLine($"[PASS] AspireIntegration.AspireOptions 构造函数数量: {ctors_AspireOptions.Length}");
        var methods_AspireOptions = type_AspireOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AspireIntegration.AspireOptions 公开方法数量: {methods_AspireOptions.Length}");
        foreach (var m in methods_AspireOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AspireIntegration.AspireOptions 未找到，尝试无命名空间...");
        type_AspireOptions = Type.GetType("AspireOptions");
        if (type_AspireOptions != null)
            Console.WriteLine("[PASS] 类型 AspireOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AspireOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AspireIntegration.AspireService
    var type_AspireService = Type.GetType("AspireIntegration.AspireService");
    if (type_AspireService != null)
    {
        Console.WriteLine("[PASS] 类型 AspireIntegration.AspireService (class) 存在");
        var ctors_AspireService = type_AspireService.GetConstructors();
        Console.WriteLine($"[PASS] AspireIntegration.AspireService 构造函数数量: {ctors_AspireService.Length}");
        var methods_AspireService = type_AspireService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AspireIntegration.AspireService 公开方法数量: {methods_AspireService.Length}");
        foreach (var m in methods_AspireService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AspireIntegration.AspireService 未找到，尝试无命名空间...");
        type_AspireService = Type.GetType("AspireService");
        if (type_AspireService != null)
            Console.WriteLine("[PASS] 类型 AspireService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AspireService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AspireIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("AspireIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 AspireIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] AspireIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AspireIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AspireIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AspireIntegration.HealthReport
    var type_HealthReport = Type.GetType("AspireIntegration.HealthReport");
    if (type_HealthReport != null)
    {
        Console.WriteLine("[PASS] 类型 AspireIntegration.HealthReport (class) 存在");
        var ctors_HealthReport = type_HealthReport.GetConstructors();
        Console.WriteLine($"[PASS] AspireIntegration.HealthReport 构造函数数量: {ctors_HealthReport.Length}");
        var methods_HealthReport = type_HealthReport.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AspireIntegration.HealthReport 公开方法数量: {methods_HealthReport.Length}");
        foreach (var m in methods_HealthReport)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AspireIntegration.HealthReport 未找到，尝试无命名空间...");
        type_HealthReport = Type.GetType("HealthReport");
        if (type_HealthReport != null)
            Console.WriteLine("[PASS] 类型 HealthReport (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HealthReport 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AspireIntegration.MetricsSnapshot
    var type_MetricsSnapshot = Type.GetType("AspireIntegration.MetricsSnapshot");
    if (type_MetricsSnapshot != null)
    {
        Console.WriteLine("[PASS] 类型 AspireIntegration.MetricsSnapshot (class) 存在");
        var ctors_MetricsSnapshot = type_MetricsSnapshot.GetConstructors();
        Console.WriteLine($"[PASS] AspireIntegration.MetricsSnapshot 构造函数数量: {ctors_MetricsSnapshot.Length}");
        var methods_MetricsSnapshot = type_MetricsSnapshot.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AspireIntegration.MetricsSnapshot 公开方法数量: {methods_MetricsSnapshot.Length}");
        foreach (var m in methods_MetricsSnapshot)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AspireIntegration.MetricsSnapshot 未找到，尝试无命名空间...");
        type_MetricsSnapshot = Type.GetType("MetricsSnapshot");
        if (type_MetricsSnapshot != null)
            Console.WriteLine("[PASS] 类型 MetricsSnapshot (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MetricsSnapshot 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: AspireIntegration.IAspireService
    var type_IAspireService = Type.GetType("AspireIntegration.IAspireService");
    if (type_IAspireService != null)
    {
        Console.WriteLine("[PASS] 类型 AspireIntegration.IAspireService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AspireIntegration.IAspireService 未找到，尝试无命名空间...");
        type_IAspireService = Type.GetType("IAspireService");
        if (type_IAspireService != null)
            Console.WriteLine("[PASS] 类型 IAspireService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAspireService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
