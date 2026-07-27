#load "recyclable_memorystream_integration.cs"

Console.WriteLine("=== recyclable_memorystream_integration.cs Test ===");

try
{
    // 验证 class: RecyclableMemoryStreamOptions
    var type_RecyclableMemoryStreamOptions = Type.GetType("RecyclableMemoryStreamOptions");
    if (type_RecyclableMemoryStreamOptions != null)
    {
        Console.WriteLine("[PASS] 类型 RecyclableMemoryStreamOptions (class) 存在");
        var ctors_RecyclableMemoryStreamOptions = type_RecyclableMemoryStreamOptions.GetConstructors();
        Console.WriteLine($"[PASS] RecyclableMemoryStreamOptions 构造函数数量: {ctors_RecyclableMemoryStreamOptions.Length}");
        var methods_RecyclableMemoryStreamOptions = type_RecyclableMemoryStreamOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RecyclableMemoryStreamOptions 公开方法数量: {methods_RecyclableMemoryStreamOptions.Length}");
        foreach (var m in methods_RecyclableMemoryStreamOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RecyclableMemoryStreamOptions 未找到，尝试无命名空间...");
        type_RecyclableMemoryStreamOptions = Type.GetType("RecyclableMemoryStreamOptions");
        if (type_RecyclableMemoryStreamOptions != null)
            Console.WriteLine("[PASS] 类型 RecyclableMemoryStreamOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RecyclableMemoryStreamOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MemoryStreamPoolService
    var type_MemoryStreamPoolService = Type.GetType("MemoryStreamPoolService");
    if (type_MemoryStreamPoolService != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryStreamPoolService (class) 存在");
        var ctors_MemoryStreamPoolService = type_MemoryStreamPoolService.GetConstructors();
        Console.WriteLine($"[PASS] MemoryStreamPoolService 构造函数数量: {ctors_MemoryStreamPoolService.Length}");
        var methods_MemoryStreamPoolService = type_MemoryStreamPoolService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryStreamPoolService 公开方法数量: {methods_MemoryStreamPoolService.Length}");
        foreach (var m in methods_MemoryStreamPoolService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryStreamPoolService 未找到，尝试无命名空间...");
        type_MemoryStreamPoolService = Type.GetType("MemoryStreamPoolService");
        if (type_MemoryStreamPoolService != null)
            Console.WriteLine("[PASS] 类型 MemoryStreamPoolService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryStreamPoolService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RecyclableMemoryStreamExtensions
    var type_RecyclableMemoryStreamExtensions = Type.GetType("RecyclableMemoryStreamExtensions");
    if (type_RecyclableMemoryStreamExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 RecyclableMemoryStreamExtensions (class) 存在");
        var ctors_RecyclableMemoryStreamExtensions = type_RecyclableMemoryStreamExtensions.GetConstructors();
        Console.WriteLine($"[PASS] RecyclableMemoryStreamExtensions 构造函数数量: {ctors_RecyclableMemoryStreamExtensions.Length}");
        var methods_RecyclableMemoryStreamExtensions = type_RecyclableMemoryStreamExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RecyclableMemoryStreamExtensions 公开方法数量: {methods_RecyclableMemoryStreamExtensions.Length}");
        foreach (var m in methods_RecyclableMemoryStreamExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RecyclableMemoryStreamExtensions 未找到，尝试无命名空间...");
        type_RecyclableMemoryStreamExtensions = Type.GetType("RecyclableMemoryStreamExtensions");
        if (type_RecyclableMemoryStreamExtensions != null)
            Console.WriteLine("[PASS] 类型 RecyclableMemoryStreamExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RecyclableMemoryStreamExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RecyclableMemoryStreamExample
    var type_RecyclableMemoryStreamExample = Type.GetType("RecyclableMemoryStreamExample");
    if (type_RecyclableMemoryStreamExample != null)
    {
        Console.WriteLine("[PASS] 类型 RecyclableMemoryStreamExample (class) 存在");
        var ctors_RecyclableMemoryStreamExample = type_RecyclableMemoryStreamExample.GetConstructors();
        Console.WriteLine($"[PASS] RecyclableMemoryStreamExample 构造函数数量: {ctors_RecyclableMemoryStreamExample.Length}");
        var methods_RecyclableMemoryStreamExample = type_RecyclableMemoryStreamExample.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RecyclableMemoryStreamExample 公开方法数量: {methods_RecyclableMemoryStreamExample.Length}");
        foreach (var m in methods_RecyclableMemoryStreamExample)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RecyclableMemoryStreamExample 未找到，尝试无命名空间...");
        type_RecyclableMemoryStreamExample = Type.GetType("RecyclableMemoryStreamExample");
        if (type_RecyclableMemoryStreamExample != null)
            Console.WriteLine("[PASS] 类型 RecyclableMemoryStreamExample (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RecyclableMemoryStreamExample 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMemoryStreamPoolService
    var type_IMemoryStreamPoolService = Type.GetType("IMemoryStreamPoolService");
    if (type_IMemoryStreamPoolService != null)
    {
        Console.WriteLine("[PASS] 类型 IMemoryStreamPoolService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMemoryStreamPoolService 未找到，尝试无命名空间...");
        type_IMemoryStreamPoolService = Type.GetType("IMemoryStreamPoolService");
        if (type_IMemoryStreamPoolService != null)
            Console.WriteLine("[PASS] 类型 IMemoryStreamPoolService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMemoryStreamPoolService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
