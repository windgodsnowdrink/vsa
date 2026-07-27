#load "jpush_sms_integration.cs"

Console.WriteLine("=== jpush_sms_integration.cs Test ===");

try
{
    // 验证 class: RateLimiter
    var type_RateLimiter = Type.GetType("RateLimiter");
    if (type_RateLimiter != null)
    {
        Console.WriteLine("[PASS] 类型 RateLimiter (class) 存在");
        var ctors_RateLimiter = type_RateLimiter.GetConstructors();
        Console.WriteLine($"[PASS] RateLimiter 构造函数数量: {ctors_RateLimiter.Length}");
        var methods_RateLimiter = type_RateLimiter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RateLimiter 公开方法数量: {methods_RateLimiter.Length}");
        foreach (var m in methods_RateLimiter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RateLimiter 未找到，尝试无命名空间...");
        type_RateLimiter = Type.GetType("RateLimiter");
        if (type_RateLimiter != null)
            Console.WriteLine("[PASS] 类型 RateLimiter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RateLimiter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JpushSmsOptions
    var type_JpushSmsOptions = Type.GetType("JpushSmsOptions");
    if (type_JpushSmsOptions != null)
    {
        Console.WriteLine("[PASS] 类型 JpushSmsOptions (class) 存在");
        var ctors_JpushSmsOptions = type_JpushSmsOptions.GetConstructors();
        Console.WriteLine($"[PASS] JpushSmsOptions 构造函数数量: {ctors_JpushSmsOptions.Length}");
        var methods_JpushSmsOptions = type_JpushSmsOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JpushSmsOptions 公开方法数量: {methods_JpushSmsOptions.Length}");
        foreach (var m in methods_JpushSmsOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JpushSmsOptions 未找到，尝试无命名空间...");
        type_JpushSmsOptions = Type.GetType("JpushSmsOptions");
        if (type_JpushSmsOptions != null)
            Console.WriteLine("[PASS] 类型 JpushSmsOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JpushSmsOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JpushSmsService
    var type_JpushSmsService = Type.GetType("JpushSmsService");
    if (type_JpushSmsService != null)
    {
        Console.WriteLine("[PASS] 类型 JpushSmsService (class) 存在");
        var ctors_JpushSmsService = type_JpushSmsService.GetConstructors();
        Console.WriteLine($"[PASS] JpushSmsService 构造函数数量: {ctors_JpushSmsService.Length}");
        var methods_JpushSmsService = type_JpushSmsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JpushSmsService 公开方法数量: {methods_JpushSmsService.Length}");
        foreach (var m in methods_JpushSmsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JpushSmsService 未找到，尝试无命名空间...");
        type_JpushSmsService = Type.GetType("JpushSmsService");
        if (type_JpushSmsService != null)
            Console.WriteLine("[PASS] 类型 JpushSmsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JpushSmsService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IJpushSmsService
    var type_IJpushSmsService = Type.GetType("IJpushSmsService");
    if (type_IJpushSmsService != null)
    {
        Console.WriteLine("[PASS] 类型 IJpushSmsService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IJpushSmsService 未找到，尝试无命名空间...");
        type_IJpushSmsService = Type.GetType("IJpushSmsService");
        if (type_IJpushSmsService != null)
            Console.WriteLine("[PASS] 类型 IJpushSmsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IJpushSmsService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: SmsMessage
    var type_SmsMessage = Type.GetType("SmsMessage");
    if (type_SmsMessage != null)
    {
        Console.WriteLine("[PASS] 类型 SmsMessage (record) 存在");
        var ctors_SmsMessage = type_SmsMessage.GetConstructors();
        Console.WriteLine($"[PASS] SmsMessage 构造函数数量: {ctors_SmsMessage.Length}");
        var methods_SmsMessage = type_SmsMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmsMessage 公开方法数量: {methods_SmsMessage.Length}");
        foreach (var m in methods_SmsMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmsMessage 未找到，尝试无命名空间...");
        type_SmsMessage = Type.GetType("SmsMessage");
        if (type_SmsMessage != null)
            Console.WriteLine("[PASS] 类型 SmsMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmsMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
