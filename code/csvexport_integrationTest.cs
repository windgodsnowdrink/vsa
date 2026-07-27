#load "csvexport_integration.cs"

Console.WriteLine("=== csvexport_integration.cs Test ===");

try
{
    // 验证 class: CsvService
    var type_CsvService = Type.GetType("CsvService");
    if (type_CsvService != null)
    {
        Console.WriteLine("[PASS] 类型 CsvService (class) 存在");
        var ctors_CsvService = type_CsvService.GetConstructors();
        Console.WriteLine($"[PASS] CsvService 构造函数数量: {ctors_CsvService.Length}");
        var methods_CsvService = type_CsvService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CsvService 公开方法数量: {methods_CsvService.Length}");
        foreach (var m in methods_CsvService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CsvService 未找到，尝试无命名空间...");
        type_CsvService = Type.GetType("CsvService");
        if (type_CsvService != null)
            Console.WriteLine("[PASS] 类型 CsvService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CsvService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: in
    var type_in = Type.GetType("in");
    if (type_in != null)
    {
        Console.WriteLine("[PASS] 类型 in (record) 存在");
        var ctors_in = type_in.GetConstructors();
        Console.WriteLine($"[PASS] in 构造函数数量: {ctors_in.Length}");
        var methods_in = type_in.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] in 公开方法数量: {methods_in.Length}");
        foreach (var m in methods_in)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 in 未找到，尝试无命名空间...");
        type_in = Type.GetType("in");
        if (type_in != null)
            Console.WriteLine("[PASS] 类型 in (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 in 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
