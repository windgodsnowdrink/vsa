#load "qrcode_barcode_integration.cs"

Console.WriteLine("=== qrcode_barcode_integration Test ===");

try
{
    var t0 = typeof(QrBarcodeIntegration.QrBarcodeService);
    Console.WriteLine($"[PASS] QrBarcodeService 存在");
    var t1 = typeof(QrBarcodeIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(QrBarcodeIntegration.IQrBarcodeService);
    Console.WriteLine($"[PASS] IQrBarcodeService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}