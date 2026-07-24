#load "loic_integration.cs"

Console.WriteLine("=== loic_integration.cs Test ===");

try
{
    // 验证 class: SerialConfig
    var type_SerialConfig = Type.GetType("SerialConfig");
    if (type_SerialConfig != null)
    {
        Console.WriteLine("[PASS] 类型 SerialConfig (class) 存在");
        var ctors_SerialConfig = type_SerialConfig.GetConstructors();
        Console.WriteLine($"[PASS] SerialConfig 构造函数数量: {ctors_SerialConfig.Length}");
        var methods_SerialConfig = type_SerialConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SerialConfig 公开方法数量: {methods_SerialConfig.Length}");
        foreach (var m in methods_SerialConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SerialConfig 未找到，尝试无命名空间...");
        type_SerialConfig = Type.GetType("SerialConfig");
        if (type_SerialConfig != null)
            Console.WriteLine("[PASS] 类型 SerialConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SerialConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SerialChannel
    var type_SerialChannel = Type.GetType("SerialChannel");
    if (type_SerialChannel != null)
    {
        Console.WriteLine("[PASS] 类型 SerialChannel (class) 存在");
        var ctors_SerialChannel = type_SerialChannel.GetConstructors();
        Console.WriteLine($"[PASS] SerialChannel 构造函数数量: {ctors_SerialChannel.Length}");
        var methods_SerialChannel = type_SerialChannel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SerialChannel 公开方法数量: {methods_SerialChannel.Length}");
        foreach (var m in methods_SerialChannel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SerialChannel 未找到，尝试无命名空间...");
        type_SerialChannel = Type.GetType("SerialChannel");
        if (type_SerialChannel != null)
            Console.WriteLine("[PASS] 类型 SerialChannel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SerialChannel 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LoicProtocol
    var type_LoicProtocol = Type.GetType("LoicProtocol");
    if (type_LoicProtocol != null)
    {
        Console.WriteLine("[PASS] 类型 LoicProtocol (class) 存在");
        var ctors_LoicProtocol = type_LoicProtocol.GetConstructors();
        Console.WriteLine($"[PASS] LoicProtocol 构造函数数量: {ctors_LoicProtocol.Length}");
        var methods_LoicProtocol = type_LoicProtocol.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoicProtocol 公开方法数量: {methods_LoicProtocol.Length}");
        foreach (var m in methods_LoicProtocol)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoicProtocol 未找到，尝试无命名空间...");
        type_LoicProtocol = Type.GetType("LoicProtocol");
        if (type_LoicProtocol != null)
            Console.WriteLine("[PASS] 类型 LoicProtocol (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoicProtocol 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LoicExtensions
    var type_LoicExtensions = Type.GetType("LoicExtensions");
    if (type_LoicExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 LoicExtensions (class) 存在");
        var ctors_LoicExtensions = type_LoicExtensions.GetConstructors();
        Console.WriteLine($"[PASS] LoicExtensions 构造函数数量: {ctors_LoicExtensions.Length}");
        var methods_LoicExtensions = type_LoicExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoicExtensions 公开方法数量: {methods_LoicExtensions.Length}");
        foreach (var m in methods_LoicExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoicExtensions 未找到，尝试无命名空间...");
        type_LoicExtensions = Type.GetType("LoicExtensions");
        if (type_LoicExtensions != null)
            Console.WriteLine("[PASS] 类型 LoicExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoicExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LoicBackgroundService
    var type_LoicBackgroundService = Type.GetType("LoicBackgroundService");
    if (type_LoicBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 LoicBackgroundService (class) 存在");
        var ctors_LoicBackgroundService = type_LoicBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] LoicBackgroundService 构造函数数量: {ctors_LoicBackgroundService.Length}");
        var methods_LoicBackgroundService = type_LoicBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoicBackgroundService 公开方法数量: {methods_LoicBackgroundService.Length}");
        foreach (var m in methods_LoicBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoicBackgroundService 未找到，尝试无命名空间...");
        type_LoicBackgroundService = Type.GetType("LoicBackgroundService");
        if (type_LoicBackgroundService != null)
            Console.WriteLine("[PASS] 类型 LoicBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoicBackgroundService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MainWindow
    var type_MainWindow = Type.GetType("MainWindow");
    if (type_MainWindow != null)
    {
        Console.WriteLine("[PASS] 类型 MainWindow (class) 存在");
        var ctors_MainWindow = type_MainWindow.GetConstructors();
        Console.WriteLine($"[PASS] MainWindow 构造函数数量: {ctors_MainWindow.Length}");
        var methods_MainWindow = type_MainWindow.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MainWindow 公开方法数量: {methods_MainWindow.Length}");
        foreach (var m in methods_MainWindow)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MainWindow 未找到，尝试无命名空间...");
        type_MainWindow = Type.GetType("MainWindow");
        if (type_MainWindow != null)
            Console.WriteLine("[PASS] 类型 MainWindow (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MainWindow 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
