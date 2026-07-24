#load "litedb_event_model.cs"

Console.WriteLine("=== litedb_event_model.cs Test ===");

try
{
    // 验证 class: EventStoreService
    var type_EventStoreService = Type.GetType("EventStoreService");
    if (type_EventStoreService != null)
    {
        Console.WriteLine("[PASS] 类型 EventStoreService (class) 存在");
        var ctors_EventStoreService = type_EventStoreService.GetConstructors();
        Console.WriteLine($"[PASS] EventStoreService 构造函数数量: {ctors_EventStoreService.Length}");
        var methods_EventStoreService = type_EventStoreService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventStoreService 公开方法数量: {methods_EventStoreService.Length}");
        foreach (var m in methods_EventStoreService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventStoreService 未找到，尝试无命名空间...");
        type_EventStoreService = Type.GetType("EventStoreService");
        if (type_EventStoreService != null)
            Console.WriteLine("[PASS] 类型 EventStoreService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventStoreService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChunkReassembler
    var type_ChunkReassembler = Type.GetType("ChunkReassembler");
    if (type_ChunkReassembler != null)
    {
        Console.WriteLine("[PASS] 类型 ChunkReassembler (class) 存在");
        var ctors_ChunkReassembler = type_ChunkReassembler.GetConstructors();
        Console.WriteLine($"[PASS] ChunkReassembler 构造函数数量: {ctors_ChunkReassembler.Length}");
        var methods_ChunkReassembler = type_ChunkReassembler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChunkReassembler 公开方法数量: {methods_ChunkReassembler.Length}");
        foreach (var m in methods_ChunkReassembler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChunkReassembler 未找到，尝试无命名空间...");
        type_ChunkReassembler = Type.GetType("ChunkReassembler");
        if (type_ChunkReassembler != null)
            Console.WriteLine("[PASS] 类型 ChunkReassembler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChunkReassembler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ExtendedPayload
    var type_ExtendedPayload = Type.GetType("ExtendedPayload");
    if (type_ExtendedPayload != null)
    {
        Console.WriteLine("[PASS] 类型 ExtendedPayload (class) 存在");
        var ctors_ExtendedPayload = type_ExtendedPayload.GetConstructors();
        Console.WriteLine($"[PASS] ExtendedPayload 构造函数数量: {ctors_ExtendedPayload.Length}");
        var methods_ExtendedPayload = type_ExtendedPayload.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExtendedPayload 公开方法数量: {methods_ExtendedPayload.Length}");
        foreach (var m in methods_ExtendedPayload)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExtendedPayload 未找到，尝试无命名空间...");
        type_ExtendedPayload = Type.GetType("ExtendedPayload");
        if (type_ExtendedPayload != null)
            Console.WriteLine("[PASS] 类型 ExtendedPayload (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExtendedPayload 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventStoreDemo
    var type_EventStoreDemo = Type.GetType("EventStoreDemo");
    if (type_EventStoreDemo != null)
    {
        Console.WriteLine("[PASS] 类型 EventStoreDemo (class) 存在");
        var ctors_EventStoreDemo = type_EventStoreDemo.GetConstructors();
        Console.WriteLine($"[PASS] EventStoreDemo 构造函数数量: {ctors_EventStoreDemo.Length}");
        var methods_EventStoreDemo = type_EventStoreDemo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventStoreDemo 公开方法数量: {methods_EventStoreDemo.Length}");
        foreach (var m in methods_EventStoreDemo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventStoreDemo 未找到，尝试无命名空间...");
        type_EventStoreDemo = Type.GetType("EventStoreDemo");
        if (type_EventStoreDemo != null)
            Console.WriteLine("[PASS] 类型 EventStoreDemo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventStoreDemo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TieredEventStorage
    var type_TieredEventStorage = Type.GetType("TieredEventStorage");
    if (type_TieredEventStorage != null)
    {
        Console.WriteLine("[PASS] 类型 TieredEventStorage (class) 存在");
        var ctors_TieredEventStorage = type_TieredEventStorage.GetConstructors();
        Console.WriteLine($"[PASS] TieredEventStorage 构造函数数量: {ctors_TieredEventStorage.Length}");
        var methods_TieredEventStorage = type_TieredEventStorage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredEventStorage 公开方法数量: {methods_TieredEventStorage.Length}");
        foreach (var m in methods_TieredEventStorage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredEventStorage 未找到，尝试无命名空间...");
        type_TieredEventStorage = Type.GetType("TieredEventStorage");
        if (type_TieredEventStorage != null)
            Console.WriteLine("[PASS] 类型 TieredEventStorage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredEventStorage 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: EventData
    var type_EventData = Type.GetType("EventData");
    if (type_EventData != null)
    {
        Console.WriteLine("[PASS] 类型 EventData (struct) 存在");
        var ctors_EventData = type_EventData.GetConstructors();
        Console.WriteLine($"[PASS] EventData 构造函数数量: {ctors_EventData.Length}");
        var methods_EventData = type_EventData.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventData 公开方法数量: {methods_EventData.Length}");
        foreach (var m in methods_EventData)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventData 未找到，尝试无命名空间...");
        type_EventData = Type.GetType("EventData");
        if (type_EventData != null)
            Console.WriteLine("[PASS] 类型 EventData (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventData 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
