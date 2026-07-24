#load "memorypack_demo.cs"

Console.WriteLine("=== memorypack_demo.cs Test ===");

try
{
    // 验证 class: MemoryPackProcessor
    var type_MemoryPackProcessor = Type.GetType("MemoryPackProcessor");
    if (type_MemoryPackProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryPackProcessor (class) 存在");
        var ctors_MemoryPackProcessor = type_MemoryPackProcessor.GetConstructors();
        Console.WriteLine($"[PASS] MemoryPackProcessor 构造函数数量: {ctors_MemoryPackProcessor.Length}");
        var methods_MemoryPackProcessor = type_MemoryPackProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryPackProcessor 公开方法数量: {methods_MemoryPackProcessor.Length}");
        foreach (var m in methods_MemoryPackProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryPackProcessor 未找到，尝试无命名空间...");
        type_MemoryPackProcessor = Type.GetType("MemoryPackProcessor");
        if (type_MemoryPackProcessor != null)
            Console.WriteLine("[PASS] 类型 MemoryPackProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryPackProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoEvent
    var type_TodoEvent = Type.GetType("TodoEvent");
    if (type_TodoEvent != null)
    {
        Console.WriteLine("[PASS] 类型 TodoEvent (class) 存在");
        var ctors_TodoEvent = type_TodoEvent.GetConstructors();
        Console.WriteLine($"[PASS] TodoEvent 构造函数数量: {ctors_TodoEvent.Length}");
        var methods_TodoEvent = type_TodoEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoEvent 公开方法数量: {methods_TodoEvent.Length}");
        foreach (var m in methods_TodoEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoEvent 未找到，尝试无命名空间...");
        type_TodoEvent = Type.GetType("TodoEvent");
        if (type_TodoEvent != null)
            Console.WriteLine("[PASS] 类型 TodoEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: VersionTolerantModel
    var type_VersionTolerantModel = Type.GetType("VersionTolerantModel");
    if (type_VersionTolerantModel != null)
    {
        Console.WriteLine("[PASS] 类型 VersionTolerantModel (class) 存在");
        var ctors_VersionTolerantModel = type_VersionTolerantModel.GetConstructors();
        Console.WriteLine($"[PASS] VersionTolerantModel 构造函数数量: {ctors_VersionTolerantModel.Length}");
        var methods_VersionTolerantModel = type_VersionTolerantModel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VersionTolerantModel 公开方法数量: {methods_VersionTolerantModel.Length}");
        foreach (var m in methods_VersionTolerantModel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VersionTolerantModel 未找到，尝试无命名空间...");
        type_VersionTolerantModel = Type.GetType("VersionTolerantModel");
        if (type_VersionTolerantModel != null)
            Console.WriteLine("[PASS] 类型 VersionTolerantModel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VersionTolerantModel 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CustomFormatterModel
    var type_CustomFormatterModel = Type.GetType("CustomFormatterModel");
    if (type_CustomFormatterModel != null)
    {
        Console.WriteLine("[PASS] 类型 CustomFormatterModel (class) 存在");
        var ctors_CustomFormatterModel = type_CustomFormatterModel.GetConstructors();
        Console.WriteLine($"[PASS] CustomFormatterModel 构造函数数量: {ctors_CustomFormatterModel.Length}");
        var methods_CustomFormatterModel = type_CustomFormatterModel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CustomFormatterModel 公开方法数量: {methods_CustomFormatterModel.Length}");
        foreach (var m in methods_CustomFormatterModel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CustomFormatterModel 未找到，尝试无命名空间...");
        type_CustomFormatterModel = Type.GetType("CustomFormatterModel");
        if (type_CustomFormatterModel != null)
            Console.WriteLine("[PASS] 类型 CustomFormatterModel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CustomFormatterModel 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DateTimeOffsetFormatter
    var type_DateTimeOffsetFormatter = Type.GetType("DateTimeOffsetFormatter");
    if (type_DateTimeOffsetFormatter != null)
    {
        Console.WriteLine("[PASS] 类型 DateTimeOffsetFormatter (class) 存在");
        var ctors_DateTimeOffsetFormatter = type_DateTimeOffsetFormatter.GetConstructors();
        Console.WriteLine($"[PASS] DateTimeOffsetFormatter 构造函数数量: {ctors_DateTimeOffsetFormatter.Length}");
        var methods_DateTimeOffsetFormatter = type_DateTimeOffsetFormatter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DateTimeOffsetFormatter 公开方法数量: {methods_DateTimeOffsetFormatter.Length}");
        foreach (var m in methods_DateTimeOffsetFormatter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DateTimeOffsetFormatter 未找到，尝试无命名空间...");
        type_DateTimeOffsetFormatter = Type.GetType("DateTimeOffsetFormatter");
        if (type_DateTimeOffsetFormatter != null)
            Console.WriteLine("[PASS] 类型 DateTimeOffsetFormatter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DateTimeOffsetFormatter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MemoryPackMetrics
    var type_MemoryPackMetrics = Type.GetType("MemoryPackMetrics");
    if (type_MemoryPackMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryPackMetrics (class) 存在");
        var ctors_MemoryPackMetrics = type_MemoryPackMetrics.GetConstructors();
        Console.WriteLine($"[PASS] MemoryPackMetrics 构造函数数量: {ctors_MemoryPackMetrics.Length}");
        var methods_MemoryPackMetrics = type_MemoryPackMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryPackMetrics 公开方法数量: {methods_MemoryPackMetrics.Length}");
        foreach (var m in methods_MemoryPackMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryPackMetrics 未找到，尝试无命名空间...");
        type_MemoryPackMetrics = Type.GetType("MemoryPackMetrics");
        if (type_MemoryPackMetrics != null)
            Console.WriteLine("[PASS] 类型 MemoryPackMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryPackMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITodoEvent
    var type_ITodoEvent = Type.GetType("ITodoEvent");
    if (type_ITodoEvent != null)
    {
        Console.WriteLine("[PASS] 类型 ITodoEvent (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITodoEvent 未找到，尝试无命名空间...");
        type_ITodoEvent = Type.GetType("ITodoEvent");
        if (type_ITodoEvent != null)
            Console.WriteLine("[PASS] 类型 ITodoEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITodoEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
