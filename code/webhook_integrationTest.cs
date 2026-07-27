#load "webhook_integration.cs"

Console.WriteLine("=== webhook_integration.cs Test ===");

try
{
    // 验证 class: WebhookOptions
    var type_WebhookOptions = Type.GetType("WebhookOptions");
    if (type_WebhookOptions != null)
    {
        Console.WriteLine("[PASS] 类型 WebhookOptions (class) 存在");
        var ctors_WebhookOptions = type_WebhookOptions.GetConstructors();
        Console.WriteLine($"[PASS] WebhookOptions 构造函数数量: {ctors_WebhookOptions.Length}");
        var methods_WebhookOptions = type_WebhookOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WebhookOptions 公开方法数量: {methods_WebhookOptions.Length}");
        foreach (var m in methods_WebhookOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WebhookOptions 未找到，尝试无命名空间...");
        type_WebhookOptions = Type.GetType("WebhookOptions");
        if (type_WebhookOptions != null)
            Console.WriteLine("[PASS] 类型 WebhookOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WebhookOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WebhookNotification
    var type_WebhookNotification = Type.GetType("WebhookNotification");
    if (type_WebhookNotification != null)
    {
        Console.WriteLine("[PASS] 类型 WebhookNotification (class) 存在");
        var ctors_WebhookNotification = type_WebhookNotification.GetConstructors();
        Console.WriteLine($"[PASS] WebhookNotification 构造函数数量: {ctors_WebhookNotification.Length}");
        var methods_WebhookNotification = type_WebhookNotification.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WebhookNotification 公开方法数量: {methods_WebhookNotification.Length}");
        foreach (var m in methods_WebhookNotification)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WebhookNotification 未找到，尝试无命名空间...");
        type_WebhookNotification = Type.GetType("WebhookNotification");
        if (type_WebhookNotification != null)
            Console.WriteLine("[PASS] 类型 WebhookNotification (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WebhookNotification 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WebhookMetrics
    var type_WebhookMetrics = Type.GetType("WebhookMetrics");
    if (type_WebhookMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 WebhookMetrics (class) 存在");
        var ctors_WebhookMetrics = type_WebhookMetrics.GetConstructors();
        Console.WriteLine($"[PASS] WebhookMetrics 构造函数数量: {ctors_WebhookMetrics.Length}");
        var methods_WebhookMetrics = type_WebhookMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WebhookMetrics 公开方法数量: {methods_WebhookMetrics.Length}");
        foreach (var m in methods_WebhookMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WebhookMetrics 未找到，尝试无命名空间...");
        type_WebhookMetrics = Type.GetType("WebhookMetrics");
        if (type_WebhookMetrics != null)
            Console.WriteLine("[PASS] 类型 WebhookMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WebhookMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WebhookDispatcher
    var type_WebhookDispatcher = Type.GetType("WebhookDispatcher");
    if (type_WebhookDispatcher != null)
    {
        Console.WriteLine("[PASS] 类型 WebhookDispatcher (class) 存在");
        var ctors_WebhookDispatcher = type_WebhookDispatcher.GetConstructors();
        Console.WriteLine($"[PASS] WebhookDispatcher 构造函数数量: {ctors_WebhookDispatcher.Length}");
        var methods_WebhookDispatcher = type_WebhookDispatcher.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WebhookDispatcher 公开方法数量: {methods_WebhookDispatcher.Length}");
        foreach (var m in methods_WebhookDispatcher)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WebhookDispatcher 未找到，尝试无命名空间...");
        type_WebhookDispatcher = Type.GetType("WebhookDispatcher");
        if (type_WebhookDispatcher != null)
            Console.WriteLine("[PASS] 类型 WebhookDispatcher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WebhookDispatcher 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WebhookExtensions
    var type_WebhookExtensions = Type.GetType("WebhookExtensions");
    if (type_WebhookExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 WebhookExtensions (class) 存在");
        var ctors_WebhookExtensions = type_WebhookExtensions.GetConstructors();
        Console.WriteLine($"[PASS] WebhookExtensions 构造函数数量: {ctors_WebhookExtensions.Length}");
        var methods_WebhookExtensions = type_WebhookExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WebhookExtensions 公开方法数量: {methods_WebhookExtensions.Length}");
        foreach (var m in methods_WebhookExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WebhookExtensions 未找到，尝试无命名空间...");
        type_WebhookExtensions = Type.GetType("WebhookExtensions");
        if (type_WebhookExtensions != null)
            Console.WriteLine("[PASS] 类型 WebhookExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WebhookExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WebhookBackgroundService
    var type_WebhookBackgroundService = Type.GetType("WebhookBackgroundService");
    if (type_WebhookBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 WebhookBackgroundService (class) 存在");
        var ctors_WebhookBackgroundService = type_WebhookBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] WebhookBackgroundService 构造函数数量: {ctors_WebhookBackgroundService.Length}");
        var methods_WebhookBackgroundService = type_WebhookBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WebhookBackgroundService 公开方法数量: {methods_WebhookBackgroundService.Length}");
        foreach (var m in methods_WebhookBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WebhookBackgroundService 未找到，尝试无命名空间...");
        type_WebhookBackgroundService = Type.GetType("WebhookBackgroundService");
        if (type_WebhookBackgroundService != null)
            Console.WriteLine("[PASS] 类型 WebhookBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WebhookBackgroundService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IWebhookDispatcher
    var type_IWebhookDispatcher = Type.GetType("IWebhookDispatcher");
    if (type_IWebhookDispatcher != null)
    {
        Console.WriteLine("[PASS] 类型 IWebhookDispatcher (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IWebhookDispatcher 未找到，尝试无命名空间...");
        type_IWebhookDispatcher = Type.GetType("IWebhookDispatcher");
        if (type_IWebhookDispatcher != null)
            Console.WriteLine("[PASS] 类型 IWebhookDispatcher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IWebhookDispatcher 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
