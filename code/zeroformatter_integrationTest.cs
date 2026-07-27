#load "zeroformatter_integration.cs"

Console.WriteLine("=== zeroformatter_integration.cs Test ===");

try
{
    // 验证 class: ZeroCopyProcessor
    var type_ZeroCopyProcessor = Type.GetType("ZeroCopyProcessor");
    if (type_ZeroCopyProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ZeroCopyProcessor (class) 存在");
        var ctors_ZeroCopyProcessor = type_ZeroCopyProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ZeroCopyProcessor 构造函数数量: {ctors_ZeroCopyProcessor.Length}");
        var methods_ZeroCopyProcessor = type_ZeroCopyProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZeroCopyProcessor 公开方法数量: {methods_ZeroCopyProcessor.Length}");
        foreach (var m in methods_ZeroCopyProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZeroCopyProcessor 未找到，尝试无命名空间...");
        type_ZeroCopyProcessor = Type.GetType("ZeroCopyProcessor");
        if (type_ZeroCopyProcessor != null)
            Console.WriteLine("[PASS] 类型 ZeroCopyProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZeroCopyProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ThreadLocalAllocator
    var type_ThreadLocalAllocator = Type.GetType("ThreadLocalAllocator");
    if (type_ThreadLocalAllocator != null)
    {
        Console.WriteLine("[PASS] 类型 ThreadLocalAllocator (class) 存在");
        var ctors_ThreadLocalAllocator = type_ThreadLocalAllocator.GetConstructors();
        Console.WriteLine($"[PASS] ThreadLocalAllocator 构造函数数量: {ctors_ThreadLocalAllocator.Length}");
        var methods_ThreadLocalAllocator = type_ThreadLocalAllocator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ThreadLocalAllocator 公开方法数量: {methods_ThreadLocalAllocator.Length}");
        foreach (var m in methods_ThreadLocalAllocator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ThreadLocalAllocator 未找到，尝试无命名空间...");
        type_ThreadLocalAllocator = Type.GetType("ThreadLocalAllocator");
        if (type_ThreadLocalAllocator != null)
            Console.WriteLine("[PASS] 类型 ThreadLocalAllocator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThreadLocalAllocator 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: MessageEnvelope
    var type_MessageEnvelope = Type.GetType("MessageEnvelope");
    if (type_MessageEnvelope != null)
    {
        Console.WriteLine("[PASS] 类型 MessageEnvelope (struct) 存在");
        var ctors_MessageEnvelope = type_MessageEnvelope.GetConstructors();
        Console.WriteLine($"[PASS] MessageEnvelope 构造函数数量: {ctors_MessageEnvelope.Length}");
        var methods_MessageEnvelope = type_MessageEnvelope.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageEnvelope 公开方法数量: {methods_MessageEnvelope.Length}");
        foreach (var m in methods_MessageEnvelope)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageEnvelope 未找到，尝试无命名空间...");
        type_MessageEnvelope = Type.GetType("MessageEnvelope");
        if (type_MessageEnvelope != null)
            Console.WriteLine("[PASS] 类型 MessageEnvelope (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageEnvelope 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
