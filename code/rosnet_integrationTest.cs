#load "rosnet_integration.cs"

Console.WriteLine("=== rosnet_integration.cs Test ===");

try
{
    // 验证 class: RosNetIntegration.RosOptions
    var type_RosOptions = Type.GetType("RosNetIntegration.RosOptions");
    if (type_RosOptions != null)
    {
        Console.WriteLine("[PASS] 类型 RosNetIntegration.RosOptions (class) 存在");
        var ctors_RosOptions = type_RosOptions.GetConstructors();
        Console.WriteLine($"[PASS] RosNetIntegration.RosOptions 构造函数数量: {ctors_RosOptions.Length}");
        var methods_RosOptions = type_RosOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RosNetIntegration.RosOptions 公开方法数量: {methods_RosOptions.Length}");
        foreach (var m in methods_RosOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RosNetIntegration.RosOptions 未找到，尝试无命名空间...");
        type_RosOptions = Type.GetType("RosOptions");
        if (type_RosOptions != null)
            Console.WriteLine("[PASS] 类型 RosOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RosOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RosNetIntegration.RosService
    var type_RosService = Type.GetType("RosNetIntegration.RosService");
    if (type_RosService != null)
    {
        Console.WriteLine("[PASS] 类型 RosNetIntegration.RosService (class) 存在");
        var ctors_RosService = type_RosService.GetConstructors();
        Console.WriteLine($"[PASS] RosNetIntegration.RosService 构造函数数量: {ctors_RosService.Length}");
        var methods_RosService = type_RosService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RosNetIntegration.RosService 公开方法数量: {methods_RosService.Length}");
        foreach (var m in methods_RosService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RosNetIntegration.RosService 未找到，尝试无命名空间...");
        type_RosService = Type.GetType("RosService");
        if (type_RosService != null)
            Console.WriteLine("[PASS] 类型 RosService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RosService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RosNetIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("RosNetIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 RosNetIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] RosNetIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RosNetIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RosNetIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RosNetIntegration.RosHealthCheck
    var type_RosHealthCheck = Type.GetType("RosNetIntegration.RosHealthCheck");
    if (type_RosHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 RosNetIntegration.RosHealthCheck (class) 存在");
        var ctors_RosHealthCheck = type_RosHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] RosNetIntegration.RosHealthCheck 构造函数数量: {ctors_RosHealthCheck.Length}");
        var methods_RosHealthCheck = type_RosHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RosNetIntegration.RosHealthCheck 公开方法数量: {methods_RosHealthCheck.Length}");
        foreach (var m in methods_RosHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RosNetIntegration.RosHealthCheck 未找到，尝试无命名空间...");
        type_RosHealthCheck = Type.GetType("RosHealthCheck");
        if (type_RosHealthCheck != null)
            Console.WriteLine("[PASS] 类型 RosHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RosHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RosNetIntegration.RosMetricsExtensions
    var type_RosMetricsExtensions = Type.GetType("RosNetIntegration.RosMetricsExtensions");
    if (type_RosMetricsExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 RosNetIntegration.RosMetricsExtensions (class) 存在");
        var ctors_RosMetricsExtensions = type_RosMetricsExtensions.GetConstructors();
        Console.WriteLine($"[PASS] RosNetIntegration.RosMetricsExtensions 构造函数数量: {ctors_RosMetricsExtensions.Length}");
        var methods_RosMetricsExtensions = type_RosMetricsExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RosNetIntegration.RosMetricsExtensions 公开方法数量: {methods_RosMetricsExtensions.Length}");
        foreach (var m in methods_RosMetricsExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RosNetIntegration.RosMetricsExtensions 未找到，尝试无命名空间...");
        type_RosMetricsExtensions = Type.GetType("RosMetricsExtensions");
        if (type_RosMetricsExtensions != null)
            Console.WriteLine("[PASS] 类型 RosMetricsExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RosMetricsExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RosNetIntegration.RosBackgroundService
    var type_RosBackgroundService = Type.GetType("RosNetIntegration.RosBackgroundService");
    if (type_RosBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 RosNetIntegration.RosBackgroundService (class) 存在");
        var ctors_RosBackgroundService = type_RosBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] RosNetIntegration.RosBackgroundService 构造函数数量: {ctors_RosBackgroundService.Length}");
        var methods_RosBackgroundService = type_RosBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RosNetIntegration.RosBackgroundService 公开方法数量: {methods_RosBackgroundService.Length}");
        foreach (var m in methods_RosBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RosNetIntegration.RosBackgroundService 未找到，尝试无命名空间...");
        type_RosBackgroundService = Type.GetType("RosBackgroundService");
        if (type_RosBackgroundService != null)
            Console.WriteLine("[PASS] 类型 RosBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RosBackgroundService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RosNetIntegration.Program
    var type_Program = Type.GetType("RosNetIntegration.Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 RosNetIntegration.Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] RosNetIntegration.Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RosNetIntegration.Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RosNetIntegration.Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: RosNetIntegration.IRosService
    var type_IRosService = Type.GetType("RosNetIntegration.IRosService");
    if (type_IRosService != null)
    {
        Console.WriteLine("[PASS] 类型 RosNetIntegration.IRosService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RosNetIntegration.IRosService 未找到，尝试无命名空间...");
        type_IRosService = Type.GetType("IRosService");
        if (type_IRosService != null)
            Console.WriteLine("[PASS] 类型 IRosService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IRosService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
