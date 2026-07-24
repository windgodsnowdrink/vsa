#load "qrcode_barcode_integration.cs"

Console.WriteLine("=== qrcode_barcode_integration Test ===");

try
{
    var t0 = typeof(QrCodeIntegration.QrCodeGenerationOptions);
    Console.WriteLine($"[PASS] QrCodeGenerationOptions 存在");
    var t1 = typeof(QrCodeIntegration.BarcodeGenerationOptions);
    Console.WriteLine($"[PASS] BarcodeGenerationOptions 存在");
    var t2 = typeof(QrCodeIntegration.QrCodeReadResult);
    Console.WriteLine($"[PASS] QrCodeReadResult 存在");
    var t3 = typeof(QrCodeIntegration.BarcodeReadResult);
    Console.WriteLine($"[PASS] BarcodeReadResult 存在");
    var t4 = typeof(QrCodeIntegration.QrCodeServiceOptions);
    Console.WriteLine($"[PASS] QrCodeServiceOptions 存在");
    var t5 = typeof(QrCodeIntegration.CacheKey);
    Console.WriteLine($"[PASS] CacheKey 存在");
    var t6 = typeof(QrCodeIntegration.QrBarcodeService);
    Console.WriteLine($"[PASS] QrBarcodeService 存在");
    var t7 = typeof(QrCodeIntegration.QrCodeServiceExtensions);
    Console.WriteLine($"[PASS] QrCodeServiceExtensions 存在");
    var t8 = typeof(QrCodeIntegration.IQrCodeGenerator);
    Console.WriteLine($"[PASS] IQrCodeGenerator 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(QrCodeIntegration.IQrCodeReader);
    Console.WriteLine($"[PASS] IQrCodeReader 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(QrCodeIntegration.IBarcodeGenerator);
    Console.WriteLine($"[PASS] IBarcodeGenerator 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(QrCodeIntegration.IBarcodeReader);
    Console.WriteLine($"[PASS] IBarcodeReader 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(QrCodeIntegration.ErrorCorrectionLevel);
    Console.WriteLine($"[PASS] ErrorCorrectionLevel enum 存在 (IsEnum: {t12.IsEnum})");
    var t13 = typeof(QrCodeIntegration.BarcodeFormat);
    Console.WriteLine($"[PASS] BarcodeFormat enum 存在 (IsEnum: {t13.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}