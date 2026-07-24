#load "mdns_integration.cs"

Console.WriteLine("=== mdns_integration.cs Test ===");

try
{
    // 验证 class: MdnsOptions
    var type_MdnsOptions = Type.GetType("MdnsOptions");
    if (type_MdnsOptions != null)
    {
        Console.WriteLine("[PASS] 类型 MdnsOptions (class) 存在");
        var ctors_MdnsOptions = type_MdnsOptions.GetConstructors();
        Console.WriteLine($"[PASS] MdnsOptions 构造函数数量: {ctors_MdnsOptions.Length}");
        var methods_MdnsOptions = type_MdnsOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MdnsOptions 公开方法数量: {methods_MdnsOptions.Length}");
        foreach (var m in methods_MdnsOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MdnsOptions 未找到，尝试无命名空间...");
        type_MdnsOptions = Type.GetType("MdnsOptions");
        if (type_MdnsOptions != null)
            Console.WriteLine("[PASS] 类型 MdnsOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MdnsOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MdnsService
    var type_MdnsService = Type.GetType("MdnsService");
    if (type_MdnsService != null)
    {
        Console.WriteLine("[PASS] 类型 MdnsService (class) 存在");
        var ctors_MdnsService = type_MdnsService.GetConstructors();
        Console.WriteLine($"[PASS] MdnsService 构造函数数量: {ctors_MdnsService.Length}");
        var methods_MdnsService = type_MdnsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MdnsService 公开方法数量: {methods_MdnsService.Length}");
        foreach (var m in methods_MdnsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MdnsService 未找到，尝试无命名空间...");
        type_MdnsService = Type.GetType("MdnsService");
        if (type_MdnsService != null)
            Console.WriteLine("[PASS] 类型 MdnsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MdnsService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MdnsExtensions
    var type_MdnsExtensions = Type.GetType("MdnsExtensions");
    if (type_MdnsExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MdnsExtensions (class) 存在");
        var ctors_MdnsExtensions = type_MdnsExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MdnsExtensions 构造函数数量: {ctors_MdnsExtensions.Length}");
        var methods_MdnsExtensions = type_MdnsExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MdnsExtensions 公开方法数量: {methods_MdnsExtensions.Length}");
        foreach (var m in methods_MdnsExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MdnsExtensions 未找到，尝试无命名空间...");
        type_MdnsExtensions = Type.GetType("MdnsExtensions");
        if (type_MdnsExtensions != null)
            Console.WriteLine("[PASS] 类型 MdnsExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MdnsExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMdnsService
    var type_IMdnsService = Type.GetType("IMdnsService");
    if (type_IMdnsService != null)
    {
        Console.WriteLine("[PASS] 类型 IMdnsService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMdnsService 未找到，尝试无命名空间...");
        type_IMdnsService = Type.GetType("IMdnsService");
        if (type_IMdnsService != null)
            Console.WriteLine("[PASS] 类型 IMdnsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMdnsService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
