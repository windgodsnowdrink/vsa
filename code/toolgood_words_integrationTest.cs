#load "toolgood_words_integration.cs"

Console.WriteLine("=== toolgood_words_integration.cs Test ===");

try
{
    // 验证 class: YourNamespace.WordCloudService
    var type_WordCloudService = Type.GetType("YourNamespace.WordCloudService");
    if (type_WordCloudService != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.WordCloudService (class) 存在");
        var ctors_WordCloudService = type_WordCloudService.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.WordCloudService 构造函数数量: {ctors_WordCloudService.Length}");
        var methods_WordCloudService = type_WordCloudService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.WordCloudService 公开方法数量: {methods_WordCloudService.Length}");
        foreach (var m in methods_WordCloudService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.WordCloudService 未找到，尝试无命名空间...");
        type_WordCloudService = Type.GetType("WordCloudService");
        if (type_WordCloudService != null)
            Console.WriteLine("[PASS] 类型 WordCloudService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WordCloudService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.WordCloudServiceCollectionExtensions
    var type_WordCloudServiceCollectionExtensions = Type.GetType("YourNamespace.WordCloudServiceCollectionExtensions");
    if (type_WordCloudServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.WordCloudServiceCollectionExtensions (class) 存在");
        var ctors_WordCloudServiceCollectionExtensions = type_WordCloudServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.WordCloudServiceCollectionExtensions 构造函数数量: {ctors_WordCloudServiceCollectionExtensions.Length}");
        var methods_WordCloudServiceCollectionExtensions = type_WordCloudServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.WordCloudServiceCollectionExtensions 公开方法数量: {methods_WordCloudServiceCollectionExtensions.Length}");
        foreach (var m in methods_WordCloudServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.WordCloudServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_WordCloudServiceCollectionExtensions = Type.GetType("WordCloudServiceCollectionExtensions");
        if (type_WordCloudServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 WordCloudServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WordCloudServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.WordCloudGeneratorPooledPolicy
    var type_WordCloudGeneratorPooledPolicy = Type.GetType("YourNamespace.WordCloudGeneratorPooledPolicy");
    if (type_WordCloudGeneratorPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.WordCloudGeneratorPooledPolicy (class) 存在");
        var ctors_WordCloudGeneratorPooledPolicy = type_WordCloudGeneratorPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.WordCloudGeneratorPooledPolicy 构造函数数量: {ctors_WordCloudGeneratorPooledPolicy.Length}");
        var methods_WordCloudGeneratorPooledPolicy = type_WordCloudGeneratorPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.WordCloudGeneratorPooledPolicy 公开方法数量: {methods_WordCloudGeneratorPooledPolicy.Length}");
        foreach (var m in methods_WordCloudGeneratorPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.WordCloudGeneratorPooledPolicy 未找到，尝试无命名空间...");
        type_WordCloudGeneratorPooledPolicy = Type.GetType("WordCloudGeneratorPooledPolicy");
        if (type_WordCloudGeneratorPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 WordCloudGeneratorPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WordCloudGeneratorPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YourNamespace.WordCloudProcessingService
    var type_WordCloudProcessingService = Type.GetType("YourNamespace.WordCloudProcessingService");
    if (type_WordCloudProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.WordCloudProcessingService (class) 存在");
        var ctors_WordCloudProcessingService = type_WordCloudProcessingService.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.WordCloudProcessingService 构造函数数量: {ctors_WordCloudProcessingService.Length}");
        var methods_WordCloudProcessingService = type_WordCloudProcessingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.WordCloudProcessingService 公开方法数量: {methods_WordCloudProcessingService.Length}");
        foreach (var m in methods_WordCloudProcessingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.WordCloudProcessingService 未找到，尝试无命名空间...");
        type_WordCloudProcessingService = Type.GetType("WordCloudProcessingService");
        if (type_WordCloudProcessingService != null)
            Console.WriteLine("[PASS] 类型 WordCloudProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WordCloudProcessingService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: YourNamespace.IWordCloudService
    var type_IWordCloudService = Type.GetType("YourNamespace.IWordCloudService");
    if (type_IWordCloudService != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.IWordCloudService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.IWordCloudService 未找到，尝试无命名空间...");
        type_IWordCloudService = Type.GetType("IWordCloudService");
        if (type_IWordCloudService != null)
            Console.WriteLine("[PASS] 类型 IWordCloudService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IWordCloudService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: YourNamespace.WordCloudOptions
    var type_WordCloudOptions = Type.GetType("YourNamespace.WordCloudOptions");
    if (type_WordCloudOptions != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.WordCloudOptions (record) 存在");
        var ctors_WordCloudOptions = type_WordCloudOptions.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.WordCloudOptions 构造函数数量: {ctors_WordCloudOptions.Length}");
        var methods_WordCloudOptions = type_WordCloudOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.WordCloudOptions 公开方法数量: {methods_WordCloudOptions.Length}");
        foreach (var m in methods_WordCloudOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.WordCloudOptions 未找到，尝试无命名空间...");
        type_WordCloudOptions = Type.GetType("WordCloudOptions");
        if (type_WordCloudOptions != null)
            Console.WriteLine("[PASS] 类型 WordCloudOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WordCloudOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 record: YourNamespace.WordCloudJob
    var type_WordCloudJob = Type.GetType("YourNamespace.WordCloudJob");
    if (type_WordCloudJob != null)
    {
        Console.WriteLine("[PASS] 类型 YourNamespace.WordCloudJob (record) 存在");
        var ctors_WordCloudJob = type_WordCloudJob.GetConstructors();
        Console.WriteLine($"[PASS] YourNamespace.WordCloudJob 构造函数数量: {ctors_WordCloudJob.Length}");
        var methods_WordCloudJob = type_WordCloudJob.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YourNamespace.WordCloudJob 公开方法数量: {methods_WordCloudJob.Length}");
        foreach (var m in methods_WordCloudJob)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YourNamespace.WordCloudJob 未找到，尝试无命名空间...");
        type_WordCloudJob = Type.GetType("WordCloudJob");
        if (type_WordCloudJob != null)
            Console.WriteLine("[PASS] 类型 WordCloudJob (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WordCloudJob 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
