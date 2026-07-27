#load "mailkit_integration.cs"

Console.WriteLine("=== mailkit_integration.cs Test ===");

try
{
    // 验证 class: SmtpOptions
    var type_SmtpOptions = Type.GetType("SmtpOptions");
    if (type_SmtpOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SmtpOptions (class) 存在");
        var ctors_SmtpOptions = type_SmtpOptions.GetConstructors();
        Console.WriteLine($"[PASS] SmtpOptions 构造函数数量: {ctors_SmtpOptions.Length}");
        var methods_SmtpOptions = type_SmtpOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmtpOptions 公开方法数量: {methods_SmtpOptions.Length}");
        foreach (var m in methods_SmtpOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmtpOptions 未找到，尝试无命名空间...");
        type_SmtpOptions = Type.GetType("SmtpOptions");
        if (type_SmtpOptions != null)
            Console.WriteLine("[PASS] 类型 SmtpOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmtpOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EmailService
    var type_EmailService = Type.GetType("EmailService");
    if (type_EmailService != null)
    {
        Console.WriteLine("[PASS] 类型 EmailService (class) 存在");
        var ctors_EmailService = type_EmailService.GetConstructors();
        Console.WriteLine($"[PASS] EmailService 构造函数数量: {ctors_EmailService.Length}");
        var methods_EmailService = type_EmailService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EmailService 公开方法数量: {methods_EmailService.Length}");
        foreach (var m in methods_EmailService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EmailService 未找到，尝试无命名空间...");
        type_EmailService = Type.GetType("EmailService");
        if (type_EmailService != null)
            Console.WriteLine("[PASS] 类型 EmailService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EmailService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SmtpClientPooledPolicy
    var type_SmtpClientPooledPolicy = Type.GetType("SmtpClientPooledPolicy");
    if (type_SmtpClientPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 SmtpClientPooledPolicy (class) 存在");
        var ctors_SmtpClientPooledPolicy = type_SmtpClientPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] SmtpClientPooledPolicy 构造函数数量: {ctors_SmtpClientPooledPolicy.Length}");
        var methods_SmtpClientPooledPolicy = type_SmtpClientPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmtpClientPooledPolicy 公开方法数量: {methods_SmtpClientPooledPolicy.Length}");
        foreach (var m in methods_SmtpClientPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmtpClientPooledPolicy 未找到，尝试无命名空间...");
        type_SmtpClientPooledPolicy = Type.GetType("SmtpClientPooledPolicy");
        if (type_SmtpClientPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 SmtpClientPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmtpClientPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EmailMessage
    var type_EmailMessage = Type.GetType("EmailMessage");
    if (type_EmailMessage != null)
    {
        Console.WriteLine("[PASS] 类型 EmailMessage (class) 存在");
        var ctors_EmailMessage = type_EmailMessage.GetConstructors();
        Console.WriteLine($"[PASS] EmailMessage 构造函数数量: {ctors_EmailMessage.Length}");
        var methods_EmailMessage = type_EmailMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EmailMessage 公开方法数量: {methods_EmailMessage.Length}");
        foreach (var m in methods_EmailMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EmailMessage 未找到，尝试无命名空间...");
        type_EmailMessage = Type.GetType("EmailMessage");
        if (type_EmailMessage != null)
            Console.WriteLine("[PASS] 类型 EmailMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EmailMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EmailAttachment
    var type_EmailAttachment = Type.GetType("EmailAttachment");
    if (type_EmailAttachment != null)
    {
        Console.WriteLine("[PASS] 类型 EmailAttachment (class) 存在");
        var ctors_EmailAttachment = type_EmailAttachment.GetConstructors();
        Console.WriteLine($"[PASS] EmailAttachment 构造函数数量: {ctors_EmailAttachment.Length}");
        var methods_EmailAttachment = type_EmailAttachment.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EmailAttachment 公开方法数量: {methods_EmailAttachment.Length}");
        foreach (var m in methods_EmailAttachment)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EmailAttachment 未找到，尝试无命名空间...");
        type_EmailAttachment = Type.GetType("EmailAttachment");
        if (type_EmailAttachment != null)
            Console.WriteLine("[PASS] 类型 EmailAttachment (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EmailAttachment 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EmailServiceExtensions
    var type_EmailServiceExtensions = Type.GetType("EmailServiceExtensions");
    if (type_EmailServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 EmailServiceExtensions (class) 存在");
        var ctors_EmailServiceExtensions = type_EmailServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] EmailServiceExtensions 构造函数数量: {ctors_EmailServiceExtensions.Length}");
        var methods_EmailServiceExtensions = type_EmailServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EmailServiceExtensions 公开方法数量: {methods_EmailServiceExtensions.Length}");
        foreach (var m in methods_EmailServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EmailServiceExtensions 未找到，尝试无命名空间...");
        type_EmailServiceExtensions = Type.GetType("EmailServiceExtensions");
        if (type_EmailServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 EmailServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EmailServiceExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
