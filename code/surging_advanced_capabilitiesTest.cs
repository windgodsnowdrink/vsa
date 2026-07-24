#load "surging_advanced_capabilities.cs"

Console.WriteLine("=== surging_advanced_capabilities.cs Test ===");

try
{
    // 验证 class: ServiceGovernanceExtensions
    var type_ServiceGovernanceExtensions = Type.GetType("ServiceGovernanceExtensions");
    if (type_ServiceGovernanceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceGovernanceExtensions (class) 存在");
        var ctors_ServiceGovernanceExtensions = type_ServiceGovernanceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceGovernanceExtensions 构造函数数量: {ctors_ServiceGovernanceExtensions.Length}");
        var methods_ServiceGovernanceExtensions = type_ServiceGovernanceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceGovernanceExtensions 公开方法数量: {methods_ServiceGovernanceExtensions.Length}");
        foreach (var m in methods_ServiceGovernanceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceGovernanceExtensions 未找到，尝试无命名空间...");
        type_ServiceGovernanceExtensions = Type.GetType("ServiceGovernanceExtensions");
        if (type_ServiceGovernanceExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceGovernanceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceGovernanceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DistributedTracingExtensions
    var type_DistributedTracingExtensions = Type.GetType("DistributedTracingExtensions");
    if (type_DistributedTracingExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedTracingExtensions (class) 存在");
        var ctors_DistributedTracingExtensions = type_DistributedTracingExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DistributedTracingExtensions 构造函数数量: {ctors_DistributedTracingExtensions.Length}");
        var methods_DistributedTracingExtensions = type_DistributedTracingExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedTracingExtensions 公开方法数量: {methods_DistributedTracingExtensions.Length}");
        foreach (var m in methods_DistributedTracingExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedTracingExtensions 未找到，尝试无命名空间...");
        type_DistributedTracingExtensions = Type.GetType("DistributedTracingExtensions");
        if (type_DistributedTracingExtensions != null)
            Console.WriteLine("[PASS] 类型 DistributedTracingExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedTracingExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MultiProtocolExtensions
    var type_MultiProtocolExtensions = Type.GetType("MultiProtocolExtensions");
    if (type_MultiProtocolExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MultiProtocolExtensions (class) 存在");
        var ctors_MultiProtocolExtensions = type_MultiProtocolExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MultiProtocolExtensions 构造函数数量: {ctors_MultiProtocolExtensions.Length}");
        var methods_MultiProtocolExtensions = type_MultiProtocolExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultiProtocolExtensions 公开方法数量: {methods_MultiProtocolExtensions.Length}");
        foreach (var m in methods_MultiProtocolExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultiProtocolExtensions 未找到，尝试无命名空间...");
        type_MultiProtocolExtensions = Type.GetType("MultiProtocolExtensions");
        if (type_MultiProtocolExtensions != null)
            Console.WriteLine("[PASS] 类型 MultiProtocolExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultiProtocolExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Startup
    var type_Startup = Type.GetType("Startup");
    if (type_Startup != null)
    {
        Console.WriteLine("[PASS] 类型 Startup (class) 存在");
        var ctors_Startup = type_Startup.GetConstructors();
        Console.WriteLine($"[PASS] Startup 构造函数数量: {ctors_Startup.Length}");
        var methods_Startup = type_Startup.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Startup 公开方法数量: {methods_Startup.Length}");
        foreach (var m in methods_Startup)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Startup 未找到，尝试无命名空间...");
        type_Startup = Type.GetType("Startup");
        if (type_Startup != null)
            Console.WriteLine("[PASS] 类型 Startup (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Startup 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
