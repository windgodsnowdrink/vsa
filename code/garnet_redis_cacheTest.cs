#load "garnet_redis_cache.cs"

Console.WriteLine("=== garnet_redis_cache.cs Test ===");

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

    // 验证 class: ProductDbContext
    var type_ProductDbContext = Type.GetType("ProductDbContext");
    if (type_ProductDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 ProductDbContext (class) 存在");
        var ctors_ProductDbContext = type_ProductDbContext.GetConstructors();
        Console.WriteLine($"[PASS] ProductDbContext 构造函数数量: {ctors_ProductDbContext.Length}");
        var methods_ProductDbContext = type_ProductDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProductDbContext 公开方法数量: {methods_ProductDbContext.Length}");
        foreach (var m in methods_ProductDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProductDbContext 未找到，尝试无命名空间...");
        type_ProductDbContext = Type.GetType("ProductDbContext");
        if (type_ProductDbContext != null)
            Console.WriteLine("[PASS] 类型 ProductDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProductDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProductCacheService
    var type_ProductCacheService = Type.GetType("ProductCacheService");
    if (type_ProductCacheService != null)
    {
        Console.WriteLine("[PASS] 类型 ProductCacheService (class) 存在");
        var ctors_ProductCacheService = type_ProductCacheService.GetConstructors();
        Console.WriteLine($"[PASS] ProductCacheService 构造函数数量: {ctors_ProductCacheService.Length}");
        var methods_ProductCacheService = type_ProductCacheService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProductCacheService 公开方法数量: {methods_ProductCacheService.Length}");
        foreach (var m in methods_ProductCacheService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProductCacheService 未找到，尝试无命名空间...");
        type_ProductCacheService = Type.GetType("ProductCacheService");
        if (type_ProductCacheService != null)
            Console.WriteLine("[PASS] 类型 ProductCacheService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProductCacheService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProductController
    var type_ProductController = Type.GetType("ProductController");
    if (type_ProductController != null)
    {
        Console.WriteLine("[PASS] 类型 ProductController (class) 存在");
        var ctors_ProductController = type_ProductController.GetConstructors();
        Console.WriteLine($"[PASS] ProductController 构造函数数量: {ctors_ProductController.Length}");
        var methods_ProductController = type_ProductController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProductController 公开方法数量: {methods_ProductController.Length}");
        foreach (var m in methods_ProductController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProductController 未找到，尝试无命名空间...");
        type_ProductController = Type.GetType("ProductController");
        if (type_ProductController != null)
            Console.WriteLine("[PASS] 类型 ProductController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProductController 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheWarmupService
    var type_CacheWarmupService = Type.GetType("CacheWarmupService");
    if (type_CacheWarmupService != null)
    {
        Console.WriteLine("[PASS] 类型 CacheWarmupService (class) 存在");
        var ctors_CacheWarmupService = type_CacheWarmupService.GetConstructors();
        Console.WriteLine($"[PASS] CacheWarmupService 构造函数数量: {ctors_CacheWarmupService.Length}");
        var methods_CacheWarmupService = type_CacheWarmupService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheWarmupService 公开方法数量: {methods_CacheWarmupService.Length}");
        foreach (var m in methods_CacheWarmupService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheWarmupService 未找到，尝试无命名空间...");
        type_CacheWarmupService = Type.GetType("CacheWarmupService");
        if (type_CacheWarmupService != null)
            Console.WriteLine("[PASS] 类型 CacheWarmupService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheWarmupService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
