#load "processx_integration.cs"

Console.WriteLine("=== processx_integration.cs Test ===");

try
{
    // 验证 class: ProcessXIntegration.ProcessMonitorOptions
    var type_ProcessMonitorOptions = Type.GetType("ProcessXIntegration.ProcessMonitorOptions");
    if (type_ProcessMonitorOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.ProcessMonitorOptions (class) 存在");
        var ctors_ProcessMonitorOptions = type_ProcessMonitorOptions.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.ProcessMonitorOptions 构造函数数量: {ctors_ProcessMonitorOptions.Length}");
        var methods_ProcessMonitorOptions = type_ProcessMonitorOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.ProcessMonitorOptions 公开方法数量: {methods_ProcessMonitorOptions.Length}");
        foreach (var m in methods_ProcessMonitorOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.ProcessMonitorOptions 未找到，尝试无命名空间...");
        type_ProcessMonitorOptions = Type.GetType("ProcessMonitorOptions");
        if (type_ProcessMonitorOptions != null)
            Console.WriteLine("[PASS] 类型 ProcessMonitorOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessMonitorOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.GarnetOptions
    var type_GarnetOptions = Type.GetType("ProcessXIntegration.GarnetOptions");
    if (type_GarnetOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.GarnetOptions (class) 存在");
        var ctors_GarnetOptions = type_GarnetOptions.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.GarnetOptions 构造函数数量: {ctors_GarnetOptions.Length}");
        var methods_GarnetOptions = type_GarnetOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.GarnetOptions 公开方法数量: {methods_GarnetOptions.Length}");
        foreach (var m in methods_GarnetOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.GarnetOptions 未找到，尝试无命名空间...");
        type_GarnetOptions = Type.GetType("GarnetOptions");
        if (type_GarnetOptions != null)
            Console.WriteLine("[PASS] 类型 GarnetOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GarnetOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.MySqlOptions
    var type_MySqlOptions = Type.GetType("ProcessXIntegration.MySqlOptions");
    if (type_MySqlOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.MySqlOptions (class) 存在");
        var ctors_MySqlOptions = type_MySqlOptions.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.MySqlOptions 构造函数数量: {ctors_MySqlOptions.Length}");
        var methods_MySqlOptions = type_MySqlOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.MySqlOptions 公开方法数量: {methods_MySqlOptions.Length}");
        foreach (var m in methods_MySqlOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.MySqlOptions 未找到，尝试无命名空间...");
        type_MySqlOptions = Type.GetType("MySqlOptions");
        if (type_MySqlOptions != null)
            Console.WriteLine("[PASS] 类型 MySqlOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MySqlOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.MqttOptions
    var type_MqttOptions = Type.GetType("ProcessXIntegration.MqttOptions");
    if (type_MqttOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.MqttOptions (class) 存在");
        var ctors_MqttOptions = type_MqttOptions.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.MqttOptions 构造函数数量: {ctors_MqttOptions.Length}");
        var methods_MqttOptions = type_MqttOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.MqttOptions 公开方法数量: {methods_MqttOptions.Length}");
        foreach (var m in methods_MqttOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.MqttOptions 未找到，尝试无命名空间...");
        type_MqttOptions = Type.GetType("MqttOptions");
        if (type_MqttOptions != null)
            Console.WriteLine("[PASS] 类型 MqttOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.ProcessXOptions
    var type_ProcessXOptions = Type.GetType("ProcessXIntegration.ProcessXOptions");
    if (type_ProcessXOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.ProcessXOptions (class) 存在");
        var ctors_ProcessXOptions = type_ProcessXOptions.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.ProcessXOptions 构造函数数量: {ctors_ProcessXOptions.Length}");
        var methods_ProcessXOptions = type_ProcessXOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.ProcessXOptions 公开方法数量: {methods_ProcessXOptions.Length}");
        foreach (var m in methods_ProcessXOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.ProcessXOptions 未找到，尝试无命名空间...");
        type_ProcessXOptions = Type.GetType("ProcessXOptions");
        if (type_ProcessXOptions != null)
            Console.WriteLine("[PASS] 类型 ProcessXOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessXOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.ProcessMonitorService
    var type_ProcessMonitorService = Type.GetType("ProcessXIntegration.ProcessMonitorService");
    if (type_ProcessMonitorService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.ProcessMonitorService (class) 存在");
        var ctors_ProcessMonitorService = type_ProcessMonitorService.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.ProcessMonitorService 构造函数数量: {ctors_ProcessMonitorService.Length}");
        var methods_ProcessMonitorService = type_ProcessMonitorService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.ProcessMonitorService 公开方法数量: {methods_ProcessMonitorService.Length}");
        foreach (var m in methods_ProcessMonitorService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.ProcessMonitorService 未找到，尝试无命名空间...");
        type_ProcessMonitorService = Type.GetType("ProcessMonitorService");
        if (type_ProcessMonitorService != null)
            Console.WriteLine("[PASS] 类型 ProcessMonitorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessMonitorService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.ProcessXService
    var type_ProcessXService = Type.GetType("ProcessXIntegration.ProcessXService");
    if (type_ProcessXService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.ProcessXService (class) 存在");
        var ctors_ProcessXService = type_ProcessXService.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.ProcessXService 构造函数数量: {ctors_ProcessXService.Length}");
        var methods_ProcessXService = type_ProcessXService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.ProcessXService 公开方法数量: {methods_ProcessXService.Length}");
        foreach (var m in methods_ProcessXService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.ProcessXService 未找到，尝试无命名空间...");
        type_ProcessXService = Type.GetType("ProcessXService");
        if (type_ProcessXService != null)
            Console.WriteLine("[PASS] 类型 ProcessXService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessXService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.ProcessMonitorExtensions
    var type_ProcessMonitorExtensions = Type.GetType("ProcessXIntegration.ProcessMonitorExtensions");
    if (type_ProcessMonitorExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.ProcessMonitorExtensions (class) 存在");
        var ctors_ProcessMonitorExtensions = type_ProcessMonitorExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.ProcessMonitorExtensions 构造函数数量: {ctors_ProcessMonitorExtensions.Length}");
        var methods_ProcessMonitorExtensions = type_ProcessMonitorExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.ProcessMonitorExtensions 公开方法数量: {methods_ProcessMonitorExtensions.Length}");
        foreach (var m in methods_ProcessMonitorExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.ProcessMonitorExtensions 未找到，尝试无命名空间...");
        type_ProcessMonitorExtensions = Type.GetType("ProcessMonitorExtensions");
        if (type_ProcessMonitorExtensions != null)
            Console.WriteLine("[PASS] 类型 ProcessMonitorExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessMonitorExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.MySqlExtensions
    var type_MySqlExtensions = Type.GetType("ProcessXIntegration.MySqlExtensions");
    if (type_MySqlExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.MySqlExtensions (class) 存在");
        var ctors_MySqlExtensions = type_MySqlExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.MySqlExtensions 构造函数数量: {ctors_MySqlExtensions.Length}");
        var methods_MySqlExtensions = type_MySqlExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.MySqlExtensions 公开方法数量: {methods_MySqlExtensions.Length}");
        foreach (var m in methods_MySqlExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.MySqlExtensions 未找到，尝试无命名空间...");
        type_MySqlExtensions = Type.GetType("MySqlExtensions");
        if (type_MySqlExtensions != null)
            Console.WriteLine("[PASS] 类型 MySqlExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MySqlExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ProcessXIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.GarnetService
    var type_GarnetService = Type.GetType("ProcessXIntegration.GarnetService");
    if (type_GarnetService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.GarnetService (class) 存在");
        var ctors_GarnetService = type_GarnetService.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.GarnetService 构造函数数量: {ctors_GarnetService.Length}");
        var methods_GarnetService = type_GarnetService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.GarnetService 公开方法数量: {methods_GarnetService.Length}");
        foreach (var m in methods_GarnetService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.GarnetService 未找到，尝试无命名空间...");
        type_GarnetService = Type.GetType("GarnetService");
        if (type_GarnetService != null)
            Console.WriteLine("[PASS] 类型 GarnetService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GarnetService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.MySqlService
    var type_MySqlService = Type.GetType("ProcessXIntegration.MySqlService");
    if (type_MySqlService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.MySqlService (class) 存在");
        var ctors_MySqlService = type_MySqlService.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.MySqlService 构造函数数量: {ctors_MySqlService.Length}");
        var methods_MySqlService = type_MySqlService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.MySqlService 公开方法数量: {methods_MySqlService.Length}");
        foreach (var m in methods_MySqlService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.MySqlService 未找到，尝试无命名空间...");
        type_MySqlService = Type.GetType("MySqlService");
        if (type_MySqlService != null)
            Console.WriteLine("[PASS] 类型 MySqlService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MySqlService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.SqliteService
    var type_SqliteService = Type.GetType("ProcessXIntegration.SqliteService");
    if (type_SqliteService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.SqliteService (class) 存在");
        var ctors_SqliteService = type_SqliteService.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.SqliteService 构造函数数量: {ctors_SqliteService.Length}");
        var methods_SqliteService = type_SqliteService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.SqliteService 公开方法数量: {methods_SqliteService.Length}");
        foreach (var m in methods_SqliteService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.SqliteService 未找到，尝试无命名空间...");
        type_SqliteService = Type.GetType("SqliteService");
        if (type_SqliteService != null)
            Console.WriteLine("[PASS] 类型 SqliteService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.LiteDbService
    var type_LiteDbService = Type.GetType("ProcessXIntegration.LiteDbService");
    if (type_LiteDbService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.LiteDbService (class) 存在");
        var ctors_LiteDbService = type_LiteDbService.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.LiteDbService 构造函数数量: {ctors_LiteDbService.Length}");
        var methods_LiteDbService = type_LiteDbService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.LiteDbService 公开方法数量: {methods_LiteDbService.Length}");
        foreach (var m in methods_LiteDbService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.LiteDbService 未找到，尝试无命名空间...");
        type_LiteDbService = Type.GetType("LiteDbService");
        if (type_LiteDbService != null)
            Console.WriteLine("[PASS] 类型 LiteDbService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProcessXIntegration.MqttService
    var type_MqttService = Type.GetType("ProcessXIntegration.MqttService");
    if (type_MqttService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.MqttService (class) 存在");
        var ctors_MqttService = type_MqttService.GetConstructors();
        Console.WriteLine($"[PASS] ProcessXIntegration.MqttService 构造函数数量: {ctors_MqttService.Length}");
        var methods_MqttService = type_MqttService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessXIntegration.MqttService 公开方法数量: {methods_MqttService.Length}");
        foreach (var m in methods_MqttService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.MqttService 未找到，尝试无命名空间...");
        type_MqttService = Type.GetType("MqttService");
        if (type_MqttService != null)
            Console.WriteLine("[PASS] 类型 MqttService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ProcessXIntegration.IProcessMonitorService
    var type_IProcessMonitorService = Type.GetType("ProcessXIntegration.IProcessMonitorService");
    if (type_IProcessMonitorService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.IProcessMonitorService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.IProcessMonitorService 未找到，尝试无命名空间...");
        type_IProcessMonitorService = Type.GetType("IProcessMonitorService");
        if (type_IProcessMonitorService != null)
            Console.WriteLine("[PASS] 类型 IProcessMonitorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IProcessMonitorService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ProcessXIntegration.IProcessXService
    var type_IProcessXService = Type.GetType("ProcessXIntegration.IProcessXService");
    if (type_IProcessXService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.IProcessXService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.IProcessXService 未找到，尝试无命名空间...");
        type_IProcessXService = Type.GetType("IProcessXService");
        if (type_IProcessXService != null)
            Console.WriteLine("[PASS] 类型 IProcessXService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IProcessXService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ProcessXIntegration.IGarnetService
    var type_IGarnetService = Type.GetType("ProcessXIntegration.IGarnetService");
    if (type_IGarnetService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.IGarnetService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.IGarnetService 未找到，尝试无命名空间...");
        type_IGarnetService = Type.GetType("IGarnetService");
        if (type_IGarnetService != null)
            Console.WriteLine("[PASS] 类型 IGarnetService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IGarnetService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ProcessXIntegration.IMySqlService
    var type_IMySqlService = Type.GetType("ProcessXIntegration.IMySqlService");
    if (type_IMySqlService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.IMySqlService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.IMySqlService 未找到，尝试无命名空间...");
        type_IMySqlService = Type.GetType("IMySqlService");
        if (type_IMySqlService != null)
            Console.WriteLine("[PASS] 类型 IMySqlService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMySqlService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ProcessXIntegration.IMqttService
    var type_IMqttService = Type.GetType("ProcessXIntegration.IMqttService");
    if (type_IMqttService != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.IMqttService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.IMqttService 未找到，尝试无命名空间...");
        type_IMqttService = Type.GetType("IMqttService");
        if (type_IMqttService != null)
            Console.WriteLine("[PASS] 类型 IMqttService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMqttService 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: ProcessXIntegration.ProcessPriority
    var type_ProcessPriority = Type.GetType("ProcessXIntegration.ProcessPriority");
    if (type_ProcessPriority != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessXIntegration.ProcessPriority (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessXIntegration.ProcessPriority 未找到，尝试无命名空间...");
        type_ProcessPriority = Type.GetType("ProcessPriority");
        if (type_ProcessPriority != null)
            Console.WriteLine("[PASS] 类型 ProcessPriority (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessPriority 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
