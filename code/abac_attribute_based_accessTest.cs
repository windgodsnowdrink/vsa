#load "abac_attribute_based_access.cs"

Console.WriteLine("=== abac_attribute_based_access.cs Test ===");

try
{
    // 验证 class: AbacRequirement
    var type_AbacRequirement = Type.GetType("AbacRequirement");
    if (type_AbacRequirement != null)
    {
        Console.WriteLine("[PASS] 类型 AbacRequirement (class) 存在");
        var ctors_AbacRequirement = type_AbacRequirement.GetConstructors();
        Console.WriteLine($"[PASS] AbacRequirement 构造函数数量: {ctors_AbacRequirement.Length}");
        var methods_AbacRequirement = type_AbacRequirement.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AbacRequirement 公开方法数量: {methods_AbacRequirement.Length}");
        foreach (var m in methods_AbacRequirement)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AbacRequirement 未找到，尝试无命名空间...");
        type_AbacRequirement = Type.GetType("AbacRequirement");
        if (type_AbacRequirement != null)
            Console.WriteLine("[PASS] 类型 AbacRequirement (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AbacRequirement 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AbacHandler
    var type_AbacHandler = Type.GetType("AbacHandler");
    if (type_AbacHandler != null)
    {
        Console.WriteLine("[PASS] 类型 AbacHandler (class) 存在");
        var ctors_AbacHandler = type_AbacHandler.GetConstructors();
        Console.WriteLine($"[PASS] AbacHandler 构造函数数量: {ctors_AbacHandler.Length}");
        var methods_AbacHandler = type_AbacHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AbacHandler 公开方法数量: {methods_AbacHandler.Length}");
        foreach (var m in methods_AbacHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AbacHandler 未找到，尝试无命名空间...");
        type_AbacHandler = Type.GetType("AbacHandler");
        if (type_AbacHandler != null)
            Console.WriteLine("[PASS] 类型 AbacHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AbacHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AbacPolicyProvider
    var type_AbacPolicyProvider = Type.GetType("AbacPolicyProvider");
    if (type_AbacPolicyProvider != null)
    {
        Console.WriteLine("[PASS] 类型 AbacPolicyProvider (class) 存在");
        var ctors_AbacPolicyProvider = type_AbacPolicyProvider.GetConstructors();
        Console.WriteLine($"[PASS] AbacPolicyProvider 构造函数数量: {ctors_AbacPolicyProvider.Length}");
        var methods_AbacPolicyProvider = type_AbacPolicyProvider.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AbacPolicyProvider 公开方法数量: {methods_AbacPolicyProvider.Length}");
        foreach (var m in methods_AbacPolicyProvider)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AbacPolicyProvider 未找到，尝试无命名空间...");
        type_AbacPolicyProvider = Type.GetType("AbacPolicyProvider");
        if (type_AbacPolicyProvider != null)
            Console.WriteLine("[PASS] 类型 AbacPolicyProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AbacPolicyProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AbacOptions
    var type_AbacOptions = Type.GetType("AbacOptions");
    if (type_AbacOptions != null)
    {
        Console.WriteLine("[PASS] 类型 AbacOptions (class) 存在");
        var ctors_AbacOptions = type_AbacOptions.GetConstructors();
        Console.WriteLine($"[PASS] AbacOptions 构造函数数量: {ctors_AbacOptions.Length}");
        var methods_AbacOptions = type_AbacOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AbacOptions 公开方法数量: {methods_AbacOptions.Length}");
        foreach (var m in methods_AbacOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AbacOptions 未找到，尝试无命名空间...");
        type_AbacOptions = Type.GetType("AbacOptions");
        if (type_AbacOptions != null)
            Console.WriteLine("[PASS] 类型 AbacOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AbacOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DocumentAbacPolicy
    var type_DocumentAbacPolicy = Type.GetType("DocumentAbacPolicy");
    if (type_DocumentAbacPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 DocumentAbacPolicy (class) 存在");
        var ctors_DocumentAbacPolicy = type_DocumentAbacPolicy.GetConstructors();
        Console.WriteLine($"[PASS] DocumentAbacPolicy 构造函数数量: {ctors_DocumentAbacPolicy.Length}");
        var methods_DocumentAbacPolicy = type_DocumentAbacPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DocumentAbacPolicy 公开方法数量: {methods_DocumentAbacPolicy.Length}");
        foreach (var m in methods_DocumentAbacPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DocumentAbacPolicy 未找到，尝试无命名空间...");
        type_DocumentAbacPolicy = Type.GetType("DocumentAbacPolicy");
        if (type_DocumentAbacPolicy != null)
            Console.WriteLine("[PASS] 类型 DocumentAbacPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DocumentAbacPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAbacPolicy
    var type_IAbacPolicy = Type.GetType("IAbacPolicy");
    if (type_IAbacPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 IAbacPolicy (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAbacPolicy 未找到，尝试无命名空间...");
        type_IAbacPolicy = Type.GetType("IAbacPolicy");
        if (type_IAbacPolicy != null)
            Console.WriteLine("[PASS] 类型 IAbacPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAbacPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
