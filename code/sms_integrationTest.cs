#load "sms_integration.cs"

Console.WriteLine("=== sms_integration.cs Test ===");

try
{
    // 验证 class: SmsOptions
    var type_SmsOptions = Type.GetType("SmsOptions");
    if (type_SmsOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SmsOptions (class) 存在");
        var ctors_SmsOptions = type_SmsOptions.GetConstructors();
        Console.WriteLine($"[PASS] SmsOptions 构造函数数量: {ctors_SmsOptions.Length}");
        var methods_SmsOptions = type_SmsOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmsOptions 公开方法数量: {methods_SmsOptions.Length}");
        foreach (var m in methods_SmsOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmsOptions 未找到，尝试无命名空间...");
        type_SmsOptions = Type.GetType("SmsOptions");
        if (type_SmsOptions != null)
            Console.WriteLine("[PASS] 类型 SmsOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmsOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SmsSender
    var type_SmsSender = Type.GetType("SmsSender");
    if (type_SmsSender != null)
    {
        Console.WriteLine("[PASS] 类型 SmsSender (class) 存在");
        var ctors_SmsSender = type_SmsSender.GetConstructors();
        Console.WriteLine($"[PASS] SmsSender 构造函数数量: {ctors_SmsSender.Length}");
        var methods_SmsSender = type_SmsSender.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmsSender 公开方法数量: {methods_SmsSender.Length}");
        foreach (var m in methods_SmsSender)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmsSender 未找到，尝试无命名空间...");
        type_SmsSender = Type.GetType("SmsSender");
        if (type_SmsSender != null)
            Console.WriteLine("[PASS] 类型 SmsSender (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmsSender 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SmsExtensions
    var type_SmsExtensions = Type.GetType("SmsExtensions");
    if (type_SmsExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SmsExtensions (class) 存在");
        var ctors_SmsExtensions = type_SmsExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SmsExtensions 构造函数数量: {ctors_SmsExtensions.Length}");
        var methods_SmsExtensions = type_SmsExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmsExtensions 公开方法数量: {methods_SmsExtensions.Length}");
        foreach (var m in methods_SmsExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmsExtensions 未找到，尝试无命名空间...");
        type_SmsExtensions = Type.GetType("SmsExtensions");
        if (type_SmsExtensions != null)
            Console.WriteLine("[PASS] 类型 SmsExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmsExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ISmsSender
    var type_ISmsSender = Type.GetType("ISmsSender");
    if (type_ISmsSender != null)
    {
        Console.WriteLine("[PASS] 类型 ISmsSender (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ISmsSender 未找到，尝试无命名空间...");
        type_ISmsSender = Type.GetType("ISmsSender");
        if (type_ISmsSender != null)
            Console.WriteLine("[PASS] 类型 ISmsSender (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISmsSender 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
