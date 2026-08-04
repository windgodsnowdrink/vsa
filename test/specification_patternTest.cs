#load "specification_pattern.cs"

Console.WriteLine("=== specification_pattern Test ===");

try
{
    var t0 = typeof(BaseSpecification);
    Console.WriteLine($"[PASS] BaseSpecification 存在");
    var t1 = typeof(SpecificationEvaluator);
    Console.WriteLine($"[PASS] SpecificationEvaluator 存在");
    var t2 = typeof(ISpecification);
    Console.WriteLine($"[PASS] ISpecification 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}