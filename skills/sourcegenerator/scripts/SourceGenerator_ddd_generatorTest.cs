#load "SourceGenerator_ddd_generator.cs"

Console.WriteLine("=== SourceGenerator_ddd_generator Test ===");

try
{
    var t0 = typeof(DddGenerator);
    Console.WriteLine($"[PASS] DddGenerator 存在");
    var t1 = typeof(DddSyntaxReceiver);
    Console.WriteLine($"[PASS] DddSyntaxReceiver 存在");
    var t2 = typeof(EntityInfo);
    Console.WriteLine($"[PASS] EntityInfo 存在");
    var t3 = typeof(Create);
    Console.WriteLine($"[PASS] Create record 存在");
    var t4 = typeof(Update);
    Console.WriteLine($"[PASS] Update record 存在");
    var t5 = typeof(Delete);
    Console.WriteLine($"[PASS] Delete record 存在");
    var t6 = typeof(Get);
    Console.WriteLine($"[PASS] Get record 存在");
    var t7 = typeof(GetAll);
    Console.WriteLine($"[PASS] GetAll record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}