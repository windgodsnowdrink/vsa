#load "priority_engine.cs"

Console.WriteLine("=== priority_engine Test ===");

try
{
    var t0 = typeof(VSA.TodoSystem.Core.PriorityCalculationContext);
    Console.WriteLine($"[PASS] PriorityCalculationContext 存在");
    var t1 = typeof(VSA.TodoSystem.Core.HistoricalData);
    Console.WriteLine($"[PASS] HistoricalData 存在");
    var t2 = typeof(VSA.TodoSystem.Core.TeamWorkload);
    Console.WriteLine($"[PASS] TeamWorkload 存在");
    var t3 = typeof(VSA.TodoSystem.Core.BusinessImpact);
    Console.WriteLine($"[PASS] BusinessImpact 存在");
    var t4 = typeof(VSA.TodoSystem.Core.DeadlineProximity);
    Console.WriteLine($"[PASS] DeadlineProximity 存在");
    var t5 = typeof(VSA.TodoSystem.Core.ResourceRequirements);
    Console.WriteLine($"[PASS] ResourceRequirements 存在");
    var t6 = typeof(VSA.TodoSystem.Core.RiskAssessment);
    Console.WriteLine($"[PASS] RiskAssessment 存在");
    var t7 = typeof(VSA.TodoSystem.Core.PriorityWeights);
    Console.WriteLine($"[PASS] PriorityWeights 存在");
    var t8 = typeof(VSA.TodoSystem.Core.PriorityResult);
    Console.WriteLine($"[PASS] PriorityResult 存在");
    var t9 = typeof(VSA.TodoSystem.Core.UrgencyPriorityStrategy);
    Console.WriteLine($"[PASS] UrgencyPriorityStrategy 存在");
    var t10 = typeof(VSA.TodoSystem.Core.ImportancePriorityStrategy);
    Console.WriteLine($"[PASS] ImportancePriorityStrategy 存在");
    var t11 = typeof(VSA.TodoSystem.Core.PriorityCalculator);
    Console.WriteLine($"[PASS] PriorityCalculator 存在");
    var t12 = typeof(VSA.TodoSystem.Core.PriorityCalculatorExtensions);
    Console.WriteLine($"[PASS] PriorityCalculatorExtensions 存在");
    var t13 = typeof(VSA.TodoSystem.Core.IPriorityStrategy);
    Console.WriteLine($"[PASS] IPriorityStrategy 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(VSA.TodoSystem.Core.OptimizationCriteria);
    Console.WriteLine($"[PASS] OptimizationCriteria enum 存在 (IsEnum: {t14.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}