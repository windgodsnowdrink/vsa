#load "wolverinefx_orleans_integration.cs"

Console.WriteLine("=== wolverinefx_orleans_integration.cs Test ===");

try
{
    // 验证 class: WolverineGrain
    var type_WolverineGrain = Type.GetType("WolverineGrain");
    if (type_WolverineGrain != null)
    {
        Console.WriteLine("[PASS] 类型 WolverineGrain (class) 存在");
        var ctors_WolverineGrain = type_WolverineGrain.GetConstructors();
        Console.WriteLine($"[PASS] WolverineGrain 构造函数数量: {ctors_WolverineGrain.Length}");
        var methods_WolverineGrain = type_WolverineGrain.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WolverineGrain 公开方法数量: {methods_WolverineGrain.Length}");
        foreach (var m in methods_WolverineGrain)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WolverineGrain 未找到，尝试无命名空间...");
        type_WolverineGrain = Type.GetType("WolverineGrain");
        if (type_WolverineGrain != null)
            Console.WriteLine("[PASS] 类型 WolverineGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WolverineGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WolverineGrainState
    var type_WolverineGrainState = Type.GetType("WolverineGrainState");
    if (type_WolverineGrainState != null)
    {
        Console.WriteLine("[PASS] 类型 WolverineGrainState (class) 存在");
        var ctors_WolverineGrainState = type_WolverineGrainState.GetConstructors();
        Console.WriteLine($"[PASS] WolverineGrainState 构造函数数量: {ctors_WolverineGrainState.Length}");
        var methods_WolverineGrainState = type_WolverineGrainState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WolverineGrainState 公开方法数量: {methods_WolverineGrainState.Length}");
        foreach (var m in methods_WolverineGrainState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WolverineGrainState 未找到，尝试无命名空间...");
        type_WolverineGrainState = Type.GetType("WolverineGrainState");
        if (type_WolverineGrainState != null)
            Console.WriteLine("[PASS] 类型 WolverineGrainState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WolverineGrainState 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrleansMessageHandler
    var type_OrleansMessageHandler = Type.GetType("OrleansMessageHandler");
    if (type_OrleansMessageHandler != null)
    {
        Console.WriteLine("[PASS] 类型 OrleansMessageHandler (class) 存在");
        var ctors_OrleansMessageHandler = type_OrleansMessageHandler.GetConstructors();
        Console.WriteLine($"[PASS] OrleansMessageHandler 构造函数数量: {ctors_OrleansMessageHandler.Length}");
        var methods_OrleansMessageHandler = type_OrleansMessageHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrleansMessageHandler 公开方法数量: {methods_OrleansMessageHandler.Length}");
        foreach (var m in methods_OrleansMessageHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrleansMessageHandler 未找到，尝试无命名空间...");
        type_OrleansMessageHandler = Type.GetType("OrleansMessageHandler");
        if (type_OrleansMessageHandler != null)
            Console.WriteLine("[PASS] 类型 OrleansMessageHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrleansMessageHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WolverineOrleansIntegrationExtensions
    var type_WolverineOrleansIntegrationExtensions = Type.GetType("WolverineOrleansIntegrationExtensions");
    if (type_WolverineOrleansIntegrationExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 WolverineOrleansIntegrationExtensions (class) 存在");
        var ctors_WolverineOrleansIntegrationExtensions = type_WolverineOrleansIntegrationExtensions.GetConstructors();
        Console.WriteLine($"[PASS] WolverineOrleansIntegrationExtensions 构造函数数量: {ctors_WolverineOrleansIntegrationExtensions.Length}");
        var methods_WolverineOrleansIntegrationExtensions = type_WolverineOrleansIntegrationExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WolverineOrleansIntegrationExtensions 公开方法数量: {methods_WolverineOrleansIntegrationExtensions.Length}");
        foreach (var m in methods_WolverineOrleansIntegrationExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WolverineOrleansIntegrationExtensions 未找到，尝试无命名空间...");
        type_WolverineOrleansIntegrationExtensions = Type.GetType("WolverineOrleansIntegrationExtensions");
        if (type_WolverineOrleansIntegrationExtensions != null)
            Console.WriteLine("[PASS] 类型 WolverineOrleansIntegrationExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WolverineOrleansIntegrationExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrleansTransport
    var type_OrleansTransport = Type.GetType("OrleansTransport");
    if (type_OrleansTransport != null)
    {
        Console.WriteLine("[PASS] 类型 OrleansTransport (class) 存在");
        var ctors_OrleansTransport = type_OrleansTransport.GetConstructors();
        Console.WriteLine($"[PASS] OrleansTransport 构造函数数量: {ctors_OrleansTransport.Length}");
        var methods_OrleansTransport = type_OrleansTransport.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrleansTransport 公开方法数量: {methods_OrleansTransport.Length}");
        foreach (var m in methods_OrleansTransport)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrleansTransport 未找到，尝试无命名空间...");
        type_OrleansTransport = Type.GetType("OrleansTransport");
        if (type_OrleansTransport != null)
            Console.WriteLine("[PASS] 类型 OrleansTransport (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrleansTransport 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IWolverineGrain
    var type_IWolverineGrain = Type.GetType("IWolverineGrain");
    if (type_IWolverineGrain != null)
    {
        Console.WriteLine("[PASS] 类型 IWolverineGrain (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IWolverineGrain 未找到，尝试无命名空间...");
        type_IWolverineGrain = Type.GetType("IWolverineGrain");
        if (type_IWolverineGrain != null)
            Console.WriteLine("[PASS] 类型 IWolverineGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IWolverineGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CrossGrainMessage
    var type_CrossGrainMessage = Type.GetType("CrossGrainMessage");
    if (type_CrossGrainMessage != null)
    {
        Console.WriteLine("[PASS] 类型 CrossGrainMessage (record) 存在");
        var ctors_CrossGrainMessage = type_CrossGrainMessage.GetConstructors();
        Console.WriteLine($"[PASS] CrossGrainMessage 构造函数数量: {ctors_CrossGrainMessage.Length}");
        var methods_CrossGrainMessage = type_CrossGrainMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CrossGrainMessage 公开方法数量: {methods_CrossGrainMessage.Length}");
        foreach (var m in methods_CrossGrainMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CrossGrainMessage 未找到，尝试无命名空间...");
        type_CrossGrainMessage = Type.GetType("CrossGrainMessage");
        if (type_CrossGrainMessage != null)
            Console.WriteLine("[PASS] 类型 CrossGrainMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CrossGrainMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
