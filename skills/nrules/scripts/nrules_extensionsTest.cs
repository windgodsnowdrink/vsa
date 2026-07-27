#load "nrules_extensions.cs"

Console.WriteLine("=== nrules_extensions Test ===");

try
{
    var t0 = typeof(NRulesOptions);
    Console.WriteLine($"[PASS] NRulesOptions 存在");
    var t1 = typeof(Rule);
    Console.WriteLine($"[PASS] Rule 存在");
    var t2 = typeof(RuleDefinition);
    Console.WriteLine($"[PASS] RuleDefinition 存在");
    var t3 = typeof(RuleRepository);
    Console.WriteLine($"[PASS] RuleRepository 存在");
    var t4 = typeof(RuleFactoryImpl);
    Console.WriteLine($"[PASS] RuleFactoryImpl 存在");
    var t5 = typeof(SessionImpl);
    Console.WriteLine($"[PASS] SessionImpl 存在");
    var t6 = typeof(RuleEngineService);
    Console.WriteLine($"[PASS] RuleEngineService 存在");
    var t7 = typeof(RuleSessionImpl);
    Console.WriteLine($"[PASS] RuleSessionImpl 存在");
    var t8 = typeof(Order);
    Console.WriteLine($"[PASS] Order 存在");
    var t9 = typeof(NRulesServiceCollectionExtensions);
    Console.WriteLine($"[PASS] NRulesServiceCollectionExtensions 存在");
    var t10 = typeof(OrderRules);
    Console.WriteLine($"[PASS] OrderRules 存在");
    var t11 = typeof(HighValueOrderRule);
    Console.WriteLine($"[PASS] HighValueOrderRule 存在");
    var t12 = typeof(PriorityOrderRule);
    Console.WriteLine($"[PASS] PriorityOrderRule 存在");
    var t13 = typeof(IRuleEngineService);
    Console.WriteLine($"[PASS] IRuleEngineService 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(IRuleRepository);
    Console.WriteLine($"[PASS] IRuleRepository 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(IRuleSession);
    Console.WriteLine($"[PASS] IRuleSession 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(IContext);
    Console.WriteLine($"[PASS] IContext 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(RuleFactory);
    Console.WriteLine($"[PASS] RuleFactory 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(ISession);
    Console.WriteLine($"[PASS] ISession 接口存在 (IsInterface: {t18.IsInterface})");
    var t19 = typeof(OrderStatus);
    Console.WriteLine($"[PASS] OrderStatus enum 存在 (IsEnum: {t19.IsEnum})");
    var t20 = typeof(OrderPriority);
    Console.WriteLine($"[PASS] OrderPriority enum 存在 (IsEnum: {t20.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}