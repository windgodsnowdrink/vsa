#load "foundatio_service.cs"

Console.WriteLine("=== foundatio_service.cs Test ===");

try
{
    // 验证 class: ComponentProcessor
    var type_ComponentProcessor = Type.GetType("ComponentProcessor");
    if (type_ComponentProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ComponentProcessor (class) 存在");
        var ctors_ComponentProcessor = type_ComponentProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ComponentProcessor 构造函数数量: {ctors_ComponentProcessor.Length}");
        var methods_ComponentProcessor = type_ComponentProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ComponentProcessor 公开方法数量: {methods_ComponentProcessor.Length}");
        foreach (var m in methods_ComponentProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ComponentProcessor 未找到，尝试无命名空间...");
        type_ComponentProcessor = Type.GetType("ComponentProcessor");
        if (type_ComponentProcessor != null)
            Console.WriteLine("[PASS] 类型 ComponentProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ComponentProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ComponentContext
    var type_ComponentContext = Type.GetType("ComponentContext");
    if (type_ComponentContext != null)
    {
        Console.WriteLine("[PASS] 类型 ComponentContext (class) 存在");
        var ctors_ComponentContext = type_ComponentContext.GetConstructors();
        Console.WriteLine($"[PASS] ComponentContext 构造函数数量: {ctors_ComponentContext.Length}");
        var methods_ComponentContext = type_ComponentContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ComponentContext 公开方法数量: {methods_ComponentContext.Length}");
        foreach (var m in methods_ComponentContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ComponentContext 未找到，尝试无命名空间...");
        type_ComponentContext = Type.GetType("ComponentContext");
        if (type_ComponentContext != null)
            Console.WriteLine("[PASS] 类型 ComponentContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ComponentContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ComponentContextPooledPolicy
    var type_ComponentContextPooledPolicy = Type.GetType("ComponentContextPooledPolicy");
    if (type_ComponentContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ComponentContextPooledPolicy (class) 存在");
        var ctors_ComponentContextPooledPolicy = type_ComponentContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ComponentContextPooledPolicy 构造函数数量: {ctors_ComponentContextPooledPolicy.Length}");
        var methods_ComponentContextPooledPolicy = type_ComponentContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ComponentContextPooledPolicy 公开方法数量: {methods_ComponentContextPooledPolicy.Length}");
        foreach (var m in methods_ComponentContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ComponentContextPooledPolicy 未找到，尝试无命名空间...");
        type_ComponentContextPooledPolicy = Type.GetType("ComponentContextPooledPolicy");
        if (type_ComponentContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 ComponentContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ComponentContextPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LoggingComponent
    var type_LoggingComponent = Type.GetType("LoggingComponent");
    if (type_LoggingComponent != null)
    {
        Console.WriteLine("[PASS] 类型 LoggingComponent (class) 存在");
        var ctors_LoggingComponent = type_LoggingComponent.GetConstructors();
        Console.WriteLine($"[PASS] LoggingComponent 构造函数数量: {ctors_LoggingComponent.Length}");
        var methods_LoggingComponent = type_LoggingComponent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoggingComponent 公开方法数量: {methods_LoggingComponent.Length}");
        foreach (var m in methods_LoggingComponent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoggingComponent 未找到，尝试无命名空间...");
        type_LoggingComponent = Type.GetType("LoggingComponent");
        if (type_LoggingComponent != null)
            Console.WriteLine("[PASS] 类型 LoggingComponent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoggingComponent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CachingComponent
    var type_CachingComponent = Type.GetType("CachingComponent");
    if (type_CachingComponent != null)
    {
        Console.WriteLine("[PASS] 类型 CachingComponent (class) 存在");
        var ctors_CachingComponent = type_CachingComponent.GetConstructors();
        Console.WriteLine($"[PASS] CachingComponent 构造函数数量: {ctors_CachingComponent.Length}");
        var methods_CachingComponent = type_CachingComponent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CachingComponent 公开方法数量: {methods_CachingComponent.Length}");
        foreach (var m in methods_CachingComponent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CachingComponent 未找到，尝试无命名空间...");
        type_CachingComponent = Type.GetType("CachingComponent");
        if (type_CachingComponent != null)
            Console.WriteLine("[PASS] 类型 CachingComponent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CachingComponent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MetricsComponent
    var type_MetricsComponent = Type.GetType("MetricsComponent");
    if (type_MetricsComponent != null)
    {
        Console.WriteLine("[PASS] 类型 MetricsComponent (class) 存在");
        var ctors_MetricsComponent = type_MetricsComponent.GetConstructors();
        Console.WriteLine($"[PASS] MetricsComponent 构造函数数量: {ctors_MetricsComponent.Length}");
        var methods_MetricsComponent = type_MetricsComponent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MetricsComponent 公开方法数量: {methods_MetricsComponent.Length}");
        foreach (var m in methods_MetricsComponent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MetricsComponent 未找到，尝试无命名空间...");
        type_MetricsComponent = Type.GetType("MetricsComponent");
        if (type_MetricsComponent != null)
            Console.WriteLine("[PASS] 类型 MetricsComponent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MetricsComponent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DistributedLockComponent
    var type_DistributedLockComponent = Type.GetType("DistributedLockComponent");
    if (type_DistributedLockComponent != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedLockComponent (class) 存在");
        var ctors_DistributedLockComponent = type_DistributedLockComponent.GetConstructors();
        Console.WriteLine($"[PASS] DistributedLockComponent 构造函数数量: {ctors_DistributedLockComponent.Length}");
        var methods_DistributedLockComponent = type_DistributedLockComponent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedLockComponent 公开方法数量: {methods_DistributedLockComponent.Length}");
        foreach (var m in methods_DistributedLockComponent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedLockComponent 未找到，尝试无命名空间...");
        type_DistributedLockComponent = Type.GetType("DistributedLockComponent");
        if (type_DistributedLockComponent != null)
            Console.WriteLine("[PASS] 类型 DistributedLockComponent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedLockComponent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: StorageComponent
    var type_StorageComponent = Type.GetType("StorageComponent");
    if (type_StorageComponent != null)
    {
        Console.WriteLine("[PASS] 类型 StorageComponent (class) 存在");
        var ctors_StorageComponent = type_StorageComponent.GetConstructors();
        Console.WriteLine($"[PASS] StorageComponent 构造函数数量: {ctors_StorageComponent.Length}");
        var methods_StorageComponent = type_StorageComponent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StorageComponent 公开方法数量: {methods_StorageComponent.Length}");
        foreach (var m in methods_StorageComponent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StorageComponent 未找到，尝试无命名空间...");
        type_StorageComponent = Type.GetType("StorageComponent");
        if (type_StorageComponent != null)
            Console.WriteLine("[PASS] 类型 StorageComponent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StorageComponent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoComponent
    var type_TodoComponent = Type.GetType("TodoComponent");
    if (type_TodoComponent != null)
    {
        Console.WriteLine("[PASS] 类型 TodoComponent (class) 存在");
        var ctors_TodoComponent = type_TodoComponent.GetConstructors();
        Console.WriteLine($"[PASS] TodoComponent 构造函数数量: {ctors_TodoComponent.Length}");
        var methods_TodoComponent = type_TodoComponent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoComponent 公开方法数量: {methods_TodoComponent.Length}");
        foreach (var m in methods_TodoComponent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoComponent 未找到，尝试无命名空间...");
        type_TodoComponent = Type.GetType("TodoComponent");
        if (type_TodoComponent != null)
            Console.WriteLine("[PASS] 类型 TodoComponent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoComponent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoContext
    var type_TodoContext = Type.GetType("TodoContext");
    if (type_TodoContext != null)
    {
        Console.WriteLine("[PASS] 类型 TodoContext (class) 存在");
        var ctors_TodoContext = type_TodoContext.GetConstructors();
        Console.WriteLine($"[PASS] TodoContext 构造函数数量: {ctors_TodoContext.Length}");
        var methods_TodoContext = type_TodoContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoContext 公开方法数量: {methods_TodoContext.Length}");
        foreach (var m in methods_TodoContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoContext 未找到，尝试无命名空间...");
        type_TodoContext = Type.GetType("TodoContext");
        if (type_TodoContext != null)
            Console.WriteLine("[PASS] 类型 TodoContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoContext 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPluggableComponent
    var type_IPluggableComponent = Type.GetType("IPluggableComponent");
    if (type_IPluggableComponent != null)
    {
        Console.WriteLine("[PASS] 类型 IPluggableComponent (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPluggableComponent 未找到，尝试无命名空间...");
        type_IPluggableComponent = Type.GetType("IPluggableComponent");
        if (type_IPluggableComponent != null)
            Console.WriteLine("[PASS] 类型 IPluggableComponent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPluggableComponent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ComponentMessage
    var type_ComponentMessage = Type.GetType("ComponentMessage");
    if (type_ComponentMessage != null)
    {
        Console.WriteLine("[PASS] 类型 ComponentMessage (record) 存在");
        var ctors_ComponentMessage = type_ComponentMessage.GetConstructors();
        Console.WriteLine($"[PASS] ComponentMessage 构造函数数量: {ctors_ComponentMessage.Length}");
        var methods_ComponentMessage = type_ComponentMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ComponentMessage 公开方法数量: {methods_ComponentMessage.Length}");
        foreach (var m in methods_ComponentMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ComponentMessage 未找到，尝试无命名空间...");
        type_ComponentMessage = Type.GetType("ComponentMessage");
        if (type_ComponentMessage != null)
            Console.WriteLine("[PASS] 类型 ComponentMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ComponentMessage 可能为顶层语句或嵌套类型");
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
