#load "disruptor_integration.cs"

Console.WriteLine("=== disruptor_integration.cs Test ===");

try
{
    // 验证 class: MessageValidator
    var type_MessageValidator = Type.GetType("MessageValidator");
    if (type_MessageValidator != null)
    {
        Console.WriteLine("[PASS] 类型 MessageValidator (class) 存在");
        var ctors_MessageValidator = type_MessageValidator.GetConstructors();
        Console.WriteLine($"[PASS] MessageValidator 构造函数数量: {ctors_MessageValidator.Length}");
        var methods_MessageValidator = type_MessageValidator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageValidator 公开方法数量: {methods_MessageValidator.Length}");
        foreach (var m in methods_MessageValidator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageValidator 未找到，尝试无命名空间...");
        type_MessageValidator = Type.GetType("MessageValidator");
        if (type_MessageValidator != null)
            Console.WriteLine("[PASS] 类型 MessageValidator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageValidator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageTransformer
    var type_MessageTransformer = Type.GetType("MessageTransformer");
    if (type_MessageTransformer != null)
    {
        Console.WriteLine("[PASS] 类型 MessageTransformer (class) 存在");
        var ctors_MessageTransformer = type_MessageTransformer.GetConstructors();
        Console.WriteLine($"[PASS] MessageTransformer 构造函数数量: {ctors_MessageTransformer.Length}");
        var methods_MessageTransformer = type_MessageTransformer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageTransformer 公开方法数量: {methods_MessageTransformer.Length}");
        foreach (var m in methods_MessageTransformer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageTransformer 未找到，尝试无命名空间...");
        type_MessageTransformer = Type.GetType("MessageTransformer");
        if (type_MessageTransformer != null)
            Console.WriteLine("[PASS] 类型 MessageTransformer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageTransformer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessagePersister
    var type_MessagePersister = Type.GetType("MessagePersister");
    if (type_MessagePersister != null)
    {
        Console.WriteLine("[PASS] 类型 MessagePersister (class) 存在");
        var ctors_MessagePersister = type_MessagePersister.GetConstructors();
        Console.WriteLine($"[PASS] MessagePersister 构造函数数量: {ctors_MessagePersister.Length}");
        var methods_MessagePersister = type_MessagePersister.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessagePersister 公开方法数量: {methods_MessagePersister.Length}");
        foreach (var m in methods_MessagePersister)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessagePersister 未找到，尝试无命名空间...");
        type_MessagePersister = Type.GetType("MessagePersister");
        if (type_MessagePersister != null)
            Console.WriteLine("[PASS] 类型 MessagePersister (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessagePersister 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: MessageEvent
    var type_MessageEvent = Type.GetType("MessageEvent");
    if (type_MessageEvent != null)
    {
        Console.WriteLine("[PASS] 类型 MessageEvent (struct) 存在");
        var ctors_MessageEvent = type_MessageEvent.GetConstructors();
        Console.WriteLine($"[PASS] MessageEvent 构造函数数量: {ctors_MessageEvent.Length}");
        var methods_MessageEvent = type_MessageEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageEvent 公开方法数量: {methods_MessageEvent.Length}");
        foreach (var m in methods_MessageEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageEvent 未找到，尝试无命名空间...");
        type_MessageEvent = Type.GetType("MessageEvent");
        if (type_MessageEvent != null)
            Console.WriteLine("[PASS] 类型 MessageEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
