#load "minio_advanced.cs"

Console.WriteLine("=== minio_advanced.cs Test ===");

try
{
    // 验证 class: AdvancedMinioService
    var type_AdvancedMinioService = Type.GetType("AdvancedMinioService");
    if (type_AdvancedMinioService != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedMinioService (class) 存在");
        var ctors_AdvancedMinioService = type_AdvancedMinioService.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedMinioService 构造函数数量: {ctors_AdvancedMinioService.Length}");
        var methods_AdvancedMinioService = type_AdvancedMinioService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedMinioService 公开方法数量: {methods_AdvancedMinioService.Length}");
        foreach (var m in methods_AdvancedMinioService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedMinioService 未找到，尝试无命名空间...");
        type_AdvancedMinioService = Type.GetType("AdvancedMinioService");
        if (type_AdvancedMinioService != null)
            Console.WriteLine("[PASS] 类型 AdvancedMinioService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedMinioService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MinioServiceExtensions
    var type_MinioServiceExtensions = Type.GetType("MinioServiceExtensions");
    if (type_MinioServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MinioServiceExtensions (class) 存在");
        var ctors_MinioServiceExtensions = type_MinioServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MinioServiceExtensions 构造函数数量: {ctors_MinioServiceExtensions.Length}");
        var methods_MinioServiceExtensions = type_MinioServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinioServiceExtensions 公开方法数量: {methods_MinioServiceExtensions.Length}");
        foreach (var m in methods_MinioServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinioServiceExtensions 未找到，尝试无命名空间...");
        type_MinioServiceExtensions = Type.GetType("MinioServiceExtensions");
        if (type_MinioServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 MinioServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinioServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AlignedMemoryPool
    var type_AlignedMemoryPool = Type.GetType("AlignedMemoryPool");
    if (type_AlignedMemoryPool != null)
    {
        Console.WriteLine("[PASS] 类型 AlignedMemoryPool (class) 存在");
        var ctors_AlignedMemoryPool = type_AlignedMemoryPool.GetConstructors();
        Console.WriteLine($"[PASS] AlignedMemoryPool 构造函数数量: {ctors_AlignedMemoryPool.Length}");
        var methods_AlignedMemoryPool = type_AlignedMemoryPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AlignedMemoryPool 公开方法数量: {methods_AlignedMemoryPool.Length}");
        foreach (var m in methods_AlignedMemoryPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AlignedMemoryPool 未找到，尝试无命名空间...");
        type_AlignedMemoryPool = Type.GetType("AlignedMemoryPool");
        if (type_AlignedMemoryPool != null)
            Console.WriteLine("[PASS] 类型 AlignedMemoryPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AlignedMemoryPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AlignedMemoryOwner
    var type_AlignedMemoryOwner = Type.GetType("AlignedMemoryOwner");
    if (type_AlignedMemoryOwner != null)
    {
        Console.WriteLine("[PASS] 类型 AlignedMemoryOwner (class) 存在");
        var ctors_AlignedMemoryOwner = type_AlignedMemoryOwner.GetConstructors();
        Console.WriteLine($"[PASS] AlignedMemoryOwner 构造函数数量: {ctors_AlignedMemoryOwner.Length}");
        var methods_AlignedMemoryOwner = type_AlignedMemoryOwner.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AlignedMemoryOwner 公开方法数量: {methods_AlignedMemoryOwner.Length}");
        foreach (var m in methods_AlignedMemoryOwner)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AlignedMemoryOwner 未找到，尝试无命名空间...");
        type_AlignedMemoryOwner = Type.GetType("AlignedMemoryOwner");
        if (type_AlignedMemoryOwner != null)
            Console.WriteLine("[PASS] 类型 AlignedMemoryOwner (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AlignedMemoryOwner 可能为顶层语句或嵌套类型");
    }

    // 验证 record: MinioObject
    var type_MinioObject = Type.GetType("MinioObject");
    if (type_MinioObject != null)
    {
        Console.WriteLine("[PASS] 类型 MinioObject (record) 存在");
        var ctors_MinioObject = type_MinioObject.GetConstructors();
        Console.WriteLine($"[PASS] MinioObject 构造函数数量: {ctors_MinioObject.Length}");
        var methods_MinioObject = type_MinioObject.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinioObject 公开方法数量: {methods_MinioObject.Length}");
        foreach (var m in methods_MinioObject)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinioObject 未找到，尝试无命名空间...");
        type_MinioObject = Type.GetType("MinioObject");
        if (type_MinioObject != null)
            Console.WriteLine("[PASS] 类型 MinioObject (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinioObject 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UploadPart
    var type_UploadPart = Type.GetType("UploadPart");
    if (type_UploadPart != null)
    {
        Console.WriteLine("[PASS] 类型 UploadPart (record) 存在");
        var ctors_UploadPart = type_UploadPart.GetConstructors();
        Console.WriteLine($"[PASS] UploadPart 构造函数数量: {ctors_UploadPart.Length}");
        var methods_UploadPart = type_UploadPart.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UploadPart 公开方法数量: {methods_UploadPart.Length}");
        foreach (var m in methods_UploadPart)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UploadPart 未找到，尝试无命名空间...");
        type_UploadPart = Type.GetType("UploadPart");
        if (type_UploadPart != null)
            Console.WriteLine("[PASS] 类型 UploadPart (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UploadPart 可能为顶层语句或嵌套类型");
    }

    // 验证 record: MinioUploadEvent
    var type_MinioUploadEvent = Type.GetType("MinioUploadEvent");
    if (type_MinioUploadEvent != null)
    {
        Console.WriteLine("[PASS] 类型 MinioUploadEvent (record) 存在");
        var ctors_MinioUploadEvent = type_MinioUploadEvent.GetConstructors();
        Console.WriteLine($"[PASS] MinioUploadEvent 构造函数数量: {ctors_MinioUploadEvent.Length}");
        var methods_MinioUploadEvent = type_MinioUploadEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinioUploadEvent 公开方法数量: {methods_MinioUploadEvent.Length}");
        foreach (var m in methods_MinioUploadEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinioUploadEvent 未找到，尝试无命名空间...");
        type_MinioUploadEvent = Type.GetType("MinioUploadEvent");
        if (type_MinioUploadEvent != null)
            Console.WriteLine("[PASS] 类型 MinioUploadEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinioUploadEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
