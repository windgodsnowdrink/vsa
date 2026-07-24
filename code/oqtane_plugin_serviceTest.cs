#load "oqtane_plugin_service.cs"

Console.WriteLine("=== oqtane_plugin_service.cs Test ===");

try
{
    // 验证 class: OqtanePluginLoader
    var type_OqtanePluginLoader = Type.GetType("OqtanePluginLoader");
    if (type_OqtanePluginLoader != null)
    {
        Console.WriteLine("[PASS] 类型 OqtanePluginLoader (class) 存在");
        var ctors_OqtanePluginLoader = type_OqtanePluginLoader.GetConstructors();
        Console.WriteLine($"[PASS] OqtanePluginLoader 构造函数数量: {ctors_OqtanePluginLoader.Length}");
        var methods_OqtanePluginLoader = type_OqtanePluginLoader.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OqtanePluginLoader 公开方法数量: {methods_OqtanePluginLoader.Length}");
        foreach (var m in methods_OqtanePluginLoader)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OqtanePluginLoader 未找到，尝试无命名空间...");
        type_OqtanePluginLoader = Type.GetType("OqtanePluginLoader");
        if (type_OqtanePluginLoader != null)
            Console.WriteLine("[PASS] 类型 OqtanePluginLoader (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OqtanePluginLoader 可能为顶层语句或嵌套类型");
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

    // 验证 class: TodoPlugin
    var type_TodoPlugin = Type.GetType("TodoPlugin");
    if (type_TodoPlugin != null)
    {
        Console.WriteLine("[PASS] 类型 TodoPlugin (class) 存在");
        var ctors_TodoPlugin = type_TodoPlugin.GetConstructors();
        Console.WriteLine($"[PASS] TodoPlugin 构造函数数量: {ctors_TodoPlugin.Length}");
        var methods_TodoPlugin = type_TodoPlugin.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoPlugin 公开方法数量: {methods_TodoPlugin.Length}");
        foreach (var m in methods_TodoPlugin)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoPlugin 未找到，尝试无命名空间...");
        type_TodoPlugin = Type.GetType("TodoPlugin");
        if (type_TodoPlugin != null)
            Console.WriteLine("[PASS] 类型 TodoPlugin (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoPlugin 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IOqtanePlugin
    var type_IOqtanePlugin = Type.GetType("IOqtanePlugin");
    if (type_IOqtanePlugin != null)
    {
        Console.WriteLine("[PASS] 类型 IOqtanePlugin (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IOqtanePlugin 未找到，尝试无命名空间...");
        type_IOqtanePlugin = Type.GetType("IOqtanePlugin");
        if (type_IOqtanePlugin != null)
            Console.WriteLine("[PASS] 类型 IOqtanePlugin (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IOqtanePlugin 可能为顶层语句或嵌套类型");
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

    // 验证 record: TodoCommand
    var type_TodoCommand = Type.GetType("TodoCommand");
    if (type_TodoCommand != null)
    {
        Console.WriteLine("[PASS] 类型 TodoCommand (record) 存在");
        var ctors_TodoCommand = type_TodoCommand.GetConstructors();
        Console.WriteLine($"[PASS] TodoCommand 构造函数数量: {ctors_TodoCommand.Length}");
        var methods_TodoCommand = type_TodoCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoCommand 公开方法数量: {methods_TodoCommand.Length}");
        foreach (var m in methods_TodoCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoCommand 未找到，尝试无命名空间...");
        type_TodoCommand = Type.GetType("TodoCommand");
        if (type_TodoCommand != null)
            Console.WriteLine("[PASS] 类型 TodoCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TodoItem
    var type_TodoItem = Type.GetType("TodoItem");
    if (type_TodoItem != null)
    {
        Console.WriteLine("[PASS] 类型 TodoItem (record) 存在");
        var ctors_TodoItem = type_TodoItem.GetConstructors();
        Console.WriteLine($"[PASS] TodoItem 构造函数数量: {ctors_TodoItem.Length}");
        var methods_TodoItem = type_TodoItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoItem 公开方法数量: {methods_TodoItem.Length}");
        foreach (var m in methods_TodoItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoItem 未找到，尝试无命名空间...");
        type_TodoItem = Type.GetType("TodoItem");
        if (type_TodoItem != null)
            Console.WriteLine("[PASS] 类型 TodoItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoItem 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
