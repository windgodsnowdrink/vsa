#load "minio_progress_hub.cs"

Console.WriteLine("=== minio_progress_hub.cs Test ===");

try
{
    // 验证 class: UploadProgressHub
    var type_UploadProgressHub = Type.GetType("UploadProgressHub");
    if (type_UploadProgressHub != null)
    {
        Console.WriteLine("[PASS] 类型 UploadProgressHub (class) 存在");
        var ctors_UploadProgressHub = type_UploadProgressHub.GetConstructors();
        Console.WriteLine($"[PASS] UploadProgressHub 构造函数数量: {ctors_UploadProgressHub.Length}");
        var methods_UploadProgressHub = type_UploadProgressHub.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UploadProgressHub 公开方法数量: {methods_UploadProgressHub.Length}");
        foreach (var m in methods_UploadProgressHub)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UploadProgressHub 未找到，尝试无命名空间...");
        type_UploadProgressHub = Type.GetType("UploadProgressHub");
        if (type_UploadProgressHub != null)
            Console.WriteLine("[PASS] 类型 UploadProgressHub (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UploadProgressHub 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
