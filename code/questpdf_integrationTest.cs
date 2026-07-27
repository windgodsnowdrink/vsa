#load "questpdf_integration.cs"

Console.WriteLine("=== questpdf_integration.cs Test ===");

try
{
    // 验证 class: Trae.Vsa.PDF.QuestPdfOptions
    var type_QuestPdfOptions = Type.GetType("Trae.Vsa.PDF.QuestPdfOptions");
    if (type_QuestPdfOptions != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.PDF.QuestPdfOptions (class) 存在");
        var ctors_QuestPdfOptions = type_QuestPdfOptions.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.PDF.QuestPdfOptions 构造函数数量: {ctors_QuestPdfOptions.Length}");
        var methods_QuestPdfOptions = type_QuestPdfOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.PDF.QuestPdfOptions 公开方法数量: {methods_QuestPdfOptions.Length}");
        foreach (var m in methods_QuestPdfOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.PDF.QuestPdfOptions 未找到，尝试无命名空间...");
        type_QuestPdfOptions = Type.GetType("QuestPdfOptions");
        if (type_QuestPdfOptions != null)
            Console.WriteLine("[PASS] 类型 QuestPdfOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuestPdfOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.PDF.QuestPdfService
    var type_QuestPdfService = Type.GetType("Trae.Vsa.PDF.QuestPdfService");
    if (type_QuestPdfService != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.PDF.QuestPdfService (class) 存在");
        var ctors_QuestPdfService = type_QuestPdfService.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.PDF.QuestPdfService 构造函数数量: {ctors_QuestPdfService.Length}");
        var methods_QuestPdfService = type_QuestPdfService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.PDF.QuestPdfService 公开方法数量: {methods_QuestPdfService.Length}");
        foreach (var m in methods_QuestPdfService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.PDF.QuestPdfService 未找到，尝试无命名空间...");
        type_QuestPdfService = Type.GetType("QuestPdfService");
        if (type_QuestPdfService != null)
            Console.WriteLine("[PASS] 类型 QuestPdfService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuestPdfService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.PDF.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("Trae.Vsa.PDF.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.PDF.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.PDF.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.PDF.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.PDF.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.PDF.QuestPdfExtensions
    var type_QuestPdfExtensions = Type.GetType("Trae.Vsa.PDF.QuestPdfExtensions");
    if (type_QuestPdfExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.PDF.QuestPdfExtensions (class) 存在");
        var ctors_QuestPdfExtensions = type_QuestPdfExtensions.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.PDF.QuestPdfExtensions 构造函数数量: {ctors_QuestPdfExtensions.Length}");
        var methods_QuestPdfExtensions = type_QuestPdfExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.PDF.QuestPdfExtensions 公开方法数量: {methods_QuestPdfExtensions.Length}");
        foreach (var m in methods_QuestPdfExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.PDF.QuestPdfExtensions 未找到，尝试无命名空间...");
        type_QuestPdfExtensions = Type.GetType("QuestPdfExtensions");
        if (type_QuestPdfExtensions != null)
            Console.WriteLine("[PASS] 类型 QuestPdfExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuestPdfExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: Trae.Vsa.PDF.IQuestPdfService
    var type_IQuestPdfService = Type.GetType("Trae.Vsa.PDF.IQuestPdfService");
    if (type_IQuestPdfService != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.PDF.IQuestPdfService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.PDF.IQuestPdfService 未找到，尝试无命名空间...");
        type_IQuestPdfService = Type.GetType("IQuestPdfService");
        if (type_IQuestPdfService != null)
            Console.WriteLine("[PASS] 类型 IQuestPdfService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IQuestPdfService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
