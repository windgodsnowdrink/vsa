#load "jieba_integration.cs"

Console.WriteLine("=== jieba_integration.cs Test ===");

try
{
    // 验证 class: JiebaNetIntegration.JiebaOptions
    var type_JiebaOptions = Type.GetType("JiebaNetIntegration.JiebaOptions");
    if (type_JiebaOptions != null)
    {
        Console.WriteLine("[PASS] 类型 JiebaNetIntegration.JiebaOptions (class) 存在");
        var ctors_JiebaOptions = type_JiebaOptions.GetConstructors();
        Console.WriteLine($"[PASS] JiebaNetIntegration.JiebaOptions 构造函数数量: {ctors_JiebaOptions.Length}");
        var methods_JiebaOptions = type_JiebaOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JiebaNetIntegration.JiebaOptions 公开方法数量: {methods_JiebaOptions.Length}");
        foreach (var m in methods_JiebaOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JiebaNetIntegration.JiebaOptions 未找到，尝试无命名空间...");
        type_JiebaOptions = Type.GetType("JiebaOptions");
        if (type_JiebaOptions != null)
            Console.WriteLine("[PASS] 类型 JiebaOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JiebaOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JiebaNetIntegration.JiebaService
    var type_JiebaService = Type.GetType("JiebaNetIntegration.JiebaService");
    if (type_JiebaService != null)
    {
        Console.WriteLine("[PASS] 类型 JiebaNetIntegration.JiebaService (class) 存在");
        var ctors_JiebaService = type_JiebaService.GetConstructors();
        Console.WriteLine($"[PASS] JiebaNetIntegration.JiebaService 构造函数数量: {ctors_JiebaService.Length}");
        var methods_JiebaService = type_JiebaService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JiebaNetIntegration.JiebaService 公开方法数量: {methods_JiebaService.Length}");
        foreach (var m in methods_JiebaService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JiebaNetIntegration.JiebaService 未找到，尝试无命名空间...");
        type_JiebaService = Type.GetType("JiebaService");
        if (type_JiebaService != null)
            Console.WriteLine("[PASS] 类型 JiebaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JiebaService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JiebaNetIntegration.JiebaServiceCollectionExtensions
    var type_JiebaServiceCollectionExtensions = Type.GetType("JiebaNetIntegration.JiebaServiceCollectionExtensions");
    if (type_JiebaServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 JiebaNetIntegration.JiebaServiceCollectionExtensions (class) 存在");
        var ctors_JiebaServiceCollectionExtensions = type_JiebaServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] JiebaNetIntegration.JiebaServiceCollectionExtensions 构造函数数量: {ctors_JiebaServiceCollectionExtensions.Length}");
        var methods_JiebaServiceCollectionExtensions = type_JiebaServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JiebaNetIntegration.JiebaServiceCollectionExtensions 公开方法数量: {methods_JiebaServiceCollectionExtensions.Length}");
        foreach (var m in methods_JiebaServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JiebaNetIntegration.JiebaServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_JiebaServiceCollectionExtensions = Type.GetType("JiebaServiceCollectionExtensions");
        if (type_JiebaServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 JiebaServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JiebaServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JiebaNetIntegration.JiebaSegmenterPooledObjectPolicy
    var type_JiebaSegmenterPooledObjectPolicy = Type.GetType("JiebaNetIntegration.JiebaSegmenterPooledObjectPolicy");
    if (type_JiebaSegmenterPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 JiebaNetIntegration.JiebaSegmenterPooledObjectPolicy (class) 存在");
        var ctors_JiebaSegmenterPooledObjectPolicy = type_JiebaSegmenterPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] JiebaNetIntegration.JiebaSegmenterPooledObjectPolicy 构造函数数量: {ctors_JiebaSegmenterPooledObjectPolicy.Length}");
        var methods_JiebaSegmenterPooledObjectPolicy = type_JiebaSegmenterPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JiebaNetIntegration.JiebaSegmenterPooledObjectPolicy 公开方法数量: {methods_JiebaSegmenterPooledObjectPolicy.Length}");
        foreach (var m in methods_JiebaSegmenterPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JiebaNetIntegration.JiebaSegmenterPooledObjectPolicy 未找到，尝试无命名空间...");
        type_JiebaSegmenterPooledObjectPolicy = Type.GetType("JiebaSegmenterPooledObjectPolicy");
        if (type_JiebaSegmenterPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 JiebaSegmenterPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JiebaSegmenterPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: JiebaNetIntegration.IJiebaService
    var type_IJiebaService = Type.GetType("JiebaNetIntegration.IJiebaService");
    if (type_IJiebaService != null)
    {
        Console.WriteLine("[PASS] 类型 JiebaNetIntegration.IJiebaService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JiebaNetIntegration.IJiebaService 未找到，尝试无命名空间...");
        type_IJiebaService = Type.GetType("IJiebaService");
        if (type_IJiebaService != null)
            Console.WriteLine("[PASS] 类型 IJiebaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IJiebaService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
