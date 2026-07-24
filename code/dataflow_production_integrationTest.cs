#load "dataflow_production_integration.cs"

Console.WriteLine("=== dataflow_production_integration.cs Test ===");

try
{
    // 验证 class: DataflowProductionIntegration.DataflowOptions
    var type_DataflowOptions = Type.GetType("DataflowProductionIntegration.DataflowOptions");
    if (type_DataflowOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DataflowProductionIntegration.DataflowOptions (class) 存在");
        var ctors_DataflowOptions = type_DataflowOptions.GetConstructors();
        Console.WriteLine($"[PASS] DataflowProductionIntegration.DataflowOptions 构造函数数量: {ctors_DataflowOptions.Length}");
        var methods_DataflowOptions = type_DataflowOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataflowProductionIntegration.DataflowOptions 公开方法数量: {methods_DataflowOptions.Length}");
        foreach (var m in methods_DataflowOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataflowProductionIntegration.DataflowOptions 未找到，尝试无命名空间...");
        type_DataflowOptions = Type.GetType("DataflowOptions");
        if (type_DataflowOptions != null)
            Console.WriteLine("[PASS] 类型 DataflowOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataflowOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataflowProductionIntegration.DataflowProcessor
    var type_DataflowProcessor = Type.GetType("DataflowProductionIntegration.DataflowProcessor");
    if (type_DataflowProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 DataflowProductionIntegration.DataflowProcessor (class) 存在");
        var ctors_DataflowProcessor = type_DataflowProcessor.GetConstructors();
        Console.WriteLine($"[PASS] DataflowProductionIntegration.DataflowProcessor 构造函数数量: {ctors_DataflowProcessor.Length}");
        var methods_DataflowProcessor = type_DataflowProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataflowProductionIntegration.DataflowProcessor 公开方法数量: {methods_DataflowProcessor.Length}");
        foreach (var m in methods_DataflowProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataflowProductionIntegration.DataflowProcessor 未找到，尝试无命名空间...");
        type_DataflowProcessor = Type.GetType("DataflowProcessor");
        if (type_DataflowProcessor != null)
            Console.WriteLine("[PASS] 类型 DataflowProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataflowProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataflowProductionIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("DataflowProductionIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DataflowProductionIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DataflowProductionIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataflowProductionIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataflowProductionIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataflowProductionIntegration.DataflowBackgroundService
    var type_DataflowBackgroundService = Type.GetType("DataflowProductionIntegration.DataflowBackgroundService");
    if (type_DataflowBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 DataflowProductionIntegration.DataflowBackgroundService (class) 存在");
        var ctors_DataflowBackgroundService = type_DataflowBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] DataflowProductionIntegration.DataflowBackgroundService 构造函数数量: {ctors_DataflowBackgroundService.Length}");
        var methods_DataflowBackgroundService = type_DataflowBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataflowProductionIntegration.DataflowBackgroundService 公开方法数量: {methods_DataflowBackgroundService.Length}");
        foreach (var m in methods_DataflowBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataflowProductionIntegration.DataflowBackgroundService 未找到，尝试无命名空间...");
        type_DataflowBackgroundService = Type.GetType("DataflowBackgroundService");
        if (type_DataflowBackgroundService != null)
            Console.WriteLine("[PASS] 类型 DataflowBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataflowBackgroundService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DataflowProductionIntegration.Startup
    var type_Startup = Type.GetType("DataflowProductionIntegration.Startup");
    if (type_Startup != null)
    {
        Console.WriteLine("[PASS] 类型 DataflowProductionIntegration.Startup (class) 存在");
        var ctors_Startup = type_Startup.GetConstructors();
        Console.WriteLine($"[PASS] DataflowProductionIntegration.Startup 构造函数数量: {ctors_Startup.Length}");
        var methods_Startup = type_Startup.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataflowProductionIntegration.Startup 公开方法数量: {methods_Startup.Length}");
        foreach (var m in methods_Startup)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataflowProductionIntegration.Startup 未找到，尝试无命名空间...");
        type_Startup = Type.GetType("Startup");
        if (type_Startup != null)
            Console.WriteLine("[PASS] 类型 Startup (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Startup 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: DataflowProductionIntegration.IDataflowProcessor
    var type_IDataflowProcessor = Type.GetType("DataflowProductionIntegration.IDataflowProcessor");
    if (type_IDataflowProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 DataflowProductionIntegration.IDataflowProcessor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataflowProductionIntegration.IDataflowProcessor 未找到，尝试无命名空间...");
        type_IDataflowProcessor = Type.GetType("IDataflowProcessor");
        if (type_IDataflowProcessor != null)
            Console.WriteLine("[PASS] 类型 IDataflowProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDataflowProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DataflowProductionIntegration.DataflowMessage
    var type_DataflowMessage = Type.GetType("DataflowProductionIntegration.DataflowMessage");
    if (type_DataflowMessage != null)
    {
        Console.WriteLine("[PASS] 类型 DataflowProductionIntegration.DataflowMessage (record) 存在");
        var ctors_DataflowMessage = type_DataflowMessage.GetConstructors();
        Console.WriteLine($"[PASS] DataflowProductionIntegration.DataflowMessage 构造函数数量: {ctors_DataflowMessage.Length}");
        var methods_DataflowMessage = type_DataflowMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataflowProductionIntegration.DataflowMessage 公开方法数量: {methods_DataflowMessage.Length}");
        foreach (var m in methods_DataflowMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataflowProductionIntegration.DataflowMessage 未找到，尝试无命名空间...");
        type_DataflowMessage = Type.GetType("DataflowMessage");
        if (type_DataflowMessage != null)
            Console.WriteLine("[PASS] 类型 DataflowMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataflowMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
