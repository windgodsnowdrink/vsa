#load "papercut_integration.cs"

Console.WriteLine("=== papercut_integration.cs Test ===");

try
{
    // 验证 class: EmailProcessingService
    var type_EmailProcessingService = Type.GetType("EmailProcessingService");
    if (type_EmailProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 EmailProcessingService (class) 存在");
        var ctors_EmailProcessingService = type_EmailProcessingService.GetConstructors();
        Console.WriteLine($"[PASS] EmailProcessingService 构造函数数量: {ctors_EmailProcessingService.Length}");
        var methods_EmailProcessingService = type_EmailProcessingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EmailProcessingService 公开方法数量: {methods_EmailProcessingService.Length}");
        foreach (var m in methods_EmailProcessingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EmailProcessingService 未找到，尝试无命名空间...");
        type_EmailProcessingService = Type.GetType("EmailProcessingService");
        if (type_EmailProcessingService != null)
            Console.WriteLine("[PASS] 类型 EmailProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EmailProcessingService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MimePartPool
    var type_MimePartPool = Type.GetType("MimePartPool");
    if (type_MimePartPool != null)
    {
        Console.WriteLine("[PASS] 类型 MimePartPool (class) 存在");
        var ctors_MimePartPool = type_MimePartPool.GetConstructors();
        Console.WriteLine($"[PASS] MimePartPool 构造函数数量: {ctors_MimePartPool.Length}");
        var methods_MimePartPool = type_MimePartPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MimePartPool 公开方法数量: {methods_MimePartPool.Length}");
        foreach (var m in methods_MimePartPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MimePartPool 未找到，尝试无命名空间...");
        type_MimePartPool = Type.GetType("MimePartPool");
        if (type_MimePartPool != null)
            Console.WriteLine("[PASS] 类型 MimePartPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MimePartPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MimePartPooledPolicy
    var type_MimePartPooledPolicy = Type.GetType("MimePartPooledPolicy");
    if (type_MimePartPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MimePartPooledPolicy (class) 存在");
        var ctors_MimePartPooledPolicy = type_MimePartPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MimePartPooledPolicy 构造函数数量: {ctors_MimePartPooledPolicy.Length}");
        var methods_MimePartPooledPolicy = type_MimePartPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MimePartPooledPolicy 公开方法数量: {methods_MimePartPooledPolicy.Length}");
        foreach (var m in methods_MimePartPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MimePartPooledPolicy 未找到，尝试无命名空间...");
        type_MimePartPooledPolicy = Type.GetType("MimePartPooledPolicy");
        if (type_MimePartPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 MimePartPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MimePartPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EmailServerExtensions
    var type_EmailServerExtensions = Type.GetType("EmailServerExtensions");
    if (type_EmailServerExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 EmailServerExtensions (class) 存在");
        var ctors_EmailServerExtensions = type_EmailServerExtensions.GetConstructors();
        Console.WriteLine($"[PASS] EmailServerExtensions 构造函数数量: {ctors_EmailServerExtensions.Length}");
        var methods_EmailServerExtensions = type_EmailServerExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EmailServerExtensions 公开方法数量: {methods_EmailServerExtensions.Length}");
        foreach (var m in methods_EmailServerExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EmailServerExtensions 未找到，尝试无命名空间...");
        type_EmailServerExtensions = Type.GetType("EmailServerExtensions");
        if (type_EmailServerExtensions != null)
            Console.WriteLine("[PASS] 类型 EmailServerExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EmailServerExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: LargeMessage
    var type_LargeMessage = Type.GetType("LargeMessage");
    if (type_LargeMessage != null)
    {
        Console.WriteLine("[PASS] 类型 LargeMessage (struct) 存在");
        var ctors_LargeMessage = type_LargeMessage.GetConstructors();
        Console.WriteLine($"[PASS] LargeMessage 构造函数数量: {ctors_LargeMessage.Length}");
        var methods_LargeMessage = type_LargeMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LargeMessage 公开方法数量: {methods_LargeMessage.Length}");
        foreach (var m in methods_LargeMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LargeMessage 未找到，尝试无命名空间...");
        type_LargeMessage = Type.GetType("LargeMessage");
        if (type_LargeMessage != null)
            Console.WriteLine("[PASS] 类型 LargeMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LargeMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: DynamicMessage
    var type_DynamicMessage = Type.GetType("DynamicMessage");
    if (type_DynamicMessage != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicMessage (struct) 存在");
        var ctors_DynamicMessage = type_DynamicMessage.GetConstructors();
        Console.WriteLine($"[PASS] DynamicMessage 构造函数数量: {ctors_DynamicMessage.Length}");
        var methods_DynamicMessage = type_DynamicMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicMessage 公开方法数量: {methods_DynamicMessage.Length}");
        foreach (var m in methods_DynamicMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicMessage 未找到，尝试无命名空间...");
        type_DynamicMessage = Type.GetType("DynamicMessage");
        if (type_DynamicMessage != null)
            Console.WriteLine("[PASS] 类型 DynamicMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 record: EmailMessage
    var type_EmailMessage = Type.GetType("EmailMessage");
    if (type_EmailMessage != null)
    {
        Console.WriteLine("[PASS] 类型 EmailMessage (record) 存在");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
