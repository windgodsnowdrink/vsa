#load "version_metadata_service.cs"

Console.WriteLine("=== version_metadata_service.cs Test ===");

try
{
    // 验证 class: VersionMetadataService
    var type_VersionMetadataService = Type.GetType("VersionMetadataService");
    if (type_VersionMetadataService != null)
    {
        Console.WriteLine("[PASS] 类型 VersionMetadataService (class) 存在");
        var ctors_VersionMetadataService = type_VersionMetadataService.GetConstructors();
        Console.WriteLine($"[PASS] VersionMetadataService 构造函数数量: {ctors_VersionMetadataService.Length}");
        var methods_VersionMetadataService = type_VersionMetadataService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VersionMetadataService 公开方法数量: {methods_VersionMetadataService.Length}");
        foreach (var m in methods_VersionMetadataService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VersionMetadataService 未找到，尝试无命名空间...");
        type_VersionMetadataService = Type.GetType("VersionMetadataService");
        if (type_VersionMetadataService != null)
            Console.WriteLine("[PASS] 类型 VersionMetadataService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VersionMetadataService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: VersionMetadata
    var type_VersionMetadata = Type.GetType("VersionMetadata");
    if (type_VersionMetadata != null)
    {
        Console.WriteLine("[PASS] 类型 VersionMetadata (class) 存在");
        var ctors_VersionMetadata = type_VersionMetadata.GetConstructors();
        Console.WriteLine($"[PASS] VersionMetadata 构造函数数量: {ctors_VersionMetadata.Length}");
        var methods_VersionMetadata = type_VersionMetadata.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VersionMetadata 公开方法数量: {methods_VersionMetadata.Length}");
        foreach (var m in methods_VersionMetadata)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VersionMetadata 未找到，尝试无命名空间...");
        type_VersionMetadata = Type.GetType("VersionMetadata");
        if (type_VersionMetadata != null)
            Console.WriteLine("[PASS] 类型 VersionMetadata (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VersionMetadata 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
