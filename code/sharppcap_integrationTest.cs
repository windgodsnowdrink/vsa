#load "sharppcap_integration.cs"

Console.WriteLine("=== sharppcap_integration.cs Test ===");

try
{
    // 验证 class: PacketCaptureOptions
    var type_PacketCaptureOptions = Type.GetType("PacketCaptureOptions");
    if (type_PacketCaptureOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PacketCaptureOptions (class) 存在");
        var ctors_PacketCaptureOptions = type_PacketCaptureOptions.GetConstructors();
        Console.WriteLine($"[PASS] PacketCaptureOptions 构造函数数量: {ctors_PacketCaptureOptions.Length}");
        var methods_PacketCaptureOptions = type_PacketCaptureOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PacketCaptureOptions 公开方法数量: {methods_PacketCaptureOptions.Length}");
        foreach (var m in methods_PacketCaptureOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PacketCaptureOptions 未找到，尝试无命名空间...");
        type_PacketCaptureOptions = Type.GetType("PacketCaptureOptions");
        if (type_PacketCaptureOptions != null)
            Console.WriteLine("[PASS] 类型 PacketCaptureOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PacketCaptureOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PacketCaptureService
    var type_PacketCaptureService = Type.GetType("PacketCaptureService");
    if (type_PacketCaptureService != null)
    {
        Console.WriteLine("[PASS] 类型 PacketCaptureService (class) 存在");
        var ctors_PacketCaptureService = type_PacketCaptureService.GetConstructors();
        Console.WriteLine($"[PASS] PacketCaptureService 构造函数数量: {ctors_PacketCaptureService.Length}");
        var methods_PacketCaptureService = type_PacketCaptureService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PacketCaptureService 公开方法数量: {methods_PacketCaptureService.Length}");
        foreach (var m in methods_PacketCaptureService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PacketCaptureService 未找到，尝试无命名空间...");
        type_PacketCaptureService = Type.GetType("PacketCaptureService");
        if (type_PacketCaptureService != null)
            Console.WriteLine("[PASS] 类型 PacketCaptureService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PacketCaptureService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PacketCaptureExtensions
    var type_PacketCaptureExtensions = Type.GetType("PacketCaptureExtensions");
    if (type_PacketCaptureExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PacketCaptureExtensions (class) 存在");
        var ctors_PacketCaptureExtensions = type_PacketCaptureExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PacketCaptureExtensions 构造函数数量: {ctors_PacketCaptureExtensions.Length}");
        var methods_PacketCaptureExtensions = type_PacketCaptureExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PacketCaptureExtensions 公开方法数量: {methods_PacketCaptureExtensions.Length}");
        foreach (var m in methods_PacketCaptureExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PacketCaptureExtensions 未找到，尝试无命名空间...");
        type_PacketCaptureExtensions = Type.GetType("PacketCaptureExtensions");
        if (type_PacketCaptureExtensions != null)
            Console.WriteLine("[PASS] 类型 PacketCaptureExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PacketCaptureExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPacketCaptureService
    var type_IPacketCaptureService = Type.GetType("IPacketCaptureService");
    if (type_IPacketCaptureService != null)
    {
        Console.WriteLine("[PASS] 类型 IPacketCaptureService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPacketCaptureService 未找到，尝试无命名空间...");
        type_IPacketCaptureService = Type.GetType("IPacketCaptureService");
        if (type_IPacketCaptureService != null)
            Console.WriteLine("[PASS] 类型 IPacketCaptureService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPacketCaptureService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
