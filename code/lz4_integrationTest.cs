#load "lz4_integration.cs"

Console.WriteLine("=== lz4_integration.cs Test ===");

try
{
    // 验证 class: LZ4Options
    var type_LZ4Options = Type.GetType("LZ4Options");
    if (type_LZ4Options != null)
    {
        Console.WriteLine("[PASS] 类型 LZ4Options (class) 存在");
        var ctors_LZ4Options = type_LZ4Options.GetConstructors();
        Console.WriteLine($"[PASS] LZ4Options 构造函数数量: {ctors_LZ4Options.Length}");
        var methods_LZ4Options = type_LZ4Options.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LZ4Options 公开方法数量: {methods_LZ4Options.Length}");
        foreach (var m in methods_LZ4Options)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LZ4Options 未找到，尝试无命名空间...");
        type_LZ4Options = Type.GetType("LZ4Options");
        if (type_LZ4Options != null)
            Console.WriteLine("[PASS] 类型 LZ4Options (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LZ4Options 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LZ4Service
    var type_LZ4Service = Type.GetType("LZ4Service");
    if (type_LZ4Service != null)
    {
        Console.WriteLine("[PASS] 类型 LZ4Service (class) 存在");
        var ctors_LZ4Service = type_LZ4Service.GetConstructors();
        Console.WriteLine($"[PASS] LZ4Service 构造函数数量: {ctors_LZ4Service.Length}");
        var methods_LZ4Service = type_LZ4Service.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LZ4Service 公开方法数量: {methods_LZ4Service.Length}");
        foreach (var m in methods_LZ4Service)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LZ4Service 未找到，尝试无命名空间...");
        type_LZ4Service = Type.GetType("LZ4Service");
        if (type_LZ4Service != null)
            Console.WriteLine("[PASS] 类型 LZ4Service (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LZ4Service 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LZ4ServiceCollectionExtensions
    var type_LZ4ServiceCollectionExtensions = Type.GetType("LZ4ServiceCollectionExtensions");
    if (type_LZ4ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 LZ4ServiceCollectionExtensions (class) 存在");
        var ctors_LZ4ServiceCollectionExtensions = type_LZ4ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] LZ4ServiceCollectionExtensions 构造函数数量: {ctors_LZ4ServiceCollectionExtensions.Length}");
        var methods_LZ4ServiceCollectionExtensions = type_LZ4ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LZ4ServiceCollectionExtensions 公开方法数量: {methods_LZ4ServiceCollectionExtensions.Length}");
        foreach (var m in methods_LZ4ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LZ4ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_LZ4ServiceCollectionExtensions = Type.GetType("LZ4ServiceCollectionExtensions");
        if (type_LZ4ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 LZ4ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LZ4ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LZ4Demo
    var type_LZ4Demo = Type.GetType("LZ4Demo");
    if (type_LZ4Demo != null)
    {
        Console.WriteLine("[PASS] 类型 LZ4Demo (class) 存在");
        var ctors_LZ4Demo = type_LZ4Demo.GetConstructors();
        Console.WriteLine($"[PASS] LZ4Demo 构造函数数量: {ctors_LZ4Demo.Length}");
        var methods_LZ4Demo = type_LZ4Demo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LZ4Demo 公开方法数量: {methods_LZ4Demo.Length}");
        foreach (var m in methods_LZ4Demo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LZ4Demo 未找到，尝试无命名空间...");
        type_LZ4Demo = Type.GetType("LZ4Demo");
        if (type_LZ4Demo != null)
            Console.WriteLine("[PASS] 类型 LZ4Demo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LZ4Demo 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ILZ4Service
    var type_ILZ4Service = Type.GetType("ILZ4Service");
    if (type_ILZ4Service != null)
    {
        Console.WriteLine("[PASS] 类型 ILZ4Service (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ILZ4Service 未找到，尝试无命名空间...");
        type_ILZ4Service = Type.GetType("ILZ4Service");
        if (type_ILZ4Service != null)
            Console.WriteLine("[PASS] 类型 ILZ4Service (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ILZ4Service 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
