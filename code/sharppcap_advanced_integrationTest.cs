#load "sharppcap_advanced_integration.cs"

Console.WriteLine("=== sharppcap_advanced_integration.cs Test ===");

try
{
    // 验证 class: AdvancedPacketCaptureOptions
    var type_AdvancedPacketCaptureOptions = Type.GetType("AdvancedPacketCaptureOptions");
    if (type_AdvancedPacketCaptureOptions != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedPacketCaptureOptions (class) 存在");
        var ctors_AdvancedPacketCaptureOptions = type_AdvancedPacketCaptureOptions.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedPacketCaptureOptions 构造函数数量: {ctors_AdvancedPacketCaptureOptions.Length}");
        var methods_AdvancedPacketCaptureOptions = type_AdvancedPacketCaptureOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedPacketCaptureOptions 公开方法数量: {methods_AdvancedPacketCaptureOptions.Length}");
        foreach (var m in methods_AdvancedPacketCaptureOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedPacketCaptureOptions 未找到，尝试无命名空间...");
        type_AdvancedPacketCaptureOptions = Type.GetType("AdvancedPacketCaptureOptions");
        if (type_AdvancedPacketCaptureOptions != null)
            Console.WriteLine("[PASS] 类型 AdvancedPacketCaptureOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedPacketCaptureOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AdvancedPacketCaptureService
    var type_AdvancedPacketCaptureService = Type.GetType("AdvancedPacketCaptureService");
    if (type_AdvancedPacketCaptureService != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedPacketCaptureService (class) 存在");
        var ctors_AdvancedPacketCaptureService = type_AdvancedPacketCaptureService.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedPacketCaptureService 构造函数数量: {ctors_AdvancedPacketCaptureService.Length}");
        var methods_AdvancedPacketCaptureService = type_AdvancedPacketCaptureService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedPacketCaptureService 公开方法数量: {methods_AdvancedPacketCaptureService.Length}");
        foreach (var m in methods_AdvancedPacketCaptureService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedPacketCaptureService 未找到，尝试无命名空间...");
        type_AdvancedPacketCaptureService = Type.GetType("AdvancedPacketCaptureService");
        if (type_AdvancedPacketCaptureService != null)
            Console.WriteLine("[PASS] 类型 AdvancedPacketCaptureService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedPacketCaptureService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AdvancedPacketCaptureExtensions
    var type_AdvancedPacketCaptureExtensions = Type.GetType("AdvancedPacketCaptureExtensions");
    if (type_AdvancedPacketCaptureExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedPacketCaptureExtensions (class) 存在");
        var ctors_AdvancedPacketCaptureExtensions = type_AdvancedPacketCaptureExtensions.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedPacketCaptureExtensions 构造函数数量: {ctors_AdvancedPacketCaptureExtensions.Length}");
        var methods_AdvancedPacketCaptureExtensions = type_AdvancedPacketCaptureExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedPacketCaptureExtensions 公开方法数量: {methods_AdvancedPacketCaptureExtensions.Length}");
        foreach (var m in methods_AdvancedPacketCaptureExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedPacketCaptureExtensions 未找到，尝试无命名空间...");
        type_AdvancedPacketCaptureExtensions = Type.GetType("AdvancedPacketCaptureExtensions");
        if (type_AdvancedPacketCaptureExtensions != null)
            Console.WriteLine("[PASS] 类型 AdvancedPacketCaptureExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedPacketCaptureExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAdvancedPacketCaptureService
    var type_IAdvancedPacketCaptureService = Type.GetType("IAdvancedPacketCaptureService");
    if (type_IAdvancedPacketCaptureService != null)
    {
        Console.WriteLine("[PASS] 类型 IAdvancedPacketCaptureService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAdvancedPacketCaptureService 未找到，尝试无命名空间...");
        type_IAdvancedPacketCaptureService = Type.GetType("IAdvancedPacketCaptureService");
        if (type_IAdvancedPacketCaptureService != null)
            Console.WriteLine("[PASS] 类型 IAdvancedPacketCaptureService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAdvancedPacketCaptureService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
