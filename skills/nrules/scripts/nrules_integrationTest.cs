#load "nrules_integration.cs"

Console.WriteLine("=== nrules_integration Test ===");

try
{
    var t0 = typeof(OrderDiscountRule);
    Console.WriteLine($"[PASS] OrderDiscountRule 存在");
    var t1 = typeof(BulkOrderDiscountRule);
    Console.WriteLine($"[PASS] BulkOrderDiscountRule 存在");
    var t2 = typeof(RuleEngineOptions);
    Console.WriteLine($"[PASS] RuleEngineOptions 存在");
    var t3 = typeof(RuleEngine);
    Console.WriteLine($"[PASS] RuleEngine 存在");
    var t4 = typeof(RuleEngineExtensions);
    Console.WriteLine($"[PASS] RuleEngineExtensions 存在");
    var t5 = typeof(ElevatorStateMachine);
    Console.WriteLine($"[PASS] ElevatorStateMachine 存在");
    var t6 = typeof(ElevatorDispatchRule);
    Console.WriteLine($"[PASS] ElevatorDispatchRule 存在");
    var t7 = typeof(OfflineElevatorRule);
    Console.WriteLine($"[PASS] OfflineElevatorRule 存在");
    var t8 = typeof(ElevatorRuleEngineSample);
    Console.WriteLine($"[PASS] ElevatorRuleEngineSample 存在");
    var t9 = typeof(Order);
    Console.WriteLine($"[PASS] Order record 存在");
    var t10 = typeof(Discount);
    Console.WriteLine($"[PASS] Discount record 存在");
    var t11 = typeof(ElevatorSignal);
    Console.WriteLine($"[PASS] ElevatorSignal record 存在");
    var t12 = typeof(ElevatorDispatch);
    Console.WriteLine($"[PASS] ElevatorDispatch record 存在");
    var t13 = typeof(ElevatorAlert);
    Console.WriteLine($"[PASS] ElevatorAlert record 存在");
    var t14 = typeof(ElevatorDirection);
    Console.WriteLine($"[PASS] ElevatorDirection enum 存在 (IsEnum: {t14.IsEnum})");
    var t15 = typeof(ElevatorAlertType);
    Console.WriteLine($"[PASS] ElevatorAlertType enum 存在 (IsEnum: {t15.IsEnum})");
    var t16 = typeof(ElevatorStatus);
    Console.WriteLine($"[PASS] ElevatorStatus enum 存在 (IsEnum: {t16.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}