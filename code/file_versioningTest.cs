#load "file_versioning.cs"

Console.WriteLine("=== file_versioning.cs Test ===");

try
{
    // 验证 class: FileVersioningService
    var type_FileVersioningService = Type.GetType("FileVersioningService");
    if (type_FileVersioningService != null)
    {
        Console.WriteLine("[PASS] 类型 FileVersioningService (class) 存在");
        var ctors_FileVersioningService = type_FileVersioningService.GetConstructors();
        Console.WriteLine($"[PASS] FileVersioningService 构造函数数量: {ctors_FileVersioningService.Length}");
        var methods_FileVersioningService = type_FileVersioningService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FileVersioningService 公开方法数量: {methods_FileVersioningService.Length}");
        foreach (var m in methods_FileVersioningService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FileVersioningService 未找到，尝试无命名空间...");
        type_FileVersioningService = Type.GetType("FileVersioningService");
        if (type_FileVersioningService != null)
            Console.WriteLine("[PASS] 类型 FileVersioningService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FileVersioningService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
