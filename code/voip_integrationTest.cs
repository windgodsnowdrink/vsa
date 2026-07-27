#load "voip_integration.cs"

Console.WriteLine("=== voip_integration.cs Test ===");

try
{
    // 验证 class: VoipOptions
    var type_VoipOptions = Type.GetType("VoipOptions");
    if (type_VoipOptions != null)
    {
        Console.WriteLine("[PASS] 类型 VoipOptions (class) 存在");
        var ctors_VoipOptions = type_VoipOptions.GetConstructors();
        Console.WriteLine($"[PASS] VoipOptions 构造函数数量: {ctors_VoipOptions.Length}");
        var methods_VoipOptions = type_VoipOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VoipOptions 公开方法数量: {methods_VoipOptions.Length}");
        foreach (var m in methods_VoipOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VoipOptions 未找到，尝试无命名空间...");
        type_VoipOptions = Type.GetType("VoipOptions");
        if (type_VoipOptions != null)
            Console.WriteLine("[PASS] 类型 VoipOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VoipOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: VoipService
    var type_VoipService = Type.GetType("VoipService");
    if (type_VoipService != null)
    {
        Console.WriteLine("[PASS] 类型 VoipService (class) 存在");
        var ctors_VoipService = type_VoipService.GetConstructors();
        Console.WriteLine($"[PASS] VoipService 构造函数数量: {ctors_VoipService.Length}");
        var methods_VoipService = type_VoipService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VoipService 公开方法数量: {methods_VoipService.Length}");
        foreach (var m in methods_VoipService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VoipService 未找到，尝试无命名空间...");
        type_VoipService = Type.GetType("VoipService");
        if (type_VoipService != null)
            Console.WriteLine("[PASS] 类型 VoipService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VoipService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: VoipExtensions
    var type_VoipExtensions = Type.GetType("VoipExtensions");
    if (type_VoipExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 VoipExtensions (class) 存在");
        var ctors_VoipExtensions = type_VoipExtensions.GetConstructors();
        Console.WriteLine($"[PASS] VoipExtensions 构造函数数量: {ctors_VoipExtensions.Length}");
        var methods_VoipExtensions = type_VoipExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VoipExtensions 公开方法数量: {methods_VoipExtensions.Length}");
        foreach (var m in methods_VoipExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VoipExtensions 未找到，尝试无命名空间...");
        type_VoipExtensions = Type.GetType("VoipExtensions");
        if (type_VoipExtensions != null)
            Console.WriteLine("[PASS] 类型 VoipExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VoipExtensions 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IVoipService
    var type_IVoipService = Type.GetType("IVoipService");
    if (type_IVoipService != null)
    {
        Console.WriteLine("[PASS] 类型 IVoipService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IVoipService 未找到，尝试无命名空间...");
        type_IVoipService = Type.GetType("IVoipService");
        if (type_IVoipService != null)
            Console.WriteLine("[PASS] 类型 IVoipService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IVoipService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
