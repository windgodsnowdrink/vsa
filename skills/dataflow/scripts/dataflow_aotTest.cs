#load "dataflow_aot.cs"

Console.WriteLine("=== dataflow_aot Test ===");

try
{
    var t0 = typeof(Dataflow.AOT.DataflowOptions);
    Console.WriteLine($"[PASS] DataflowOptions 存在");
    var t1 = typeof(Dataflow.AOT.DataflowResult);
    Console.WriteLine($"[PASS] DataflowResult 存在");
    var t2 = typeof(Dataflow.AOT.DataflowStatus);
    Console.WriteLine($"[PASS] DataflowStatus 存在");
    var t3 = typeof(Dataflow.AOT.DataflowService);
    Console.WriteLine($"[PASS] DataflowService 存在");
    var t4 = typeof(Dataflow.AOT.DataflowAotEngine);
    Console.WriteLine($"[PASS] DataflowAotEngine 存在");
    var t5 = typeof(Dataflow.AOT.DataflowExtensions);
    Console.WriteLine($"[PASS] DataflowExtensions 存在");
    var t6 = typeof(Dataflow.AOT.IDataflowService);
    Console.WriteLine($"[PASS] IDataflowService 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}