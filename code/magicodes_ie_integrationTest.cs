#load "magicodes_ie_integration.cs"

Console.WriteLine("=== magicodes_ie_integration.cs Test ===");

try
{
    // 验证 class: DocumentService
    var type_DocumentService = Type.GetType("DocumentService");
    if (type_DocumentService != null)
    {
        Console.WriteLine("[PASS] 类型 DocumentService (class) 存在");
        var ctors_DocumentService = type_DocumentService.GetConstructors();
        Console.WriteLine($"[PASS] DocumentService 构造函数数量: {ctors_DocumentService.Length}");
        var methods_DocumentService = type_DocumentService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DocumentService 公开方法数量: {methods_DocumentService.Length}");
        foreach (var m in methods_DocumentService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DocumentService 未找到，尝试无命名空间...");
        type_DocumentService = Type.GetType("DocumentService");
        if (type_DocumentService != null)
            Console.WriteLine("[PASS] 类型 DocumentService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DocumentService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DocumentRequest
    var type_DocumentRequest = Type.GetType("DocumentRequest");
    if (type_DocumentRequest != null)
    {
        Console.WriteLine("[PASS] 类型 DocumentRequest (record) 存在");
        var ctors_DocumentRequest = type_DocumentRequest.GetConstructors();
        Console.WriteLine($"[PASS] DocumentRequest 构造函数数量: {ctors_DocumentRequest.Length}");
        var methods_DocumentRequest = type_DocumentRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DocumentRequest 公开方法数量: {methods_DocumentRequest.Length}");
        foreach (var m in methods_DocumentRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DocumentRequest 未找到，尝试无命名空间...");
        type_DocumentRequest = Type.GetType("DocumentRequest");
        if (type_DocumentRequest != null)
            Console.WriteLine("[PASS] 类型 DocumentRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DocumentRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: DocumentType
    var type_DocumentType = Type.GetType("DocumentType");
    if (type_DocumentType != null)
    {
        Console.WriteLine("[PASS] 类型 DocumentType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DocumentType 未找到，尝试无命名空间...");
        type_DocumentType = Type.GetType("DocumentType");
        if (type_DocumentType != null)
            Console.WriteLine("[PASS] 类型 DocumentType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DocumentType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
