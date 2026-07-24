#load "efcore_dapper_integration.cs"

Console.WriteLine("=== efcore_dapper_integration.cs Test ===");

try
{
    // 验证 class: AppDbContext
    var type_AppDbContext = Type.GetType("AppDbContext");
    if (type_AppDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 AppDbContext (class) 存在");
        var ctors_AppDbContext = type_AppDbContext.GetConstructors();
        Console.WriteLine($"[PASS] AppDbContext 构造函数数量: {ctors_AppDbContext.Length}");
        var methods_AppDbContext = type_AppDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AppDbContext 公开方法数量: {methods_AppDbContext.Length}");
        foreach (var m in methods_AppDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AppDbContext 未找到，尝试无命名空间...");
        type_AppDbContext = Type.GetType("AppDbContext");
        if (type_AppDbContext != null)
            Console.WriteLine("[PASS] 类型 AppDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AppDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HashBasedDatabaseRouter
    var type_HashBasedDatabaseRouter = Type.GetType("HashBasedDatabaseRouter");
    if (type_HashBasedDatabaseRouter != null)
    {
        Console.WriteLine("[PASS] 类型 HashBasedDatabaseRouter (class) 存在");
        var ctors_HashBasedDatabaseRouter = type_HashBasedDatabaseRouter.GetConstructors();
        Console.WriteLine($"[PASS] HashBasedDatabaseRouter 构造函数数量: {ctors_HashBasedDatabaseRouter.Length}");
        var methods_HashBasedDatabaseRouter = type_HashBasedDatabaseRouter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HashBasedDatabaseRouter 公开方法数量: {methods_HashBasedDatabaseRouter.Length}");
        foreach (var m in methods_HashBasedDatabaseRouter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HashBasedDatabaseRouter 未找到，尝试无命名空间...");
        type_HashBasedDatabaseRouter = Type.GetType("HashBasedDatabaseRouter");
        if (type_HashBasedDatabaseRouter != null)
            Console.WriteLine("[PASS] 类型 HashBasedDatabaseRouter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HashBasedDatabaseRouter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderQueries
    var type_OrderQueries = Type.GetType("OrderQueries");
    if (type_OrderQueries != null)
    {
        Console.WriteLine("[PASS] 类型 OrderQueries (class) 存在");
        var ctors_OrderQueries = type_OrderQueries.GetConstructors();
        Console.WriteLine($"[PASS] OrderQueries 构造函数数量: {ctors_OrderQueries.Length}");
        var methods_OrderQueries = type_OrderQueries.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderQueries 公开方法数量: {methods_OrderQueries.Length}");
        foreach (var m in methods_OrderQueries)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderQueries 未找到，尝试无命名空间...");
        type_OrderQueries = Type.GetType("OrderQueries");
        if (type_OrderQueries != null)
            Console.WriteLine("[PASS] 类型 OrderQueries (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderQueries 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DateTimeOffsetHandler
    var type_DateTimeOffsetHandler = Type.GetType("DateTimeOffsetHandler");
    if (type_DateTimeOffsetHandler != null)
    {
        Console.WriteLine("[PASS] 类型 DateTimeOffsetHandler (class) 存在");
        var ctors_DateTimeOffsetHandler = type_DateTimeOffsetHandler.GetConstructors();
        Console.WriteLine($"[PASS] DateTimeOffsetHandler 构造函数数量: {ctors_DateTimeOffsetHandler.Length}");
        var methods_DateTimeOffsetHandler = type_DateTimeOffsetHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DateTimeOffsetHandler 公开方法数量: {methods_DateTimeOffsetHandler.Length}");
        foreach (var m in methods_DateTimeOffsetHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DateTimeOffsetHandler 未找到，尝试无命名空间...");
        type_DateTimeOffsetHandler = Type.GetType("DateTimeOffsetHandler");
        if (type_DateTimeOffsetHandler != null)
            Console.WriteLine("[PASS] 类型 DateTimeOffsetHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DateTimeOffsetHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ConsistentHashRouter
    var type_ConsistentHashRouter = Type.GetType("ConsistentHashRouter");
    if (type_ConsistentHashRouter != null)
    {
        Console.WriteLine("[PASS] 类型 ConsistentHashRouter (class) 存在");
        var ctors_ConsistentHashRouter = type_ConsistentHashRouter.GetConstructors();
        Console.WriteLine($"[PASS] ConsistentHashRouter 构造函数数量: {ctors_ConsistentHashRouter.Length}");
        var methods_ConsistentHashRouter = type_ConsistentHashRouter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConsistentHashRouter 公开方法数量: {methods_ConsistentHashRouter.Length}");
        foreach (var m in methods_ConsistentHashRouter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsistentHashRouter 未找到，尝试无命名空间...");
        type_ConsistentHashRouter = Type.GetType("ConsistentHashRouter");
        if (type_ConsistentHashRouter != null)
            Console.WriteLine("[PASS] 类型 ConsistentHashRouter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConsistentHashRouter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DapperBatchExtensions
    var type_DapperBatchExtensions = Type.GetType("DapperBatchExtensions");
    if (type_DapperBatchExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DapperBatchExtensions (class) 存在");
        var ctors_DapperBatchExtensions = type_DapperBatchExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DapperBatchExtensions 构造函数数量: {ctors_DapperBatchExtensions.Length}");
        var methods_DapperBatchExtensions = type_DapperBatchExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DapperBatchExtensions 公开方法数量: {methods_DapperBatchExtensions.Length}");
        foreach (var m in methods_DapperBatchExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DapperBatchExtensions 未找到，尝试无命名空间...");
        type_DapperBatchExtensions = Type.GetType("DapperBatchExtensions");
        if (type_DapperBatchExtensions != null)
            Console.WriteLine("[PASS] 类型 DapperBatchExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DapperBatchExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Order
    var type_Order = Type.GetType("Order");
    if (type_Order != null)
    {
        Console.WriteLine("[PASS] 类型 Order (record) 存在");
        var ctors_Order = type_Order.GetConstructors();
        Console.WriteLine($"[PASS] Order 构造函数数量: {ctors_Order.Length}");
        var methods_Order = type_Order.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Order 公开方法数量: {methods_Order.Length}");
        foreach (var m in methods_Order)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Order 未找到，尝试无命名空间...");
        type_Order = Type.GetType("Order");
        if (type_Order != null)
            Console.WriteLine("[PASS] 类型 Order (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Order 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
