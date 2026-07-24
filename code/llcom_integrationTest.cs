#load "llcom_integration.cs"

Console.WriteLine("=== llcom_integration.cs Test ===");

try
{
    // 验证 class: ZeroCopyCommunicationPipe
    var type_ZeroCopyCommunicationPipe = Type.GetType("ZeroCopyCommunicationPipe");
    if (type_ZeroCopyCommunicationPipe != null)
    {
        Console.WriteLine("[PASS] 类型 ZeroCopyCommunicationPipe (class) 存在");
        var ctors_ZeroCopyCommunicationPipe = type_ZeroCopyCommunicationPipe.GetConstructors();
        Console.WriteLine($"[PASS] ZeroCopyCommunicationPipe 构造函数数量: {ctors_ZeroCopyCommunicationPipe.Length}");
        var methods_ZeroCopyCommunicationPipe = type_ZeroCopyCommunicationPipe.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZeroCopyCommunicationPipe 公开方法数量: {methods_ZeroCopyCommunicationPipe.Length}");
        foreach (var m in methods_ZeroCopyCommunicationPipe)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZeroCopyCommunicationPipe 未找到，尝试无命名空间...");
        type_ZeroCopyCommunicationPipe = Type.GetType("ZeroCopyCommunicationPipe");
        if (type_ZeroCopyCommunicationPipe != null)
            Console.WriteLine("[PASS] 类型 ZeroCopyCommunicationPipe (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZeroCopyCommunicationPipe 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LoicProtocolHandler
    var type_LoicProtocolHandler = Type.GetType("LoicProtocolHandler");
    if (type_LoicProtocolHandler != null)
    {
        Console.WriteLine("[PASS] 类型 LoicProtocolHandler (class) 存在");
        var ctors_LoicProtocolHandler = type_LoicProtocolHandler.GetConstructors();
        Console.WriteLine($"[PASS] LoicProtocolHandler 构造函数数量: {ctors_LoicProtocolHandler.Length}");
        var methods_LoicProtocolHandler = type_LoicProtocolHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoicProtocolHandler 公开方法数量: {methods_LoicProtocolHandler.Length}");
        foreach (var m in methods_LoicProtocolHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoicProtocolHandler 未找到，尝试无命名空间...");
        type_LoicProtocolHandler = Type.GetType("LoicProtocolHandler");
        if (type_LoicProtocolHandler != null)
            Console.WriteLine("[PASS] 类型 LoicProtocolHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoicProtocolHandler 可能为顶层语句或嵌套类型");
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

    // 验证 class: MainViewModel
    var type_MainViewModel = Type.GetType("MainViewModel");
    if (type_MainViewModel != null)
    {
        Console.WriteLine("[PASS] 类型 MainViewModel (class) 存在");
        var ctors_MainViewModel = type_MainViewModel.GetConstructors();
        Console.WriteLine($"[PASS] MainViewModel 构造函数数量: {ctors_MainViewModel.Length}");
        var methods_MainViewModel = type_MainViewModel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MainViewModel 公开方法数量: {methods_MainViewModel.Length}");
        foreach (var m in methods_MainViewModel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MainViewModel 未找到，尝试无命名空间...");
        type_MainViewModel = Type.GetType("MainViewModel");
        if (type_MainViewModel != null)
            Console.WriteLine("[PASS] 类型 MainViewModel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MainViewModel 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
