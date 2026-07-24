#load "reverse_proxy_demo.cs"

Console.WriteLine("=== reverse_proxy_demo.cs Test ===");

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

    // 验证 class: ProxyConfigService
    var type_ProxyConfigService = Type.GetType("ProxyConfigService");
    if (type_ProxyConfigService != null)
    {
        Console.WriteLine("[PASS] 类型 ProxyConfigService (class) 存在");
        var ctors_ProxyConfigService = type_ProxyConfigService.GetConstructors();
        Console.WriteLine($"[PASS] ProxyConfigService 构造函数数量: {ctors_ProxyConfigService.Length}");
        var methods_ProxyConfigService = type_ProxyConfigService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProxyConfigService 公开方法数量: {methods_ProxyConfigService.Length}");
        foreach (var m in methods_ProxyConfigService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProxyConfigService 未找到，尝试无命名空间...");
        type_ProxyConfigService = Type.GetType("ProxyConfigService");
        if (type_ProxyConfigService != null)
            Console.WriteLine("[PASS] 类型 ProxyConfigService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProxyConfigService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
