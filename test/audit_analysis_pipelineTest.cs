#load "audit_analysis_pipeline.cs"

Console.WriteLine("=== audit_analysis_pipeline Test ===");

try
{
    var t0 = typeof(AuditAnalysisPipeline);
    Console.WriteLine($"[PASS] AuditAnalysisPipeline 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}