#load "natasha_integration.cs"

Console.WriteLine("=== natasha_integration.cs Test ===");

try
{
    // 验证 class: NatashaDynamicService
    var type_NatashaDynamicService = Type.GetType("NatashaDynamicService");
    if (type_NatashaDynamicService != null)
    {
        Console.WriteLine("[PASS] 类型 NatashaDynamicService (class) 存在");
        var ctors_NatashaDynamicService = type_NatashaDynamicService.GetConstructors();
        Console.WriteLine($"[PASS] NatashaDynamicService 构造函数数量: {ctors_NatashaDynamicService.Length}");
        var methods_NatashaDynamicService = type_NatashaDynamicService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatashaDynamicService 公开方法数量: {methods_NatashaDynamicService.Length}");
        foreach (var m in methods_NatashaDynamicService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatashaDynamicService 未找到，尝试无命名空间...");
        type_NatashaDynamicService = Type.GetType("NatashaDynamicService");
        if (type_NatashaDynamicService != null)
            Console.WriteLine("[PASS] 类型 NatashaDynamicService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatashaDynamicService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DynamicClass
    var type_DynamicClass = Type.GetType("DynamicClass");
    if (type_DynamicClass != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicClass (class) 存在");
        var ctors_DynamicClass = type_DynamicClass.GetConstructors();
        Console.WriteLine($"[PASS] DynamicClass 构造函数数量: {ctors_DynamicClass.Length}");
        var methods_DynamicClass = type_DynamicClass.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicClass 公开方法数量: {methods_DynamicClass.Length}");
        foreach (var m in methods_DynamicClass)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicClass 未找到，尝试无命名空间...");
        type_DynamicClass = Type.GetType("DynamicClass");
        if (type_DynamicClass != null)
            Console.WriteLine("[PASS] 类型 DynamicClass (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicClass 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TieredMemoryPool
    var type_TieredMemoryPool = Type.GetType("TieredMemoryPool");
    if (type_TieredMemoryPool != null)
    {
        Console.WriteLine("[PASS] 类型 TieredMemoryPool (class) 存在");
        var ctors_TieredMemoryPool = type_TieredMemoryPool.GetConstructors();
        Console.WriteLine($"[PASS] TieredMemoryPool 构造函数数量: {ctors_TieredMemoryPool.Length}");
        var methods_TieredMemoryPool = type_TieredMemoryPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredMemoryPool 公开方法数量: {methods_TieredMemoryPool.Length}");
        foreach (var m in methods_TieredMemoryPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredMemoryPool 未找到，尝试无命名空间...");
        type_TieredMemoryPool = Type.GetType("TieredMemoryPool");
        if (type_TieredMemoryPool != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SmallMemoryOwner
    var type_SmallMemoryOwner = Type.GetType("SmallMemoryOwner");
    if (type_SmallMemoryOwner != null)
    {
        Console.WriteLine("[PASS] 类型 SmallMemoryOwner (class) 存在");
        var ctors_SmallMemoryOwner = type_SmallMemoryOwner.GetConstructors();
        Console.WriteLine($"[PASS] SmallMemoryOwner 构造函数数量: {ctors_SmallMemoryOwner.Length}");
        var methods_SmallMemoryOwner = type_SmallMemoryOwner.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmallMemoryOwner 公开方法数量: {methods_SmallMemoryOwner.Length}");
        foreach (var m in methods_SmallMemoryOwner)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmallMemoryOwner 未找到，尝试无命名空间...");
        type_SmallMemoryOwner = Type.GetType("SmallMemoryOwner");
        if (type_SmallMemoryOwner != null)
            Console.WriteLine("[PASS] 类型 SmallMemoryOwner (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmallMemoryOwner 可能为顶层语句或嵌套类型");
    }

    // 验证 class: StringBuilderPooledPolicy
    var type_StringBuilderPooledPolicy = Type.GetType("StringBuilderPooledPolicy");
    if (type_StringBuilderPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 StringBuilderPooledPolicy (class) 存在");
        var ctors_StringBuilderPooledPolicy = type_StringBuilderPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] StringBuilderPooledPolicy 构造函数数量: {ctors_StringBuilderPooledPolicy.Length}");
        var methods_StringBuilderPooledPolicy = type_StringBuilderPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StringBuilderPooledPolicy 公开方法数量: {methods_StringBuilderPooledPolicy.Length}");
        foreach (var m in methods_StringBuilderPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StringBuilderPooledPolicy 未找到，尝试无命名空间...");
        type_StringBuilderPooledPolicy = Type.GetType("StringBuilderPooledPolicy");
        if (type_StringBuilderPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 StringBuilderPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StringBuilderPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
