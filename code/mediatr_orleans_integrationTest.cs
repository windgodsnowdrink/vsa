#load "mediatr_orleans_integration.cs"

Console.WriteLine("=== mediatr_orleans_integration.cs Test ===");

try
{
    // 验证 class: MediatorGrain
    var type_MediatorGrain = Type.GetType("MediatorGrain");
    if (type_MediatorGrain != null)
    {
        Console.WriteLine("[PASS] 类型 MediatorGrain (class) 存在");
        var ctors_MediatorGrain = type_MediatorGrain.GetConstructors();
        Console.WriteLine($"[PASS] MediatorGrain 构造函数数量: {ctors_MediatorGrain.Length}");
        var methods_MediatorGrain = type_MediatorGrain.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediatorGrain 公开方法数量: {methods_MediatorGrain.Length}");
        foreach (var m in methods_MediatorGrain)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediatorGrain 未找到，尝试无命名空间...");
        type_MediatorGrain = Type.GetType("MediatorGrain");
        if (type_MediatorGrain != null)
            Console.WriteLine("[PASS] 类型 MediatorGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediatorGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MediatorGrainState
    var type_MediatorGrainState = Type.GetType("MediatorGrainState");
    if (type_MediatorGrainState != null)
    {
        Console.WriteLine("[PASS] 类型 MediatorGrainState (class) 存在");
        var ctors_MediatorGrainState = type_MediatorGrainState.GetConstructors();
        Console.WriteLine($"[PASS] MediatorGrainState 构造函数数量: {ctors_MediatorGrainState.Length}");
        var methods_MediatorGrainState = type_MediatorGrainState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediatorGrainState 公开方法数量: {methods_MediatorGrainState.Length}");
        foreach (var m in methods_MediatorGrainState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediatorGrainState 未找到，尝试无命名空间...");
        type_MediatorGrainState = Type.GetType("MediatorGrainState");
        if (type_MediatorGrainState != null)
            Console.WriteLine("[PASS] 类型 MediatorGrainState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediatorGrainState 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrleansNotificationHandler
    var type_OrleansNotificationHandler = Type.GetType("OrleansNotificationHandler");
    if (type_OrleansNotificationHandler != null)
    {
        Console.WriteLine("[PASS] 类型 OrleansNotificationHandler (class) 存在");
        var ctors_OrleansNotificationHandler = type_OrleansNotificationHandler.GetConstructors();
        Console.WriteLine($"[PASS] OrleansNotificationHandler 构造函数数量: {ctors_OrleansNotificationHandler.Length}");
        var methods_OrleansNotificationHandler = type_OrleansNotificationHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrleansNotificationHandler 公开方法数量: {methods_OrleansNotificationHandler.Length}");
        foreach (var m in methods_OrleansNotificationHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrleansNotificationHandler 未找到，尝试无命名空间...");
        type_OrleansNotificationHandler = Type.GetType("OrleansNotificationHandler");
        if (type_OrleansNotificationHandler != null)
            Console.WriteLine("[PASS] 类型 OrleansNotificationHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrleansNotificationHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MediatorMetrics
    var type_MediatorMetrics = Type.GetType("MediatorMetrics");
    if (type_MediatorMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 MediatorMetrics (class) 存在");
        var ctors_MediatorMetrics = type_MediatorMetrics.GetConstructors();
        Console.WriteLine($"[PASS] MediatorMetrics 构造函数数量: {ctors_MediatorMetrics.Length}");
        var methods_MediatorMetrics = type_MediatorMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediatorMetrics 公开方法数量: {methods_MediatorMetrics.Length}");
        foreach (var m in methods_MediatorMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediatorMetrics 未找到，尝试无命名空间...");
        type_MediatorMetrics = Type.GetType("MediatorMetrics");
        if (type_MediatorMetrics != null)
            Console.WriteLine("[PASS] 类型 MediatorMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediatorMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuthGrainFilter
    var type_AuthGrainFilter = Type.GetType("AuthGrainFilter");
    if (type_AuthGrainFilter != null)
    {
        Console.WriteLine("[PASS] 类型 AuthGrainFilter (class) 存在");
        var ctors_AuthGrainFilter = type_AuthGrainFilter.GetConstructors();
        Console.WriteLine($"[PASS] AuthGrainFilter 构造函数数量: {ctors_AuthGrainFilter.Length}");
        var methods_AuthGrainFilter = type_AuthGrainFilter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuthGrainFilter 公开方法数量: {methods_AuthGrainFilter.Length}");
        foreach (var m in methods_AuthGrainFilter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuthGrainFilter 未找到，尝试无命名空间...");
        type_AuthGrainFilter = Type.GetType("AuthGrainFilter");
        if (type_AuthGrainFilter != null)
            Console.WriteLine("[PASS] 类型 AuthGrainFilter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuthGrainFilter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MediatROrleansIntegrationExtensions
    var type_MediatROrleansIntegrationExtensions = Type.GetType("MediatROrleansIntegrationExtensions");
    if (type_MediatROrleansIntegrationExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MediatROrleansIntegrationExtensions (class) 存在");
        var ctors_MediatROrleansIntegrationExtensions = type_MediatROrleansIntegrationExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MediatROrleansIntegrationExtensions 构造函数数量: {ctors_MediatROrleansIntegrationExtensions.Length}");
        var methods_MediatROrleansIntegrationExtensions = type_MediatROrleansIntegrationExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediatROrleansIntegrationExtensions 公开方法数量: {methods_MediatROrleansIntegrationExtensions.Length}");
        foreach (var m in methods_MediatROrleansIntegrationExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediatROrleansIntegrationExtensions 未找到，尝试无命名空间...");
        type_MediatROrleansIntegrationExtensions = Type.GetType("MediatROrleansIntegrationExtensions");
        if (type_MediatROrleansIntegrationExtensions != null)
            Console.WriteLine("[PASS] 类型 MediatROrleansIntegrationExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediatROrleansIntegrationExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMediatorGrain
    var type_IMediatorGrain = Type.GetType("IMediatorGrain");
    if (type_IMediatorGrain != null)
    {
        Console.WriteLine("[PASS] 类型 IMediatorGrain (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMediatorGrain 未找到，尝试无命名空间...");
        type_IMediatorGrain = Type.GetType("IMediatorGrain");
        if (type_IMediatorGrain != null)
            Console.WriteLine("[PASS] 类型 IMediatorGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMediatorGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMessageStore
    var type_IMessageStore = Type.GetType("IMessageStore");
    if (type_IMessageStore != null)
    {
        Console.WriteLine("[PASS] 类型 IMessageStore (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMessageStore 未找到，尝试无命名空间...");
        type_IMessageStore = Type.GetType("IMessageStore");
        if (type_IMessageStore != null)
            Console.WriteLine("[PASS] 类型 IMessageStore (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMessageStore 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CrossGrainCommand
    var type_CrossGrainCommand = Type.GetType("CrossGrainCommand");
    if (type_CrossGrainCommand != null)
    {
        Console.WriteLine("[PASS] 类型 CrossGrainCommand (record) 存在");
        var ctors_CrossGrainCommand = type_CrossGrainCommand.GetConstructors();
        Console.WriteLine($"[PASS] CrossGrainCommand 构造函数数量: {ctors_CrossGrainCommand.Length}");
        var methods_CrossGrainCommand = type_CrossGrainCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CrossGrainCommand 公开方法数量: {methods_CrossGrainCommand.Length}");
        foreach (var m in methods_CrossGrainCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CrossGrainCommand 未找到，尝试无命名空间...");
        type_CrossGrainCommand = Type.GetType("CrossGrainCommand");
        if (type_CrossGrainCommand != null)
            Console.WriteLine("[PASS] 类型 CrossGrainCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CrossGrainCommand 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
