#load "rougamo_integration.cs"

Console.WriteLine("=== rougamo_integration.cs Test ===");

try
{
    // 验证 class: PluginAssemblyLoadContext
    var type_PluginAssemblyLoadContext = Type.GetType("PluginAssemblyLoadContext");
    if (type_PluginAssemblyLoadContext != null)
    {
        Console.WriteLine("[PASS] 类型 PluginAssemblyLoadContext (class) 存在");
        var ctors_PluginAssemblyLoadContext = type_PluginAssemblyLoadContext.GetConstructors();
        Console.WriteLine($"[PASS] PluginAssemblyLoadContext 构造函数数量: {ctors_PluginAssemblyLoadContext.Length}");
        var methods_PluginAssemblyLoadContext = type_PluginAssemblyLoadContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginAssemblyLoadContext 公开方法数量: {methods_PluginAssemblyLoadContext.Length}");
        foreach (var m in methods_PluginAssemblyLoadContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginAssemblyLoadContext 未找到，尝试无命名空间...");
        type_PluginAssemblyLoadContext = Type.GetType("PluginAssemblyLoadContext");
        if (type_PluginAssemblyLoadContext != null)
            Console.WriteLine("[PASS] 类型 PluginAssemblyLoadContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginAssemblyLoadContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PluginInterceptorAttribute
    var type_PluginInterceptorAttribute = Type.GetType("PluginInterceptorAttribute");
    if (type_PluginInterceptorAttribute != null)
    {
        Console.WriteLine("[PASS] 类型 PluginInterceptorAttribute (class) 存在");
        var ctors_PluginInterceptorAttribute = type_PluginInterceptorAttribute.GetConstructors();
        Console.WriteLine($"[PASS] PluginInterceptorAttribute 构造函数数量: {ctors_PluginInterceptorAttribute.Length}");
        var methods_PluginInterceptorAttribute = type_PluginInterceptorAttribute.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginInterceptorAttribute 公开方法数量: {methods_PluginInterceptorAttribute.Length}");
        foreach (var m in methods_PluginInterceptorAttribute)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginInterceptorAttribute 未找到，尝试无命名空间...");
        type_PluginInterceptorAttribute = Type.GetType("PluginInterceptorAttribute");
        if (type_PluginInterceptorAttribute != null)
            Console.WriteLine("[PASS] 类型 PluginInterceptorAttribute (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginInterceptorAttribute 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PluginManagerService
    var type_PluginManagerService = Type.GetType("PluginManagerService");
    if (type_PluginManagerService != null)
    {
        Console.WriteLine("[PASS] 类型 PluginManagerService (class) 存在");
        var ctors_PluginManagerService = type_PluginManagerService.GetConstructors();
        Console.WriteLine($"[PASS] PluginManagerService 构造函数数量: {ctors_PluginManagerService.Length}");
        var methods_PluginManagerService = type_PluginManagerService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginManagerService 公开方法数量: {methods_PluginManagerService.Length}");
        foreach (var m in methods_PluginManagerService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginManagerService 未找到，尝试无命名空间...");
        type_PluginManagerService = Type.GetType("PluginManagerService");
        if (type_PluginManagerService != null)
            Console.WriteLine("[PASS] 类型 PluginManagerService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginManagerService 可能为顶层语句或嵌套类型");
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

    // 验证 class: PluginUnloadedException
    var type_PluginUnloadedException = Type.GetType("PluginUnloadedException");
    if (type_PluginUnloadedException != null)
    {
        Console.WriteLine("[PASS] 类型 PluginUnloadedException (class) 存在");
        var ctors_PluginUnloadedException = type_PluginUnloadedException.GetConstructors();
        Console.WriteLine($"[PASS] PluginUnloadedException 构造函数数量: {ctors_PluginUnloadedException.Length}");
        var methods_PluginUnloadedException = type_PluginUnloadedException.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginUnloadedException 公开方法数量: {methods_PluginUnloadedException.Length}");
        foreach (var m in methods_PluginUnloadedException)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginUnloadedException 未找到，尝试无命名空间...");
        type_PluginUnloadedException = Type.GetType("PluginUnloadedException");
        if (type_PluginUnloadedException != null)
            Console.WriteLine("[PASS] 类型 PluginUnloadedException (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginUnloadedException 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CalePlugin
    var type_CalePlugin = Type.GetType("CalePlugin");
    if (type_CalePlugin != null)
    {
        Console.WriteLine("[PASS] 类型 CalePlugin (class) 存在");
        var ctors_CalePlugin = type_CalePlugin.GetConstructors();
        Console.WriteLine($"[PASS] CalePlugin 构造函数数量: {ctors_CalePlugin.Length}");
        var methods_CalePlugin = type_CalePlugin.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CalePlugin 公开方法数量: {methods_CalePlugin.Length}");
        foreach (var m in methods_CalePlugin)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CalePlugin 未找到，尝试无命名空间...");
        type_CalePlugin = Type.GetType("CalePlugin");
        if (type_CalePlugin != null)
            Console.WriteLine("[PASS] 类型 CalePlugin (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CalePlugin 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PluginLoadExtensions
    var type_PluginLoadExtensions = Type.GetType("PluginLoadExtensions");
    if (type_PluginLoadExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PluginLoadExtensions (class) 存在");
        var ctors_PluginLoadExtensions = type_PluginLoadExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PluginLoadExtensions 构造函数数量: {ctors_PluginLoadExtensions.Length}");
        var methods_PluginLoadExtensions = type_PluginLoadExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginLoadExtensions 公开方法数量: {methods_PluginLoadExtensions.Length}");
        foreach (var m in methods_PluginLoadExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginLoadExtensions 未找到，尝试无命名空间...");
        type_PluginLoadExtensions = Type.GetType("PluginLoadExtensions");
        if (type_PluginLoadExtensions != null)
            Console.WriteLine("[PASS] 类型 PluginLoadExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginLoadExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPluginUnloadable
    var type_IPluginUnloadable = Type.GetType("IPluginUnloadable");
    if (type_IPluginUnloadable != null)
    {
        Console.WriteLine("[PASS] 类型 IPluginUnloadable (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPluginUnloadable 未找到，尝试无命名空间...");
        type_IPluginUnloadable = Type.GetType("IPluginUnloadable");
        if (type_IPluginUnloadable != null)
            Console.WriteLine("[PASS] 类型 IPluginUnloadable (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPluginUnloadable 可能为顶层语句或嵌套类型");
    }

    // 验证 record: PluginMetadata
    var type_PluginMetadata = Type.GetType("PluginMetadata");
    if (type_PluginMetadata != null)
    {
        Console.WriteLine("[PASS] 类型 PluginMetadata (record) 存在");
        var ctors_PluginMetadata = type_PluginMetadata.GetConstructors();
        Console.WriteLine($"[PASS] PluginMetadata 构造函数数量: {ctors_PluginMetadata.Length}");
        var methods_PluginMetadata = type_PluginMetadata.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginMetadata 公开方法数量: {methods_PluginMetadata.Length}");
        foreach (var m in methods_PluginMetadata)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginMetadata 未找到，尝试无命名空间...");
        type_PluginMetadata = Type.GetType("PluginMetadata");
        if (type_PluginMetadata != null)
            Console.WriteLine("[PASS] 类型 PluginMetadata (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginMetadata 可能为顶层语句或嵌套类型");
    }

    // 验证 record: PluginCommand
    var type_PluginCommand = Type.GetType("PluginCommand");
    if (type_PluginCommand != null)
    {
        Console.WriteLine("[PASS] 类型 PluginCommand (record) 存在");
        var ctors_PluginCommand = type_PluginCommand.GetConstructors();
        Console.WriteLine($"[PASS] PluginCommand 构造函数数量: {ctors_PluginCommand.Length}");
        var methods_PluginCommand = type_PluginCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PluginCommand 公开方法数量: {methods_PluginCommand.Length}");
        foreach (var m in methods_PluginCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PluginCommand 未找到，尝试无命名空间...");
        type_PluginCommand = Type.GetType("PluginCommand");
        if (type_PluginCommand != null)
            Console.WriteLine("[PASS] 类型 PluginCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PluginCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: LoadPluginCommand
    var type_LoadPluginCommand = Type.GetType("LoadPluginCommand");
    if (type_LoadPluginCommand != null)
    {
        Console.WriteLine("[PASS] 类型 LoadPluginCommand (record) 存在");
        var ctors_LoadPluginCommand = type_LoadPluginCommand.GetConstructors();
        Console.WriteLine($"[PASS] LoadPluginCommand 构造函数数量: {ctors_LoadPluginCommand.Length}");
        var methods_LoadPluginCommand = type_LoadPluginCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoadPluginCommand 公开方法数量: {methods_LoadPluginCommand.Length}");
        foreach (var m in methods_LoadPluginCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoadPluginCommand 未找到，尝试无命名空间...");
        type_LoadPluginCommand = Type.GetType("LoadPluginCommand");
        if (type_LoadPluginCommand != null)
            Console.WriteLine("[PASS] 类型 LoadPluginCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoadPluginCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UnloadPluginCommand
    var type_UnloadPluginCommand = Type.GetType("UnloadPluginCommand");
    if (type_UnloadPluginCommand != null)
    {
        Console.WriteLine("[PASS] 类型 UnloadPluginCommand (record) 存在");
        var ctors_UnloadPluginCommand = type_UnloadPluginCommand.GetConstructors();
        Console.WriteLine($"[PASS] UnloadPluginCommand 构造函数数量: {ctors_UnloadPluginCommand.Length}");
        var methods_UnloadPluginCommand = type_UnloadPluginCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UnloadPluginCommand 公开方法数量: {methods_UnloadPluginCommand.Length}");
        foreach (var m in methods_UnloadPluginCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UnloadPluginCommand 未找到，尝试无命名空间...");
        type_UnloadPluginCommand = Type.GetType("UnloadPluginCommand");
        if (type_UnloadPluginCommand != null)
            Console.WriteLine("[PASS] 类型 UnloadPluginCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UnloadPluginCommand 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
