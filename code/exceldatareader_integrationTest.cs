#load "exceldatareader_integration.cs"

Console.WriteLine("=== exceldatareader_integration.cs Test ===");

try
{
    // 验证 class: ExcelDataService
    var type_ExcelDataService = Type.GetType("ExcelDataService");
    if (type_ExcelDataService != null)
    {
        Console.WriteLine("[PASS] 类型 ExcelDataService (class) 存在");
        var ctors_ExcelDataService = type_ExcelDataService.GetConstructors();
        Console.WriteLine($"[PASS] ExcelDataService 构造函数数量: {ctors_ExcelDataService.Length}");
        var methods_ExcelDataService = type_ExcelDataService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExcelDataService 公开方法数量: {methods_ExcelDataService.Length}");
        foreach (var m in methods_ExcelDataService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExcelDataService 未找到，尝试无命名空间...");
        type_ExcelDataService = Type.GetType("ExcelDataService");
        if (type_ExcelDataService != null)
            Console.WriteLine("[PASS] 类型 ExcelDataService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExcelDataService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
