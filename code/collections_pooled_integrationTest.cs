#load "collections_pooled_integration.cs"

Console.WriteLine("=== collections_pooled_integration.cs Test ===");

try
{
    // 验证 class: PooledCollectionsOptions
    var type_PooledCollectionsOptions = Type.GetType("PooledCollectionsOptions");
    if (type_PooledCollectionsOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PooledCollectionsOptions (class) 存在");
        var ctors_PooledCollectionsOptions = type_PooledCollectionsOptions.GetConstructors();
        Console.WriteLine($"[PASS] PooledCollectionsOptions 构造函数数量: {ctors_PooledCollectionsOptions.Length}");
        var methods_PooledCollectionsOptions = type_PooledCollectionsOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PooledCollectionsOptions 公开方法数量: {methods_PooledCollectionsOptions.Length}");
        foreach (var m in methods_PooledCollectionsOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PooledCollectionsOptions 未找到，尝试无命名空间...");
        type_PooledCollectionsOptions = Type.GetType("PooledCollectionsOptions");
        if (type_PooledCollectionsOptions != null)
            Console.WriteLine("[PASS] 类型 PooledCollectionsOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PooledCollectionsOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PooledCollectionsService
    var type_PooledCollectionsService = Type.GetType("PooledCollectionsService");
    if (type_PooledCollectionsService != null)
    {
        Console.WriteLine("[PASS] 类型 PooledCollectionsService (class) 存在");
        var ctors_PooledCollectionsService = type_PooledCollectionsService.GetConstructors();
        Console.WriteLine($"[PASS] PooledCollectionsService 构造函数数量: {ctors_PooledCollectionsService.Length}");
        var methods_PooledCollectionsService = type_PooledCollectionsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PooledCollectionsService 公开方法数量: {methods_PooledCollectionsService.Length}");
        foreach (var m in methods_PooledCollectionsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PooledCollectionsService 未找到，尝试无命名空间...");
        type_PooledCollectionsService = Type.GetType("PooledCollectionsService");
        if (type_PooledCollectionsService != null)
            Console.WriteLine("[PASS] 类型 PooledCollectionsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PooledCollectionsService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PooledCollectionsExtensions
    var type_PooledCollectionsExtensions = Type.GetType("PooledCollectionsExtensions");
    if (type_PooledCollectionsExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PooledCollectionsExtensions (class) 存在");
        var ctors_PooledCollectionsExtensions = type_PooledCollectionsExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PooledCollectionsExtensions 构造函数数量: {ctors_PooledCollectionsExtensions.Length}");
        var methods_PooledCollectionsExtensions = type_PooledCollectionsExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PooledCollectionsExtensions 公开方法数量: {methods_PooledCollectionsExtensions.Length}");
        foreach (var m in methods_PooledCollectionsExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PooledCollectionsExtensions 未找到，尝试无命名空间...");
        type_PooledCollectionsExtensions = Type.GetType("PooledCollectionsExtensions");
        if (type_PooledCollectionsExtensions != null)
            Console.WriteLine("[PASS] 类型 PooledCollectionsExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PooledCollectionsExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PooledCollectionsExample
    var type_PooledCollectionsExample = Type.GetType("PooledCollectionsExample");
    if (type_PooledCollectionsExample != null)
    {
        Console.WriteLine("[PASS] 类型 PooledCollectionsExample (class) 存在");
        var ctors_PooledCollectionsExample = type_PooledCollectionsExample.GetConstructors();
        Console.WriteLine($"[PASS] PooledCollectionsExample 构造函数数量: {ctors_PooledCollectionsExample.Length}");
        var methods_PooledCollectionsExample = type_PooledCollectionsExample.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PooledCollectionsExample 公开方法数量: {methods_PooledCollectionsExample.Length}");
        foreach (var m in methods_PooledCollectionsExample)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PooledCollectionsExample 未找到，尝试无命名空间...");
        type_PooledCollectionsExample = Type.GetType("PooledCollectionsExample");
        if (type_PooledCollectionsExample != null)
            Console.WriteLine("[PASS] 类型 PooledCollectionsExample (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PooledCollectionsExample 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPooledCollectionsService
    var type_IPooledCollectionsService = Type.GetType("IPooledCollectionsService");
    if (type_IPooledCollectionsService != null)
    {
        Console.WriteLine("[PASS] 类型 IPooledCollectionsService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPooledCollectionsService 未找到，尝试无命名空间...");
        type_IPooledCollectionsService = Type.GetType("IPooledCollectionsService");
        if (type_IPooledCollectionsService != null)
            Console.WriteLine("[PASS] 类型 IPooledCollectionsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPooledCollectionsService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
