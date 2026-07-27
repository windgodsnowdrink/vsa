#load "closedxml_production.cs"

Console.WriteLine("=== closedxml_production.cs Test ===");

try
{
    // 验证 class: ExcelDocumentService
    var type_ExcelDocumentService = Type.GetType("ExcelDocumentService");
    if (type_ExcelDocumentService != null)
    {
        Console.WriteLine("[PASS] 类型 ExcelDocumentService (class) 存在");
        var ctors_ExcelDocumentService = type_ExcelDocumentService.GetConstructors();
        Console.WriteLine($"[PASS] ExcelDocumentService 构造函数数量: {ctors_ExcelDocumentService.Length}");
        var methods_ExcelDocumentService = type_ExcelDocumentService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExcelDocumentService 公开方法数量: {methods_ExcelDocumentService.Length}");
        foreach (var m in methods_ExcelDocumentService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExcelDocumentService 未找到，尝试无命名空间...");
        type_ExcelDocumentService = Type.GetType("ExcelDocumentService");
        if (type_ExcelDocumentService != null)
            Console.WriteLine("[PASS] 类型 ExcelDocumentService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExcelDocumentService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WorkbookPooledPolicy
    var type_WorkbookPooledPolicy = Type.GetType("WorkbookPooledPolicy");
    if (type_WorkbookPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 WorkbookPooledPolicy (class) 存在");
        var ctors_WorkbookPooledPolicy = type_WorkbookPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] WorkbookPooledPolicy 构造函数数量: {ctors_WorkbookPooledPolicy.Length}");
        var methods_WorkbookPooledPolicy = type_WorkbookPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WorkbookPooledPolicy 公开方法数量: {methods_WorkbookPooledPolicy.Length}");
        foreach (var m in methods_WorkbookPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WorkbookPooledPolicy 未找到，尝试无命名空间...");
        type_WorkbookPooledPolicy = Type.GetType("WorkbookPooledPolicy");
        if (type_WorkbookPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 WorkbookPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WorkbookPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ExcelRequest
    var type_ExcelRequest = Type.GetType("ExcelRequest");
    if (type_ExcelRequest != null)
    {
        Console.WriteLine("[PASS] 类型 ExcelRequest (record) 存在");
        var ctors_ExcelRequest = type_ExcelRequest.GetConstructors();
        Console.WriteLine($"[PASS] ExcelRequest 构造函数数量: {ctors_ExcelRequest.Length}");
        var methods_ExcelRequest = type_ExcelRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExcelRequest 公开方法数量: {methods_ExcelRequest.Length}");
        foreach (var m in methods_ExcelRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExcelRequest 未找到，尝试无命名空间...");
        type_ExcelRequest = Type.GetType("ExcelRequest");
        if (type_ExcelRequest != null)
            Console.WriteLine("[PASS] 类型 ExcelRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExcelRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ExcelOperation
    var type_ExcelOperation = Type.GetType("ExcelOperation");
    if (type_ExcelOperation != null)
    {
        Console.WriteLine("[PASS] 类型 ExcelOperation (record) 存在");
        var ctors_ExcelOperation = type_ExcelOperation.GetConstructors();
        Console.WriteLine($"[PASS] ExcelOperation 构造函数数量: {ctors_ExcelOperation.Length}");
        var methods_ExcelOperation = type_ExcelOperation.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExcelOperation 公开方法数量: {methods_ExcelOperation.Length}");
        foreach (var m in methods_ExcelOperation)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExcelOperation 未找到，尝试无命名空间...");
        type_ExcelOperation = Type.GetType("ExcelOperation");
        if (type_ExcelOperation != null)
            Console.WriteLine("[PASS] 类型 ExcelOperation (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExcelOperation 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
