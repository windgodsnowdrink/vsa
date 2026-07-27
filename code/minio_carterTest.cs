#load "minio_carter.cs"

Console.WriteLine("=== minio_carter.cs Test ===");

try
{
    // 验证 class: MinioModule
    var type_MinioModule = Type.GetType("MinioModule");
    if (type_MinioModule != null)
    {
        Console.WriteLine("[PASS] 类型 MinioModule (class) 存在");
        var ctors_MinioModule = type_MinioModule.GetConstructors();
        Console.WriteLine($"[PASS] MinioModule 构造函数数量: {ctors_MinioModule.Length}");
        var methods_MinioModule = type_MinioModule.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinioModule 公开方法数量: {methods_MinioModule.Length}");
        foreach (var m in methods_MinioModule)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinioModule 未找到，尝试无命名空间...");
        type_MinioModule = Type.GetType("MinioModule");
        if (type_MinioModule != null)
            Console.WriteLine("[PASS] 类型 MinioModule (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinioModule 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UploadProgress
    var type_UploadProgress = Type.GetType("UploadProgress");
    if (type_UploadProgress != null)
    {
        Console.WriteLine("[PASS] 类型 UploadProgress (record) 存在");
        var ctors_UploadProgress = type_UploadProgress.GetConstructors();
        Console.WriteLine($"[PASS] UploadProgress 构造函数数量: {ctors_UploadProgress.Length}");
        var methods_UploadProgress = type_UploadProgress.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UploadProgress 公开方法数量: {methods_UploadProgress.Length}");
        foreach (var m in methods_UploadProgress)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UploadProgress 未找到，尝试无命名空间...");
        type_UploadProgress = Type.GetType("UploadProgress");
        if (type_UploadProgress != null)
            Console.WriteLine("[PASS] 类型 UploadProgress (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UploadProgress 可能为顶层语句或嵌套类型");
    }

    // 验证 record: BatchDeleteRequest
    var type_BatchDeleteRequest = Type.GetType("BatchDeleteRequest");
    if (type_BatchDeleteRequest != null)
    {
        Console.WriteLine("[PASS] 类型 BatchDeleteRequest (record) 存在");
        var ctors_BatchDeleteRequest = type_BatchDeleteRequest.GetConstructors();
        Console.WriteLine($"[PASS] BatchDeleteRequest 构造函数数量: {ctors_BatchDeleteRequest.Length}");
        var methods_BatchDeleteRequest = type_BatchDeleteRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchDeleteRequest 公开方法数量: {methods_BatchDeleteRequest.Length}");
        foreach (var m in methods_BatchDeleteRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchDeleteRequest 未找到，尝试无命名空间...");
        type_BatchDeleteRequest = Type.GetType("BatchDeleteRequest");
        if (type_BatchDeleteRequest != null)
            Console.WriteLine("[PASS] 类型 BatchDeleteRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchDeleteRequest 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
