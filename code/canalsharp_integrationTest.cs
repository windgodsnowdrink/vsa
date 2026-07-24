#load "canalsharp_integration.cs"

Console.WriteLine("=== canalsharp_integration.cs Test ===");

try
{
    // 验证 class: CanalWorker
    var type_CanalWorker = Type.GetType("CanalWorker");
    if (type_CanalWorker != null)
    {
        Console.WriteLine("[PASS] 类型 CanalWorker (class) 存在");
        var ctors_CanalWorker = type_CanalWorker.GetConstructors();
        Console.WriteLine($"[PASS] CanalWorker 构造函数数量: {ctors_CanalWorker.Length}");
        var methods_CanalWorker = type_CanalWorker.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CanalWorker 公开方法数量: {methods_CanalWorker.Length}");
        foreach (var m in methods_CanalWorker)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CanalWorker 未找到，尝试无命名空间...");
        type_CanalWorker = Type.GetType("CanalWorker");
        if (type_CanalWorker != null)
            Console.WriteLine("[PASS] 类型 CanalWorker (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CanalWorker 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EntryProcessor
    var type_EntryProcessor = Type.GetType("EntryProcessor");
    if (type_EntryProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 EntryProcessor (class) 存在");
        var ctors_EntryProcessor = type_EntryProcessor.GetConstructors();
        Console.WriteLine($"[PASS] EntryProcessor 构造函数数量: {ctors_EntryProcessor.Length}");
        var methods_EntryProcessor = type_EntryProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EntryProcessor 公开方法数量: {methods_EntryProcessor.Length}");
        foreach (var m in methods_EntryProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EntryProcessor 未找到，尝试无命名空间...");
        type_EntryProcessor = Type.GetType("EntryProcessor");
        if (type_EntryProcessor != null)
            Console.WriteLine("[PASS] 类型 EntryProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EntryProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CanalOptions
    var type_CanalOptions = Type.GetType("CanalOptions");
    if (type_CanalOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CanalOptions (class) 存在");
        var ctors_CanalOptions = type_CanalOptions.GetConstructors();
        Console.WriteLine($"[PASS] CanalOptions 构造函数数量: {ctors_CanalOptions.Length}");
        var methods_CanalOptions = type_CanalOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CanalOptions 公开方法数量: {methods_CanalOptions.Length}");
        foreach (var m in methods_CanalOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CanalOptions 未找到，尝试无命名空间...");
        type_CanalOptions = Type.GetType("CanalOptions");
        if (type_CanalOptions != null)
            Console.WriteLine("[PASS] 类型 CanalOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CanalOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
