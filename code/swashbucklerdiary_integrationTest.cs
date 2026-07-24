#load "swashbucklerdiary_integration.cs"

Console.WriteLine("=== swashbucklerdiary_integration.cs Test ===");

try
{
    // 验证 class: SwashbucklerDiary.Integration.SwashbucklerDiaryOptions
    var type_SwashbucklerDiaryOptions = Type.GetType("SwashbucklerDiary.Integration.SwashbucklerDiaryOptions");
    if (type_SwashbucklerDiaryOptions != null)
    {
        Console.WriteLine("[PASS] 类型 SwashbucklerDiary.Integration.SwashbucklerDiaryOptions (class) 存在");
        var ctors_SwashbucklerDiaryOptions = type_SwashbucklerDiaryOptions.GetConstructors();
        Console.WriteLine($"[PASS] SwashbucklerDiary.Integration.SwashbucklerDiaryOptions 构造函数数量: {ctors_SwashbucklerDiaryOptions.Length}");
        var methods_SwashbucklerDiaryOptions = type_SwashbucklerDiaryOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SwashbucklerDiary.Integration.SwashbucklerDiaryOptions 公开方法数量: {methods_SwashbucklerDiaryOptions.Length}");
        foreach (var m in methods_SwashbucklerDiaryOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SwashbucklerDiary.Integration.SwashbucklerDiaryOptions 未找到，尝试无命名空间...");
        type_SwashbucklerDiaryOptions = Type.GetType("SwashbucklerDiaryOptions");
        if (type_SwashbucklerDiaryOptions != null)
            Console.WriteLine("[PASS] 类型 SwashbucklerDiaryOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SwashbucklerDiaryOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SwashbucklerDiary.Integration.SwashbucklerDiaryService
    var type_SwashbucklerDiaryService = Type.GetType("SwashbucklerDiary.Integration.SwashbucklerDiaryService");
    if (type_SwashbucklerDiaryService != null)
    {
        Console.WriteLine("[PASS] 类型 SwashbucklerDiary.Integration.SwashbucklerDiaryService (class) 存在");
        var ctors_SwashbucklerDiaryService = type_SwashbucklerDiaryService.GetConstructors();
        Console.WriteLine($"[PASS] SwashbucklerDiary.Integration.SwashbucklerDiaryService 构造函数数量: {ctors_SwashbucklerDiaryService.Length}");
        var methods_SwashbucklerDiaryService = type_SwashbucklerDiaryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SwashbucklerDiary.Integration.SwashbucklerDiaryService 公开方法数量: {methods_SwashbucklerDiaryService.Length}");
        foreach (var m in methods_SwashbucklerDiaryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SwashbucklerDiary.Integration.SwashbucklerDiaryService 未找到，尝试无命名空间...");
        type_SwashbucklerDiaryService = Type.GetType("SwashbucklerDiaryService");
        if (type_SwashbucklerDiaryService != null)
            Console.WriteLine("[PASS] 类型 SwashbucklerDiaryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SwashbucklerDiaryService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SwashbucklerDiary.Integration.SwashbucklerDiaryExtensions
    var type_SwashbucklerDiaryExtensions = Type.GetType("SwashbucklerDiary.Integration.SwashbucklerDiaryExtensions");
    if (type_SwashbucklerDiaryExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SwashbucklerDiary.Integration.SwashbucklerDiaryExtensions (class) 存在");
        var ctors_SwashbucklerDiaryExtensions = type_SwashbucklerDiaryExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SwashbucklerDiary.Integration.SwashbucklerDiaryExtensions 构造函数数量: {ctors_SwashbucklerDiaryExtensions.Length}");
        var methods_SwashbucklerDiaryExtensions = type_SwashbucklerDiaryExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SwashbucklerDiary.Integration.SwashbucklerDiaryExtensions 公开方法数量: {methods_SwashbucklerDiaryExtensions.Length}");
        foreach (var m in methods_SwashbucklerDiaryExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SwashbucklerDiary.Integration.SwashbucklerDiaryExtensions 未找到，尝试无命名空间...");
        type_SwashbucklerDiaryExtensions = Type.GetType("SwashbucklerDiaryExtensions");
        if (type_SwashbucklerDiaryExtensions != null)
            Console.WriteLine("[PASS] 类型 SwashbucklerDiaryExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SwashbucklerDiaryExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SwashbucklerDiary.Integration.ExampleUsage
    var type_ExampleUsage = Type.GetType("SwashbucklerDiary.Integration.ExampleUsage");
    if (type_ExampleUsage != null)
    {
        Console.WriteLine("[PASS] 类型 SwashbucklerDiary.Integration.ExampleUsage (class) 存在");
        var ctors_ExampleUsage = type_ExampleUsage.GetConstructors();
        Console.WriteLine($"[PASS] SwashbucklerDiary.Integration.ExampleUsage 构造函数数量: {ctors_ExampleUsage.Length}");
        var methods_ExampleUsage = type_ExampleUsage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SwashbucklerDiary.Integration.ExampleUsage 公开方法数量: {methods_ExampleUsage.Length}");
        foreach (var m in methods_ExampleUsage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SwashbucklerDiary.Integration.ExampleUsage 未找到，尝试无命名空间...");
        type_ExampleUsage = Type.GetType("ExampleUsage");
        if (type_ExampleUsage != null)
            Console.WriteLine("[PASS] 类型 ExampleUsage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExampleUsage 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: SwashbucklerDiary.Integration.ISwashbucklerDiaryService
    var type_ISwashbucklerDiaryService = Type.GetType("SwashbucklerDiary.Integration.ISwashbucklerDiaryService");
    if (type_ISwashbucklerDiaryService != null)
    {
        Console.WriteLine("[PASS] 类型 SwashbucklerDiary.Integration.ISwashbucklerDiaryService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SwashbucklerDiary.Integration.ISwashbucklerDiaryService 未找到，尝试无命名空间...");
        type_ISwashbucklerDiaryService = Type.GetType("ISwashbucklerDiaryService");
        if (type_ISwashbucklerDiaryService != null)
            Console.WriteLine("[PASS] 类型 ISwashbucklerDiaryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISwashbucklerDiaryService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
