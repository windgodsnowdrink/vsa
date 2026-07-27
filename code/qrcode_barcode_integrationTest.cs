#load "qrcode_barcode_integration.cs"

Console.WriteLine("=== qrcode_barcode_integration.cs Test ===");

try
{
    // 验证 class: QrBarcodeIntegration.QrBarcodeService
    var type_QrBarcodeService = Type.GetType("QrBarcodeIntegration.QrBarcodeService");
    if (type_QrBarcodeService != null)
    {
        Console.WriteLine("[PASS] 类型 QrBarcodeIntegration.QrBarcodeService (class) 存在");
        var ctors_QrBarcodeService = type_QrBarcodeService.GetConstructors();
        Console.WriteLine($"[PASS] QrBarcodeIntegration.QrBarcodeService 构造函数数量: {ctors_QrBarcodeService.Length}");
        var methods_QrBarcodeService = type_QrBarcodeService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QrBarcodeIntegration.QrBarcodeService 公开方法数量: {methods_QrBarcodeService.Length}");
        foreach (var m in methods_QrBarcodeService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QrBarcodeIntegration.QrBarcodeService 未找到，尝试无命名空间...");
        type_QrBarcodeService = Type.GetType("QrBarcodeService");
        if (type_QrBarcodeService != null)
            Console.WriteLine("[PASS] 类型 QrBarcodeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QrBarcodeService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QrBarcodeIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("QrBarcodeIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 QrBarcodeIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] QrBarcodeIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QrBarcodeIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QrBarcodeIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: QrBarcodeIntegration.IQrBarcodeService
    var type_IQrBarcodeService = Type.GetType("QrBarcodeIntegration.IQrBarcodeService");
    if (type_IQrBarcodeService != null)
    {
        Console.WriteLine("[PASS] 类型 QrBarcodeIntegration.IQrBarcodeService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QrBarcodeIntegration.IQrBarcodeService 未找到，尝试无命名空间...");
        type_IQrBarcodeService = Type.GetType("IQrBarcodeService");
        if (type_IQrBarcodeService != null)
            Console.WriteLine("[PASS] 类型 IQrBarcodeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IQrBarcodeService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
