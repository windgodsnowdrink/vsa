#load "hotchocolate_production.cs"

Console.WriteLine("=== hotchocolate_production.cs Test ===");

try
{
    // 验证 class: HashBasedPartitionStrategy
    var type_HashBasedPartitionStrategy = Type.GetType("HashBasedPartitionStrategy");
    if (type_HashBasedPartitionStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 HashBasedPartitionStrategy (class) 存在");
        var ctors_HashBasedPartitionStrategy = type_HashBasedPartitionStrategy.GetConstructors();
        Console.WriteLine($"[PASS] HashBasedPartitionStrategy 构造函数数量: {ctors_HashBasedPartitionStrategy.Length}");
        var methods_HashBasedPartitionStrategy = type_HashBasedPartitionStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HashBasedPartitionStrategy 公开方法数量: {methods_HashBasedPartitionStrategy.Length}");
        foreach (var m in methods_HashBasedPartitionStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HashBasedPartitionStrategy 未找到，尝试无命名空间...");
        type_HashBasedPartitionStrategy = Type.GetType("HashBasedPartitionStrategy");
        if (type_HashBasedPartitionStrategy != null)
            Console.WriteLine("[PASS] 类型 HashBasedPartitionStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HashBasedPartitionStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PermissionDirectiveType
    var type_PermissionDirectiveType = Type.GetType("PermissionDirectiveType");
    if (type_PermissionDirectiveType != null)
    {
        Console.WriteLine("[PASS] 类型 PermissionDirectiveType (class) 存在");
        var ctors_PermissionDirectiveType = type_PermissionDirectiveType.GetConstructors();
        Console.WriteLine($"[PASS] PermissionDirectiveType 构造函数数量: {ctors_PermissionDirectiveType.Length}");
        var methods_PermissionDirectiveType = type_PermissionDirectiveType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PermissionDirectiveType 公开方法数量: {methods_PermissionDirectiveType.Length}");
        foreach (var m in methods_PermissionDirectiveType)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PermissionDirectiveType 未找到，尝试无命名空间...");
        type_PermissionDirectiveType = Type.GetType("PermissionDirectiveType");
        if (type_PermissionDirectiveType != null)
            Console.WriteLine("[PASS] 类型 PermissionDirectiveType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PermissionDirectiveType 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PermissionDirective
    var type_PermissionDirective = Type.GetType("PermissionDirective");
    if (type_PermissionDirective != null)
    {
        Console.WriteLine("[PASS] 类型 PermissionDirective (class) 存在");
        var ctors_PermissionDirective = type_PermissionDirective.GetConstructors();
        Console.WriteLine($"[PASS] PermissionDirective 构造函数数量: {ctors_PermissionDirective.Length}");
        var methods_PermissionDirective = type_PermissionDirective.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PermissionDirective 公开方法数量: {methods_PermissionDirective.Length}");
        foreach (var m in methods_PermissionDirective)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PermissionDirective 未找到，尝试无命名空间...");
        type_PermissionDirective = Type.GetType("PermissionDirective");
        if (type_PermissionDirective != null)
            Console.WriteLine("[PASS] 类型 PermissionDirective (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PermissionDirective 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MonitoredDataLoader
    var type_MonitoredDataLoader = Type.GetType("MonitoredDataLoader");
    if (type_MonitoredDataLoader != null)
    {
        Console.WriteLine("[PASS] 类型 MonitoredDataLoader (class) 存在");
        var ctors_MonitoredDataLoader = type_MonitoredDataLoader.GetConstructors();
        Console.WriteLine($"[PASS] MonitoredDataLoader 构造函数数量: {ctors_MonitoredDataLoader.Length}");
        var methods_MonitoredDataLoader = type_MonitoredDataLoader.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MonitoredDataLoader 公开方法数量: {methods_MonitoredDataLoader.Length}");
        foreach (var m in methods_MonitoredDataLoader)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MonitoredDataLoader 未找到，尝试无命名空间...");
        type_MonitoredDataLoader = Type.GetType("MonitoredDataLoader");
        if (type_MonitoredDataLoader != null)
            Console.WriteLine("[PASS] 类型 MonitoredDataLoader (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MonitoredDataLoader 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
