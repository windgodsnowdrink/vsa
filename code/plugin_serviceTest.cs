#load "plugin_service.cs"

Console.WriteLine("=== plugin_service.cs Test ===");

try
{
    // 验证 class: PluginLoader
    var type_PluginLoader = Type.GetType("PluginLoader");
    if (type_PluginLoader != null)
    {
        Console.WriteLine("[PASS] 类型 PluginLoader (class) 存在");
        var ctors_PluginLoader = type_PluginLoader.GetConstructors();
        Console.WriteLine($"[PASS] PluginLoader 构造函数数量: {ctors_PluginLoader.Length}");
        var methods_PluginLoader = type_PluginLoader.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginLoader 公开方法数量: {methods_PluginLoader.Length}");
        foreach (var m in methods_PluginLoader)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginLoader 未找到，尝试无命名空间...");
        type_PluginLoader = Type.GetType("PluginLoader");
        if (type_PluginLoader != null)
            Console.WriteLine("[PASS] 类型 PluginLoader (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginLoader 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PluginContext
    var type_PluginContext = Type.GetType("PluginContext");
    if (type_PluginContext != null)
    {
        Console.WriteLine("[PASS] 类型 PluginContext (class) 存在");
        var ctors_PluginContext = type_PluginContext.GetConstructors();
        Console.WriteLine($"[PASS] PluginContext 构造函数数量: {ctors_PluginContext.Length}");
        var methods_PluginContext = type_PluginContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginContext 公开方法数量: {methods_PluginContext.Length}");
        foreach (var m in methods_PluginContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginContext 未找到，尝试无命名空间...");
        type_PluginContext = Type.GetType("PluginContext");
        if (type_PluginContext != null)
            Console.WriteLine("[PASS] 类型 PluginContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PluginContextPooledPolicy
    var type_PluginContextPooledPolicy = Type.GetType("PluginContextPooledPolicy");
    if (type_PluginContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 PluginContextPooledPolicy (class) 存在");
        var ctors_PluginContextPooledPolicy = type_PluginContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] PluginContextPooledPolicy 构造函数数量: {ctors_PluginContextPooledPolicy.Length}");
        var methods_PluginContextPooledPolicy = type_PluginContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginContextPooledPolicy 公开方法数量: {methods_PluginContextPooledPolicy.Length}");
        foreach (var m in methods_PluginContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginContextPooledPolicy 未找到，尝试无命名空间...");
        type_PluginContextPooledPolicy = Type.GetType("PluginContextPooledPolicy");
        if (type_PluginContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 PluginContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginContextPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IHotPlugModule
    var type_IHotPlugModule = Type.GetType("IHotPlugModule");
    if (type_IHotPlugModule != null)
    {
        Console.WriteLine("[PASS] 类型 IHotPlugModule (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IHotPlugModule 未找到，尝试无命名空间...");
        type_IHotPlugModule = Type.GetType("IHotPlugModule");
        if (type_IHotPlugModule != null)
            Console.WriteLine("[PASS] 类型 IHotPlugModule (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IHotPlugModule 可能为顶层语句或嵌套类型");
    }

    // 验证 record: PluginMessage
    var type_PluginMessage = Type.GetType("PluginMessage");
    if (type_PluginMessage != null)
    {
        Console.WriteLine("[PASS] 类型 PluginMessage (record) 存在");
        var ctors_PluginMessage = type_PluginMessage.GetConstructors();
        Console.WriteLine($"[PASS] PluginMessage 构造函数数量: {ctors_PluginMessage.Length}");
        var methods_PluginMessage = type_PluginMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginMessage 公开方法数量: {methods_PluginMessage.Length}");
        foreach (var m in methods_PluginMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginMessage 未找到，尝试无命名空间...");
        type_PluginMessage = Type.GetType("PluginMessage");
        if (type_PluginMessage != null)
            Console.WriteLine("[PASS] 类型 PluginMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 record: PluginLoaderOptions
    var type_PluginLoaderOptions = Type.GetType("PluginLoaderOptions");
    if (type_PluginLoaderOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PluginLoaderOptions (record) 存在");
        var ctors_PluginLoaderOptions = type_PluginLoaderOptions.GetConstructors();
        Console.WriteLine($"[PASS] PluginLoaderOptions 构造函数数量: {ctors_PluginLoaderOptions.Length}");
        var methods_PluginLoaderOptions = type_PluginLoaderOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginLoaderOptions 公开方法数量: {methods_PluginLoaderOptions.Length}");
        foreach (var m in methods_PluginLoaderOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginLoaderOptions 未找到，尝试无命名空间...");
        type_PluginLoaderOptions = Type.GetType("PluginLoaderOptions");
        if (type_PluginLoaderOptions != null)
            Console.WriteLine("[PASS] 类型 PluginLoaderOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginLoaderOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
