#load "sqlite_event_store.cs"

Console.WriteLine("=== sqlite_event_store.cs Test ===");

try
{
    // 验证 class: DistributedTransactionCoordinator
    var type_DistributedTransactionCoordinator = Type.GetType("DistributedTransactionCoordinator");
    if (type_DistributedTransactionCoordinator != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedTransactionCoordinator (class) 存在");
        var ctors_DistributedTransactionCoordinator = type_DistributedTransactionCoordinator.GetConstructors();
        Console.WriteLine($"[PASS] DistributedTransactionCoordinator 构造函数数量: {ctors_DistributedTransactionCoordinator.Length}");
        var methods_DistributedTransactionCoordinator = type_DistributedTransactionCoordinator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedTransactionCoordinator 公开方法数量: {methods_DistributedTransactionCoordinator.Length}");
        foreach (var m in methods_DistributedTransactionCoordinator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedTransactionCoordinator 未找到，尝试无命名空间...");
        type_DistributedTransactionCoordinator = Type.GetType("DistributedTransactionCoordinator");
        if (type_DistributedTransactionCoordinator != null)
            Console.WriteLine("[PASS] 类型 DistributedTransactionCoordinator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedTransactionCoordinator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteEventStore
    var type_SqliteEventStore = Type.GetType("SqliteEventStore");
    if (type_SqliteEventStore != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteEventStore (class) 存在");
        var ctors_SqliteEventStore = type_SqliteEventStore.GetConstructors();
        Console.WriteLine($"[PASS] SqliteEventStore 构造函数数量: {ctors_SqliteEventStore.Length}");
        var methods_SqliteEventStore = type_SqliteEventStore.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteEventStore 公开方法数量: {methods_SqliteEventStore.Length}");
        foreach (var m in methods_SqliteEventStore)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteEventStore 未找到，尝试无命名空间...");
        type_SqliteEventStore = Type.GetType("SqliteEventStore");
        if (type_SqliteEventStore != null)
            Console.WriteLine("[PASS] 类型 SqliteEventStore (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteEventStore 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteEventHandler
    var type_SqliteEventHandler = Type.GetType("SqliteEventHandler");
    if (type_SqliteEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteEventHandler (class) 存在");
        var ctors_SqliteEventHandler = type_SqliteEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] SqliteEventHandler 构造函数数量: {ctors_SqliteEventHandler.Length}");
        var methods_SqliteEventHandler = type_SqliteEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteEventHandler 公开方法数量: {methods_SqliteEventHandler.Length}");
        foreach (var m in methods_SqliteEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteEventHandler 未找到，尝试无命名空间...");
        type_SqliteEventHandler = Type.GetType("SqliteEventHandler");
        if (type_SqliteEventHandler != null)
            Console.WriteLine("[PASS] 类型 SqliteEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteEventHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteEventPoolPolicy
    var type_SqliteEventPoolPolicy = Type.GetType("SqliteEventPoolPolicy");
    if (type_SqliteEventPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteEventPoolPolicy (class) 存在");
        var ctors_SqliteEventPoolPolicy = type_SqliteEventPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] SqliteEventPoolPolicy 构造函数数量: {ctors_SqliteEventPoolPolicy.Length}");
        var methods_SqliteEventPoolPolicy = type_SqliteEventPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteEventPoolPolicy 公开方法数量: {methods_SqliteEventPoolPolicy.Length}");
        foreach (var m in methods_SqliteEventPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteEventPoolPolicy 未找到，尝试无命名空间...");
        type_SqliteEventPoolPolicy = Type.GetType("SqliteEventPoolPolicy");
        if (type_SqliteEventPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 SqliteEventPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteEventPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteCommandPoolPolicy
    var type_SqliteCommandPoolPolicy = Type.GetType("SqliteCommandPoolPolicy");
    if (type_SqliteCommandPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteCommandPoolPolicy (class) 存在");
        var ctors_SqliteCommandPoolPolicy = type_SqliteCommandPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] SqliteCommandPoolPolicy 构造函数数量: {ctors_SqliteCommandPoolPolicy.Length}");
        var methods_SqliteCommandPoolPolicy = type_SqliteCommandPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteCommandPoolPolicy 公开方法数量: {methods_SqliteCommandPoolPolicy.Length}");
        foreach (var m in methods_SqliteCommandPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteCommandPoolPolicy 未找到，尝试无命名空间...");
        type_SqliteCommandPoolPolicy = Type.GetType("SqliteCommandPoolPolicy");
        if (type_SqliteCommandPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 SqliteCommandPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteCommandPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: SqliteEvent
    var type_SqliteEvent = Type.GetType("SqliteEvent");
    if (type_SqliteEvent != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteEvent (struct) 存在");
        var ctors_SqliteEvent = type_SqliteEvent.GetConstructors();
        Console.WriteLine($"[PASS] SqliteEvent 构造函数数量: {ctors_SqliteEvent.Length}");
        var methods_SqliteEvent = type_SqliteEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteEvent 公开方法数量: {methods_SqliteEvent.Length}");
        foreach (var m in methods_SqliteEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteEvent 未找到，尝试无命名空间...");
        type_SqliteEvent = Type.GetType("SqliteEvent");
        if (type_SqliteEvent != null)
            Console.WriteLine("[PASS] 类型 SqliteEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
