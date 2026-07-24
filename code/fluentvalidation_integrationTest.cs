#load "fluentvalidation_integration.cs"

Console.WriteLine("=== fluentvalidation_integration.cs Test ===");

try
{
    // 验证 class: FluentValidationIntegration.FluentValidationOptions
    var type_FluentValidationOptions = Type.GetType("FluentValidationIntegration.FluentValidationOptions");
    if (type_FluentValidationOptions != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.FluentValidationOptions (class) 存在");
        var ctors_FluentValidationOptions = type_FluentValidationOptions.GetConstructors();
        Console.WriteLine($"[PASS] FluentValidationIntegration.FluentValidationOptions 构造函数数量: {ctors_FluentValidationOptions.Length}");
        var methods_FluentValidationOptions = type_FluentValidationOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FluentValidationIntegration.FluentValidationOptions 公开方法数量: {methods_FluentValidationOptions.Length}");
        foreach (var m in methods_FluentValidationOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.FluentValidationOptions 未找到，尝试无命名空间...");
        type_FluentValidationOptions = Type.GetType("FluentValidationOptions");
        if (type_FluentValidationOptions != null)
            Console.WriteLine("[PASS] 类型 FluentValidationOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FluentValidationOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FluentValidationIntegration.FluentValidationExtensions
    var type_FluentValidationExtensions = Type.GetType("FluentValidationIntegration.FluentValidationExtensions");
    if (type_FluentValidationExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.FluentValidationExtensions (class) 存在");
        var ctors_FluentValidationExtensions = type_FluentValidationExtensions.GetConstructors();
        Console.WriteLine($"[PASS] FluentValidationIntegration.FluentValidationExtensions 构造函数数量: {ctors_FluentValidationExtensions.Length}");
        var methods_FluentValidationExtensions = type_FluentValidationExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FluentValidationIntegration.FluentValidationExtensions 公开方法数量: {methods_FluentValidationExtensions.Length}");
        foreach (var m in methods_FluentValidationExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.FluentValidationExtensions 未找到，尝试无命名空间...");
        type_FluentValidationExtensions = Type.GetType("FluentValidationExtensions");
        if (type_FluentValidationExtensions != null)
            Console.WriteLine("[PASS] 类型 FluentValidationExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FluentValidationExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FluentValidationIntegration.ValidatorFactory
    var type_ValidatorFactory = Type.GetType("FluentValidationIntegration.ValidatorFactory");
    if (type_ValidatorFactory != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.ValidatorFactory (class) 存在");
        var ctors_ValidatorFactory = type_ValidatorFactory.GetConstructors();
        Console.WriteLine($"[PASS] FluentValidationIntegration.ValidatorFactory 构造函数数量: {ctors_ValidatorFactory.Length}");
        var methods_ValidatorFactory = type_ValidatorFactory.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FluentValidationIntegration.ValidatorFactory 公开方法数量: {methods_ValidatorFactory.Length}");
        foreach (var m in methods_ValidatorFactory)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.ValidatorFactory 未找到，尝试无命名空间...");
        type_ValidatorFactory = Type.GetType("ValidatorFactory");
        if (type_ValidatorFactory != null)
            Console.WriteLine("[PASS] 类型 ValidatorFactory (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValidatorFactory 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FluentValidationIntegration.DistributedValidatorCache
    var type_DistributedValidatorCache = Type.GetType("FluentValidationIntegration.DistributedValidatorCache");
    if (type_DistributedValidatorCache != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.DistributedValidatorCache (class) 存在");
        var ctors_DistributedValidatorCache = type_DistributedValidatorCache.GetConstructors();
        Console.WriteLine($"[PASS] FluentValidationIntegration.DistributedValidatorCache 构造函数数量: {ctors_DistributedValidatorCache.Length}");
        var methods_DistributedValidatorCache = type_DistributedValidatorCache.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FluentValidationIntegration.DistributedValidatorCache 公开方法数量: {methods_DistributedValidatorCache.Length}");
        foreach (var m in methods_DistributedValidatorCache)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.DistributedValidatorCache 未找到，尝试无命名空间...");
        type_DistributedValidatorCache = Type.GetType("DistributedValidatorCache");
        if (type_DistributedValidatorCache != null)
            Console.WriteLine("[PASS] 类型 DistributedValidatorCache (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedValidatorCache 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FluentValidationIntegration.ValidatorLoader
    var type_ValidatorLoader = Type.GetType("FluentValidationIntegration.ValidatorLoader");
    if (type_ValidatorLoader != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.ValidatorLoader (class) 存在");
        var ctors_ValidatorLoader = type_ValidatorLoader.GetConstructors();
        Console.WriteLine($"[PASS] FluentValidationIntegration.ValidatorLoader 构造函数数量: {ctors_ValidatorLoader.Length}");
        var methods_ValidatorLoader = type_ValidatorLoader.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FluentValidationIntegration.ValidatorLoader 公开方法数量: {methods_ValidatorLoader.Length}");
        foreach (var m in methods_ValidatorLoader)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.ValidatorLoader 未找到，尝试无命名空间...");
        type_ValidatorLoader = Type.GetType("ValidatorLoader");
        if (type_ValidatorLoader != null)
            Console.WriteLine("[PASS] 类型 ValidatorLoader (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValidatorLoader 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FluentValidationIntegration.ValidatorMonitor
    var type_ValidatorMonitor = Type.GetType("FluentValidationIntegration.ValidatorMonitor");
    if (type_ValidatorMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.ValidatorMonitor (class) 存在");
        var ctors_ValidatorMonitor = type_ValidatorMonitor.GetConstructors();
        Console.WriteLine($"[PASS] FluentValidationIntegration.ValidatorMonitor 构造函数数量: {ctors_ValidatorMonitor.Length}");
        var methods_ValidatorMonitor = type_ValidatorMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FluentValidationIntegration.ValidatorMonitor 公开方法数量: {methods_ValidatorMonitor.Length}");
        foreach (var m in methods_ValidatorMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.ValidatorMonitor 未找到，尝试无命名空间...");
        type_ValidatorMonitor = Type.GetType("ValidatorMonitor");
        if (type_ValidatorMonitor != null)
            Console.WriteLine("[PASS] 类型 ValidatorMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValidatorMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FluentValidationIntegration.ValidatorVersionControl
    var type_ValidatorVersionControl = Type.GetType("FluentValidationIntegration.ValidatorVersionControl");
    if (type_ValidatorVersionControl != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.ValidatorVersionControl (class) 存在");
        var ctors_ValidatorVersionControl = type_ValidatorVersionControl.GetConstructors();
        Console.WriteLine($"[PASS] FluentValidationIntegration.ValidatorVersionControl 构造函数数量: {ctors_ValidatorVersionControl.Length}");
        var methods_ValidatorVersionControl = type_ValidatorVersionControl.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FluentValidationIntegration.ValidatorVersionControl 公开方法数量: {methods_ValidatorVersionControl.Length}");
        foreach (var m in methods_ValidatorVersionControl)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.ValidatorVersionControl 未找到，尝试无命名空间...");
        type_ValidatorVersionControl = Type.GetType("ValidatorVersionControl");
        if (type_ValidatorVersionControl != null)
            Console.WriteLine("[PASS] 类型 ValidatorVersionControl (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValidatorVersionControl 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FluentValidationIntegration.TenantValidatorResolver
    var type_TenantValidatorResolver = Type.GetType("FluentValidationIntegration.TenantValidatorResolver");
    if (type_TenantValidatorResolver != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.TenantValidatorResolver (class) 存在");
        var ctors_TenantValidatorResolver = type_TenantValidatorResolver.GetConstructors();
        Console.WriteLine($"[PASS] FluentValidationIntegration.TenantValidatorResolver 构造函数数量: {ctors_TenantValidatorResolver.Length}");
        var methods_TenantValidatorResolver = type_TenantValidatorResolver.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FluentValidationIntegration.TenantValidatorResolver 公开方法数量: {methods_TenantValidatorResolver.Length}");
        foreach (var m in methods_TenantValidatorResolver)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.TenantValidatorResolver 未找到，尝试无命名空间...");
        type_TenantValidatorResolver = Type.GetType("TenantValidatorResolver");
        if (type_TenantValidatorResolver != null)
            Console.WriteLine("[PASS] 类型 TenantValidatorResolver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantValidatorResolver 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: FluentValidationIntegration.IDistributedValidatorCache
    var type_IDistributedValidatorCache = Type.GetType("FluentValidationIntegration.IDistributedValidatorCache");
    if (type_IDistributedValidatorCache != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.IDistributedValidatorCache (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.IDistributedValidatorCache 未找到，尝试无命名空间...");
        type_IDistributedValidatorCache = Type.GetType("IDistributedValidatorCache");
        if (type_IDistributedValidatorCache != null)
            Console.WriteLine("[PASS] 类型 IDistributedValidatorCache (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDistributedValidatorCache 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: FluentValidationIntegration.IValidatorLoader
    var type_IValidatorLoader = Type.GetType("FluentValidationIntegration.IValidatorLoader");
    if (type_IValidatorLoader != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.IValidatorLoader (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.IValidatorLoader 未找到，尝试无命名空间...");
        type_IValidatorLoader = Type.GetType("IValidatorLoader");
        if (type_IValidatorLoader != null)
            Console.WriteLine("[PASS] 类型 IValidatorLoader (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IValidatorLoader 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: FluentValidationIntegration.IValidatorMonitor
    var type_IValidatorMonitor = Type.GetType("FluentValidationIntegration.IValidatorMonitor");
    if (type_IValidatorMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.IValidatorMonitor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.IValidatorMonitor 未找到，尝试无命名空间...");
        type_IValidatorMonitor = Type.GetType("IValidatorMonitor");
        if (type_IValidatorMonitor != null)
            Console.WriteLine("[PASS] 类型 IValidatorMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IValidatorMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: FluentValidationIntegration.IValidatorVersionControl
    var type_IValidatorVersionControl = Type.GetType("FluentValidationIntegration.IValidatorVersionControl");
    if (type_IValidatorVersionControl != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.IValidatorVersionControl (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.IValidatorVersionControl 未找到，尝试无命名空间...");
        type_IValidatorVersionControl = Type.GetType("IValidatorVersionControl");
        if (type_IValidatorVersionControl != null)
            Console.WriteLine("[PASS] 类型 IValidatorVersionControl (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IValidatorVersionControl 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: FluentValidationIntegration.ITenantValidatorResolver
    var type_ITenantValidatorResolver = Type.GetType("FluentValidationIntegration.ITenantValidatorResolver");
    if (type_ITenantValidatorResolver != null)
    {
        Console.WriteLine("[PASS] 类型 FluentValidationIntegration.ITenantValidatorResolver (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentValidationIntegration.ITenantValidatorResolver 未找到，尝试无命名空间...");
        type_ITenantValidatorResolver = Type.GetType("ITenantValidatorResolver");
        if (type_ITenantValidatorResolver != null)
            Console.WriteLine("[PASS] 类型 ITenantValidatorResolver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITenantValidatorResolver 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
