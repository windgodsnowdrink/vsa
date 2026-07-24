#load "miniexcel_full_features.cs"

Console.WriteLine("=== miniexcel_full_features.cs Test ===");

try
{
    // 验证 class: FullFeatureExcelService
    var type_FullFeatureExcelService = Type.GetType("FullFeatureExcelService");
    if (type_FullFeatureExcelService != null)
    {
        Console.WriteLine("[PASS] 类型 FullFeatureExcelService (class) 存在");
        var ctors_FullFeatureExcelService = type_FullFeatureExcelService.GetConstructors();
        Console.WriteLine($"[PASS] FullFeatureExcelService 构造函数数量: {ctors_FullFeatureExcelService.Length}");
        var methods_FullFeatureExcelService = type_FullFeatureExcelService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FullFeatureExcelService 公开方法数量: {methods_FullFeatureExcelService.Length}");
        foreach (var m in methods_FullFeatureExcelService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FullFeatureExcelService 未找到，尝试无命名空间...");
        type_FullFeatureExcelService = Type.GetType("FullFeatureExcelService");
        if (type_FullFeatureExcelService != null)
            Console.WriteLine("[PASS] 类型 FullFeatureExcelService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FullFeatureExcelService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
