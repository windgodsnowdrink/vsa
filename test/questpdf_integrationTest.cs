#load "questpdf_integration.cs"

Console.WriteLine("=== questpdf_integration Test ===");

try
{
    var t0 = typeof(Trae.Vsa.PDF.QuestPdfOptions);
    Console.WriteLine($"[PASS] QuestPdfOptions 存在");
    var t1 = typeof(Trae.Vsa.PDF.QuestPdfService);
    Console.WriteLine($"[PASS] QuestPdfService 存在");
    var t2 = typeof(Trae.Vsa.PDF.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(Trae.Vsa.PDF.QuestPdfExtensions);
    Console.WriteLine($"[PASS] QuestPdfExtensions 存在");
    var t4 = typeof(Trae.Vsa.PDF.IQuestPdfService);
    Console.WriteLine($"[PASS] IQuestPdfService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}