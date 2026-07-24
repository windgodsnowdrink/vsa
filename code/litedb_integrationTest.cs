#load "litedb_integration.cs"

Console.WriteLine("=== litedb_integration.cs Test ===");

try
{
    // 验证 class: LiteDbOptions
    var type_LiteDbOptions = Type.GetType("LiteDbOptions");
    if (type_LiteDbOptions != null)
    {
        Console.WriteLine("[PASS] 类型 LiteDbOptions (class) 存在");
        var ctors_LiteDbOptions = type_LiteDbOptions.GetConstructors();
        Console.WriteLine($"[PASS] LiteDbOptions 构造函数数量: {ctors_LiteDbOptions.Length}");
        var methods_LiteDbOptions = type_LiteDbOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiteDbOptions 公开方法数量: {methods_LiteDbOptions.Length}");
        foreach (var m in methods_LiteDbOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiteDbOptions 未找到，尝试无命名空间...");
        type_LiteDbOptions = Type.GetType("LiteDbOptions");
        if (type_LiteDbOptions != null)
            Console.WriteLine("[PASS] 类型 LiteDbOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiteDbConnectionPool
    var type_LiteDbConnectionPool = Type.GetType("LiteDbConnectionPool");
    if (type_LiteDbConnectionPool != null)
    {
        Console.WriteLine("[PASS] 类型 LiteDbConnectionPool (class) 存在");
        var ctors_LiteDbConnectionPool = type_LiteDbConnectionPool.GetConstructors();
        Console.WriteLine($"[PASS] LiteDbConnectionPool 构造函数数量: {ctors_LiteDbConnectionPool.Length}");
        var methods_LiteDbConnectionPool = type_LiteDbConnectionPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiteDbConnectionPool 公开方法数量: {methods_LiteDbConnectionPool.Length}");
        foreach (var m in methods_LiteDbConnectionPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiteDbConnectionPool 未找到，尝试无命名空间...");
        type_LiteDbConnectionPool = Type.GetType("LiteDbConnectionPool");
        if (type_LiteDbConnectionPool != null)
            Console.WriteLine("[PASS] 类型 LiteDbConnectionPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbConnectionPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiteDbServiceCollectionExtensions
    var type_LiteDbServiceCollectionExtensions = Type.GetType("LiteDbServiceCollectionExtensions");
    if (type_LiteDbServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 LiteDbServiceCollectionExtensions (class) 存在");
        var ctors_LiteDbServiceCollectionExtensions = type_LiteDbServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] LiteDbServiceCollectionExtensions 构造函数数量: {ctors_LiteDbServiceCollectionExtensions.Length}");
        var methods_LiteDbServiceCollectionExtensions = type_LiteDbServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiteDbServiceCollectionExtensions 公开方法数量: {methods_LiteDbServiceCollectionExtensions.Length}");
        foreach (var m in methods_LiteDbServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiteDbServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_LiteDbServiceCollectionExtensions = Type.GetType("LiteDbServiceCollectionExtensions");
        if (type_LiteDbServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 LiteDbServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiteDbExampleService
    var type_LiteDbExampleService = Type.GetType("LiteDbExampleService");
    if (type_LiteDbExampleService != null)
    {
        Console.WriteLine("[PASS] 类型 LiteDbExampleService (class) 存在");
        var ctors_LiteDbExampleService = type_LiteDbExampleService.GetConstructors();
        Console.WriteLine($"[PASS] LiteDbExampleService 构造函数数量: {ctors_LiteDbExampleService.Length}");
        var methods_LiteDbExampleService = type_LiteDbExampleService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiteDbExampleService 公开方法数量: {methods_LiteDbExampleService.Length}");
        foreach (var m in methods_LiteDbExampleService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiteDbExampleService 未找到，尝试无命名空间...");
        type_LiteDbExampleService = Type.GetType("LiteDbExampleService");
        if (type_LiteDbExampleService != null)
            Console.WriteLine("[PASS] 类型 LiteDbExampleService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbExampleService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ExampleEntity
    var type_ExampleEntity = Type.GetType("ExampleEntity");
    if (type_ExampleEntity != null)
    {
        Console.WriteLine("[PASS] 类型 ExampleEntity (class) 存在");
        var ctors_ExampleEntity = type_ExampleEntity.GetConstructors();
        Console.WriteLine($"[PASS] ExampleEntity 构造函数数量: {ctors_ExampleEntity.Length}");
        var methods_ExampleEntity = type_ExampleEntity.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExampleEntity 公开方法数量: {methods_ExampleEntity.Length}");
        foreach (var m in methods_ExampleEntity)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExampleEntity 未找到，尝试无命名空间...");
        type_ExampleEntity = Type.GetType("ExampleEntity");
        if (type_ExampleEntity != null)
            Console.WriteLine("[PASS] 类型 ExampleEntity (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExampleEntity 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiteDbContext
    var type_LiteDbContext = Type.GetType("LiteDbContext");
    if (type_LiteDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 LiteDbContext (class) 存在");
        var ctors_LiteDbContext = type_LiteDbContext.GetConstructors();
        Console.WriteLine($"[PASS] LiteDbContext 构造函数数量: {ctors_LiteDbContext.Length}");
        var methods_LiteDbContext = type_LiteDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiteDbContext 公开方法数量: {methods_LiteDbContext.Length}");
        foreach (var m in methods_LiteDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiteDbContext 未找到，尝试无命名空间...");
        type_LiteDbContext = Type.GetType("LiteDbContext");
        if (type_LiteDbContext != null)
            Console.WriteLine("[PASS] 类型 LiteDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiteDbBulkOperations
    var type_LiteDbBulkOperations = Type.GetType("LiteDbBulkOperations");
    if (type_LiteDbBulkOperations != null)
    {
        Console.WriteLine("[PASS] 类型 LiteDbBulkOperations (class) 存在");
        var ctors_LiteDbBulkOperations = type_LiteDbBulkOperations.GetConstructors();
        Console.WriteLine($"[PASS] LiteDbBulkOperations 构造函数数量: {ctors_LiteDbBulkOperations.Length}");
        var methods_LiteDbBulkOperations = type_LiteDbBulkOperations.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiteDbBulkOperations 公开方法数量: {methods_LiteDbBulkOperations.Length}");
        foreach (var m in methods_LiteDbBulkOperations)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiteDbBulkOperations 未找到，尝试无命名空间...");
        type_LiteDbBulkOperations = Type.GetType("LiteDbBulkOperations");
        if (type_LiteDbBulkOperations != null)
            Console.WriteLine("[PASS] 类型 LiteDbBulkOperations (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbBulkOperations 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ILiteDbConnectionPool
    var type_ILiteDbConnectionPool = Type.GetType("ILiteDbConnectionPool");
    if (type_ILiteDbConnectionPool != null)
    {
        Console.WriteLine("[PASS] 类型 ILiteDbConnectionPool (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ILiteDbConnectionPool 未找到，尝试无命名空间...");
        type_ILiteDbConnectionPool = Type.GetType("ILiteDbConnectionPool");
        if (type_ILiteDbConnectionPool != null)
            Console.WriteLine("[PASS] 类型 ILiteDbConnectionPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ILiteDbConnectionPool 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
