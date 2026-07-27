#load "roslynator_analyzer.cs"

Console.WriteLine("=== roslynator_analyzer Test ===");

try
{
    var t0 = typeof(Roslynator.Analyzer.CodeAnalyzer);
    Console.WriteLine($"[PASS] CodeAnalyzer 存在");
    var t1 = typeof(Roslynator.Analyzer.AnalysisEngine);
    Console.WriteLine($"[PASS] AnalysisEngine 存在");
    var t2 = typeof(Roslynator.Analyzer.ProjectLoader);
    Console.WriteLine($"[PASS] ProjectLoader 存在");
    var t3 = typeof(Roslynator.Analyzer.DiagnosticReporter);
    Console.WriteLine($"[PASS] DiagnosticReporter 存在");
    var t4 = typeof(Roslynator.Analyzer.SyntaxAnalyzer);
    Console.WriteLine($"[PASS] SyntaxAnalyzer 存在");
    var t5 = typeof(Roslynator.Analyzer.AnalysisResult);
    Console.WriteLine($"[PASS] AnalysisResult 存在");
    var t6 = typeof(Roslynator.Analyzer.AnalyzerOptions);
    Console.WriteLine($"[PASS] AnalyzerOptions 存在");
    var t7 = typeof(Roslynator.Analyzer.DiagnosticInfo);
    Console.WriteLine($"[PASS] DiagnosticInfo 存在");
    var t8 = typeof(Roslynator.Analyzer.AnalysisReport);
    Console.WriteLine($"[PASS] AnalysisReport 存在");
    var t9 = typeof(Roslynator.Analyzer.AnalysisSummary);
    Console.WriteLine($"[PASS] AnalysisSummary 存在");
    var t10 = typeof(Roslynator.Analyzer.ICodeAnalyzer);
    Console.WriteLine($"[PASS] ICodeAnalyzer 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(Roslynator.Analyzer.IAnalysisEngine);
    Console.WriteLine($"[PASS] IAnalysisEngine 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(Roslynator.Analyzer.IProjectLoader);
    Console.WriteLine($"[PASS] IProjectLoader 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(Roslynator.Analyzer.IDiagnosticReporter);
    Console.WriteLine($"[PASS] IDiagnosticReporter 接口存在 (IsInterface: {t13.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}