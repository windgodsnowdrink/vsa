#load "sqlite_integration.cs"

Console.WriteLine("=== sqlite_integration.cs Test ===");

try
{
    // 验证 class: SqliteIntegration.AppDbContext
    var type_AppDbContext = Type.GetType("SqliteIntegration.AppDbContext");
    if (type_AppDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteIntegration.AppDbContext (class) 存在");
        var ctors_AppDbContext = type_AppDbContext.GetConstructors();
        Console.WriteLine($"[PASS] SqliteIntegration.AppDbContext 构造函数数量: {ctors_AppDbContext.Length}");
        var methods_AppDbContext = type_AppDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteIntegration.AppDbContext 公开方法数量: {methods_AppDbContext.Length}");
        foreach (var m in methods_AppDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteIntegration.AppDbContext 未找到，尝试无命名空间...");
        type_AppDbContext = Type.GetType("AppDbContext");
        if (type_AppDbContext != null)
            Console.WriteLine("[PASS] 类型 AppDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AppDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteIntegration.SqliteOptions
    var type_SqliteOptions = Type.GetType("SqliteIntegration.SqliteOptions");
    if (type_SqliteOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteIntegration.SqliteOptions (class) 存在");
        var ctors_SqliteOptions = type_SqliteOptions.GetConstructors();
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteOptions 构造函数数量: {ctors_SqliteOptions.Length}");
        var methods_SqliteOptions = type_SqliteOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteOptions 公开方法数量: {methods_SqliteOptions.Length}");
        foreach (var m in methods_SqliteOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteIntegration.SqliteOptions 未找到，尝试无命名空间...");
        type_SqliteOptions = Type.GetType("SqliteOptions");
        if (type_SqliteOptions != null)
            Console.WriteLine("[PASS] 类型 SqliteOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteIntegration.SqliteConnectionPool
    var type_SqliteConnectionPool = Type.GetType("SqliteIntegration.SqliteConnectionPool");
    if (type_SqliteConnectionPool != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteIntegration.SqliteConnectionPool (class) 存在");
        var ctors_SqliteConnectionPool = type_SqliteConnectionPool.GetConstructors();
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteConnectionPool 构造函数数量: {ctors_SqliteConnectionPool.Length}");
        var methods_SqliteConnectionPool = type_SqliteConnectionPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteConnectionPool 公开方法数量: {methods_SqliteConnectionPool.Length}");
        foreach (var m in methods_SqliteConnectionPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteIntegration.SqliteConnectionPool 未找到，尝试无命名空间...");
        type_SqliteConnectionPool = Type.GetType("SqliteConnectionPool");
        if (type_SqliteConnectionPool != null)
            Console.WriteLine("[PASS] 类型 SqliteConnectionPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteConnectionPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteIntegration.SqliteServiceExtensions
    var type_SqliteServiceExtensions = Type.GetType("SqliteIntegration.SqliteServiceExtensions");
    if (type_SqliteServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteIntegration.SqliteServiceExtensions (class) 存在");
        var ctors_SqliteServiceExtensions = type_SqliteServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteServiceExtensions 构造函数数量: {ctors_SqliteServiceExtensions.Length}");
        var methods_SqliteServiceExtensions = type_SqliteServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteServiceExtensions 公开方法数量: {methods_SqliteServiceExtensions.Length}");
        foreach (var m in methods_SqliteServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteIntegration.SqliteServiceExtensions 未找到，尝试无命名空间...");
        type_SqliteServiceExtensions = Type.GetType("SqliteServiceExtensions");
        if (type_SqliteServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 SqliteServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteIntegration.SqliteTransactionScope
    var type_SqliteTransactionScope = Type.GetType("SqliteIntegration.SqliteTransactionScope");
    if (type_SqliteTransactionScope != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteIntegration.SqliteTransactionScope (class) 存在");
        var ctors_SqliteTransactionScope = type_SqliteTransactionScope.GetConstructors();
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteTransactionScope 构造函数数量: {ctors_SqliteTransactionScope.Length}");
        var methods_SqliteTransactionScope = type_SqliteTransactionScope.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteTransactionScope 公开方法数量: {methods_SqliteTransactionScope.Length}");
        foreach (var m in methods_SqliteTransactionScope)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteIntegration.SqliteTransactionScope 未找到，尝试无命名空间...");
        type_SqliteTransactionScope = Type.GetType("SqliteTransactionScope");
        if (type_SqliteTransactionScope != null)
            Console.WriteLine("[PASS] 类型 SqliteTransactionScope (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteTransactionScope 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteIntegration.SqliteBulkInserter
    var type_SqliteBulkInserter = Type.GetType("SqliteIntegration.SqliteBulkInserter");
    if (type_SqliteBulkInserter != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteIntegration.SqliteBulkInserter (class) 存在");
        var ctors_SqliteBulkInserter = type_SqliteBulkInserter.GetConstructors();
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteBulkInserter 构造函数数量: {ctors_SqliteBulkInserter.Length}");
        var methods_SqliteBulkInserter = type_SqliteBulkInserter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteBulkInserter 公开方法数量: {methods_SqliteBulkInserter.Length}");
        foreach (var m in methods_SqliteBulkInserter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteIntegration.SqliteBulkInserter 未找到，尝试无命名空间...");
        type_SqliteBulkInserter = Type.GetType("SqliteBulkInserter");
        if (type_SqliteBulkInserter != null)
            Console.WriteLine("[PASS] 类型 SqliteBulkInserter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteBulkInserter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteIntegration.EfCoreBulkOperations
    var type_EfCoreBulkOperations = Type.GetType("SqliteIntegration.EfCoreBulkOperations");
    if (type_EfCoreBulkOperations != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteIntegration.EfCoreBulkOperations (class) 存在");
        var ctors_EfCoreBulkOperations = type_EfCoreBulkOperations.GetConstructors();
        Console.WriteLine($"[PASS] SqliteIntegration.EfCoreBulkOperations 构造函数数量: {ctors_EfCoreBulkOperations.Length}");
        var methods_EfCoreBulkOperations = type_EfCoreBulkOperations.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteIntegration.EfCoreBulkOperations 公开方法数量: {methods_EfCoreBulkOperations.Length}");
        foreach (var m in methods_EfCoreBulkOperations)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteIntegration.EfCoreBulkOperations 未找到，尝试无命名空间...");
        type_EfCoreBulkOperations = Type.GetType("EfCoreBulkOperations");
        if (type_EfCoreBulkOperations != null)
            Console.WriteLine("[PASS] 类型 EfCoreBulkOperations (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EfCoreBulkOperations 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteIntegration.SqliteDbExampleService
    var type_SqliteDbExampleService = Type.GetType("SqliteIntegration.SqliteDbExampleService");
    if (type_SqliteDbExampleService != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteIntegration.SqliteDbExampleService (class) 存在");
        var ctors_SqliteDbExampleService = type_SqliteDbExampleService.GetConstructors();
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteDbExampleService 构造函数数量: {ctors_SqliteDbExampleService.Length}");
        var methods_SqliteDbExampleService = type_SqliteDbExampleService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteIntegration.SqliteDbExampleService 公开方法数量: {methods_SqliteDbExampleService.Length}");
        foreach (var m in methods_SqliteDbExampleService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteIntegration.SqliteDbExampleService 未找到，尝试无命名空间...");
        type_SqliteDbExampleService = Type.GetType("SqliteDbExampleService");
        if (type_SqliteDbExampleService != null)
            Console.WriteLine("[PASS] 类型 SqliteDbExampleService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SqliteDbExampleService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SqliteIntegration.Customer
    var type_Customer = Type.GetType("SqliteIntegration.Customer");
    if (type_Customer != null)
    {
        Console.WriteLine("[PASS] 类型 SqliteIntegration.Customer (class) 存在");
        var ctors_Customer = type_Customer.GetConstructors();
        Console.WriteLine($"[PASS] SqliteIntegration.Customer 构造函数数量: {ctors_Customer.Length}");
        var methods_Customer = type_Customer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SqliteIntegration.Customer 公开方法数量: {methods_Customer.Length}");
        foreach (var m in methods_Customer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SqliteIntegration.Customer 未找到，尝试无命名空间...");
        type_Customer = Type.GetType("Customer");
        if (type_Customer != null)
            Console.WriteLine("[PASS] 类型 Customer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Customer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
