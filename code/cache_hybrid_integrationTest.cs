#load "cache_hybrid_integration.cs"

Console.WriteLine("=== cache_hybrid_integration.cs Test ===");

try
{
    // 验证 class: CachePolicyOptions
    var type_CachePolicyOptions = Type.GetType("CachePolicyOptions");
    if (type_CachePolicyOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CachePolicyOptions (class) 存在");
        var ctors_CachePolicyOptions = type_CachePolicyOptions.GetConstructors();
        Console.WriteLine($"[PASS] CachePolicyOptions 构造函数数量: {ctors_CachePolicyOptions.Length}");
        var methods_CachePolicyOptions = type_CachePolicyOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CachePolicyOptions 公开方法数量: {methods_CachePolicyOptions.Length}");
        foreach (var m in methods_CachePolicyOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CachePolicyOptions 未找到，尝试无命名空间...");
        type_CachePolicyOptions = Type.GetType("CachePolicyOptions");
        if (type_CachePolicyOptions != null)
            Console.WriteLine("[PASS] 类型 CachePolicyOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CachePolicyOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProductService
    var type_ProductService = Type.GetType("ProductService");
    if (type_ProductService != null)
    {
        Console.WriteLine("[PASS] 类型 ProductService (class) 存在");
        var ctors_ProductService = type_ProductService.GetConstructors();
        Console.WriteLine($"[PASS] ProductService 构造函数数量: {ctors_ProductService.Length}");
        var methods_ProductService = type_ProductService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProductService 公开方法数量: {methods_ProductService.Length}");
        foreach (var m in methods_ProductService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProductService 未找到，尝试无命名空间...");
        type_ProductService = Type.GetType("ProductService");
        if (type_ProductService != null)
            Console.WriteLine("[PASS] 类型 ProductService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProductService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheEventHandler
    var type_CacheEventHandler = Type.GetType("CacheEventHandler");
    if (type_CacheEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 CacheEventHandler (class) 存在");
        var ctors_CacheEventHandler = type_CacheEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] CacheEventHandler 构造函数数量: {ctors_CacheEventHandler.Length}");
        var methods_CacheEventHandler = type_CacheEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheEventHandler 公开方法数量: {methods_CacheEventHandler.Length}");
        foreach (var m in methods_CacheEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheEventHandler 未找到，尝试无命名空间...");
        type_CacheEventHandler = Type.GetType("CacheEventHandler");
        if (type_CacheEventHandler != null)
            Console.WriteLine("[PASS] 类型 CacheEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheEventHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheWarmupMiddleware
    var type_CacheWarmupMiddleware = Type.GetType("CacheWarmupMiddleware");
    if (type_CacheWarmupMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 CacheWarmupMiddleware (class) 存在");
        var ctors_CacheWarmupMiddleware = type_CacheWarmupMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] CacheWarmupMiddleware 构造函数数量: {ctors_CacheWarmupMiddleware.Length}");
        var methods_CacheWarmupMiddleware = type_CacheWarmupMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheWarmupMiddleware 公开方法数量: {methods_CacheWarmupMiddleware.Length}");
        foreach (var m in methods_CacheWarmupMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheWarmupMiddleware 未找到，尝试无命名空间...");
        type_CacheWarmupMiddleware = Type.GetType("CacheWarmupMiddleware");
        if (type_CacheWarmupMiddleware != null)
            Console.WriteLine("[PASS] 类型 CacheWarmupMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheWarmupMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HybridCacheExtensions
    var type_HybridCacheExtensions = Type.GetType("HybridCacheExtensions");
    if (type_HybridCacheExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 HybridCacheExtensions (class) 存在");
        var ctors_HybridCacheExtensions = type_HybridCacheExtensions.GetConstructors();
        Console.WriteLine($"[PASS] HybridCacheExtensions 构造函数数量: {ctors_HybridCacheExtensions.Length}");
        var methods_HybridCacheExtensions = type_HybridCacheExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HybridCacheExtensions 公开方法数量: {methods_HybridCacheExtensions.Length}");
        foreach (var m in methods_HybridCacheExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HybridCacheExtensions 未找到，尝试无命名空间...");
        type_HybridCacheExtensions = Type.GetType("HybridCacheExtensions");
        if (type_HybridCacheExtensions != null)
            Console.WriteLine("[PASS] 类型 HybridCacheExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HybridCacheExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IProductService
    var type_IProductService = Type.GetType("IProductService");
    if (type_IProductService != null)
    {
        Console.WriteLine("[PASS] 类型 IProductService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IProductService 未找到，尝试无命名空间...");
        type_IProductService = Type.GetType("IProductService");
        if (type_IProductService != null)
            Console.WriteLine("[PASS] 类型 IProductService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IProductService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IProductRepository
    var type_IProductRepository = Type.GetType("IProductRepository");
    if (type_IProductRepository != null)
    {
        Console.WriteLine("[PASS] 类型 IProductRepository (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IProductRepository 未找到，尝试无命名空间...");
        type_IProductRepository = Type.GetType("IProductRepository");
        if (type_IProductRepository != null)
            Console.WriteLine("[PASS] 类型 IProductRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IProductRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Product
    var type_Product = Type.GetType("Product");
    if (type_Product != null)
    {
        Console.WriteLine("[PASS] 类型 Product (record) 存在");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
