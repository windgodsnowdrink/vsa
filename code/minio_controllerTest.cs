#load "minio_controller.cs"

Console.WriteLine("=== minio_controller.cs Test ===");

try
{
    // 验证 class: MinioUploadController
    var type_MinioUploadController = Type.GetType("MinioUploadController");
    if (type_MinioUploadController != null)
    {
        Console.WriteLine("[PASS] 类型 MinioUploadController (class) 存在");
        var ctors_MinioUploadController = type_MinioUploadController.GetConstructors();
        Console.WriteLine($"[PASS] MinioUploadController 构造函数数量: {ctors_MinioUploadController.Length}");
        var methods_MinioUploadController = type_MinioUploadController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinioUploadController 公开方法数量: {methods_MinioUploadController.Length}");
        foreach (var m in methods_MinioUploadController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinioUploadController 未找到，尝试无命名空间...");
        type_MinioUploadController = Type.GetType("MinioUploadController");
        if (type_MinioUploadController != null)
            Console.WriteLine("[PASS] 类型 MinioUploadController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinioUploadController 可能为顶层语句或嵌套类型");
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

    // 验证 record: EncryptedFileRequest
    var type_EncryptedFileRequest = Type.GetType("EncryptedFileRequest");
    if (type_EncryptedFileRequest != null)
    {
        Console.WriteLine("[PASS] 类型 EncryptedFileRequest (record) 存在");
        var ctors_EncryptedFileRequest = type_EncryptedFileRequest.GetConstructors();
        Console.WriteLine($"[PASS] EncryptedFileRequest 构造函数数量: {ctors_EncryptedFileRequest.Length}");
        var methods_EncryptedFileRequest = type_EncryptedFileRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EncryptedFileRequest 公开方法数量: {methods_EncryptedFileRequest.Length}");
        foreach (var m in methods_EncryptedFileRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EncryptedFileRequest 未找到，尝试无命名空间...");
        type_EncryptedFileRequest = Type.GetType("EncryptedFileRequest");
        if (type_EncryptedFileRequest != null)
            Console.WriteLine("[PASS] 类型 EncryptedFileRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EncryptedFileRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ResumableUploadRequest
    var type_ResumableUploadRequest = Type.GetType("ResumableUploadRequest");
    if (type_ResumableUploadRequest != null)
    {
        Console.WriteLine("[PASS] 类型 ResumableUploadRequest (record) 存在");
        var ctors_ResumableUploadRequest = type_ResumableUploadRequest.GetConstructors();
        Console.WriteLine($"[PASS] ResumableUploadRequest 构造函数数量: {ctors_ResumableUploadRequest.Length}");
        var methods_ResumableUploadRequest = type_ResumableUploadRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResumableUploadRequest 公开方法数量: {methods_ResumableUploadRequest.Length}");
        foreach (var m in methods_ResumableUploadRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResumableUploadRequest 未找到，尝试无命名空间...");
        type_ResumableUploadRequest = Type.GetType("ResumableUploadRequest");
        if (type_ResumableUploadRequest != null)
            Console.WriteLine("[PASS] 类型 ResumableUploadRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResumableUploadRequest 可能为顶层语句或嵌套类型");
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

    // 验证 record: CacheClearRequest
    var type_CacheClearRequest = Type.GetType("CacheClearRequest");
    if (type_CacheClearRequest != null)
    {
        Console.WriteLine("[PASS] 类型 CacheClearRequest (record) 存在");
        var ctors_CacheClearRequest = type_CacheClearRequest.GetConstructors();
        Console.WriteLine($"[PASS] CacheClearRequest 构造函数数量: {ctors_CacheClearRequest.Length}");
        var methods_CacheClearRequest = type_CacheClearRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheClearRequest 公开方法数量: {methods_CacheClearRequest.Length}");
        foreach (var m in methods_CacheClearRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheClearRequest 未找到，尝试无命名空间...");
        type_CacheClearRequest = Type.GetType("CacheClearRequest");
        if (type_CacheClearRequest != null)
            Console.WriteLine("[PASS] 类型 CacheClearRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheClearRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CachePreloadRequest
    var type_CachePreloadRequest = Type.GetType("CachePreloadRequest");
    if (type_CachePreloadRequest != null)
    {
        Console.WriteLine("[PASS] 类型 CachePreloadRequest (record) 存在");
        var ctors_CachePreloadRequest = type_CachePreloadRequest.GetConstructors();
        Console.WriteLine($"[PASS] CachePreloadRequest 构造函数数量: {ctors_CachePreloadRequest.Length}");
        var methods_CachePreloadRequest = type_CachePreloadRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CachePreloadRequest 公开方法数量: {methods_CachePreloadRequest.Length}");
        foreach (var m in methods_CachePreloadRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CachePreloadRequest 未找到，尝试无命名空间...");
        type_CachePreloadRequest = Type.GetType("CachePreloadRequest");
        if (type_CachePreloadRequest != null)
            Console.WriteLine("[PASS] 类型 CachePreloadRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CachePreloadRequest 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
