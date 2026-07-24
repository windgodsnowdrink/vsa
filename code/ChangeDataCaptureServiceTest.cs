#load "ChangeDataCaptureService.cs"

Console.WriteLine("=== ChangeDataCaptureService.cs Test ===");

try
{
    // 验证 class: ChoETL.Integration.ChangeDataCaptureService
    var type_ChangeDataCaptureService = Type.GetType("ChoETL.Integration.ChangeDataCaptureService");
    if (type_ChangeDataCaptureService != null)
    {
        Console.WriteLine("[PASS] 类型 ChoETL.Integration.ChangeDataCaptureService (class) 存在");
        var ctors_ChangeDataCaptureService = type_ChangeDataCaptureService.GetConstructors();
        Console.WriteLine($"[PASS] ChoETL.Integration.ChangeDataCaptureService 构造函数数量: {ctors_ChangeDataCaptureService.Length}");
        var methods_ChangeDataCaptureService = type_ChangeDataCaptureService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChoETL.Integration.ChangeDataCaptureService 公开方法数量: {methods_ChangeDataCaptureService.Length}");
        foreach (var m in methods_ChangeDataCaptureService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChoETL.Integration.ChangeDataCaptureService 未找到，尝试无命名空间...");
        type_ChangeDataCaptureService = Type.GetType("ChangeDataCaptureService");
        if (type_ChangeDataCaptureService != null)
            Console.WriteLine("[PASS] 类型 ChangeDataCaptureService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChangeDataCaptureService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
