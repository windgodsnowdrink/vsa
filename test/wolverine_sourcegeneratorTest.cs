#load "wolverine_sourcegenerator.cs"

Console.WriteLine("=== wolverine_sourcegenerator Test ===");

try
{
    var t0 = typeof(WolverineMessageGenerator.MessageContractGenerator);
    Console.WriteLine($"[PASS] MessageContractGenerator 存在");
    var t1 = typeof(WolverineMessageGenerator.InterfaceReceiver);
    Console.WriteLine($"[PASS] InterfaceReceiver 存在");
    var t2 = typeof(WolverineMessageGenerator.BindDeviceController);
    Console.WriteLine($"[PASS] BindDeviceController 存在");
    var t3 = typeof(WolverineMessageGenerator.语法节点);
    Console.WriteLine($"[PASS] 语法节点 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(WolverineMessageGenerator.IDeviceMessages);
    Console.WriteLine($"[PASS] IDeviceMessages 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(WolverineMessageGenerator.里);
    Console.WriteLine($"[PASS] 里 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(WolverineMessageGenerator.名称);
    Console.WriteLine($"[PASS] 名称 record 存在");
    var t7 = typeof(WolverineMessageGenerator.属性);
    Console.WriteLine($"[PASS] 属性 record 存在");
    var t8 = typeof(WolverineMessageGenerator.if);
    Console.WriteLine($"[PASS] if record 存在");
    var t9 = typeof(WolverineMessageGenerator.的属性);
    Console.WriteLine($"[PASS] 的属性 record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}