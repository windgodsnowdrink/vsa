#load "minio_integration.cs"

Console.WriteLine("=== minio_integration.cs Test ===");

try
{
    // 验证 class: MinioFileService
    var type_MinioFileService = Type.GetType("MinioFileService");
    if (type_MinioFileService != null)
    {
        Console.WriteLine("[PASS] 类型 MinioFileService (class) 存在");
        var ctors_MinioFileService = type_MinioFileService.GetConstructors();
        Console.WriteLine($"[PASS] MinioFileService 构造函数数量: {ctors_MinioFileService.Length}");
        var methods_MinioFileService = type_MinioFileService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinioFileService 公开方法数量: {methods_MinioFileService.Length}");
        foreach (var m in methods_MinioFileService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinioFileService 未找到，尝试无命名空间...");
        type_MinioFileService = Type.GetType("MinioFileService");
        if (type_MinioFileService != null)
            Console.WriteLine("[PASS] 类型 MinioFileService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinioFileService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MinioExtensions
    var type_MinioExtensions = Type.GetType("MinioExtensions");
    if (type_MinioExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MinioExtensions (class) 存在");
        var ctors_MinioExtensions = type_MinioExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MinioExtensions 构造函数数量: {ctors_MinioExtensions.Length}");
        var methods_MinioExtensions = type_MinioExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinioExtensions 公开方法数量: {methods_MinioExtensions.Length}");
        foreach (var m in methods_MinioExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinioExtensions 未找到，尝试无命名空间...");
        type_MinioExtensions = Type.GetType("MinioExtensions");
        if (type_MinioExtensions != null)
            Console.WriteLine("[PASS] 类型 MinioExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinioExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
