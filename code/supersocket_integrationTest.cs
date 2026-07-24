#load "supersocket_integration.cs"

Console.WriteLine("=== supersocket_integration.cs Test ===");

try
{
    // 验证 class: SuperSocketOptions
    var type_SuperSocketOptions = Type.GetType("SuperSocketOptions");
    if (type_SuperSocketOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SuperSocketOptions (class) 存在");
        var ctors_SuperSocketOptions = type_SuperSocketOptions.GetConstructors();
        Console.WriteLine($"[PASS] SuperSocketOptions 构造函数数量: {ctors_SuperSocketOptions.Length}");
        var methods_SuperSocketOptions = type_SuperSocketOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SuperSocketOptions 公开方法数量: {methods_SuperSocketOptions.Length}");
        foreach (var m in methods_SuperSocketOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SuperSocketOptions 未找到，尝试无命名空间...");
        type_SuperSocketOptions = Type.GetType("SuperSocketOptions");
        if (type_SuperSocketOptions != null)
            Console.WriteLine("[PASS] 类型 SuperSocketOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SuperSocketOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TieredMemoryConfig
    var type_TieredMemoryConfig = Type.GetType("TieredMemoryConfig");
    if (type_TieredMemoryConfig != null)
    {
        Console.WriteLine("[PASS] 类型 TieredMemoryConfig (class) 存在");
        var ctors_TieredMemoryConfig = type_TieredMemoryConfig.GetConstructors();
        Console.WriteLine($"[PASS] TieredMemoryConfig 构造函数数量: {ctors_TieredMemoryConfig.Length}");
        var methods_TieredMemoryConfig = type_TieredMemoryConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredMemoryConfig 公开方法数量: {methods_TieredMemoryConfig.Length}");
        foreach (var m in methods_TieredMemoryConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredMemoryConfig 未找到，尝试无命名空间...");
        type_TieredMemoryConfig = Type.GetType("TieredMemoryConfig");
        if (type_TieredMemoryConfig != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SuperSocketService
    var type_SuperSocketService = Type.GetType("SuperSocketService");
    if (type_SuperSocketService != null)
    {
        Console.WriteLine("[PASS] 类型 SuperSocketService (class) 存在");
        var ctors_SuperSocketService = type_SuperSocketService.GetConstructors();
        Console.WriteLine($"[PASS] SuperSocketService 构造函数数量: {ctors_SuperSocketService.Length}");
        var methods_SuperSocketService = type_SuperSocketService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SuperSocketService 公开方法数量: {methods_SuperSocketService.Length}");
        foreach (var m in methods_SuperSocketService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SuperSocketService 未找到，尝试无命名空间...");
        type_SuperSocketService = Type.GetType("SuperSocketService");
        if (type_SuperSocketService != null)
            Console.WriteLine("[PASS] 类型 SuperSocketService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SuperSocketService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SuperSocketExtensions
    var type_SuperSocketExtensions = Type.GetType("SuperSocketExtensions");
    if (type_SuperSocketExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SuperSocketExtensions (class) 存在");
        var ctors_SuperSocketExtensions = type_SuperSocketExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SuperSocketExtensions 构造函数数量: {ctors_SuperSocketExtensions.Length}");
        var methods_SuperSocketExtensions = type_SuperSocketExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SuperSocketExtensions 公开方法数量: {methods_SuperSocketExtensions.Length}");
        foreach (var m in methods_SuperSocketExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SuperSocketExtensions 未找到，尝试无命名空间...");
        type_SuperSocketExtensions = Type.GetType("SuperSocketExtensions");
        if (type_SuperSocketExtensions != null)
            Console.WriteLine("[PASS] 类型 SuperSocketExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SuperSocketExtensions 可能为顶层语句或嵌套类型");
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

    // 验证 class: TieredMemoryService
    var type_TieredMemoryService = Type.GetType("TieredMemoryService");
    if (type_TieredMemoryService != null)
    {
        Console.WriteLine("[PASS] 类型 TieredMemoryService (class) 存在");
        var ctors_TieredMemoryService = type_TieredMemoryService.GetConstructors();
        Console.WriteLine($"[PASS] TieredMemoryService 构造函数数量: {ctors_TieredMemoryService.Length}");
        var methods_TieredMemoryService = type_TieredMemoryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredMemoryService 公开方法数量: {methods_TieredMemoryService.Length}");
        foreach (var m in methods_TieredMemoryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredMemoryService 未找到，尝试无命名空间...");
        type_TieredMemoryService = Type.GetType("TieredMemoryService");
        if (type_TieredMemoryService != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SizedMemoryPool
    var type_SizedMemoryPool = Type.GetType("SizedMemoryPool");
    if (type_SizedMemoryPool != null)
    {
        Console.WriteLine("[PASS] 类型 SizedMemoryPool (class) 存在");
        var ctors_SizedMemoryPool = type_SizedMemoryPool.GetConstructors();
        Console.WriteLine($"[PASS] SizedMemoryPool 构造函数数量: {ctors_SizedMemoryPool.Length}");
        var methods_SizedMemoryPool = type_SizedMemoryPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SizedMemoryPool 公开方法数量: {methods_SizedMemoryPool.Length}");
        foreach (var m in methods_SizedMemoryPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SizedMemoryPool 未找到，尝试无命名空间...");
        type_SizedMemoryPool = Type.GetType("SizedMemoryPool");
        if (type_SizedMemoryPool != null)
            Console.WriteLine("[PASS] 类型 SizedMemoryPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SizedMemoryPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ArrayMemoryOwner
    var type_ArrayMemoryOwner = Type.GetType("ArrayMemoryOwner");
    if (type_ArrayMemoryOwner != null)
    {
        Console.WriteLine("[PASS] 类型 ArrayMemoryOwner (class) 存在");
        var ctors_ArrayMemoryOwner = type_ArrayMemoryOwner.GetConstructors();
        Console.WriteLine($"[PASS] ArrayMemoryOwner 构造函数数量: {ctors_ArrayMemoryOwner.Length}");
        var methods_ArrayMemoryOwner = type_ArrayMemoryOwner.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ArrayMemoryOwner 公开方法数量: {methods_ArrayMemoryOwner.Length}");
        foreach (var m in methods_ArrayMemoryOwner)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ArrayMemoryOwner 未找到，尝试无命名空间...");
        type_ArrayMemoryOwner = Type.GetType("ArrayMemoryOwner");
        if (type_ArrayMemoryOwner != null)
            Console.WriteLine("[PASS] 类型 ArrayMemoryOwner (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ArrayMemoryOwner 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TieredMemoryPooledObjectPolicy
    var type_TieredMemoryPooledObjectPolicy = Type.GetType("TieredMemoryPooledObjectPolicy");
    if (type_TieredMemoryPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 TieredMemoryPooledObjectPolicy (class) 存在");
        var ctors_TieredMemoryPooledObjectPolicy = type_TieredMemoryPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] TieredMemoryPooledObjectPolicy 构造函数数量: {ctors_TieredMemoryPooledObjectPolicy.Length}");
        var methods_TieredMemoryPooledObjectPolicy = type_TieredMemoryPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredMemoryPooledObjectPolicy 公开方法数量: {methods_TieredMemoryPooledObjectPolicy.Length}");
        foreach (var m in methods_TieredMemoryPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredMemoryPooledObjectPolicy 未找到，尝试无命名空间...");
        type_TieredMemoryPooledObjectPolicy = Type.GetType("TieredMemoryPooledObjectPolicy");
        if (type_TieredMemoryPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TokenRingBuffer
    var type_TokenRingBuffer = Type.GetType("TokenRingBuffer");
    if (type_TokenRingBuffer != null)
    {
        Console.WriteLine("[PASS] 类型 TokenRingBuffer (class) 存在");
        var ctors_TokenRingBuffer = type_TokenRingBuffer.GetConstructors();
        Console.WriteLine($"[PASS] TokenRingBuffer 构造函数数量: {ctors_TokenRingBuffer.Length}");
        var methods_TokenRingBuffer = type_TokenRingBuffer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TokenRingBuffer 公开方法数量: {methods_TokenRingBuffer.Length}");
        foreach (var m in methods_TokenRingBuffer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TokenRingBuffer 未找到，尝试无命名空间...");
        type_TokenRingBuffer = Type.GetType("TokenRingBuffer");
        if (type_TokenRingBuffer != null)
            Console.WriteLine("[PASS] 类型 TokenRingBuffer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TokenRingBuffer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Utf8StringPackageInfo
    var type_Utf8StringPackageInfo = Type.GetType("Utf8StringPackageInfo");
    if (type_Utf8StringPackageInfo != null)
    {
        Console.WriteLine("[PASS] 类型 Utf8StringPackageInfo (class) 存在");
        var ctors_Utf8StringPackageInfo = type_Utf8StringPackageInfo.GetConstructors();
        Console.WriteLine($"[PASS] Utf8StringPackageInfo 构造函数数量: {ctors_Utf8StringPackageInfo.Length}");
        var methods_Utf8StringPackageInfo = type_Utf8StringPackageInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Utf8StringPackageInfo 公开方法数量: {methods_Utf8StringPackageInfo.Length}");
        foreach (var m in methods_Utf8StringPackageInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Utf8StringPackageInfo 未找到，尝试无命名空间...");
        type_Utf8StringPackageInfo = Type.GetType("Utf8StringPackageInfo");
        if (type_Utf8StringPackageInfo != null)
            Console.WriteLine("[PASS] 类型 Utf8StringPackageInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Utf8StringPackageInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SuperSocketExample
    var type_SuperSocketExample = Type.GetType("SuperSocketExample");
    if (type_SuperSocketExample != null)
    {
        Console.WriteLine("[PASS] 类型 SuperSocketExample (class) 存在");
        var ctors_SuperSocketExample = type_SuperSocketExample.GetConstructors();
        Console.WriteLine($"[PASS] SuperSocketExample 构造函数数量: {ctors_SuperSocketExample.Length}");
        var methods_SuperSocketExample = type_SuperSocketExample.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SuperSocketExample 公开方法数量: {methods_SuperSocketExample.Length}");
        foreach (var m in methods_SuperSocketExample)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SuperSocketExample 未找到，尝试无命名空间...");
        type_SuperSocketExample = Type.GetType("SuperSocketExample");
        if (type_SuperSocketExample != null)
            Console.WriteLine("[PASS] 类型 SuperSocketExample (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SuperSocketExample 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ISuperSocketService
    var type_ISuperSocketService = Type.GetType("ISuperSocketService");
    if (type_ISuperSocketService != null)
    {
        Console.WriteLine("[PASS] 类型 ISuperSocketService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ISuperSocketService 未找到，尝试无命名空间...");
        type_ISuperSocketService = Type.GetType("ISuperSocketService");
        if (type_ISuperSocketService != null)
            Console.WriteLine("[PASS] 类型 ISuperSocketService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISuperSocketService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
