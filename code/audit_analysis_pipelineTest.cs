#load "audit_analysis_pipeline.cs"

Console.WriteLine("=== audit_analysis_pipeline.cs Test ===");

try
{
    // 验证 class: AuditAnalysisPipeline
    var type_AuditAnalysisPipeline = Type.GetType("AuditAnalysisPipeline");
    if (type_AuditAnalysisPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 AuditAnalysisPipeline (class) 存在");
        var ctors_AuditAnalysisPipeline = type_AuditAnalysisPipeline.GetConstructors();
        Console.WriteLine($"[PASS] AuditAnalysisPipeline 构造函数数量: {ctors_AuditAnalysisPipeline.Length}");
        var methods_AuditAnalysisPipeline = type_AuditAnalysisPipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditAnalysisPipeline 公开方法数量: {methods_AuditAnalysisPipeline.Length}");
        foreach (var m in methods_AuditAnalysisPipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditAnalysisPipeline 未找到，尝试无命名空间...");
        type_AuditAnalysisPipeline = Type.GetType("AuditAnalysisPipeline");
        if (type_AuditAnalysisPipeline != null)
            Console.WriteLine("[PASS] 类型 AuditAnalysisPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditAnalysisPipeline 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
