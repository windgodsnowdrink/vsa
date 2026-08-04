#load "miniexcel_full_features.cs"

Console.WriteLine("=== miniexcel_full_features Test ===");

try
{
    var t0 = typeof(FullFeatureExcelService);
    Console.WriteLine($"[PASS] FullFeatureExcelService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}