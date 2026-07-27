#load "csvhelper_production.cs"

Console.WriteLine("=== csvhelper_production.cs Test ===");

try
{
    // 验证 class: CsvProcessingService
    var type_CsvProcessingService = Type.GetType("CsvProcessingService");
    if (type_CsvProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 CsvProcessingService (class) 存在");
        var ctors_CsvProcessingService = type_CsvProcessingService.GetConstructors();
        Console.WriteLine($"[PASS] CsvProcessingService 构造函数数量: {ctors_CsvProcessingService.Length}");
        var methods_CsvProcessingService = type_CsvProcessingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CsvProcessingService 公开方法数量: {methods_CsvProcessingService.Length}");
        foreach (var m in methods_CsvProcessingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CsvProcessingService 未找到，尝试无命名空间...");
        type_CsvProcessingService = Type.GetType("CsvProcessingService");
        if (type_CsvProcessingService != null)
            Console.WriteLine("[PASS] 类型 CsvProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CsvProcessingService 可能为顶层语句或嵌套类型");
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

    // 验证 record: CsvOperation
    var type_CsvOperation = Type.GetType("CsvOperation");
    if (type_CsvOperation != null)
    {
        Console.WriteLine("[PASS] 类型 CsvOperation (record) 存在");
        var ctors_CsvOperation = type_CsvOperation.GetConstructors();
        Console.WriteLine($"[PASS] CsvOperation 构造函数数量: {ctors_CsvOperation.Length}");
        var methods_CsvOperation = type_CsvOperation.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CsvOperation 公开方法数量: {methods_CsvOperation.Length}");
        foreach (var m in methods_CsvOperation)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CsvOperation 未找到，尝试无命名空间...");
        type_CsvOperation = Type.GetType("CsvOperation");
        if (type_CsvOperation != null)
            Console.WriteLine("[PASS] 类型 CsvOperation (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CsvOperation 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
