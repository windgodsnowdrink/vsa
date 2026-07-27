#load "minio_fastendpoints.cs"

Console.WriteLine("=== minio_fastendpoints.cs Test ===");

try
{
    // 验证 class: MultipartUploadEndpoint
    var type_MultipartUploadEndpoint = Type.GetType("MultipartUploadEndpoint");
    if (type_MultipartUploadEndpoint != null)
    {
        Console.WriteLine("[PASS] 类型 MultipartUploadEndpoint (class) 存在");
        var ctors_MultipartUploadEndpoint = type_MultipartUploadEndpoint.GetConstructors();
        Console.WriteLine($"[PASS] MultipartUploadEndpoint 构造函数数量: {ctors_MultipartUploadEndpoint.Length}");
        var methods_MultipartUploadEndpoint = type_MultipartUploadEndpoint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultipartUploadEndpoint 公开方法数量: {methods_MultipartUploadEndpoint.Length}");
        foreach (var m in methods_MultipartUploadEndpoint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultipartUploadEndpoint 未找到，尝试无命名空间...");
        type_MultipartUploadEndpoint = Type.GetType("MultipartUploadEndpoint");
        if (type_MultipartUploadEndpoint != null)
            Console.WriteLine("[PASS] 类型 MultipartUploadEndpoint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultipartUploadEndpoint 可能为顶层语句或嵌套类型");
    }

    // 验证 class: StreamUploadEndpoint
    var type_StreamUploadEndpoint = Type.GetType("StreamUploadEndpoint");
    if (type_StreamUploadEndpoint != null)
    {
        Console.WriteLine("[PASS] 类型 StreamUploadEndpoint (class) 存在");
        var ctors_StreamUploadEndpoint = type_StreamUploadEndpoint.GetConstructors();
        Console.WriteLine($"[PASS] StreamUploadEndpoint 构造函数数量: {ctors_StreamUploadEndpoint.Length}");
        var methods_StreamUploadEndpoint = type_StreamUploadEndpoint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StreamUploadEndpoint 公开方法数量: {methods_StreamUploadEndpoint.Length}");
        foreach (var m in methods_StreamUploadEndpoint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StreamUploadEndpoint 未找到，尝试无命名空间...");
        type_StreamUploadEndpoint = Type.GetType("StreamUploadEndpoint");
        if (type_StreamUploadEndpoint != null)
            Console.WriteLine("[PASS] 类型 StreamUploadEndpoint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StreamUploadEndpoint 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BatchUploadEndpoint
    var type_BatchUploadEndpoint = Type.GetType("BatchUploadEndpoint");
    if (type_BatchUploadEndpoint != null)
    {
        Console.WriteLine("[PASS] 类型 BatchUploadEndpoint (class) 存在");
        var ctors_BatchUploadEndpoint = type_BatchUploadEndpoint.GetConstructors();
        Console.WriteLine($"[PASS] BatchUploadEndpoint 构造函数数量: {ctors_BatchUploadEndpoint.Length}");
        var methods_BatchUploadEndpoint = type_BatchUploadEndpoint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchUploadEndpoint 公开方法数量: {methods_BatchUploadEndpoint.Length}");
        foreach (var m in methods_BatchUploadEndpoint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchUploadEndpoint 未找到，尝试无命名空间...");
        type_BatchUploadEndpoint = Type.GetType("BatchUploadEndpoint");
        if (type_BatchUploadEndpoint != null)
            Console.WriteLine("[PASS] 类型 BatchUploadEndpoint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchUploadEndpoint 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MultipartWithProgressEndpoint
    var type_MultipartWithProgressEndpoint = Type.GetType("MultipartWithProgressEndpoint");
    if (type_MultipartWithProgressEndpoint != null)
    {
        Console.WriteLine("[PASS] 类型 MultipartWithProgressEndpoint (class) 存在");
        var ctors_MultipartWithProgressEndpoint = type_MultipartWithProgressEndpoint.GetConstructors();
        Console.WriteLine($"[PASS] MultipartWithProgressEndpoint 构造函数数量: {ctors_MultipartWithProgressEndpoint.Length}");
        var methods_MultipartWithProgressEndpoint = type_MultipartWithProgressEndpoint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultipartWithProgressEndpoint 公开方法数量: {methods_MultipartWithProgressEndpoint.Length}");
        foreach (var m in methods_MultipartWithProgressEndpoint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultipartWithProgressEndpoint 未找到，尝试无命名空间...");
        type_MultipartWithProgressEndpoint = Type.GetType("MultipartWithProgressEndpoint");
        if (type_MultipartWithProgressEndpoint != null)
            Console.WriteLine("[PASS] 类型 MultipartWithProgressEndpoint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultipartWithProgressEndpoint 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DownloadEndpoint
    var type_DownloadEndpoint = Type.GetType("DownloadEndpoint");
    if (type_DownloadEndpoint != null)
    {
        Console.WriteLine("[PASS] 类型 DownloadEndpoint (class) 存在");
        var ctors_DownloadEndpoint = type_DownloadEndpoint.GetConstructors();
        Console.WriteLine($"[PASS] DownloadEndpoint 构造函数数量: {ctors_DownloadEndpoint.Length}");
        var methods_DownloadEndpoint = type_DownloadEndpoint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DownloadEndpoint 公开方法数量: {methods_DownloadEndpoint.Length}");
        foreach (var m in methods_DownloadEndpoint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DownloadEndpoint 未找到，尝试无命名空间...");
        type_DownloadEndpoint = Type.GetType("DownloadEndpoint");
        if (type_DownloadEndpoint != null)
            Console.WriteLine("[PASS] 类型 DownloadEndpoint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DownloadEndpoint 可能为顶层语句或嵌套类型");
    }

    // 验证 record: FileUploadRequest
    var type_FileUploadRequest = Type.GetType("FileUploadRequest");
    if (type_FileUploadRequest != null)
    {
        Console.WriteLine("[PASS] 类型 FileUploadRequest (record) 存在");
        var ctors_FileUploadRequest = type_FileUploadRequest.GetConstructors();
        Console.WriteLine($"[PASS] FileUploadRequest 构造函数数量: {ctors_FileUploadRequest.Length}");
        var methods_FileUploadRequest = type_FileUploadRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FileUploadRequest 公开方法数量: {methods_FileUploadRequest.Length}");
        foreach (var m in methods_FileUploadRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FileUploadRequest 未找到，尝试无命名空间...");
        type_FileUploadRequest = Type.GetType("FileUploadRequest");
        if (type_FileUploadRequest != null)
            Console.WriteLine("[PASS] 类型 FileUploadRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FileUploadRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: FileUploadResponse
    var type_FileUploadResponse = Type.GetType("FileUploadResponse");
    if (type_FileUploadResponse != null)
    {
        Console.WriteLine("[PASS] 类型 FileUploadResponse (record) 存在");
        var ctors_FileUploadResponse = type_FileUploadResponse.GetConstructors();
        Console.WriteLine($"[PASS] FileUploadResponse 构造函数数量: {ctors_FileUploadResponse.Length}");
        var methods_FileUploadResponse = type_FileUploadResponse.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FileUploadResponse 公开方法数量: {methods_FileUploadResponse.Length}");
        foreach (var m in methods_FileUploadResponse)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FileUploadResponse 未找到，尝试无命名空间...");
        type_FileUploadResponse = Type.GetType("FileUploadResponse");
        if (type_FileUploadResponse != null)
            Console.WriteLine("[PASS] 类型 FileUploadResponse (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FileUploadResponse 可能为顶层语句或嵌套类型");
    }

    // 验证 record: BatchFileUploadRequest
    var type_BatchFileUploadRequest = Type.GetType("BatchFileUploadRequest");
    if (type_BatchFileUploadRequest != null)
    {
        Console.WriteLine("[PASS] 类型 BatchFileUploadRequest (record) 存在");
        var ctors_BatchFileUploadRequest = type_BatchFileUploadRequest.GetConstructors();
        Console.WriteLine($"[PASS] BatchFileUploadRequest 构造函数数量: {ctors_BatchFileUploadRequest.Length}");
        var methods_BatchFileUploadRequest = type_BatchFileUploadRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchFileUploadRequest 公开方法数量: {methods_BatchFileUploadRequest.Length}");
        foreach (var m in methods_BatchFileUploadRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchFileUploadRequest 未找到，尝试无命名空间...");
        type_BatchFileUploadRequest = Type.GetType("BatchFileUploadRequest");
        if (type_BatchFileUploadRequest != null)
            Console.WriteLine("[PASS] 类型 BatchFileUploadRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchFileUploadRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: BatchUploadResponse
    var type_BatchUploadResponse = Type.GetType("BatchUploadResponse");
    if (type_BatchUploadResponse != null)
    {
        Console.WriteLine("[PASS] 类型 BatchUploadResponse (record) 存在");
        var ctors_BatchUploadResponse = type_BatchUploadResponse.GetConstructors();
        Console.WriteLine($"[PASS] BatchUploadResponse 构造函数数量: {ctors_BatchUploadResponse.Length}");
        var methods_BatchUploadResponse = type_BatchUploadResponse.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchUploadResponse 公开方法数量: {methods_BatchUploadResponse.Length}");
        foreach (var m in methods_BatchUploadResponse)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchUploadResponse 未找到，尝试无命名空间...");
        type_BatchUploadResponse = Type.GetType("BatchUploadResponse");
        if (type_BatchUploadResponse != null)
            Console.WriteLine("[PASS] 类型 BatchUploadResponse (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchUploadResponse 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UploadProgressResponse
    var type_UploadProgressResponse = Type.GetType("UploadProgressResponse");
    if (type_UploadProgressResponse != null)
    {
        Console.WriteLine("[PASS] 类型 UploadProgressResponse (record) 存在");
        var ctors_UploadProgressResponse = type_UploadProgressResponse.GetConstructors();
        Console.WriteLine($"[PASS] UploadProgressResponse 构造函数数量: {ctors_UploadProgressResponse.Length}");
        var methods_UploadProgressResponse = type_UploadProgressResponse.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UploadProgressResponse 公开方法数量: {methods_UploadProgressResponse.Length}");
        foreach (var m in methods_UploadProgressResponse)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UploadProgressResponse 未找到，尝试无命名空间...");
        type_UploadProgressResponse = Type.GetType("UploadProgressResponse");
        if (type_UploadProgressResponse != null)
            Console.WriteLine("[PASS] 类型 UploadProgressResponse (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UploadProgressResponse 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DownloadRequest
    var type_DownloadRequest = Type.GetType("DownloadRequest");
    if (type_DownloadRequest != null)
    {
        Console.WriteLine("[PASS] 类型 DownloadRequest (record) 存在");
        var ctors_DownloadRequest = type_DownloadRequest.GetConstructors();
        Console.WriteLine($"[PASS] DownloadRequest 构造函数数量: {ctors_DownloadRequest.Length}");
        var methods_DownloadRequest = type_DownloadRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DownloadRequest 公开方法数量: {methods_DownloadRequest.Length}");
        foreach (var m in methods_DownloadRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DownloadRequest 未找到，尝试无命名空间...");
        type_DownloadRequest = Type.GetType("DownloadRequest");
        if (type_DownloadRequest != null)
            Console.WriteLine("[PASS] 类型 DownloadRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DownloadRequest 可能为顶层语句或嵌套类型");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
