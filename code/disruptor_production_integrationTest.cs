#load "disruptor_production_integration.cs"

Console.WriteLine("=== disruptor_production_integration.cs Test ===");

try
{
    // 验证 class: DisruptorOptions
    var type_DisruptorOptions = Type.GetType("DisruptorOptions");
    if (type_DisruptorOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DisruptorOptions (class) 存在");
        var ctors_DisruptorOptions = type_DisruptorOptions.GetConstructors();
        Console.WriteLine($"[PASS] DisruptorOptions 构造函数数量: {ctors_DisruptorOptions.Length}");
        var methods_DisruptorOptions = type_DisruptorOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DisruptorOptions 公开方法数量: {methods_DisruptorOptions.Length}");
        foreach (var m in methods_DisruptorOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DisruptorOptions 未找到，尝试无命名空间...");
        type_DisruptorOptions = Type.GetType("DisruptorOptions");
        if (type_DisruptorOptions != null)
            Console.WriteLine("[PASS] 类型 DisruptorOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DisruptorOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DisruptorEventHandler
    var type_DisruptorEventHandler = Type.GetType("DisruptorEventHandler");
    if (type_DisruptorEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 DisruptorEventHandler (class) 存在");
        var ctors_DisruptorEventHandler = type_DisruptorEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] DisruptorEventHandler 构造函数数量: {ctors_DisruptorEventHandler.Length}");
        var methods_DisruptorEventHandler = type_DisruptorEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DisruptorEventHandler 公开方法数量: {methods_DisruptorEventHandler.Length}");
        foreach (var m in methods_DisruptorEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DisruptorEventHandler 未找到，尝试无命名空间...");
        type_DisruptorEventHandler = Type.GetType("DisruptorEventHandler");
        if (type_DisruptorEventHandler != null)
            Console.WriteLine("[PASS] 类型 DisruptorEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DisruptorEventHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DisruptorProducer
    var type_DisruptorProducer = Type.GetType("DisruptorProducer");
    if (type_DisruptorProducer != null)
    {
        Console.WriteLine("[PASS] 类型 DisruptorProducer (class) 存在");
        var ctors_DisruptorProducer = type_DisruptorProducer.GetConstructors();
        Console.WriteLine($"[PASS] DisruptorProducer 构造函数数量: {ctors_DisruptorProducer.Length}");
        var methods_DisruptorProducer = type_DisruptorProducer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DisruptorProducer 公开方法数量: {methods_DisruptorProducer.Length}");
        foreach (var m in methods_DisruptorProducer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DisruptorProducer 未找到，尝试无命名空间...");
        type_DisruptorProducer = Type.GetType("DisruptorProducer");
        if (type_DisruptorProducer != null)
            Console.WriteLine("[PASS] 类型 DisruptorProducer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DisruptorProducer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DisruptorBackgroundService
    var type_DisruptorBackgroundService = Type.GetType("DisruptorBackgroundService");
    if (type_DisruptorBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 DisruptorBackgroundService (class) 存在");
        var ctors_DisruptorBackgroundService = type_DisruptorBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] DisruptorBackgroundService 构造函数数量: {ctors_DisruptorBackgroundService.Length}");
        var methods_DisruptorBackgroundService = type_DisruptorBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DisruptorBackgroundService 公开方法数量: {methods_DisruptorBackgroundService.Length}");
        foreach (var m in methods_DisruptorBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DisruptorBackgroundService 未找到，尝试无命名空间...");
        type_DisruptorBackgroundService = Type.GetType("DisruptorBackgroundService");
        if (type_DisruptorBackgroundService != null)
            Console.WriteLine("[PASS] 类型 DisruptorBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DisruptorBackgroundService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AdvancedScenarios
    var type_AdvancedScenarios = Type.GetType("AdvancedScenarios");
    if (type_AdvancedScenarios != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedScenarios (class) 存在");
        var ctors_AdvancedScenarios = type_AdvancedScenarios.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedScenarios 构造函数数量: {ctors_AdvancedScenarios.Length}");
        var methods_AdvancedScenarios = type_AdvancedScenarios.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedScenarios 公开方法数量: {methods_AdvancedScenarios.Length}");
        foreach (var m in methods_AdvancedScenarios)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedScenarios 未找到，尝试无命名空间...");
        type_AdvancedScenarios = Type.GetType("AdvancedScenarios");
        if (type_AdvancedScenarios != null)
            Console.WriteLine("[PASS] 类型 AdvancedScenarios (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedScenarios 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BatchEventHandler
    var type_BatchEventHandler = Type.GetType("BatchEventHandler");
    if (type_BatchEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 BatchEventHandler (class) 存在");
        var ctors_BatchEventHandler = type_BatchEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] BatchEventHandler 构造函数数量: {ctors_BatchEventHandler.Length}");
        var methods_BatchEventHandler = type_BatchEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchEventHandler 公开方法数量: {methods_BatchEventHandler.Length}");
        foreach (var m in methods_BatchEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchEventHandler 未找到，尝试无命名空间...");
        type_BatchEventHandler = Type.GetType("BatchEventHandler");
        if (type_BatchEventHandler != null)
            Console.WriteLine("[PASS] 类型 BatchEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchEventHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FirstStageHandler
    var type_FirstStageHandler = Type.GetType("FirstStageHandler");
    if (type_FirstStageHandler != null)
    {
        Console.WriteLine("[PASS] 类型 FirstStageHandler (class) 存在");
        var ctors_FirstStageHandler = type_FirstStageHandler.GetConstructors();
        Console.WriteLine($"[PASS] FirstStageHandler 构造函数数量: {ctors_FirstStageHandler.Length}");
        var methods_FirstStageHandler = type_FirstStageHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FirstStageHandler 公开方法数量: {methods_FirstStageHandler.Length}");
        foreach (var m in methods_FirstStageHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FirstStageHandler 未找到，尝试无命名空间...");
        type_FirstStageHandler = Type.GetType("FirstStageHandler");
        if (type_FirstStageHandler != null)
            Console.WriteLine("[PASS] 类型 FirstStageHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FirstStageHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SecondStageHandler
    var type_SecondStageHandler = Type.GetType("SecondStageHandler");
    if (type_SecondStageHandler != null)
    {
        Console.WriteLine("[PASS] 类型 SecondStageHandler (class) 存在");
        var ctors_SecondStageHandler = type_SecondStageHandler.GetConstructors();
        Console.WriteLine($"[PASS] SecondStageHandler 构造函数数量: {ctors_SecondStageHandler.Length}");
        var methods_SecondStageHandler = type_SecondStageHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SecondStageHandler 公开方法数量: {methods_SecondStageHandler.Length}");
        foreach (var m in methods_SecondStageHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SecondStageHandler 未找到，尝试无命名空间...");
        type_SecondStageHandler = Type.GetType("SecondStageHandler");
        if (type_SecondStageHandler != null)
            Console.WriteLine("[PASS] 类型 SecondStageHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SecondStageHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FinalStageHandler
    var type_FinalStageHandler = Type.GetType("FinalStageHandler");
    if (type_FinalStageHandler != null)
    {
        Console.WriteLine("[PASS] 类型 FinalStageHandler (class) 存在");
        var ctors_FinalStageHandler = type_FinalStageHandler.GetConstructors();
        Console.WriteLine($"[PASS] FinalStageHandler 构造函数数量: {ctors_FinalStageHandler.Length}");
        var methods_FinalStageHandler = type_FinalStageHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FinalStageHandler 公开方法数量: {methods_FinalStageHandler.Length}");
        foreach (var m in methods_FinalStageHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FinalStageHandler 未找到，尝试无命名空间...");
        type_FinalStageHandler = Type.GetType("FinalStageHandler");
        if (type_FinalStageHandler != null)
            Console.WriteLine("[PASS] 类型 FinalStageHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FinalStageHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDisruptorProducer
    var type_IDisruptorProducer = Type.GetType("IDisruptorProducer");
    if (type_IDisruptorProducer != null)
    {
        Console.WriteLine("[PASS] 类型 IDisruptorProducer (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDisruptorProducer 未找到，尝试无命名空间...");
        type_IDisruptorProducer = Type.GetType("IDisruptorProducer");
        if (type_IDisruptorProducer != null)
            Console.WriteLine("[PASS] 类型 IDisruptorProducer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDisruptorProducer 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DisruptorMessage
    var type_DisruptorMessage = Type.GetType("DisruptorMessage");
    if (type_DisruptorMessage != null)
    {
        Console.WriteLine("[PASS] 类型 DisruptorMessage (record) 存在");
        var ctors_DisruptorMessage = type_DisruptorMessage.GetConstructors();
        Console.WriteLine($"[PASS] DisruptorMessage 构造函数数量: {ctors_DisruptorMessage.Length}");
        var methods_DisruptorMessage = type_DisruptorMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DisruptorMessage 公开方法数量: {methods_DisruptorMessage.Length}");
        foreach (var m in methods_DisruptorMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DisruptorMessage 未找到，尝试无命名空间...");
        type_DisruptorMessage = Type.GetType("DisruptorMessage");
        if (type_DisruptorMessage != null)
            Console.WriteLine("[PASS] 类型 DisruptorMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DisruptorMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
