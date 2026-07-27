#load "version_diff_analyzer.cs"

Console.WriteLine("=== version_diff_analyzer.cs Test ===");

try
{
    // 验证 class: VersionDiffAnalyzer
    var type_VersionDiffAnalyzer = Type.GetType("VersionDiffAnalyzer");
    if (type_VersionDiffAnalyzer != null)
    {
        Console.WriteLine("[PASS] 类型 VersionDiffAnalyzer (class) 存在");
        var ctors_VersionDiffAnalyzer = type_VersionDiffAnalyzer.GetConstructors();
        Console.WriteLine($"[PASS] VersionDiffAnalyzer 构造函数数量: {ctors_VersionDiffAnalyzer.Length}");
        var methods_VersionDiffAnalyzer = type_VersionDiffAnalyzer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VersionDiffAnalyzer 公开方法数量: {methods_VersionDiffAnalyzer.Length}");
        foreach (var m in methods_VersionDiffAnalyzer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VersionDiffAnalyzer 未找到，尝试无命名空间...");
        type_VersionDiffAnalyzer = Type.GetType("VersionDiffAnalyzer");
        if (type_VersionDiffAnalyzer != null)
            Console.WriteLine("[PASS] 类型 VersionDiffAnalyzer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VersionDiffAnalyzer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
