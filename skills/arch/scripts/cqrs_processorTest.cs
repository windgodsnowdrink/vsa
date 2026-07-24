#load "cqrs_processor.cs"

Console.WriteLine("=== cqrs_processor Test ===");

try
{
    var t0 = typeof(Cqrs.Processor.CqrsProcessor);
    Console.WriteLine($"[PASS] CqrsProcessor 存在");
    var t1 = typeof(Cqrs.Processor.ICommand);
    Console.WriteLine($"[PASS] ICommand 接口存在 (IsInterface: {t1.IsInterface})");
    var t2 = typeof(Cqrs.Processor.CreateTodoCommand);
    Console.WriteLine($"[PASS] CreateTodoCommand record 存在");
    var t3 = typeof(Cqrs.Processor.UpdateTodoCommand);
    Console.WriteLine($"[PASS] UpdateTodoCommand record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}