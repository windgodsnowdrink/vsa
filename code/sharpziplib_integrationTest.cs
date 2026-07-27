#load "sharpziplib_integration.cs"

Console.WriteLine("=== sharpziplib_integration.cs Test ===");

try
{
    // 验证 class: ZipOptions
    var type_ZipOptions = Type.GetType("ZipOptions");
    if (type_ZipOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ZipOptions (class) 存在");
        var ctors_ZipOptions = type_ZipOptions.GetConstructors();
        Console.WriteLine($"[PASS] ZipOptions 构造函数数量: {ctors_ZipOptions.Length}");
        var methods_ZipOptions = type_ZipOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZipOptions 公开方法数量: {methods_ZipOptions.Length}");
        foreach (var m in methods_ZipOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZipOptions 未找到，尝试无命名空间...");
        type_ZipOptions = Type.GetType("ZipOptions");
        if (type_ZipOptions != null)
            Console.WriteLine("[PASS] 类型 ZipOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZipOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZipService
    var type_ZipService = Type.GetType("ZipService");
    if (type_ZipService != null)
    {
        Console.WriteLine("[PASS] 类型 ZipService (class) 存在");
        var ctors_ZipService = type_ZipService.GetConstructors();
        Console.WriteLine($"[PASS] ZipService 构造函数数量: {ctors_ZipService.Length}");
        var methods_ZipService = type_ZipService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZipService 公开方法数量: {methods_ZipService.Length}");
        foreach (var m in methods_ZipService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZipService 未找到，尝试无命名空间...");
        type_ZipService = Type.GetType("ZipService");
        if (type_ZipService != null)
            Console.WriteLine("[PASS] 类型 ZipService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZipService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZipServiceCollectionExtensions
    var type_ZipServiceCollectionExtensions = Type.GetType("ZipServiceCollectionExtensions");
    if (type_ZipServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ZipServiceCollectionExtensions (class) 存在");
        var ctors_ZipServiceCollectionExtensions = type_ZipServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ZipServiceCollectionExtensions 构造函数数量: {ctors_ZipServiceCollectionExtensions.Length}");
        var methods_ZipServiceCollectionExtensions = type_ZipServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZipServiceCollectionExtensions 公开方法数量: {methods_ZipServiceCollectionExtensions.Length}");
        foreach (var m in methods_ZipServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZipServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ZipServiceCollectionExtensions = Type.GetType("ZipServiceCollectionExtensions");
        if (type_ZipServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ZipServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZipServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZipDemo
    var type_ZipDemo = Type.GetType("ZipDemo");
    if (type_ZipDemo != null)
    {
        Console.WriteLine("[PASS] 类型 ZipDemo (class) 存在");
        var ctors_ZipDemo = type_ZipDemo.GetConstructors();
        Console.WriteLine($"[PASS] ZipDemo 构造函数数量: {ctors_ZipDemo.Length}");
        var methods_ZipDemo = type_ZipDemo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZipDemo 公开方法数量: {methods_ZipDemo.Length}");
        foreach (var m in methods_ZipDemo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZipDemo 未找到，尝试无命名空间...");
        type_ZipDemo = Type.GetType("ZipDemo");
        if (type_ZipDemo != null)
            Console.WriteLine("[PASS] 类型 ZipDemo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZipDemo 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IZipService
    var type_IZipService = Type.GetType("IZipService");
    if (type_IZipService != null)
    {
        Console.WriteLine("[PASS] 类型 IZipService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IZipService 未找到，尝试无命名空间...");
        type_IZipService = Type.GetType("IZipService");
        if (type_IZipService != null)
            Console.WriteLine("[PASS] 类型 IZipService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IZipService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
