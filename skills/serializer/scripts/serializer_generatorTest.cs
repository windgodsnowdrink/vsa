#load "serializer_generator.cs"

Console.WriteLine("=== serializer_generator Test ===");

try
{
    var t0 = typeof(Serializer.Generator.Options);
    Console.WriteLine($"[PASS] Options 存在");
    var t1 = typeof(Serializer.Generator.TemplateManager);
    Console.WriteLine($"[PASS] TemplateManager 存在");
    var t2 = typeof(Serializer.Generator.CodeGenerator);
    Console.WriteLine($"[PASS] CodeGenerator 存在");
    var t3 = typeof(Serializer.Generator.ICodeGenerator);
    Console.WriteLine($"[PASS] ICodeGenerator 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(Serializer.Generator.ITemplateManager);
    Console.WriteLine($"[PASS] ITemplateManager 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}