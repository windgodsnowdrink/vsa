#load "version_core.cs"

Console.WriteLine("=== version_core Test ===");

try
{
    var t0 = typeof(VersionSkill.VersionFormatException);
    Console.WriteLine($"[PASS] VersionFormatException 存在");
    var t1 = typeof(VersionSkill.VersionOptions);
    Console.WriteLine($"[PASS] VersionOptions 存在");
    var t2 = typeof(VersionSkill.SemanticVersion);
    Console.WriteLine($"[PASS] SemanticVersion 存在");
    var t3 = typeof(VersionSkill.VersionRange);
    Console.WriteLine($"[PASS] VersionRange 存在");
    var t4 = typeof(VersionSkill.VersionCache);
    Console.WriteLine($"[PASS] VersionCache 存在");
    var t5 = typeof(VersionSkill.VersionService);
    Console.WriteLine($"[PASS] VersionService 存在");
    var t6 = typeof(VersionSkill.VersionSkillExtensions);
    Console.WriteLine($"[PASS] VersionSkillExtensions 存在");
    var t7 = typeof(VersionSkill.IVersionService);
    Console.WriteLine($"[PASS] IVersionService 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(VersionSkill.RangeType);
    Console.WriteLine($"[PASS] RangeType enum 存在 (IsEnum: {t8.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}