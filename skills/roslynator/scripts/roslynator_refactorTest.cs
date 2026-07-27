#load "roslynator_refactor.cs"

Console.WriteLine("=== roslynator_refactor Test ===");

try
{
    var t0 = typeof(Roslynator.Refactor.RefactoringProvider);
    Console.WriteLine($"[PASS] RefactoringProvider 存在");
    var t1 = typeof(Roslynator.Refactor.RefactoringEngine);
    Console.WriteLine($"[PASS] RefactoringEngine 存在");
    var t2 = typeof(Roslynator.Refactor.ProjectLoader);
    Console.WriteLine($"[PASS] ProjectLoader 存在");
    var t3 = typeof(Roslynator.Refactor.RefactoringReporter);
    Console.WriteLine($"[PASS] RefactoringReporter 存在");
    var t4 = typeof(Roslynator.Refactor.ExpressionBodyRewriter);
    Console.WriteLine($"[PASS] ExpressionBodyRewriter 存在");
    var t5 = typeof(Roslynator.Refactor.StringInterpolationRewriter);
    Console.WriteLine($"[PASS] StringInterpolationRewriter 存在");
    var t6 = typeof(Roslynator.Refactor.PatternMatchingRewriter);
    Console.WriteLine($"[PASS] PatternMatchingRewriter 存在");
    var t7 = typeof(Roslynator.Refactor.TypeCastRewriter);
    Console.WriteLine($"[PASS] TypeCastRewriter 存在");
    var t8 = typeof(Roslynator.Refactor.NullCheckRewriter);
    Console.WriteLine($"[PASS] NullCheckRewriter 存在");
    var t9 = typeof(Roslynator.Refactor.UsingDeclarationRewriter);
    Console.WriteLine($"[PASS] UsingDeclarationRewriter 存在");
    var t10 = typeof(Roslynator.Refactor.RefactoringResult);
    Console.WriteLine($"[PASS] RefactoringResult 存在");
    var t11 = typeof(Roslynator.Refactor.RefactoringOptions);
    Console.WriteLine($"[PASS] RefactoringOptions 存在");
    var t12 = typeof(Roslynator.Refactor.RefactoringInfo);
    Console.WriteLine($"[PASS] RefactoringInfo 存在");
    var t13 = typeof(Roslynator.Refactor.RefactoringReport);
    Console.WriteLine($"[PASS] RefactoringReport 存在");
    var t14 = typeof(Roslynator.Refactor.RefactoringSummary);
    Console.WriteLine($"[PASS] RefactoringSummary 存在");
    var t15 = typeof(Roslynator.Refactor.IRefactoringProvider);
    Console.WriteLine($"[PASS] IRefactoringProvider 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(Roslynator.Refactor.IRefactoringEngine);
    Console.WriteLine($"[PASS] IRefactoringEngine 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(Roslynator.Refactor.IProjectLoader);
    Console.WriteLine($"[PASS] IProjectLoader 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(Roslynator.Refactor.IRefactoringReporter);
    Console.WriteLine($"[PASS] IRefactoringReporter 接口存在 (IsInterface: {t18.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}