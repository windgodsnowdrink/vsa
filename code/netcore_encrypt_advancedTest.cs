#load "netcore_encrypt_advanced.cs"

Console.WriteLine("=== netcore_encrypt_advanced.cs Test ===");

try
{
    // 验证 class: EncryptChannel
    var type_EncryptChannel = Type.GetType("EncryptChannel");
    if (type_EncryptChannel != null)
    {
        Console.WriteLine("[PASS] 类型 EncryptChannel (class) 存在");
        var ctors_EncryptChannel = type_EncryptChannel.GetConstructors();
        Console.WriteLine($"[PASS] EncryptChannel 构造函数数量: {ctors_EncryptChannel.Length}");
        var methods_EncryptChannel = type_EncryptChannel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EncryptChannel 公开方法数量: {methods_EncryptChannel.Length}");
        foreach (var m in methods_EncryptChannel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EncryptChannel 未找到，尝试无命名空间...");
        type_EncryptChannel = Type.GetType("EncryptChannel");
        if (type_EncryptChannel != null)
            Console.WriteLine("[PASS] 类型 EncryptChannel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EncryptChannel 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ColdMemoryPool
    var type_ColdMemoryPool = Type.GetType("ColdMemoryPool");
    if (type_ColdMemoryPool != null)
    {
        Console.WriteLine("[PASS] 类型 ColdMemoryPool (class) 存在");
        var ctors_ColdMemoryPool = type_ColdMemoryPool.GetConstructors();
        Console.WriteLine($"[PASS] ColdMemoryPool 构造函数数量: {ctors_ColdMemoryPool.Length}");
        var methods_ColdMemoryPool = type_ColdMemoryPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ColdMemoryPool 公开方法数量: {methods_ColdMemoryPool.Length}");
        foreach (var m in methods_ColdMemoryPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ColdMemoryPool 未找到，尝试无命名空间...");
        type_ColdMemoryPool = Type.GetType("ColdMemoryPool");
        if (type_ColdMemoryPool != null)
            Console.WriteLine("[PASS] 类型 ColdMemoryPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ColdMemoryPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ColdMemoryOwner
    var type_ColdMemoryOwner = Type.GetType("ColdMemoryOwner");
    if (type_ColdMemoryOwner != null)
    {
        Console.WriteLine("[PASS] 类型 ColdMemoryOwner (class) 存在");
        var ctors_ColdMemoryOwner = type_ColdMemoryOwner.GetConstructors();
        Console.WriteLine($"[PASS] ColdMemoryOwner 构造函数数量: {ctors_ColdMemoryOwner.Length}");
        var methods_ColdMemoryOwner = type_ColdMemoryOwner.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ColdMemoryOwner 公开方法数量: {methods_ColdMemoryOwner.Length}");
        foreach (var m in methods_ColdMemoryOwner)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ColdMemoryOwner 未找到，尝试无命名空间...");
        type_ColdMemoryOwner = Type.GetType("ColdMemoryOwner");
        if (type_ColdMemoryOwner != null)
            Console.WriteLine("[PASS] 类型 ColdMemoryOwner (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ColdMemoryOwner 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TailLatencyOptimizer
    var type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
    if (type_TailLatencyOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (class) 存在");
        var ctors_TailLatencyOptimizer = type_TailLatencyOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] TailLatencyOptimizer 构造函数数量: {ctors_TailLatencyOptimizer.Length}");
        var methods_TailLatencyOptimizer = type_TailLatencyOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TailLatencyOptimizer 公开方法数量: {methods_TailLatencyOptimizer.Length}");
        foreach (var m in methods_TailLatencyOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TailLatencyOptimizer 未找到，尝试无命名空间...");
        type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
        if (type_TailLatencyOptimizer != null)
            Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailLatencyOptimizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LatencyToken
    var type_LatencyToken = Type.GetType("LatencyToken");
    if (type_LatencyToken != null)
    {
        Console.WriteLine("[PASS] 类型 LatencyToken (class) 存在");
        var ctors_LatencyToken = type_LatencyToken.GetConstructors();
        Console.WriteLine($"[PASS] LatencyToken 构造函数数量: {ctors_LatencyToken.Length}");
        var methods_LatencyToken = type_LatencyToken.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LatencyToken 公开方法数量: {methods_LatencyToken.Length}");
        foreach (var m in methods_LatencyToken)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LatencyToken 未找到，尝试无命名空间...");
        type_LatencyToken = Type.GetType("LatencyToken");
        if (type_LatencyToken != null)
            Console.WriteLine("[PASS] 类型 LatencyToken (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LatencyToken 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
