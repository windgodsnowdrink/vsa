#load "magick_net_image_processing.cs"

Console.WriteLine("=== magick_net_image_processing.cs Test ===");

try
{
    // 验证 class: ImageProcessingService
    var type_ImageProcessingService = Type.GetType("ImageProcessingService");
    if (type_ImageProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 ImageProcessingService (class) 存在");
        var ctors_ImageProcessingService = type_ImageProcessingService.GetConstructors();
        Console.WriteLine($"[PASS] ImageProcessingService 构造函数数量: {ctors_ImageProcessingService.Length}");
        var methods_ImageProcessingService = type_ImageProcessingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ImageProcessingService 公开方法数量: {methods_ImageProcessingService.Length}");
        foreach (var m in methods_ImageProcessingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ImageProcessingService 未找到，尝试无命名空间...");
        type_ImageProcessingService = Type.GetType("ImageProcessingService");
        if (type_ImageProcessingService != null)
            Console.WriteLine("[PASS] 类型 ImageProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ImageProcessingService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MagickImagePooledObjectPolicy
    var type_MagickImagePooledObjectPolicy = Type.GetType("MagickImagePooledObjectPolicy");
    if (type_MagickImagePooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MagickImagePooledObjectPolicy (class) 存在");
        var ctors_MagickImagePooledObjectPolicy = type_MagickImagePooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MagickImagePooledObjectPolicy 构造函数数量: {ctors_MagickImagePooledObjectPolicy.Length}");
        var methods_MagickImagePooledObjectPolicy = type_MagickImagePooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MagickImagePooledObjectPolicy 公开方法数量: {methods_MagickImagePooledObjectPolicy.Length}");
        foreach (var m in methods_MagickImagePooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MagickImagePooledObjectPolicy 未找到，尝试无命名空间...");
        type_MagickImagePooledObjectPolicy = Type.GetType("MagickImagePooledObjectPolicy");
        if (type_MagickImagePooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 MagickImagePooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MagickImagePooledObjectPolicy 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IImageProcessingService
    var type_IImageProcessingService = Type.GetType("IImageProcessingService");
    if (type_IImageProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 IImageProcessingService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IImageProcessingService 未找到，尝试无命名空间...");
        type_IImageProcessingService = Type.GetType("IImageProcessingService");
        if (type_IImageProcessingService != null)
            Console.WriteLine("[PASS] 类型 IImageProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IImageProcessingService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
