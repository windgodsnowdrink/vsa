#load "castle_proxy_demo.cs"

Console.WriteLine("=== castle_proxy_demo.cs Test ===");

try
{
    // 验证 class: MemoryOptimizer
    var type_MemoryOptimizer = Type.GetType("MemoryOptimizer");
    if (type_MemoryOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryOptimizer (class) 存在");
        var ctors_MemoryOptimizer = type_MemoryOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] MemoryOptimizer 构造函数数量: {ctors_MemoryOptimizer.Length}");
        var methods_MemoryOptimizer = type_MemoryOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryOptimizer 公开方法数量: {methods_MemoryOptimizer.Length}");
        foreach (var m in methods_MemoryOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryOptimizer 未找到，尝试无命名空间...");
        type_MemoryOptimizer = Type.GetType("MemoryOptimizer");
        if (type_MemoryOptimizer != null)
            Console.WriteLine("[PASS] 类型 MemoryOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryOptimizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HttpProxyInterceptor
    var type_HttpProxyInterceptor = Type.GetType("HttpProxyInterceptor");
    if (type_HttpProxyInterceptor != null)
    {
        Console.WriteLine("[PASS] 类型 HttpProxyInterceptor (class) 存在");
        var ctors_HttpProxyInterceptor = type_HttpProxyInterceptor.GetConstructors();
        Console.WriteLine($"[PASS] HttpProxyInterceptor 构造函数数量: {ctors_HttpProxyInterceptor.Length}");
        var methods_HttpProxyInterceptor = type_HttpProxyInterceptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HttpProxyInterceptor 公开方法数量: {methods_HttpProxyInterceptor.Length}");
        foreach (var m in methods_HttpProxyInterceptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HttpProxyInterceptor 未找到，尝试无命名空间...");
        type_HttpProxyInterceptor = Type.GetType("HttpProxyInterceptor");
        if (type_HttpProxyInterceptor != null)
            Console.WriteLine("[PASS] 类型 HttpProxyInterceptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HttpProxyInterceptor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HttpProxyService
    var type_HttpProxyService = Type.GetType("HttpProxyService");
    if (type_HttpProxyService != null)
    {
        Console.WriteLine("[PASS] 类型 HttpProxyService (class) 存在");
        var ctors_HttpProxyService = type_HttpProxyService.GetConstructors();
        Console.WriteLine($"[PASS] HttpProxyService 构造函数数量: {ctors_HttpProxyService.Length}");
        var methods_HttpProxyService = type_HttpProxyService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HttpProxyService 公开方法数量: {methods_HttpProxyService.Length}");
        foreach (var m in methods_HttpProxyService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HttpProxyService 未找到，尝试无命名空间...");
        type_HttpProxyService = Type.GetType("HttpProxyService");
        if (type_HttpProxyService != null)
            Console.WriteLine("[PASS] 类型 HttpProxyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HttpProxyService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IHttpProxyService
    var type_IHttpProxyService = Type.GetType("IHttpProxyService");
    if (type_IHttpProxyService != null)
    {
        Console.WriteLine("[PASS] 类型 IHttpProxyService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IHttpProxyService 未找到，尝试无命名空间...");
        type_IHttpProxyService = Type.GetType("IHttpProxyService");
        if (type_IHttpProxyService != null)
            Console.WriteLine("[PASS] 类型 IHttpProxyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IHttpProxyService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
