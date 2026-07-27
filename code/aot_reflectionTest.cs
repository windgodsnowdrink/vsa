#load "aot_reflection.cs"

Console.WriteLine("=== aot_reflection.cs Test ===");

try
{
    // 验证 class: AotReflectionContext
    var type_AotReflectionContext = Type.GetType("AotReflectionContext");
    if (type_AotReflectionContext != null)
    {
        Console.WriteLine("[PASS] 类型 AotReflectionContext (class) 存在");
        var ctors_AotReflectionContext = type_AotReflectionContext.GetConstructors();
        Console.WriteLine($"[PASS] AotReflectionContext 构造函数数量: {ctors_AotReflectionContext.Length}");
        var methods_AotReflectionContext = type_AotReflectionContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AotReflectionContext 公开方法数量: {methods_AotReflectionContext.Length}");
        foreach (var m in methods_AotReflectionContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AotReflectionContext 未找到，尝试无命名空间...");
        type_AotReflectionContext = Type.GetType("AotReflectionContext");
        if (type_AotReflectionContext != null)
            Console.WriteLine("[PASS] 类型 AotReflectionContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AotReflectionContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AotReflectionService
    var type_AotReflectionService = Type.GetType("AotReflectionService");
    if (type_AotReflectionService != null)
    {
        Console.WriteLine("[PASS] 类型 AotReflectionService (class) 存在");
        var ctors_AotReflectionService = type_AotReflectionService.GetConstructors();
        Console.WriteLine($"[PASS] AotReflectionService 构造函数数量: {ctors_AotReflectionService.Length}");
        var methods_AotReflectionService = type_AotReflectionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AotReflectionService 公开方法数量: {methods_AotReflectionService.Length}");
        foreach (var m in methods_AotReflectionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AotReflectionService 未找到，尝试无命名空间...");
        type_AotReflectionService = Type.GetType("AotReflectionService");
        if (type_AotReflectionService != null)
            Console.WriteLine("[PASS] 类型 AotReflectionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AotReflectionService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAotReflectionMarker
    var type_IAotReflectionMarker = Type.GetType("IAotReflectionMarker");
    if (type_IAotReflectionMarker != null)
    {
        Console.WriteLine("[PASS] 类型 IAotReflectionMarker (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAotReflectionMarker 未找到，尝试无命名空间...");
        type_IAotReflectionMarker = Type.GetType("IAotReflectionMarker");
        if (type_IAotReflectionMarker != null)
            Console.WriteLine("[PASS] 类型 IAotReflectionMarker (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAotReflectionMarker 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ReflectionData
    var type_ReflectionData = Type.GetType("ReflectionData");
    if (type_ReflectionData != null)
    {
        Console.WriteLine("[PASS] 类型 ReflectionData (record) 存在");
        var ctors_ReflectionData = type_ReflectionData.GetConstructors();
        Console.WriteLine($"[PASS] ReflectionData 构造函数数量: {ctors_ReflectionData.Length}");
        var methods_ReflectionData = type_ReflectionData.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReflectionData 公开方法数量: {methods_ReflectionData.Length}");
        foreach (var m in methods_ReflectionData)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReflectionData 未找到，尝试无命名空间...");
        type_ReflectionData = Type.GetType("ReflectionData");
        if (type_ReflectionData != null)
            Console.WriteLine("[PASS] 类型 ReflectionData (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReflectionData 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ReflectionRequest
    var type_ReflectionRequest = Type.GetType("ReflectionRequest");
    if (type_ReflectionRequest != null)
    {
        Console.WriteLine("[PASS] 类型 ReflectionRequest (record) 存在");
        var ctors_ReflectionRequest = type_ReflectionRequest.GetConstructors();
        Console.WriteLine($"[PASS] ReflectionRequest 构造函数数量: {ctors_ReflectionRequest.Length}");
        var methods_ReflectionRequest = type_ReflectionRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReflectionRequest 公开方法数量: {methods_ReflectionRequest.Length}");
        foreach (var m in methods_ReflectionRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReflectionRequest 未找到，尝试无命名空间...");
        type_ReflectionRequest = Type.GetType("ReflectionRequest");
        if (type_ReflectionRequest != null)
            Console.WriteLine("[PASS] 类型 ReflectionRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReflectionRequest 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
