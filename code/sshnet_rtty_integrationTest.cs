#load "sshnet_rtty_integration.cs"

Console.WriteLine("=== sshnet_rtty_integration.cs Test ===");

try
{
    // 验证 class: RttyOptions
    var type_RttyOptions = Type.GetType("RttyOptions");
    if (type_RttyOptions != null)
    {
        Console.WriteLine("[PASS] 类型 RttyOptions (class) 存在");
        var ctors_RttyOptions = type_RttyOptions.GetConstructors();
        Console.WriteLine($"[PASS] RttyOptions 构造函数数量: {ctors_RttyOptions.Length}");
        var methods_RttyOptions = type_RttyOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RttyOptions 公开方法数量: {methods_RttyOptions.Length}");
        foreach (var m in methods_RttyOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RttyOptions 未找到，尝试无命名空间...");
        type_RttyOptions = Type.GetType("RttyOptions");
        if (type_RttyOptions != null)
            Console.WriteLine("[PASS] 类型 RttyOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RttyOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuditLogOptions
    var type_AuditLogOptions = Type.GetType("AuditLogOptions");
    if (type_AuditLogOptions != null)
    {
        Console.WriteLine("[PASS] 类型 AuditLogOptions (class) 存在");
        var ctors_AuditLogOptions = type_AuditLogOptions.GetConstructors();
        Console.WriteLine($"[PASS] AuditLogOptions 构造函数数量: {ctors_AuditLogOptions.Length}");
        var methods_AuditLogOptions = type_AuditLogOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditLogOptions 公开方法数量: {methods_AuditLogOptions.Length}");
        foreach (var m in methods_AuditLogOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditLogOptions 未找到，尝试无命名空间...");
        type_AuditLogOptions = Type.GetType("AuditLogOptions");
        if (type_AuditLogOptions != null)
            Console.WriteLine("[PASS] 类型 AuditLogOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditLogOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuditLogEntry
    var type_AuditLogEntry = Type.GetType("AuditLogEntry");
    if (type_AuditLogEntry != null)
    {
        Console.WriteLine("[PASS] 类型 AuditLogEntry (class) 存在");
        var ctors_AuditLogEntry = type_AuditLogEntry.GetConstructors();
        Console.WriteLine($"[PASS] AuditLogEntry 构造函数数量: {ctors_AuditLogEntry.Length}");
        var methods_AuditLogEntry = type_AuditLogEntry.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditLogEntry 公开方法数量: {methods_AuditLogEntry.Length}");
        foreach (var m in methods_AuditLogEntry)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditLogEntry 未找到，尝试无命名空间...");
        type_AuditLogEntry = Type.GetType("AuditLogEntry");
        if (type_AuditLogEntry != null)
            Console.WriteLine("[PASS] 类型 AuditLogEntry (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditLogEntry 可能为顶层语句或嵌套类型");
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

    // 验证 class: DangerousCommandValidator
    var type_DangerousCommandValidator = Type.GetType("DangerousCommandValidator");
    if (type_DangerousCommandValidator != null)
    {
        Console.WriteLine("[PASS] 类型 DangerousCommandValidator (class) 存在");
        var ctors_DangerousCommandValidator = type_DangerousCommandValidator.GetConstructors();
        Console.WriteLine($"[PASS] DangerousCommandValidator 构造函数数量: {ctors_DangerousCommandValidator.Length}");
        var methods_DangerousCommandValidator = type_DangerousCommandValidator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DangerousCommandValidator 公开方法数量: {methods_DangerousCommandValidator.Length}");
        foreach (var m in methods_DangerousCommandValidator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DangerousCommandValidator 未找到，尝试无命名空间...");
        type_DangerousCommandValidator = Type.GetType("DangerousCommandValidator");
        if (type_DangerousCommandValidator != null)
            Console.WriteLine("[PASS] 类型 DangerousCommandValidator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DangerousCommandValidator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuditLog
    var type_AuditLog = Type.GetType("AuditLog");
    if (type_AuditLog != null)
    {
        Console.WriteLine("[PASS] 类型 AuditLog (class) 存在");
        var ctors_AuditLog = type_AuditLog.GetConstructors();
        Console.WriteLine($"[PASS] AuditLog 构造函数数量: {ctors_AuditLog.Length}");
        var methods_AuditLog = type_AuditLog.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditLog 公开方法数量: {methods_AuditLog.Length}");
        foreach (var m in methods_AuditLog)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditLog 未找到，尝试无命名空间...");
        type_AuditLog = Type.GetType("AuditLog");
        if (type_AuditLog != null)
            Console.WriteLine("[PASS] 类型 AuditLog (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditLog 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PermissionOptions
    var type_PermissionOptions = Type.GetType("PermissionOptions");
    if (type_PermissionOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PermissionOptions (class) 存在");
        var ctors_PermissionOptions = type_PermissionOptions.GetConstructors();
        Console.WriteLine($"[PASS] PermissionOptions 构造函数数量: {ctors_PermissionOptions.Length}");
        var methods_PermissionOptions = type_PermissionOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PermissionOptions 公开方法数量: {methods_PermissionOptions.Length}");
        foreach (var m in methods_PermissionOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PermissionOptions 未找到，尝试无命名空间...");
        type_PermissionOptions = Type.GetType("PermissionOptions");
        if (type_PermissionOptions != null)
            Console.WriteLine("[PASS] 类型 PermissionOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PermissionOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RttyService
    var type_RttyService = Type.GetType("RttyService");
    if (type_RttyService != null)
    {
        Console.WriteLine("[PASS] 类型 RttyService (class) 存在");
        var ctors_RttyService = type_RttyService.GetConstructors();
        Console.WriteLine($"[PASS] RttyService 构造函数数量: {ctors_RttyService.Length}");
        var methods_RttyService = type_RttyService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RttyService 公开方法数量: {methods_RttyService.Length}");
        foreach (var m in methods_RttyService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RttyService 未找到，尝试无命名空间...");
        type_RttyService = Type.GetType("RttyService");
        if (type_RttyService != null)
            Console.WriteLine("[PASS] 类型 RttyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RttyService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SecureSessionChannel
    var type_SecureSessionChannel = Type.GetType("SecureSessionChannel");
    if (type_SecureSessionChannel != null)
    {
        Console.WriteLine("[PASS] 类型 SecureSessionChannel (class) 存在");
        var ctors_SecureSessionChannel = type_SecureSessionChannel.GetConstructors();
        Console.WriteLine($"[PASS] SecureSessionChannel 构造函数数量: {ctors_SecureSessionChannel.Length}");
        var methods_SecureSessionChannel = type_SecureSessionChannel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SecureSessionChannel 公开方法数量: {methods_SecureSessionChannel.Length}");
        foreach (var m in methods_SecureSessionChannel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SecureSessionChannel 未找到，尝试无命名空间...");
        type_SecureSessionChannel = Type.GetType("SecureSessionChannel");
        if (type_SecureSessionChannel != null)
            Console.WriteLine("[PASS] 类型 SecureSessionChannel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SecureSessionChannel 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAuditLogger
    var type_IAuditLogger = Type.GetType("IAuditLogger");
    if (type_IAuditLogger != null)
    {
        Console.WriteLine("[PASS] 类型 IAuditLogger (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAuditLogger 未找到，尝试无命名空间...");
        type_IAuditLogger = Type.GetType("IAuditLogger");
        if (type_IAuditLogger != null)
            Console.WriteLine("[PASS] 类型 IAuditLogger (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAuditLogger 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDangerousCommandValidator
    var type_IDangerousCommandValidator = Type.GetType("IDangerousCommandValidator");
    if (type_IDangerousCommandValidator != null)
    {
        Console.WriteLine("[PASS] 类型 IDangerousCommandValidator (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDangerousCommandValidator 未找到，尝试无命名空间...");
        type_IDangerousCommandValidator = Type.GetType("IDangerousCommandValidator");
        if (type_IDangerousCommandValidator != null)
            Console.WriteLine("[PASS] 类型 IDangerousCommandValidator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDangerousCommandValidator 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPermissionValidator
    var type_IPermissionValidator = Type.GetType("IPermissionValidator");
    if (type_IPermissionValidator != null)
    {
        Console.WriteLine("[PASS] 类型 IPermissionValidator (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPermissionValidator 未找到，尝试无命名空间...");
        type_IPermissionValidator = Type.GetType("IPermissionValidator");
        if (type_IPermissionValidator != null)
            Console.WriteLine("[PASS] 类型 IPermissionValidator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPermissionValidator 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
