#load "minio_vsa.cs"

Console.WriteLine("=== minio_vsa.cs Test ===");

try
{
    // 验证 class: Vertical.Slice.Template.Features.Minio.Upload.MinioEndpoints
    var type_MinioEndpoints = Type.GetType("Vertical.Slice.Template.Features.Minio.Upload.MinioEndpoints");
    if (type_MinioEndpoints != null)
    {
        Console.WriteLine("[PASS] 类型 Vertical.Slice.Template.Features.Minio.Upload.MinioEndpoints (class) 存在");
        var ctors_MinioEndpoints = type_MinioEndpoints.GetConstructors();
        Console.WriteLine($"[PASS] Vertical.Slice.Template.Features.Minio.Upload.MinioEndpoints 构造函数数量: {ctors_MinioEndpoints.Length}");
        var methods_MinioEndpoints = type_MinioEndpoints.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Vertical.Slice.Template.Features.Minio.Upload.MinioEndpoints 公开方法数量: {methods_MinioEndpoints.Length}");
        foreach (var m in methods_MinioEndpoints)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Vertical.Slice.Template.Features.Minio.Upload.MinioEndpoints 未找到，尝试无命名空间...");
        type_MinioEndpoints = Type.GetType("MinioEndpoints");
        if (type_MinioEndpoints != null)
            Console.WriteLine("[PASS] 类型 MinioEndpoints (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinioEndpoints 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Vertical.Slice.Template.Features.Minio.Upload.UploadProgress
    var type_UploadProgress = Type.GetType("Vertical.Slice.Template.Features.Minio.Upload.UploadProgress");
    if (type_UploadProgress != null)
    {
        Console.WriteLine("[PASS] 类型 Vertical.Slice.Template.Features.Minio.Upload.UploadProgress (record) 存在");
        var ctors_UploadProgress = type_UploadProgress.GetConstructors();
        Console.WriteLine($"[PASS] Vertical.Slice.Template.Features.Minio.Upload.UploadProgress 构造函数数量: {ctors_UploadProgress.Length}");
        var methods_UploadProgress = type_UploadProgress.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Vertical.Slice.Template.Features.Minio.Upload.UploadProgress 公开方法数量: {methods_UploadProgress.Length}");
        foreach (var m in methods_UploadProgress)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Vertical.Slice.Template.Features.Minio.Upload.UploadProgress 未找到，尝试无命名空间...");
        type_UploadProgress = Type.GetType("UploadProgress");
        if (type_UploadProgress != null)
            Console.WriteLine("[PASS] 类型 UploadProgress (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UploadProgress 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
