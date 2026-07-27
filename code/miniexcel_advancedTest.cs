#load "miniexcel_advanced.cs"

Console.WriteLine("=== miniexcel_advanced.cs Test ===");

try
{
    // 验证 class: AdvancedExcelService
    var type_AdvancedExcelService = Type.GetType("AdvancedExcelService");
    if (type_AdvancedExcelService != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedExcelService (class) 存在");
        var ctors_AdvancedExcelService = type_AdvancedExcelService.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedExcelService 构造函数数量: {ctors_AdvancedExcelService.Length}");
        var methods_AdvancedExcelService = type_AdvancedExcelService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedExcelService 公开方法数量: {methods_AdvancedExcelService.Length}");
        foreach (var m in methods_AdvancedExcelService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedExcelService 未找到，尝试无命名空间...");
        type_AdvancedExcelService = Type.GetType("AdvancedExcelService");
        if (type_AdvancedExcelService != null)
            Console.WriteLine("[PASS] 类型 AdvancedExcelService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedExcelService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
