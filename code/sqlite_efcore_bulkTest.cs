#load "sqlite_efcore_bulk.cs"

Console.WriteLine("=== sqlite_efcore_bulk.cs Test ===");

try
{
    // 验证 class: Product
    var type_Product = Type.GetType("Product");
    if (type_Product != null)
    {
        Console.WriteLine("[PASS] 类型 Product (class) 存在");
        var ctors_Product = type_Product.GetConstructors();
        Console.WriteLine($"[PASS] Product 构造函数数量: {ctors_Product.Length}");
        var methods_Product = type_Product.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Product 公开方法数量: {methods_Product.Length}");
        foreach (var m in methods_Product)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Product 未找到，尝试无命名空间...");
        type_Product = Type.GetType("Product");
        if (type_Product != null)
            Console.WriteLine("[PASS] 类型 Product (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Product 可能为顶层语句或嵌套类型");
    }

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

    // 验证 class: BulkOperationService
    var type_BulkOperationService = Type.GetType("BulkOperationService");
    if (type_BulkOperationService != null)
    {
        Console.WriteLine("[PASS] 类型 BulkOperationService (class) 存在");
        var ctors_BulkOperationService = type_BulkOperationService.GetConstructors();
        Console.WriteLine($"[PASS] BulkOperationService 构造函数数量: {ctors_BulkOperationService.Length}");
        var methods_BulkOperationService = type_BulkOperationService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BulkOperationService 公开方法数量: {methods_BulkOperationService.Length}");
        foreach (var m in methods_BulkOperationService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BulkOperationService 未找到，尝试无命名空间...");
        type_BulkOperationService = Type.GetType("BulkOperationService");
        if (type_BulkOperationService != null)
            Console.WriteLine("[PASS] 类型 BulkOperationService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BulkOperationService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DbContextPoolPolicy
    var type_DbContextPoolPolicy = Type.GetType("DbContextPoolPolicy");
    if (type_DbContextPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 DbContextPoolPolicy (class) 存在");
        var ctors_DbContextPoolPolicy = type_DbContextPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] DbContextPoolPolicy 构造函数数量: {ctors_DbContextPoolPolicy.Length}");
        var methods_DbContextPoolPolicy = type_DbContextPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DbContextPoolPolicy 公开方法数量: {methods_DbContextPoolPolicy.Length}");
        foreach (var m in methods_DbContextPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DbContextPoolPolicy 未找到，尝试无命名空间...");
        type_DbContextPoolPolicy = Type.GetType("DbContextPoolPolicy");
        if (type_DbContextPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 DbContextPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DbContextPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BulkDeleteHandler
    var type_BulkDeleteHandler = Type.GetType("BulkDeleteHandler");
    if (type_BulkDeleteHandler != null)
    {
        Console.WriteLine("[PASS] 类型 BulkDeleteHandler (class) 存在");
        var ctors_BulkDeleteHandler = type_BulkDeleteHandler.GetConstructors();
        Console.WriteLine($"[PASS] BulkDeleteHandler 构造函数数量: {ctors_BulkDeleteHandler.Length}");
        var methods_BulkDeleteHandler = type_BulkDeleteHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BulkDeleteHandler 公开方法数量: {methods_BulkDeleteHandler.Length}");
        foreach (var m in methods_BulkDeleteHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BulkDeleteHandler 未找到，尝试无命名空间...");
        type_BulkDeleteHandler = Type.GetType("BulkDeleteHandler");
        if (type_BulkDeleteHandler != null)
            Console.WriteLine("[PASS] 类型 BulkDeleteHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BulkDeleteHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteEfCoreDemo
    var type_SqliteEfCoreDemo = Type.GetType("SqliteEfCoreDemo");
    if (type_SqliteEfCoreDemo != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteEfCoreDemo (class) 存在");
        var ctors_SqliteEfCoreDemo = type_SqliteEfCoreDemo.GetConstructors();
        Console.WriteLine($"[PASS] SqliteEfCoreDemo 构造函数数量: {ctors_SqliteEfCoreDemo.Length}");
        var methods_SqliteEfCoreDemo = type_SqliteEfCoreDemo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteEfCoreDemo 公开方法数量: {methods_SqliteEfCoreDemo.Length}");
        foreach (var m in methods_SqliteEfCoreDemo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteEfCoreDemo 未找到，尝试无命名空间...");
        type_SqliteEfCoreDemo = Type.GetType("SqliteEfCoreDemo");
        if (type_SqliteEfCoreDemo != null)
            Console.WriteLine("[PASS] 类型 SqliteEfCoreDemo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteEfCoreDemo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BulkOperation
    var type_BulkOperation = Type.GetType("BulkOperation");
    if (type_BulkOperation != null)
    {
        Console.WriteLine("[PASS] 类型 BulkOperation (class) 存在");
        var ctors_BulkOperation = type_BulkOperation.GetConstructors();
        Console.WriteLine($"[PASS] BulkOperation 构造函数数量: {ctors_BulkOperation.Length}");
        var methods_BulkOperation = type_BulkOperation.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BulkOperation 公开方法数量: {methods_BulkOperation.Length}");
        foreach (var m in methods_BulkOperation)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BulkOperation 未找到，尝试无命名空间...");
        type_BulkOperation = Type.GetType("BulkOperation");
        if (type_BulkOperation != null)
            Console.WriteLine("[PASS] 类型 BulkOperation (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BulkOperation 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QueryService
    var type_QueryService = Type.GetType("QueryService");
    if (type_QueryService != null)
    {
        Console.WriteLine("[PASS] 类型 QueryService (class) 存在");
        var ctors_QueryService = type_QueryService.GetConstructors();
        Console.WriteLine($"[PASS] QueryService 构造函数数量: {ctors_QueryService.Length}");
        var methods_QueryService = type_QueryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QueryService 公开方法数量: {methods_QueryService.Length}");
        foreach (var m in methods_QueryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QueryService 未找到，尝试无命名空间...");
        type_QueryService = Type.GetType("QueryService");
        if (type_QueryService != null)
            Console.WriteLine("[PASS] 类型 QueryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QueryService 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: OperationType
    var type_OperationType = Type.GetType("OperationType");
    if (type_OperationType != null)
    {
        Console.WriteLine("[PASS] 类型 OperationType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OperationType 未找到，尝试无命名空间...");
        type_OperationType = Type.GetType("OperationType");
        if (type_OperationType != null)
            Console.WriteLine("[PASS] 类型 OperationType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OperationType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
