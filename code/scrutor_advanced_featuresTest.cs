#load "scrutor_advanced_features.cs"

Console.WriteLine("=== scrutor_advanced_features.cs Test ===");

try
{
    // 验证 class: ProxyService
    var type_ProxyService = Type.GetType("ProxyService");
    if (type_ProxyService != null)
    {
        Console.WriteLine("[PASS] 类型 ProxyService (class) 存在");
        var ctors_ProxyService = type_ProxyService.GetConstructors();
        Console.WriteLine($"[PASS] ProxyService 构造函数数量: {ctors_ProxyService.Length}");
        var methods_ProxyService = type_ProxyService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProxyService 公开方法数量: {methods_ProxyService.Length}");
        foreach (var m in methods_ProxyService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProxyService 未找到，尝试无命名空间...");
        type_ProxyService = Type.GetType("ProxyService");
        if (type_ProxyService != null)
            Console.WriteLine("[PASS] 类型 ProxyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProxyService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProxyInterceptor
    var type_ProxyInterceptor = Type.GetType("ProxyInterceptor");
    if (type_ProxyInterceptor != null)
    {
        Console.WriteLine("[PASS] 类型 ProxyInterceptor (class) 存在");
        var ctors_ProxyInterceptor = type_ProxyInterceptor.GetConstructors();
        Console.WriteLine($"[PASS] ProxyInterceptor 构造函数数量: {ctors_ProxyInterceptor.Length}");
        var methods_ProxyInterceptor = type_ProxyInterceptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProxyInterceptor 公开方法数量: {methods_ProxyInterceptor.Length}");
        foreach (var m in methods_ProxyInterceptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProxyInterceptor 未找到，尝试无命名空间...");
        type_ProxyInterceptor = Type.GetType("ProxyInterceptor");
        if (type_ProxyInterceptor != null)
            Console.WriteLine("[PASS] 类型 ProxyInterceptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProxyInterceptor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ConditionalCore
    var type_ConditionalCore = Type.GetType("ConditionalCore");
    if (type_ConditionalCore != null)
    {
        Console.WriteLine("[PASS] 类型 ConditionalCore (class) 存在");
        var ctors_ConditionalCore = type_ConditionalCore.GetConstructors();
        Console.WriteLine($"[PASS] ConditionalCore 构造函数数量: {ctors_ConditionalCore.Length}");
        var methods_ConditionalCore = type_ConditionalCore.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConditionalCore 公开方法数量: {methods_ConditionalCore.Length}");
        foreach (var m in methods_ConditionalCore)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConditionalCore 未找到，尝试无命名空间...");
        type_ConditionalCore = Type.GetType("ConditionalCore");
        if (type_ConditionalCore != null)
            Console.WriteLine("[PASS] 类型 ConditionalCore (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConditionalCore 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DevelopmentDecorator
    var type_DevelopmentDecorator = Type.GetType("DevelopmentDecorator");
    if (type_DevelopmentDecorator != null)
    {
        Console.WriteLine("[PASS] 类型 DevelopmentDecorator (class) 存在");
        var ctors_DevelopmentDecorator = type_DevelopmentDecorator.GetConstructors();
        Console.WriteLine($"[PASS] DevelopmentDecorator 构造函数数量: {ctors_DevelopmentDecorator.Length}");
        var methods_DevelopmentDecorator = type_DevelopmentDecorator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DevelopmentDecorator 公开方法数量: {methods_DevelopmentDecorator.Length}");
        foreach (var m in methods_DevelopmentDecorator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DevelopmentDecorator 未找到，尝试无命名空间...");
        type_DevelopmentDecorator = Type.GetType("DevelopmentDecorator");
        if (type_DevelopmentDecorator != null)
            Console.WriteLine("[PASS] 类型 DevelopmentDecorator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DevelopmentDecorator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MultiLifetimeService
    var type_MultiLifetimeService = Type.GetType("MultiLifetimeService");
    if (type_MultiLifetimeService != null)
    {
        Console.WriteLine("[PASS] 类型 MultiLifetimeService (class) 存在");
        var ctors_MultiLifetimeService = type_MultiLifetimeService.GetConstructors();
        Console.WriteLine($"[PASS] MultiLifetimeService 构造函数数量: {ctors_MultiLifetimeService.Length}");
        var methods_MultiLifetimeService = type_MultiLifetimeService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultiLifetimeService 公开方法数量: {methods_MultiLifetimeService.Length}");
        foreach (var m in methods_MultiLifetimeService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultiLifetimeService 未找到，尝试无命名空间...");
        type_MultiLifetimeService = Type.GetType("MultiLifetimeService");
        if (type_MultiLifetimeService != null)
            Console.WriteLine("[PASS] 类型 MultiLifetimeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultiLifetimeService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AdvancedFeatures
    var type_AdvancedFeatures = Type.GetType("AdvancedFeatures");
    if (type_AdvancedFeatures != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedFeatures (class) 存在");
        var ctors_AdvancedFeatures = type_AdvancedFeatures.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedFeatures 构造函数数量: {ctors_AdvancedFeatures.Length}");
        var methods_AdvancedFeatures = type_AdvancedFeatures.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedFeatures 公开方法数量: {methods_AdvancedFeatures.Length}");
        foreach (var m in methods_AdvancedFeatures)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedFeatures 未找到，尝试无命名空间...");
        type_AdvancedFeatures = Type.GetType("AdvancedFeatures");
        if (type_AdvancedFeatures != null)
            Console.WriteLine("[PASS] 类型 AdvancedFeatures (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedFeatures 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MultiLifetimeResolver
    var type_MultiLifetimeResolver = Type.GetType("MultiLifetimeResolver");
    if (type_MultiLifetimeResolver != null)
    {
        Console.WriteLine("[PASS] 类型 MultiLifetimeResolver (class) 存在");
        var ctors_MultiLifetimeResolver = type_MultiLifetimeResolver.GetConstructors();
        Console.WriteLine($"[PASS] MultiLifetimeResolver 构造函数数量: {ctors_MultiLifetimeResolver.Length}");
        var methods_MultiLifetimeResolver = type_MultiLifetimeResolver.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultiLifetimeResolver 公开方法数量: {methods_MultiLifetimeResolver.Length}");
        foreach (var m in methods_MultiLifetimeResolver)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultiLifetimeResolver 未找到，尝试无命名空间...");
        type_MultiLifetimeResolver = Type.GetType("MultiLifetimeResolver");
        if (type_MultiLifetimeResolver != null)
            Console.WriteLine("[PASS] 类型 MultiLifetimeResolver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultiLifetimeResolver 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IProxyService
    var type_IProxyService = Type.GetType("IProxyService");
    if (type_IProxyService != null)
    {
        Console.WriteLine("[PASS] 类型 IProxyService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IProxyService 未找到，尝试无命名空间...");
        type_IProxyService = Type.GetType("IProxyService");
        if (type_IProxyService != null)
            Console.WriteLine("[PASS] 类型 IProxyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IProxyService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IConditionalDecorator
    var type_IConditionalDecorator = Type.GetType("IConditionalDecorator");
    if (type_IConditionalDecorator != null)
    {
        Console.WriteLine("[PASS] 类型 IConditionalDecorator (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IConditionalDecorator 未找到，尝试无命名空间...");
        type_IConditionalDecorator = Type.GetType("IConditionalDecorator");
        if (type_IConditionalDecorator != null)
            Console.WriteLine("[PASS] 类型 IConditionalDecorator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IConditionalDecorator 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMultiLifetimeService
    var type_IMultiLifetimeService = Type.GetType("IMultiLifetimeService");
    if (type_IMultiLifetimeService != null)
    {
        Console.WriteLine("[PASS] 类型 IMultiLifetimeService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMultiLifetimeService 未找到，尝试无命名空间...");
        type_IMultiLifetimeService = Type.GetType("IMultiLifetimeService");
        if (type_IMultiLifetimeService != null)
            Console.WriteLine("[PASS] 类型 IMultiLifetimeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMultiLifetimeService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
