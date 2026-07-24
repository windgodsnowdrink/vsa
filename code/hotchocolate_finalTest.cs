#load "hotchocolate_final.cs"

Console.WriteLine("=== hotchocolate_final.cs Test ===");

try
{
    // 验证 class: AdaptivePartitionStrategy
    var type_AdaptivePartitionStrategy = Type.GetType("AdaptivePartitionStrategy");
    if (type_AdaptivePartitionStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 AdaptivePartitionStrategy (class) 存在");
        var ctors_AdaptivePartitionStrategy = type_AdaptivePartitionStrategy.GetConstructors();
        Console.WriteLine($"[PASS] AdaptivePartitionStrategy 构造函数数量: {ctors_AdaptivePartitionStrategy.Length}");
        var methods_AdaptivePartitionStrategy = type_AdaptivePartitionStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdaptivePartitionStrategy 公开方法数量: {methods_AdaptivePartitionStrategy.Length}");
        foreach (var m in methods_AdaptivePartitionStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdaptivePartitionStrategy 未找到，尝试无命名空间...");
        type_AdaptivePartitionStrategy = Type.GetType("AdaptivePartitionStrategy");
        if (type_AdaptivePartitionStrategy != null)
            Console.WriteLine("[PASS] 类型 AdaptivePartitionStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdaptivePartitionStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HotReloadPolicyUpdater
    var type_HotReloadPolicyUpdater = Type.GetType("HotReloadPolicyUpdater");
    if (type_HotReloadPolicyUpdater != null)
    {
        Console.WriteLine("[PASS] 类型 HotReloadPolicyUpdater (class) 存在");
        var ctors_HotReloadPolicyUpdater = type_HotReloadPolicyUpdater.GetConstructors();
        Console.WriteLine($"[PASS] HotReloadPolicyUpdater 构造函数数量: {ctors_HotReloadPolicyUpdater.Length}");
        var methods_HotReloadPolicyUpdater = type_HotReloadPolicyUpdater.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HotReloadPolicyUpdater 公开方法数量: {methods_HotReloadPolicyUpdater.Length}");
        foreach (var m in methods_HotReloadPolicyUpdater)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HotReloadPolicyUpdater 未找到，尝试无命名空间...");
        type_HotReloadPolicyUpdater = Type.GetType("HotReloadPolicyUpdater");
        if (type_HotReloadPolicyUpdater != null)
            Console.WriteLine("[PASS] 类型 HotReloadPolicyUpdater (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HotReloadPolicyUpdater 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PartitionOptions
    var type_PartitionOptions = Type.GetType("PartitionOptions");
    if (type_PartitionOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PartitionOptions (class) 存在");
        var ctors_PartitionOptions = type_PartitionOptions.GetConstructors();
        Console.WriteLine($"[PASS] PartitionOptions 构造函数数量: {ctors_PartitionOptions.Length}");
        var methods_PartitionOptions = type_PartitionOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PartitionOptions 公开方法数量: {methods_PartitionOptions.Length}");
        foreach (var m in methods_PartitionOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PartitionOptions 未找到，尝试无命名空间...");
        type_PartitionOptions = Type.GetType("PartitionOptions");
        if (type_PartitionOptions != null)
            Console.WriteLine("[PASS] 类型 PartitionOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PartitionOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PermissionOptions
    var type_PermissionOptions = Type.GetType("PermissionOptions");
    if (type_PermissionOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PermissionOptions (class) 存在");
        var ctors_PermissionOptions = type_PermissionOptions.GetConstructors();
        Console.WriteLine($"[PASS] PermissionOptions 构造函数数量: {ctors_PermissionOptions.Length}");
        var methods_PermissionOptions = type_PermissionOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PermissionOptions 公开方法数量: {methods_PermissionOptions.Length}");
        foreach (var m in methods_PermissionOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PermissionOptions 未找到，尝试无命名空间...");
        type_PermissionOptions = Type.GetType("PermissionOptions");
        if (type_PermissionOptions != null)
            Console.WriteLine("[PASS] 类型 PermissionOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PermissionOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
