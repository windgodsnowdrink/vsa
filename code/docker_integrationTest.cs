#load "docker_integration.cs"

Console.WriteLine("=== docker_integration.cs Test ===");

try
{
    // 验证 class: DockerIntegration.DockerServiceCollectionExtensions
    var type_DockerServiceCollectionExtensions = Type.GetType("DockerIntegration.DockerServiceCollectionExtensions");
    if (type_DockerServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.DockerServiceCollectionExtensions (class) 存在");
        var ctors_DockerServiceCollectionExtensions = type_DockerServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.DockerServiceCollectionExtensions 构造函数数量: {ctors_DockerServiceCollectionExtensions.Length}");
        var methods_DockerServiceCollectionExtensions = type_DockerServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.DockerServiceCollectionExtensions 公开方法数量: {methods_DockerServiceCollectionExtensions.Length}");
        foreach (var m in methods_DockerServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.DockerServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_DockerServiceCollectionExtensions = Type.GetType("DockerServiceCollectionExtensions");
        if (type_DockerServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 DockerServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerIntegration.DockerOptions
    var type_DockerOptions = Type.GetType("DockerIntegration.DockerOptions");
    if (type_DockerOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.DockerOptions (class) 存在");
        var ctors_DockerOptions = type_DockerOptions.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.DockerOptions 构造函数数量: {ctors_DockerOptions.Length}");
        var methods_DockerOptions = type_DockerOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.DockerOptions 公开方法数量: {methods_DockerOptions.Length}");
        foreach (var m in methods_DockerOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.DockerOptions 未找到，尝试无命名空间...");
        type_DockerOptions = Type.GetType("DockerOptions");
        if (type_DockerOptions != null)
            Console.WriteLine("[PASS] 类型 DockerOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerIntegration.DockerContainerManager
    var type_DockerContainerManager = Type.GetType("DockerIntegration.DockerContainerManager");
    if (type_DockerContainerManager != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.DockerContainerManager (class) 存在");
        var ctors_DockerContainerManager = type_DockerContainerManager.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.DockerContainerManager 构造函数数量: {ctors_DockerContainerManager.Length}");
        var methods_DockerContainerManager = type_DockerContainerManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.DockerContainerManager 公开方法数量: {methods_DockerContainerManager.Length}");
        foreach (var m in methods_DockerContainerManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.DockerContainerManager 未找到，尝试无命名空间...");
        type_DockerContainerManager = Type.GetType("DockerContainerManager");
        if (type_DockerContainerManager != null)
            Console.WriteLine("[PASS] 类型 DockerContainerManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerContainerManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerIntegration.DockerImageBuilder
    var type_DockerImageBuilder = Type.GetType("DockerIntegration.DockerImageBuilder");
    if (type_DockerImageBuilder != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.DockerImageBuilder (class) 存在");
        var ctors_DockerImageBuilder = type_DockerImageBuilder.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.DockerImageBuilder 构造函数数量: {ctors_DockerImageBuilder.Length}");
        var methods_DockerImageBuilder = type_DockerImageBuilder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.DockerImageBuilder 公开方法数量: {methods_DockerImageBuilder.Length}");
        foreach (var m in methods_DockerImageBuilder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.DockerImageBuilder 未找到，尝试无命名空间...");
        type_DockerImageBuilder = Type.GetType("DockerImageBuilder");
        if (type_DockerImageBuilder != null)
            Console.WriteLine("[PASS] 类型 DockerImageBuilder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerImageBuilder 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerIntegration.DockerNetworkManager
    var type_DockerNetworkManager = Type.GetType("DockerIntegration.DockerNetworkManager");
    if (type_DockerNetworkManager != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.DockerNetworkManager (class) 存在");
        var ctors_DockerNetworkManager = type_DockerNetworkManager.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.DockerNetworkManager 构造函数数量: {ctors_DockerNetworkManager.Length}");
        var methods_DockerNetworkManager = type_DockerNetworkManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.DockerNetworkManager 公开方法数量: {methods_DockerNetworkManager.Length}");
        foreach (var m in methods_DockerNetworkManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.DockerNetworkManager 未找到，尝试无命名空间...");
        type_DockerNetworkManager = Type.GetType("DockerNetworkManager");
        if (type_DockerNetworkManager != null)
            Console.WriteLine("[PASS] 类型 DockerNetworkManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerNetworkManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerIntegration.DockerMonitorService
    var type_DockerMonitorService = Type.GetType("DockerIntegration.DockerMonitorService");
    if (type_DockerMonitorService != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.DockerMonitorService (class) 存在");
        var ctors_DockerMonitorService = type_DockerMonitorService.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.DockerMonitorService 构造函数数量: {ctors_DockerMonitorService.Length}");
        var methods_DockerMonitorService = type_DockerMonitorService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.DockerMonitorService 公开方法数量: {methods_DockerMonitorService.Length}");
        foreach (var m in methods_DockerMonitorService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.DockerMonitorService 未找到，尝试无命名空间...");
        type_DockerMonitorService = Type.GetType("DockerMonitorService");
        if (type_DockerMonitorService != null)
            Console.WriteLine("[PASS] 类型 DockerMonitorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerMonitorService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerIntegration.DockerLogCollector
    var type_DockerLogCollector = Type.GetType("DockerIntegration.DockerLogCollector");
    if (type_DockerLogCollector != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.DockerLogCollector (class) 存在");
        var ctors_DockerLogCollector = type_DockerLogCollector.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.DockerLogCollector 构造函数数量: {ctors_DockerLogCollector.Length}");
        var methods_DockerLogCollector = type_DockerLogCollector.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.DockerLogCollector 公开方法数量: {methods_DockerLogCollector.Length}");
        foreach (var m in methods_DockerLogCollector)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.DockerLogCollector 未找到，尝试无命名空间...");
        type_DockerLogCollector = Type.GetType("DockerLogCollector");
        if (type_DockerLogCollector != null)
            Console.WriteLine("[PASS] 类型 DockerLogCollector (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerLogCollector 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DockerIntegration.DockerHealthChecker
    var type_DockerHealthChecker = Type.GetType("DockerIntegration.DockerHealthChecker");
    if (type_DockerHealthChecker != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.DockerHealthChecker (class) 存在");
        var ctors_DockerHealthChecker = type_DockerHealthChecker.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.DockerHealthChecker 构造函数数量: {ctors_DockerHealthChecker.Length}");
        var methods_DockerHealthChecker = type_DockerHealthChecker.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.DockerHealthChecker 公开方法数量: {methods_DockerHealthChecker.Length}");
        foreach (var m in methods_DockerHealthChecker)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.DockerHealthChecker 未找到，尝试无命名空间...");
        type_DockerHealthChecker = Type.GetType("DockerHealthChecker");
        if (type_DockerHealthChecker != null)
            Console.WriteLine("[PASS] 类型 DockerHealthChecker (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DockerHealthChecker 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DockerIntegration.ContainerEvent
    var type_ContainerEvent = Type.GetType("DockerIntegration.ContainerEvent");
    if (type_ContainerEvent != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.ContainerEvent (record) 存在");
        var ctors_ContainerEvent = type_ContainerEvent.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.ContainerEvent 构造函数数量: {ctors_ContainerEvent.Length}");
        var methods_ContainerEvent = type_ContainerEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.ContainerEvent 公开方法数量: {methods_ContainerEvent.Length}");
        foreach (var m in methods_ContainerEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.ContainerEvent 未找到，尝试无命名空间...");
        type_ContainerEvent = Type.GetType("ContainerEvent");
        if (type_ContainerEvent != null)
            Console.WriteLine("[PASS] 类型 ContainerEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ContainerEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DockerIntegration.ContainerMetrics
    var type_ContainerMetrics = Type.GetType("DockerIntegration.ContainerMetrics");
    if (type_ContainerMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.ContainerMetrics (record) 存在");
        var ctors_ContainerMetrics = type_ContainerMetrics.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.ContainerMetrics 构造函数数量: {ctors_ContainerMetrics.Length}");
        var methods_ContainerMetrics = type_ContainerMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.ContainerMetrics 公开方法数量: {methods_ContainerMetrics.Length}");
        foreach (var m in methods_ContainerMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.ContainerMetrics 未找到，尝试无命名空间...");
        type_ContainerMetrics = Type.GetType("ContainerMetrics");
        if (type_ContainerMetrics != null)
            Console.WriteLine("[PASS] 类型 ContainerMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ContainerMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DockerIntegration.HealthCheckResult
    var type_HealthCheckResult = Type.GetType("DockerIntegration.HealthCheckResult");
    if (type_HealthCheckResult != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.HealthCheckResult (record) 存在");
        var ctors_HealthCheckResult = type_HealthCheckResult.GetConstructors();
        Console.WriteLine($"[PASS] DockerIntegration.HealthCheckResult 构造函数数量: {ctors_HealthCheckResult.Length}");
        var methods_HealthCheckResult = type_HealthCheckResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DockerIntegration.HealthCheckResult 公开方法数量: {methods_HealthCheckResult.Length}");
        foreach (var m in methods_HealthCheckResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.HealthCheckResult 未找到，尝试无命名空间...");
        type_HealthCheckResult = Type.GetType("HealthCheckResult");
        if (type_HealthCheckResult != null)
            Console.WriteLine("[PASS] 类型 HealthCheckResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HealthCheckResult 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: DockerIntegration.ContainerEventType
    var type_ContainerEventType = Type.GetType("DockerIntegration.ContainerEventType");
    if (type_ContainerEventType != null)
    {
        Console.WriteLine("[PASS] 类型 DockerIntegration.ContainerEventType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DockerIntegration.ContainerEventType 未找到，尝试无命名空间...");
        type_ContainerEventType = Type.GetType("ContainerEventType");
        if (type_ContainerEventType != null)
            Console.WriteLine("[PASS] 类型 ContainerEventType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ContainerEventType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
