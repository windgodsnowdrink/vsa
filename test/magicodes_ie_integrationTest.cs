#load "magicodes_ie_integration.cs"

Console.WriteLine("=== magicodes_ie_integration Test ===");

try
{
    var t0 = typeof(DocumentService);
    Console.WriteLine($"[PASS] DocumentService 存在");
    var t1 = typeof(DocumentRequest);
    Console.WriteLine($"[PASS] DocumentRequest record 存在");
    var t2 = typeof(DocumentType);
    Console.WriteLine($"[PASS] DocumentType enum 存在 (IsEnum: {t2.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}